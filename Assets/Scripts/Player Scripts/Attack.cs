using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool canAttack = true;
    public bool attackDone = true;
    bool buttonUp = true;

    PlayerMove playerMove;
    Animator anim;

    private float initMaxSpeed; //the initial max speed of the player in case an attack changes it
    private float initGravMult; //the initial gravity multiplier of the player for when an attack changes it
    private float initTurnSpeed; //ditto for the turning speed of the player

    public GameObject punchCollider;
    public GameObject AirRollCollider;
    public GameObject GroundPoundCollider;
    public GameObject ChargeCollider;

    public LayerMask enemyMask;

    public bool carrying = false;
    public bool throwing = false;
    bool windingUp = false;
    int throwFrameCounter = 0;
    GameObject carryTarget; //the object crock is carrying
    public bool matchTargetPos = false; //whether the carry target should match crocks position
    public Transform carrySocket;
    Vector3 throwVector;
    float throwVectorMultiplier = 1f;

    public List<GameObject> attackEffects; //the special effect model of crocks different attacks
    /*
     0 = punch
     
    */

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponentInChildren<Animator>();
        initMaxSpeed = playerMove.maxSpeed;
        initGravMult = playerMove.gravityMult;
        initTurnSpeed = playerMove.turnSpeed;
    }
    public void AttackLogic()
    {
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.transformed)
            return;

        canAttack = PlayerManager.Instance.canMove &&
            PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal &&
            attackDone && buttonUp;

        if(canAttack && Input.GetAxis("Punch") > 0)
        {
            if (playerMove.GetGrounded())
            {
                buttonUp = false;
                GetComponent<Idle>().StopIdle();
                attackDone = false;

                //checks if there is an enemy to pick up within the punching radius.
                if (CheckForFelledEnemies())
                {
                    //Pick Up
                    PickUp();
                }
                else
                {
                    //Punch
                    Punch();
                }
            } else if (!playerMove.GetGrounded() && !playerMove.GetHiJumping())
            {
                buttonUp = false;
                GetComponent<Idle>().StopIdle();
                attackDone = false;

                //AirRoll
                AirRoll();
            }
        } else if (canAttack && Input.GetAxis("Crouch") > 0 && !playerMove.GetGrounded() && !playerMove.crouchPressed)
        {
            attackDone = false;
            //GroundPound
            GroundPound();
        }

        if (Input.GetAxis("Punch") == 0)
            buttonUp = true;

        anim.SetBool("Carrying", carrying);
        MatchTargetPos();

        if (carrying || throwing)
            Throw();

        if (carrying)
            GetComponent<Idle>().StopIdle();
    }

    public void Punch()
    {
        GetComponent<Idle>().StopIdle();
        anim.SetTrigger("Attack01");
        playerMove.maxSpeed = 2f;
        PlayerManager.Instance.currentHitSound = PlayerManager.Instance.hitSounds[0];
    }

    void AirRoll()
    {
        GetComponent<Idle>().StopIdle();
        anim.SetTrigger("Attack02");
        StartCoroutine("AirRollCoroutine");
    }

    void GroundPound()
    {
        GetComponent<Idle>().StopIdle();
        anim.SetTrigger("Attack03");
        StartCoroutine("GroundPoundCoroutine");
    }

    void PickUp()
    {
        Vector3 faceDir = carryTarget.transform.position - transform.position;
        faceDir.y = 0;
        faceDir.Normalize();
        transform.forward = faceDir;
        anim.SetTrigger("PickUp");

        carryTarget.GetComponent<Enemy>().SetCarried(true);

        PlayerManager.Instance.currentState = PlayerManager.PlayerState.carrying;
    }

    //begins the throwing, deals with short throws and long throws
    void Throw()
    {
        if((carrying || throwing) && Input.GetAxis("Punch") > 0)
        {
            throwFrameCounter++;
        }
        
        if(throwFrameCounter > 10 && Input.GetAxis("Punch") > 0)
        {
            if (!windingUp)
            {
                anim.SetTrigger("ThrowHeavy");
                windingUp = true;
            }

            playerMove.maxSpeed = 0.01f;
            throwVectorMultiplier += Time.deltaTime;
            if (throwVectorMultiplier > 3f)
                throwVectorMultiplier = 3f;
        }
        else if((throwFrameCounter > 0 && throwFrameCounter <= 10) && Input.GetAxis("Punch") == 0)
        {
            PlayerManager.Instance.canMove = false;

            throwVector = transform.forward;
            throwVector.y = 0.5f;
            throwVector.Normalize();
            throwVector *= 5f;

            throwing = false;
            anim.SetTrigger("ThrowLight");
            throwFrameCounter = 0;
        }else if(throwFrameCounter > 10 && Input.GetAxis("Punch") == 0)
        {
            throwVector = transform.forward;
            throwVector *= throwVectorMultiplier;
            throwVector *= 5f;

            throwing = false;
            anim.SetTrigger("Release");
            throwFrameCounter = 0;
        }

        //will drop the enemy if crock crouches
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.crouching)
        {
            DropEnemy();
        }
    }

    //the actual point the enemy/object leaves crock's hands. Is triggered via an animation event
    public void ThrowRelease()
    {
        matchTargetPos = false;
        carryTarget.GetComponent<Enemy>().SetCarried(false);
        carryTarget.GetComponent<Enemy>().SetThrown(true);
        carryTarget.GetComponent<Enemy>().Throw(throwVector * 5f);
        carryTarget = null;
        throwing = false;
        carrying = false;

        anim.SetBool("Carrying", carrying);

        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.crouching)
        {
            PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
        }
        windingUp = false;

        StopAttack();
        throwVectorMultiplier = 1f;
    }

    //called when crock needs to drop an enemy without tossing. Through crouching or dropping into water
    public void DropEnemy()
    {
        if (carryTarget != null)
        {
            throwVector = transform.forward;
            throwVector.y = 0.8f;
            throwVector.Normalize();

            ThrowRelease();
        }
    }

    public void StopAttack()
    {
        attackDone = true;
        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.crouching)
        {
            playerMove.maxSpeed = initMaxSpeed;
            playerMove.gravityMult = initGravMult;
            playerMove.turnSpeed = initTurnSpeed;
        }
        foreach (GameObject effect in attackEffects)
        {
            effect.SetActive(false);
        }

        //turns off all hurtboxes just in case
        GetComponentInChildren<CrockAnimListener>().HurtboxOff("PunchCollider");
        GetComponentInChildren<CrockAnimListener>().HurtboxOff("AirRollCollider");
        GetComponentInChildren<CrockAnimListener>().HurtboxOff("GroundPoundCollider");
    }

    //this function determines if crock is in front of an enemy he can pick up or not
    bool CheckForFelledEnemies()
    {
        bool canPickUp = false;

        Collider[] hitColliders = Physics.OverlapSphere(punchCollider.transform.position, punchCollider.GetComponent<SphereCollider>().radius, enemyMask);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Enemy>() != null)
            {
                if (hitCollider.gameObject.GetComponent<Enemy>().IsFelled())
                {
                    carryTarget = hitCollider.gameObject;
                    canPickUp = true;
                }
                break;
            }
            
        }

        return canPickUp;
    }

    void MatchTargetPos()
    {
        if (matchTargetPos)
        {
            //carryTarget.transform.position = carrySocket.position + (transform.right * -0.255f);
            carryTarget.transform.position = carrySocket.position + (transform.up * .1f);
            carryTarget.transform.forward = -transform.forward;
        }
    }

    public GameObject GetCarryTarget()
    {
        return carryTarget;
    }

    public void ResetCarryTarget()
    {
        matchTargetPos = false;
        carryTarget.GetComponent<Enemy>().SetCarried(false);
        carryTarget.GetComponent<Enemy>().SetThrown(true);
        carryTarget = null;
        throwing = false;
        carrying = false;
        StopAttack();

        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
    }

    IEnumerator AirRollCoroutine()
    {
        GetComponentInChildren<CrockAnimListener>().HurtboxOn("AirRollCollider");

        playerMove.maxSpeed -= 5f;
        playerMove.turnSpeed = .5f;

        Vector3 newVelocity = playerMove.controller.velocity;
        newVelocity.y = 0;
        playerMove.SetVelocity(newVelocity);

        playerMove.gravityMult = 0;
        float timer = 0.4f;
        yield return new WaitForSeconds(timer);

        GetComponentInChildren<CrockAnimListener>().HurtboxOff("AirRollCollider");

        StopAttack();
    }

    IEnumerator GroundPoundCoroutine() {

        PlayerManager.Instance.canMove = false;

        playerMove.SetSpeed(0);
        playerMove.maxSpeed = 0;
        playerMove.turnSpeed = 100f;
        playerMove.SetVelocity(Vector3.up * 2f);
        playerMove.gravityMult = 0;

        float timer = .317f; //this is how long the groundpound animation is
        yield return new WaitForSeconds(timer);


        playerMove.gravityMult = 10f; //this will be how fast crock falls.
        GetComponentInChildren<CrockAnimListener>().HurtboxOn("GroundPoundCollider");

        float timeFalling = 0;

        while (!playerMove.GetGrounded())
        {
            timeFalling += Time.deltaTime;

            //turns on groundpound effect 1 if a certain amount of time has gone by
            if(timeFalling > 0.2f)
                attackEffects[1].SetActive(true);

            yield return null;
        }
        GetComponentInChildren<CrockAnimListener>().HurtboxOff("GroundPoundCollider");

        Vector3 bounceVelocity = playerMove.controller.velocity;
        //bounceVelocity.y = -bounceVelocity.y * 0.5f;
        bounceVelocity.y = 10f;
        playerMove.SetVelocity(bounceVelocity);
        attackEffects[1].SetActive(false);

        yield return new WaitForSeconds(0.2f);

        PlayerManager.Instance.canMove = true;

        StopAttack();
    }
}
