using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;
    public Animator anim;
    public Transform model;

    _Controls controls;

    private RuntimeAnimatorController animController;

    private Swim swim;

    private bool grounded;
    private bool ungroundTimerUp = true; //short timer that prevents player from being grounded again immediately after jumping
    private bool jumping = false;
    private bool hiJumping = false;
    private RaycastHit currentPolygon;

    public float jumpHeight;
    private float oldJumpHeight;
    private float currentFormJumpHeight;
    private bool jumpFootLeft = true; //swaps each jump, used to determine the foot crock holds up

    public float maxSpeed = 10f;
    private float functionalMaxSpeed;
    private float oldMaxSpeed;
    private float speed = 0;
    public float accel = 1f;
    private float decel;

    private Vector2 input;
    private Vector3 velocity;
    public float gravityMult = 0.25f;
    public float upGravMult = 0.4f;
    public float downGravMult = 0.2f;
    private float terminalVelocity = -40.0f;
    private float oldDownGravMult;
    private float oldUpGravMult;
    private float oldGravMult;

    public float turnSpeed = 0.1f;
    private float turnSmoothing;
    private float targetAngle = 0;
    private float angle = 0;

    private bool crouching = false;
    private bool crouchingLastFrame = false;
    public bool crouchPressed = false;
    private bool floorAbove;

    private NPC currentNPC;
    private bool NPCInteract = false;

    public Transform waist;
    private float angleToLean;

    public float edgeCorrectionSpeed = 0.5f; //speed that the character falls off the edge
    private float edgeCorrectionAddition = 0; //adds more edge correction the longer it's happening
    private float oldECSpeed;
    public float noSlipDistance = 0.1f; //distance character is from edge of platform before they slip off
    public float velDampenMult;
    Vector3 inputVector; //used to dampen players input towards a slope they need to fall down

    //these two variables used to adjust how much the player is pushed into a slope
    public float slopeForceMultiplier;
    public float slopeRayMultiplier;

    private Vector2 standingCollider = new Vector2(0.89f, 1.72f);
    private Vector2 crouchingCollider = new Vector2(0.46f, 0.86f);
    private Vector2 swimmingCollider = new Vector2(0, 0.86f);

    private bool sliding = false;
    private bool slidingLastFrame = false;
    public float slideSpeed;
    public float slideMaxSpeed;
    public ParticleSystem partSliding;
    public PlaySound slidingSoundObject;
    float oldSlopeLimit; //used to increase crock's slope limit when sliding

    public Transform waterCheck; //used for determining if the player is in deep enough water to swim
    public float waterCheckRadius;
    public LayerMask waterMask;

    public PlaySound jumpSound;

    public GameObject buttonAlert; //shows up when crock can do things with a button

    Form_Stone stone;
    Form_Ghost ghost;

    const float ROOT2 = 1.414f;
    private void Awake()
    {
        //controls = InputManager.Instance.controls;
    }
    private void OnEnable()
    {
        StartCoroutine(EnableControls());
    }
    IEnumerator EnableControls()
    {
        yield return new WaitForEndOfFrame();

        controls = InputManager.controls;

        // player subscriptions--------------------------------------------------
        controls.EditableControls.Move.performed += OnMoveListener;
        controls.EditableControls.Move.canceled += OnMoveListener;

        controls.EditableControls.Jump.started += OnJumpListener;
        controls.EditableControls.Jump.canceled += OnJumpListener;

        controls.EditableControls.Crouch.performed += OnCrouchListener;
        controls.EditableControls.Crouch.canceled += OnCrouchListener;

        controls.EditableControls.Punch.started += OnPunchListener;
    }

    private void Start()
    {
        functionalMaxSpeed = maxSpeed;
        decel = accel;
        oldJumpHeight = jumpHeight;
        currentFormJumpHeight = jumpHeight;
        oldDownGravMult = downGravMult;
        oldUpGravMult = upGravMult;
        oldGravMult = gravityMult;
        oldMaxSpeed = maxSpeed;
        oldECSpeed = edgeCorrectionSpeed;
        oldSlopeLimit = controller.slopeLimit;
        animController = anim.runtimeAnimatorController;

        //waterCheck.position = transform.position + (Vector3.up * waterCheckRadius * 2f);

        swim = GetComponent<Swim>();
        stone = GetComponent<Form_Stone>();
        ghost = GetComponent<Form_Ghost>();
    }

    void Update()
    {
        bool canMove = PlayerManager.Instance.canMove;

        CheckGrounded();
        CheckSliding();
        CheckSwimming();
        CheckNPC();
        
        switch (PlayerManager.Instance.currentState)
        {
            case PlayerManager.PlayerState.normal:
                if(canMove)
                    Move(maxSpeed, turnSpeed, canMove);
                break;
            case PlayerManager.PlayerState.charging:
                if(grounded)
                    velocity = GetComponent<Charge>().ChargeMove(cam, input);
                break;
            case PlayerManager.PlayerState.crouching:
                Move(maxSpeed, turnSpeed, canMove);
                break;
            case PlayerManager.PlayerState.sliding:
                Slide();
                break;
            case PlayerManager.PlayerState.swimming:
                velocity = swim.RootSwimMove();
                break;
            case PlayerManager.PlayerState.carrying:
                Move(maxSpeed, turnSpeed, canMove);
                break;
            case PlayerManager.PlayerState.ladder:
                velocity = Vector3.zero;
                GetComponent<LadderClimb>().LadderMove();
                break;
            case PlayerManager.PlayerState.hurt:
                HurtMove();
                break;
            case PlayerManager.PlayerState.transformed:
                TransformedSwitch(canMove);
                break;
            default:
                break;

        }

        if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming &&
           PlayerManager.Instance.currentState != PlayerManager.PlayerState.ladder)
        {
            if (canMove)
            {
                if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.sliding)
                    CheckCrouch();
                anim.SetFloat("Speed", speed);
            }

            ApplyGravity();

            if (speed > 0)
                GetComponent<Idle>().StopIdleFull();


            slidingLastFrame = sliding;
            crouchingLastFrame = crouching;
        }
    }

    public void OnMoveListener(InputAction.CallbackContext obj)
    {
        input = obj.ReadValue<Vector2>();
    }

    public void OnPunchListener(InputAction.CallbackContext obj)
    {
        if (NPCInteract)
        {
            currentNPC.Engage();

            StartCoroutine(ButtonAlertDisappear());
        }
    }

    /// <summary>
    /// if crock is transformed, this function determines what to do with a switch statement
    /// </summary>
    void TransformedSwitch(bool canMove)
    {
        switch (PlayerManager.Instance.currentForm)
        {
            case PlayerManager.PlayerForm.stone:
                Move(stone.speed, stone.turnSpeed, canMove);
                stone.StoneUpdate();
                break;
            case PlayerManager.PlayerForm.ghost:
                Move(ghost.speed, ghost.turnSpeed, canMove);
                ghost.GhostUpdate(grounded);
                velocity += ghost.VerticalMovement();
                velocity.y = Mathf.Clamp(velocity.y, -ghost.maxVertSpeed, ghost.maxVertSpeed);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// call this when form is changed
    /// </summary>
    /// <param name="form">the form crock must go back to</param>
    public void ChangeForm(PlayerManager.PlayerForm form)
    {
        switch (form)
        {
            case PlayerManager.PlayerForm.none:
                PlayerManager.Instance.currentForm = PlayerManager.PlayerForm.none;
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                jumpHeight = oldJumpHeight;
                currentFormJumpHeight = oldJumpHeight;
                GetComponent<ChangeModel>().ChangeModelTo(0);
                anim.runtimeAnimatorController = animController;
                gravityMult = oldGravMult;
                break;
            case PlayerManager.PlayerForm.stone:
                PlayerManager.Instance.currentForm = PlayerManager.PlayerForm.stone;
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.transformed;
                jumpHeight = stone.jumpHeight;
                currentFormJumpHeight = stone.jumpHeight;
                GetComponent<ChangeModel>().ChangeModelTo(2);
                anim.runtimeAnimatorController = stone.animController;
                stone.StoneInit(this);
                break;
            case PlayerManager.PlayerForm.ghost:
                PlayerManager.Instance.currentForm = PlayerManager.PlayerForm.ghost;
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.transformed;
                GetComponent<ChangeModel>().ChangeModelTo(3);
                anim.runtimeAnimatorController = ghost.animController;
                gravityMult *= 0.2f;
                ghost.GhostInit(this);
                break;
            default:
                break;
        }
    }

    private void CheckNPC()
    {
        bool check = false;
        NPC npc = null;
        LayerMask npcMask = LayerMask.GetMask("NPC");
        Collider[] checkedColliders = Physics.OverlapSphere(transform.position + Vector3.up * controller.height * 0.5f, controller.radius, npcMask);

        //gets the first npc collider returned
        if(checkedColliders.Length > 0 &&
        PlayerManager.Instance.canMove &&
        checkedColliders[0].transform.root.gameObject.GetComponent<NPC>().canEngage &&
        PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
        {
            buttonAlert.SetActive(true);
            buttonAlert.GetComponent<Animator>().SetBool("Active", true);

            NPCInteract = true;
            currentNPC = checkedColliders[0].transform.root.gameObject.GetComponent<NPC>();
        }
        else
        {
            if (NPCInteract)
                StartCoroutine(ButtonAlertDisappear());
            NPCInteract = false;
        }
        /*
        //will see if any of the returned colliders are an npc
        foreach (Collider col in checkedColliders)
        {
            if (col.CompareTag("NPC") &&
                PlayerManager.Instance.canMove &&
                col.transform.root.gameObject.GetComponent<NPC>().canEngage &&
                PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
            {
                check = true;
                npc = col.transform.root.gameObject.GetComponent<NPC>();
            }
        }

        if (check)
        {
            buttonAlert.SetActive(true);
            buttonAlert.GetComponent<Animator>().SetBool("Active", true);

            NPCInteract = true;
            currentNPC = npc;
        } else
        {
            if(NPCInteract)
                StartCoroutine(ButtonAlertDisappear());
            NPCInteract = false;
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 10) //10 = enemy
        {
            //if you land on an enemy you get bounced. Doesn't work perfectly as of now
            if (!grounded)
            {
                velocity = new Vector3(velocity.x, 10f, velocity.z);
            }
        }

        //deals with crock changing forms from enemy attacks if crock is in normal form
        if (PlayerManager.Instance.currentForm == PlayerManager.PlayerForm.none && other.transform.gameObject.CompareTag("FormChanger"))
        {
            ChangeForm(other.GetComponent<FormChangerInfo>().form);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*
        if (other.gameObject.CompareTag("NPC") && PlayerManager.Instance.canMove &&
            other.gameObject.GetComponent<NPC>().canEngage && PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
        {
            buttonAlert.SetActive(true);
            buttonAlert.GetComponent<Animator>().SetBool("Active", true);

            NPCInteract = true;
            currentNPC = other.gameObject.GetComponent<NPC>();
        }
        else
        {
            if(NPCInteract)
                StartCoroutine(ButtonAlertDisappear());
            NPCInteract = false;
        }
        */
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            //Debug.Log("wow");
            //StartCoroutine(ButtonAlertDisappear());
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, groundCheckRadius);
    }

    private void LateUpdate()
    {
        //twists crocks waist if he's turning. In late update because that happens after animations
        bool normalMove = PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal;
        if (grounded && normalMove && speed > maxSpeed / 2f)
            TwistWaist();

        if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.ladder &&
           PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            FixEdgeCollision();

        //last thing in the frame is to move the character
        if(controller.enabled)
            controller.Move(velocity * Time.deltaTime);
    }

    void FixEdgeCollision()
    {
        //tries to fix unity's garbo default character controller when dealing with edges.

        //does not apply if crock is currently grounded or swimming or if he is moving upwards (jumping up usually)
        if (grounded || PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming || velocity.y >= 0)
        {
            edgeCorrectionAddition = 0;
            return;
        }

        Vector3 sphereCastOrigin = transform.position + controller.center;
        float sphereCastRadius = controller.radius + controller.skinWidth;
        float sphereCastLength = controller.center.magnitude + groundCheckRadius + 0.1f;
        Ray sphereCastRay = new Ray(sphereCastOrigin, Vector3.down);

        

        Vector3 position = sphereCastOrigin + (Vector3.down * sphereCastLength);
        /*
        Debug.DrawLine(sphereCastOrigin, position, Color.blue);
        for (int i = 0; i < 30; i++) {
            Debug.DrawLine(position,
                position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 0f), Random.Range(-1f, 1f)).normalized * sphereCastRadius,
                Color.red);
        }
        */
        RaycastHit hit;

        //checks if crock is touching ground but isn't grounded. RaycastHit gives more info than a simple sphere check
        if (Physics.SphereCast(sphereCastRay, sphereCastRadius, out hit, sphereCastLength, groundMask))
        {
            Vector3 pushDir = hit.normal + (ROOT2 * Vector3.down);
            if(controller.enabled)
                controller.Move(pushDir * (edgeCorrectionSpeed + edgeCorrectionAddition) * Time.deltaTime);

            edgeCorrectionAddition += 0.1f;
        }
    }

    void Move(float _maxSpeed, float _turnSpeed, bool canMove)
    {
        functionalMaxSpeed = Mathf.Lerp(functionalMaxSpeed, _maxSpeed, 0.1f);

        Vector3 direction = new Vector3(input.x, 0f, input.y);
        if (direction.magnitude > 1)
            direction.Normalize();

        inputVector = Quaternion.Euler(0, cam.eulerAngles.y, 0) * direction;

        //speeds up turning radius depending on speed
        //float actualTurnSpeed = _turnSpeed / speed * (Mathf.Clamp(direction.sqrMagnitude, 0.01f, 1.0f));
        //float actualTurnSpeed = Mathf.Max((speed / _maxSpeed) / _turnSpeed, 0.01f);
        //actualturnspeed needs to be an inverse number to _turnspeed due to Mathf.SmoothDampAngle. It also is multiplied by 100 to make the value in inspector smaller
        float actualTurnSpeed = (speed / _maxSpeed) / _turnSpeed;

        if (direction.magnitude >= 0.1f && canMove)
        {
            //accelerates player
            speed += accel * direction.magnitude;
            speed = Mathf.Clamp(speed, 0, functionalMaxSpeed * direction.magnitude);

            //smooths player rotation and faces the player the correct direction based off of camera position
            targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y) % 360f;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothing, actualTurnSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            //decelerates player
            float decelVal = decel * 40f * Time.deltaTime; //40 accounts for multiplying by delta time later
            
            //--------old way was to just clamp it. New way is to add value to negative speed til it equalizes at 0--------
            //speed = Mathf.Clamp(speed, 0, functionalMaxSpeed);
            if(speed <= 0 && speed > -1f * 40f * Time.deltaTime) //this just squashes speed when it gets too small
            {
                decelVal = 0;
                speed = 0;
            } else if (speed < 0)
            {
                decelVal = -decelVal;
            }

            speed -= decelVal;
        }


        Vector3 moveDir = transform.rotation * Vector3.forward * speed;

        velocity.x = moveDir.x;
        velocity.z = moveDir.z;

        //set face direction
        PlayerManager.Instance.faceDir = transform.forward;

        if(moveDir != Vector3.zero)
            SlopeCorrection();
    }

    void HurtMove()
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -velocity.y * 0.8f;
            velocity.x *= 0.7f;
            velocity.z *= 0.7f;
        }
    }

    public void SlopeCorrection()
    {
        //checks if on slope and moving, and if so, pushes the character further down to prevent bouncing when moving down a slope

        //leaves if player is in a state where this behavior is undesirable
        if (jumping)
            return;

        RaycastHit hit;
        

        //if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2f * slopeRayMultiplier, groundMask))
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, groundMask) && ungroundTimerUp)
        {
            //if player is charging, multiply the slope force by A LOT
            float correctionMult = 1f;
            if (GetComponent<Charge>().GetCharging())
                correctionMult = 10f;

            if (PlayerManager.Instance.currentForm == PlayerManager.PlayerForm.ghost)
                correctionMult = 0.02f;

            if (hit.normal != Vector3.up)
            {
                controller.Move(Vector3.down * controller.height / 2f * slopeForceMultiplier * correctionMult * Time.deltaTime);
            }
        }
    }

    void ApplyGravity()
    {
        float finalGravMult = gravityMult;
        if (velocity.y > 0)
            finalGravMult *= upGravMult;
        else if (velocity.y < 0)
            finalGravMult *= downGravMult;

        velocity += Physics.gravity * Time.deltaTime * finalGravMult;

        if (grounded && ungroundTimerUp && GetComponent<Attack>().attackDone
                                        && PlayerManager.Instance.currentState != PlayerManager.PlayerState.sliding
                                        && PlayerManager.Instance.currentState != PlayerManager.PlayerState.hurt)
            velocity.y = -1f;

        //prevents player from falling faster than terminal velocity
        if (velocity.y < terminalVelocity)
            velocity.y = terminalVelocity;

        //animation variable connection
        anim.SetFloat("YSpeed", controller.velocity.y);
    }

    void CheckGrounded()
    {
        //----------------old method of doing it with a spherecheck. This method works really well! But I wanted a raycasthit object------------------
        //grounded = ungroundTimerUp && Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        RaycastHit hit;
        bool oldGrounded = grounded;
        grounded = ungroundTimerUp &&
                   Physics.SphereCast(transform.position + controller.center, groundCheckRadius, Vector3.down, out hit,
                                      (controller.height / 2f) + .05f, groundMask);

        if(!oldGrounded && grounded)
        {
            StartCoroutine("LandingBounce");
        }

        /* Raycast to find grounded. Replaced with a spherecast to be a little more lenient
        Physics.Raycast(transform.position + controller.center, Vector3.down, out currentPolygon,
            (controller.height / 2f) + groundCheckRadius + .05f, groundMask);
        */

        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming && swim.IsPaddling())
            grounded = true;

        //if on a ladder, crock is always grounded
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.ladder)
        {
            grounded = true;
        }

        //animation variable connection
        anim.SetBool("Grounded", grounded);

        //always resets jump height and down grav mult when crock lands from a jump.
        if (jumping && grounded)
        {
            jumping = false;
            hiJumping = false;

            jumpHeight = currentFormJumpHeight;
            downGravMult = oldDownGravMult;
            upGravMult = oldUpGravMult;
            maxSpeed = oldMaxSpeed;

            //plays some footstep sounds as crock lands
            if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            {
                Footsteps footsteps = GetComponent<Footsteps>();
                footsteps.PlayFootstep(1);
                footsteps.PlayFootstep(-1);
            }
        }
    }

    public void OnCrouchListener(InputAction.CallbackContext obj)
    {
        crouchPressed = obj.performed;
    }

    void CheckCrouch()
    {
        crouching = crouchPressed && grounded && PlayerManager.Instance.currentState != PlayerManager.PlayerState.transformed;

        //keeps crock crouching if he's under a low ceiling
        floorAbove = Physics.Raycast(transform.position + (Vector3.up * controller.height), Vector3.up, (controller.height / 2f) + 0.1f);

        if (crouchingLastFrame && floorAbove)
        {
            crouching = true;
        }

        //things to do every tick when crouching
        if (crouching)
        {
            GetComponent<Idle>().StopIdleFull();
            model.up = currentPolygon.normal;
            model.forward = Vector3.Cross(transform.right, model.up);
        }
        else
        {
            model.up = Vector3.up;
            model.forward = Vector3.Cross(transform.right, Vector3.up);
        }

        //animation variable connection
        anim.SetBool("Crouching", crouching);

        //changes Player State
        if (crouching && !crouchingLastFrame)
        {
            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging)
                GetComponent<Charge>().StopCharge();
            PlayerManager.Instance.currentState = PlayerManager.PlayerState.crouching;
            decel = 0.5f;

            //makes jump height and up/down gravity larger/smaller to facilitate the backflip (highjump)
            float highJumpHeight = 5f; //these two vars used for tweaking. Jump Height is supposed to be this but will be tweaked by a different up grav
            float upGravFrac = .5f;
            downGravMult *= .4f;
            upGravMult *= upGravFrac;
            jumpHeight *= highJumpHeight * upGravFrac;

            maxSpeed *= 0.3f;

            SetControllerDimensions(crouchingCollider);

        }
        else if (!crouching && crouchingLastFrame)
        {
            PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
            decel = accel;

            SetControllerDimensions(standingCollider);

            //sets jumpheight and down grav multiplier back to normal only if crouch is exited normally and not via jump
            if (!jumping)
            {
                jumpHeight = oldJumpHeight;
                downGravMult = oldDownGravMult;
                upGravMult = oldUpGravMult;

                maxSpeed = oldMaxSpeed;
            }
        }
    }

    void CheckSwimming()
    {
        bool bottomCheck = Physics.CheckSphere(transform.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide);
        bool topCheck = Physics.CheckSphere(waterCheck.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide);
        swim.UpdateChecks(topCheck, bottomCheck);

        //enter check is higher than topcheck, used for entering the water for the first time
        Vector3 dist = waterCheck.position - transform.position;
        bool enterCheck = Physics.CheckSphere(waterCheck.position + dist + (Vector3.up * 0.3f),
            waterCheckRadius, waterMask, QueryTriggerInteraction.Collide);

        if (enterCheck)
        {
            //first time check
            if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            {
                speed = 0;
                if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging)
                    GetComponent<Charge>().StopCharge();
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.swimming;
                SetControllerDimensions(swimmingCollider);
                swim.EnterWater(transform.rotation.eulerAngles.y, velocity);
                controller.Move(waterCheck.position - transform.position - (Vector3.up * waterCheckRadius));
                anim.SetBool("Swimming", true);

                //this block finds and sets the bubblecollider water volume to the current one (water volumes should never overlap so there shouldn't be conflicts)
                //bubble collider makes bubble particles disappear when leaving the water
                Collider[] cols = Physics.OverlapSphere(transform.position, waterCheckRadius);
                foreach (Collider col in cols)
                {
                    if (col.gameObject.layer == 4) // 4 = water
                    {
                        swim.bubbleCollider.SetWaterVolume(col.gameObject);
                        break;
                    }
                }
            }//end if checks current state ISN'T swimming
        }//end if topcheck

        if(!topCheck && !bottomCheck)
        {
            if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming)
            {
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                SetControllerDimensions(standingCollider);
            }
            anim.SetBool("Swimming", false);
        }//end if topcheck, botcheck, and is swimming
    }

    void CheckSwimming2()
    {
        if(Physics.CheckSphere(waterCheck.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide) ||
            (Physics.CheckSphere(transform.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide) &&
            PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming))
        {
            //first time collision
            if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            {
                SetControllerDimensions(swimmingCollider);
                swim.EnterWater(transform.rotation.eulerAngles.y, velocity);
                controller.Move(waterCheck.position - transform.position);
            }
            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging)
                GetComponent<Charge>().StopCharge();

            PlayerManager.Instance.currentState = PlayerManager.PlayerState.swimming;
            anim.SetBool("Swimming", true);

            //this block finds and sets the bubblecollider water volume to the current one (water volumes should never overlap so there shouldn't be conflicts)
            //bubble collider makes bubble particles disappear when leaving the water
            Collider[] cols = Physics.OverlapSphere(transform.position, waterCheckRadius);
            foreach(Collider col in cols)
            {
                if(col.gameObject.layer == 4) // 4 = water
                {
                    swim.bubbleCollider.SetWaterVolume(col.gameObject);
                    break;
                }
            }

        } else if (PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
        {
            anim.SetBool("Swimming", false);
        }

        if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming && !swim.inWater)
        {
            PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
            anim.SetBool("Swimming", false);
            SetControllerDimensions(standingCollider);
        }
    }

    void CheckSliding()
    {
        //RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * controller.height * 0.5f), Vector3.down, out currentPolygon,
            controller.height + 0.1f, groundMask))
        {
            if (currentPolygon.transform.tag == "SlippySlope")
            {
                if (!slidingLastFrame)
                {
                    speed = 0;
                    GetComponent<Charge>().StopCharge();
                    
                }
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.sliding;
                sliding = true;
                controller.slopeLimit = 70f;
            }
            else
            {
                sliding = false;
                if (slidingLastFrame)
                {
                    PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                    sliding = false;
                    model.up = Vector3.up;
                    model.forward = Vector3.Cross(transform.right, model.up);
                }

                controller.slopeLimit = oldSlopeLimit;

                if (partSliding.isEmitting)
                    partSliding.Stop();
                if (slidingSoundObject.IsPlaying())
                    slidingSoundObject.Stop();
            }
        }

        anim.SetBool("Sliding", sliding);
        
    }

    void Slide()
    {
        if (grounded)
        {
            if(!partSliding.isEmitting)
                partSliding.Play();
            if(!slidingSoundObject.IsPlaying())
                slidingSoundObject.Play(transform);

            //changes crock's direction and up vector so he is flush with the ground when sliding
            model.transform.rotation = Quaternion.LookRotation(controller.velocity, currentPolygon.normal);
        }
        else if (!grounded)
        {
            if (partSliding.isEmitting)
                partSliding.Stop();
            if (slidingSoundObject.IsPlaying())
                slidingSoundObject.Stop();

            model.transform.localRotation = Quaternion.identity;
        }
        Vector3 direction = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(input.x, 0f, input.y);

        Vector3 move = Vector3.zero;
        float directionMult = 1f; //will multiply direction later so it's easier to turn
        if (grounded)
        {
            move = currentPolygon.normal;
            move.y = 0;
            //will determine how skewed from cardinal the normal is by taking magnitude
            //the higher the result, the more tilted the plane is. This is then clamped just in case
            float tilt = Mathf.Clamp(move.magnitude, 0, 1f);
            move = move.normalized * tilt;

            //attempts to make it so that you can't slide back up a slope
            float dotprod = Vector3.Dot(direction, move) + 1f;
            direction *= dotprod;
        }

        move += (direction * directionMult);

        move *= slideSpeed * 3f;
        velocity += move;

        velocity = Vector3.ClampMagnitude(velocity, slideMaxSpeed);


        speed = Mathf.Clamp(velocity.magnitude, 0, functionalMaxSpeed);

        targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothing, 0.5f);
        Quaternion newRot = Quaternion.Euler(0f, angle, 0f);
        transform.rotation = newRot;
        
        SlopeCorrection();
    }

    public void OnJumpListener(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            Jump();
            JumpAlterGrav(true);
        }
        else if (obj.canceled)
        {
            JumpAlterGrav(false);
        }
    }
    public void Jump()
    {
        if (PlayerManager.Instance.currentForm == PlayerManager.PlayerForm.ghost ||
            !PlayerManager.Instance.canMove)
            return;

        if (((PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming && grounded) ||
            (PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming && swim.IsPaddling())) &&
            !jumping)
        {
            //resets all swimming variables if jumping from swimming
            if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming)
            {
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                SetControllerDimensions(standingCollider);
            }

            //resets all ladder variables if jumping from ladder
            if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.ladder)
            {
                anim.SetBool("ClimbingLadder", false);
                GetComponent<LadderClimb>().SetRegrabTimer(.35f);
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
            }

            //sets state to normal if player is sliding
            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.sliding)
            {
                //PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
            }

            //plays the shockwave particle if crock is jumping while charging
            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging)
            {
                GetComponent<Charge>().chargeShockwave.Play();
            }

            if (crouching && speed != 0)
                return;
            if (crouching && floorAbove)
                return;

            if (crouching)
                hiJumping = true;

            GetComponent<Idle>().StopIdleFull();

            anim.SetTrigger("Jump");
            anim.SetBool("JumpFootLeft", jumpFootLeft);
            jumpFootLeft = !jumpFootLeft;
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y * gravityMult);
            jumping = true;
            jumpSound.Play(transform.position);

            ungroundTimerUp = false;
            StartCoroutine("UngroundTimer");
        }
        /*
        //variable jump height only when in normal state
        if (!GetComponent<Charge>().GetCharging() && !crouching && Input.GetAxis("Jump") > 0)
        {
            upGravMult = oldUpGravMult * .45f; //if button is held, up gravity is much less meaning the jump is much higher
        } else if (Input.GetAxis("Jump") == 0)
        {
            upGravMult = oldUpGravMult;
        }
        */
    }

    public void JumpAlterGrav(bool held)
    {
        //variable jump height only when in normal state
        if (GetComponent<Charge>().GetCharging() || crouching)
            return;

        //lowers grav if button is held. Essentially holding button = higher jump.
        float gravMult = 1f;
        if (held)
            gravMult = 0.45f;
        upGravMult = oldUpGravMult * gravMult;
    }

    //used to lean crock to one side if he's turning
    void TwistWaist()
    {
        if (Time.timeScale > 0)
        {
            //Using angle and target angle rotated vectors
            Vector3 currentDir = transform.forward;
            Vector3 targetDir = Quaternion.Euler(new Vector3(0f, (angle - targetAngle) % 360f, 0f)) * transform.forward;

            float targetAngleToLean = Vector3.SignedAngle(currentDir, targetDir, Vector3.up);

            if (Time.timeScale < 1f)
                targetAngleToLean = angleToLean;

            angleToLean = Mathf.Clamp(Mathf.Lerp(targetAngleToLean, angleToLean, .9f), -30f, 30f);
        }

        if (!PlayerManager.Instance.canMove)
        {
            angleToLean = 0;
        }
        

        waist.rotation *= Quaternion.Euler(0f, 0f, angleToLean);
    }

    public void SetControllerDimensions(Vector2 stuff)
    {
        controller.center = new Vector3(0, stuff.x, 0);
        controller.height = stuff.y;
    }

    public bool GetGrounded()
    {
        return grounded;
    }
    public bool GetJumping()
    {
        return jumping;
    }
    public bool GetHiJumping()
    {
        return hiJumping;
    }
    public bool GetNPCInteract()
    {
        return NPCInteract;
    }
    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    public void SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    //this method can be called by other scripts (like Charge) to set angle equal to target angle. This mitigates glitches with leaning
    public void SetAngleToTarget()
    {
        angle = targetAngle;
        angleToLean = 0;
    }

    //starts unground timer
    public void Unground()
    {
        ungroundTimerUp = false;
        StartCoroutine(UngroundTimer());
    }

    IEnumerator UngroundTimer()
    {
        yield return new WaitForSeconds(0.1f);
        ungroundTimerUp = true;
    }

    IEnumerator ButtonAlertDisappear()
    {
        buttonAlert.GetComponent<Animator>().SetBool("Active", false);
        yield return new WaitForSeconds(0.1f);
        buttonAlert.SetActive(false);
    }
    
    IEnumerator LandingBounce()
    {
        float timer = 0;
        float timeUp = 0.15f;

        float amount = Mathf.Clamp(-velocity.y, 0, 12) / 12f; //12 was an arbitrary vertical velocity that's roughly what crock hits when jumping on a flat plane
        while(timer < timeUp)
        {
            float xz = (Mathf.Sin((timer / timeUp) * Mathf.PI) / 4f) * amount;
            float y = (Mathf.Sin((timer / timeUp) * Mathf.PI) / 4f) * amount;
            model.transform.localScale = new Vector3(1f + xz, 1f - y, 1f + xz);
            timer += Time.deltaTime;
            yield return null;
        }

        model.transform.localScale = new Vector3(1f,1f,1f);
    }
}
