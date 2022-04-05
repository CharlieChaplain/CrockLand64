using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swim : MonoBehaviour
{
    Transform cam; //camera
    public Transform model; //crock model, the thing that gets rotated
    _Controls controls;

    PlayerMove playerMove;

    Vector2 input;
    Vector3 targetVelocity;
    Vector3 velocity;

    private Vector3 targetAngleEulers;
    private Vector3 currentAngleEulers;

    public bool inWater;
    bool underwater;

    //public Transform waterCheck;
    //public float waterCheckRadius;
    public LayerMask waterMask;
    bool topCheck;
    bool bottomCheck;

    CharacterController controller;
    Animator anim;

    public float timeToTurn = 0.1f; //time for character to arrive at target angle
    public float turnSpeed; //how quickly angles change
    Vector3 timeToTurnRef;
    float strokeTimer;
    public float strokeSpeed;
    bool diveTimerUp = true;

    public float paddleSpeed;

    public float accel;
    Vector3 velocityRef;

    public PlaySound underwaterSwoosh;
    public PlaySound enterWater;
    public PlaySound surface;

    public ResizeBubbleCollider bubbleCollider;

    public ParticleSystem partBubbles;
    public ParticleSystem partBubblesBurst;
    public ParticleSystem partWake;

    public ParticleSystem partDrips;

    public GameObject sploosh;

    int prevMusicIndex = -1; //used for changing the music when diving and resurfacing
    const int UWMUSICINDEX = 1; //the index of underwater music.

    private enum WaterStates
    {
        paddling,
        diving
    }
    private WaterStates currentWaterState = WaterStates.paddling;
    private void Awake()
    {
        controls = InputManager.Instance.controls;
    }
    private void OnEnable()
    {
        StartCoroutine(EnableControls());
    }
    IEnumerator EnableControls()
    {
        yield return new WaitForEndOfFrame();

        controls = InputManager.Instance.controls;

        // player subscriptions--------------------------------------------------
        controls.EditableControls.Movement.performed += OnMoveListener;
        controls.EditableControls.Movement.canceled += OnMoveListener;

        controls.EditableControls.Punch.started += OnPunchListener;
        controls.EditableControls.Jump.started += OnJumpListener;
        controls.EditableControls.Jump.canceled += OnJumpCancel;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerMove = GetComponent<PlayerMove>();
        cam = playerMove.cam;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckUnderwater();
    }

    public void OnMoveListener(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            return;
        input = obj.ReadValue<Vector2>();
    }

    public void OnPunchListener(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            return;
        switch (currentWaterState)
        {
            case WaterStates.paddling:
                currentWaterState = WaterStates.diving;
                Dive();
                break;
            case WaterStates.diving:
                //set targetvelocity to paddle speed
                targetVelocity = model.forward * paddleSpeed;
                break;
            default:
                break;
        }
    }
    public void OnJumpListener(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            return;
        switch (currentWaterState)
        {
            case WaterStates.paddling:
                break;
            case WaterStates.diving:
                if(strokeTimer <= 0)
                {
                    //set regular velocity to stroke speed
                    velocity = model.forward * strokeSpeed;

                    anim.SetTrigger("Stroke");
                    underwaterSwoosh.Play(transform.position);

                    strokeTimer = 1f;

                    partBubblesBurst.Stop();
                    partBubblesBurst.Play();
                }
                break;
            default:
                break;
        }
    }

    public void OnJumpCancel(InputAction.CallbackContext obj)
    {
        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            return;
        if (currentWaterState == WaterStates.diving)
        {
            //set targetveloctiy to zero when not paddling
            targetVelocity = Vector3.zero;
        }
    }

        public void UpdateChecks(bool _topCheck, bool _bottomCheck)
    {
        topCheck = _topCheck;
        bottomCheck = _bottomCheck;
    }

    public void EnterWater(float currentYRotation, Vector3 _velocity)
    {
        //drops enemy if crock is carrying one
        GetComponent<Attack>().DropEnemy();
        GetComponent<Attack>().StopAttack();

        currentWaterState = WaterStates.paddling;

        velocity = (_velocity * 0.5f) + (transform.forward * 0f);

        model.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

        //large splash if crock is moving downwards quickly
        if (velocity.y < -2f)
        {
            enterWater.Play(transform.position);
            partBubblesBurst.Play();

            Vector3 splooshPos = transform.position;
            RaycastHit hit;

            //creates ray and then reverses the direction so that it'll see the top of the water from the outside
            Ray splooshRay = new Ray(transform.position, Vector3.up);
            splooshRay.origin = splooshRay.GetPoint(5f);
            splooshRay.direction *= -1f;

            if (Physics.Raycast(splooshRay, out hit, 5f, waterMask, QueryTriggerInteraction.Collide))
            {
                splooshPos.y = hit.transform.position.y + (hit.transform.localScale.y / 2f);
            }

            GameObject splooshObj = Instantiate(sploosh);
            splooshObj.transform.position = splooshPos + Vector3.up * 0.5f * splooshObj.transform.localScale.y + Vector3.up * -.5f;
            splooshObj.GetComponent<Billboard>().cam = cam.GetComponent<Camera>();

            partDrips.transform.position = splooshPos + Vector3.up * 0.5f;
            partDrips.transform.rotation = Quaternion.Euler(-90f, 0, 0);
            partDrips.Play();
        }
        else
        {
            surface.Play(transform.position);
            if(!partWake.isPlaying)
                partWake.Play();
        }

    }

    public Vector3 RootSwimMove()
    {
        switch (currentWaterState)
        {
            case WaterStates.paddling:
                Paddle(cam);
                break;
            case WaterStates.diving:
                SwimRotate();
                SwimMove();
                break;
            default:
                break;
        }
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocityRef, accel);
        if (velocity.magnitude < 0.01f)
            velocity = Vector3.zero;

        anim.SetFloat("SwimSpeed", velocity.magnitude);

        return velocity;
    }

    void SwimRotate()
    {
        //--------------INVERT VERT FOR INVERTED Y CONTROLS--------------
        Vector3 direction = new Vector3(input.y, input.x, 0);
        if (direction.magnitude > 1)
            direction.Normalize();

        targetAngleEulers += direction * turnSpeed;
        targetAngleEulers.x = Mathf.Clamp(targetAngleEulers.x, -90f, 90f);

        currentAngleEulers = Vector3.SmoothDamp(currentAngleEulers, targetAngleEulers, ref timeToTurnRef, timeToTurn);
        currentAngleEulers.x = Mathf.Clamp(currentAngleEulers.x, -90f, 90f);

        model.rotation = Quaternion.Euler(currentAngleEulers);
        //matches root transform's y component to model transform so it matches when crock enters/leaves water
        transform.rotation = Quaternion.Euler(0, model.rotation.eulerAngles.y, 0);
    }

    void SwimMove()
    {
        //this line makes crock always swim in his forward direction while diving. Gives more control over direction.
        velocity = model.forward * velocity.magnitude;

        if (strokeTimer > 0)
            strokeTimer -= Time.deltaTime;

        if (!topCheck && diveTimerUp)
        {
            currentWaterState = WaterStates.paddling;
            Surface();
        }
    }
    void Paddle(Transform cam)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        if (direction.magnitude > 1)
            direction.Normalize();

        if (direction.magnitude >= 0.1f && PlayerManager.Instance.canMove)
        {
            //set target velocity to direction rotated by camera
            targetVelocity = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * direction * paddleSpeed;
        }
        else
        {
            //set target velocity to zero
            targetVelocity = Vector3.zero;
        }

        if (underwater && PlayerManager.Instance.canMove)
            velocity.y += .2f;


        if (velocity == Vector3.zero)
        {
            //emits less particles if staying still
            var partWakeEmission = partWake.emission;
            partWakeEmission.rateOverTime = 1f;

            if (!underwater)
                model.localRotation = Quaternion.identity;
        }
        else
        {
            var partWakeEmission = partWake.emission;
            partWakeEmission.rateOverTime = 3f;

            model.forward = velocity;
        }
        //matches root transform's y component to model transform so it matches when crock enters/leaves water
        transform.rotation = Quaternion.Euler(0, model.rotation.eulerAngles.y, 0);

    }

    void CheckUnderwater()
    {
        if (bottomCheck)
        {
            inWater = true;
            if (!topCheck)
            {
                //checks if crock needs to surface
                if (underwater && currentWaterState == WaterStates.diving)
                {
                    //currentWaterState = WaterStates.paddling;
                    //Surface();
                }

                underwater = false;
                velocity.y = 0;
                //particles
                if(!partWake.isPlaying)
                    partWake.Play();
                if(partBubbles.isPlaying)
                    partBubbles.Stop();
            }else
            {
                underwater = true;

                //particles
                if (partWake.isPlaying)
                    partWake.Stop();
                if (!partBubbles.isPlaying)
                    partBubbles.Play();
            }//end if NOT topcheck
        }else
        {
            inWater = false;
            //particles
            if (partWake.isPlaying)
                partWake.Stop();
            if (partBubbles.isPlaying)
                partBubbles.Stop();
        }//end if bottomcheck

        //gives inWater bool to footsteps script
        GetComponent<Footsteps>().isInWater = inWater;
    }
    /*
    void CheckUnderwater2()
    {
        //initial check for being in water. This sphere is at the player origin
        if (Physics.CheckSphere(transform.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide))
        {
            //--------------------IN WATER------------------
            if (!inWater)
                EnterWater(transform.rotation.eulerAngles.y, GetComponent<PlayerMove>().GetVelocity());

            inWater = true;


            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming)
            {
                //secondary check to see if player is at the top of the water. This sphere is above the previous sphere
                if (!Physics.CheckSphere(waterCheck.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide))
                {
                    if (underwater && currentWaterState == WaterStates.diving)
                    {
                        currentWaterState = WaterStates.paddling;
                        Surface();
                    }

                    velocity.y = 0;

                    if (underwater)
                    {
                        partWake.Play();
                        partBubbles.Stop();
                    }

                    underwater = false;
                }
                else
                {
                    if (!underwater)
                    {
                        partWake.Stop();
                        partBubbles.Play();
                    }

                    underwater = true;
                }
            }
        }
        else
        {
            inWater = false;

            partWake.Stop();
            partBubbles.Stop();
        }

        //gives inWater bool to footsteps script
        GetComponent<Footsteps>().isInWater = inWater;
    }*/

    public bool IsPaddling()
    {
        return !underwater;
    }

    void Dive()
    {
        StartCoroutine(DiveTimer());

        anim.SetTrigger("Stroke");
        underwaterSwoosh.Play(transform.position);

        Vector3 direction = Quaternion.Euler(45f, transform.rotation.eulerAngles.y, 0) * Vector3.forward;
        velocity = strokeSpeed * direction;
        targetVelocity = velocity;

        model.forward = velocity;

        currentAngleEulers = targetAngleEulers = model.rotation.eulerAngles;

        //changes music to going underwater
        int currentIndex = SoundManager.Instance.music.GetSourceIndex();
        if(currentIndex != UWMUSICINDEX)
        {
            prevMusicIndex = currentIndex;
            SoundManager.Instance.music.ChangeMusic(1);
        }
    }

    IEnumerator DiveTimer()
    {
        diveTimerUp = false;
        yield return new WaitForSeconds(1f);
        diveTimerUp = true;
    }

    void Surface()
    {
        //controller.Move(waterCheck.position - transform.position);

        surface.Play(transform.position);

        model.rotation = Quaternion.Euler(0, currentAngleEulers.y, 0);

        Vector3 direction = model.forward;
        direction.y = 0;
        direction.Normalize();
        targetVelocity = velocity = direction * paddleSpeed;

        //changes music to normal
        SoundManager.Instance.music.ChangeMusic(prevMusicIndex);
    }
}
