using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutter_Logic : Enemy
{
    float hurtTimer = 0;

    Enemy_Pursue pursue;
    Enemy_Wander wander;

    public GameObject attackHurtBox;
    public GameObject headTrigger;

    public float turnSpeed;
    public float accel;

    public bool attacking;

    bool canMoveDuringAttack;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        pursue = GetComponent<Enemy_Pursue>();
        wander = GetComponent<Enemy_Wander>();

        pursue.target = PlayerManager.Instance.player;

        HurtBoxInfo.ToggleHurtBox(attackHurtBox, false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (hurtTimer > 0)
            hurtTimer -= Time.deltaTime;
        else if (attacking)
            AttackMovement();
        else
        {
            Move();
            AttackLogic();
        }

        anim.SetBool("Carried", carried);
        StandUp();
    }

    //can copy and paste this Move function for basic enemies
    private void Move()
    {
        //if -- not being carried
        if (!carried)
        {
            headTrigger.SetActive(true);

            //if -- enemy is still walking around
            if (!felled && !thrown)
            {
                // LOGIC TO DECIDE MOVEMENT DIRECTION, REPLACE WITH RELEVANT LOGIC ----------------------------------------
                Vector3 intendedDir = Vector3.zero;
                if ((pursue.target.transform.position - transform.position).magnitude < pursue.chaseRadius)
                {
                    intendedDir = pursue.Pursue();
                    velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel) * pursue.speed;
                }
                else
                {
                    intendedDir = wander.Wander();
                    if (intendedDir == Vector3.zero)
                        velocity = intendedDir;
                    else
                        velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel) * wander.speed;
                }

                Vector3 XYDir = velocity;
                XYDir.y = 0;
                //prevents direction confusion if 
                if (XYDir != Vector3.zero)
                {
                    transform.forward = XYDir;
                }
                anim.SetFloat("Speed", XYDir.magnitude);
                // END MOVEMENT LOGIC -------------------------------------------------------------------------------------
            }
            //end if -- enemy is still walking around

            //checks for being grounded and applies gravity
            CheckGrounded();
            if (grounded)
            {
                velocity.y = 0;
                if (thrown)
                {
                    wander.home = transform.position;
                    thrown = false;
                }

                if (felled)
                    velocity = Vector3.zero;
            }

            ApplyGravity();

            //final move of character controller
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            headTrigger.SetActive(false);
        }
        //end if -- not being carried
    }

    public void ToggleHurtBox(bool toggle)
    {
        HurtBoxInfo.ToggleHurtBox(attackHurtBox, toggle);
    }

    //checks conditions and starts attack if they are met
    void AttackLogic()
    {
        if (felled)
            return;

        Vector3 dir = pursue.target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle < 20f && dir.magnitude < 2.2f)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            canMoveDuringAttack = true;
        }
    }
    //the logic nutter follows while "attacking" is true (the whole time the animation plays)
    void AttackMovement()
    {
        if (canMoveDuringAttack)
        {
            Vector3 dir = pursue.target.transform.position - transform.position;
            dir.y = 0;
            transform.forward = Vector3.RotateTowards(transform.forward, dir, turnSpeed, accel) * pursue.speed;
        }
    }
    public void AttackCoroutine()
    {
        canMoveDuringAttack = false;
        StartCoroutine(Attack());
    }

    //deals with getting hit by crock
    private void OnTriggerEnter(Collider other)
    {
        int layerNumber = 9; //layer 9 = player
        if (other.gameObject.layer == layerNumber && other.CompareTag("HurtBox"))
        {
            PlaySound hurtSound = null;
            if (other.gameObject.layer == 9) //layer 9 = player
                hurtSound = PlayerManager.Instance.currentHitSound;

            Hurt(other.GetComponent<HurtBoxInfo>().damage, other.transform.position, hurtSound);

            if (health <= 0)
            {
                anim.SetTrigger("Fall");
                attacking = false;
                felled = true;
                standupTimer = 0;
                anim.SetFloat("StandUp", 1f);
            }
            else
            {
                anim.SetTrigger("Hit");
                StartCoroutine(ResetHit());
                hurtTimer = 1f;
                velocity = Vector3.zero;
            }
        }
    }

    IEnumerator Attack()
    {
        for(float f = 0; f < .33f; f += Time.deltaTime)
        {
            controller.Move(transform.forward * 5f * Time.deltaTime);
            yield return null;
        }
    }

    //will reset the hit trigger if it doesn't happen immediately so the nutter doesn't wobble at a weird time
    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.05f);
        anim.ResetTrigger("Hit");
    }
}
