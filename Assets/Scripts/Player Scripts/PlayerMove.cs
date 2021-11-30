using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;
    public Animator anim;
    public Transform model;

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

    public float maxSpeed = 10f;
    private float functionalMaxSpeed;
    private float oldMaxSpeed;
    private float speed = 0;
    public float accel = 1f;
    private float decel;

    private Vector3 velocity;
    public float gravityMult = 0.25f;
    public float upGravMult = 0.4f;
    public float downGravMult = 0.2f;
    private float terminalVelocity = -25.0f;
    private float oldDownGravMult;
    private float oldUpGravMult;

    public float turnSpeed = 0.1f;
    private float turnSmoothing;
    private float targetAngle = 0;
    private float angle = 0;

    private bool crouching = false;
    private bool crouchingLastFrame = false;
    public bool crouchPressed = false;
    private bool floorAbove;

    public Transform waist;
    private float angleToLean;

    public float edgeCorrectionSpeed = 0.5f; //speed that the character falls off the edge
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

    public Transform waterCheck; //used for determining if the player is in deep enough water to swim
    public float waterCheckRadius;
    public LayerMask waterMask;

    public GameObject buttonAlert; //shows up when crock can do things with a button

    Form_Stone stone;

    const float ROOT2 = 1.414f;

    private void Start()
    {
        functionalMaxSpeed = maxSpeed;
        decel = accel;
        oldJumpHeight = jumpHeight;
        currentFormJumpHeight = jumpHeight;
        oldDownGravMult = downGravMult;
        oldUpGravMult = upGravMult;
        oldMaxSpeed = maxSpeed;
        oldECSpeed = edgeCorrectionSpeed;
        animController = anim.runtimeAnimatorController;

        swim = GetComponent<Swim>();
        stone = GetComponent<Form_Stone>();
    }

    void Update()
    {
        bool canMove = PlayerManager.Instance.canMove;

        CheckGrounded();
        CheckSliding();
        CheckSwimming();
        
        switch (PlayerManager.Instance.currentState)
        {
            case PlayerManager.PlayerState.normal:
                if(canMove)
                    Move(maxSpeed, turnSpeed, canMove);
                break;
            case PlayerManager.PlayerState.charging:
                GetComponent<Charge>().ChargeMove(cam);
                break;
            case PlayerManager.PlayerState.crouching:
                Move(maxSpeed, turnSpeed, canMove);
                break;
            case PlayerManager.PlayerState.sliding:
                Slide();
                break;
            case PlayerManager.PlayerState.swimming:
                velocity = Vector3.zero;
                swim.RootSwimMove();
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
        if (canMove)
        {
            Jump();
        }

        if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming &&
           PlayerManager.Instance.currentState != PlayerManager.PlayerState.ladder)
        {
            GetComponent<Attack>().AttackLogic();

            if (canMove)
            {

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

    /// <summary>
    /// if crock is transformed, this function determines what to do with a switch statement
    /// </summary>
    void TransformedSwitch(bool canMove)
    {
        switch (PlayerManager.Instance.currentForm)
        {
            case PlayerManager.PlayerForm.stone:
                Move(stone.speed, stone.turnSpeed, canMove);
                stone.stoneUpdate();
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
                break;
            case PlayerManager.PlayerForm.stone:
                PlayerManager.Instance.currentForm = PlayerManager.PlayerForm.stone;
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.transformed;
                jumpHeight = stone.jumpHeight;
                currentFormJumpHeight = stone.jumpHeight;
                GetComponent<ChangeModel>().ChangeModelTo(2);
                anim.runtimeAnimatorController = stone.animController;
                stone.stoneInit();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 10) //10 = enemy
        {
            //if you land on an enemy you get bounced. Doesn't work perfectly as of now
            if (!grounded)
            {
                //Debug.Log("boing");
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
        if (PlayerManager.Instance.canMove && other.gameObject.CompareTag("NPC"))
        {
            buttonAlert.SetActive(true);
            buttonAlert.GetComponent<Animator>().SetBool("Active", true);

            if (Input.GetButtonDown("Punch"))
            {
                other.GetComponent<NPC>().Engage();

                StartCoroutine(ButtonAlertDisappear());
            }
        }
        else
        {
            buttonAlert.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            StartCoroutine(ButtonAlertDisappear());
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(hitInfoPoint, 0.1f);
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
        controller.Move(velocity * Time.deltaTime);
    }

    void FixEdgeCollision()
    {
        //tries to fix unity's garbo default character controller when dealing with edges.

        //does not apply if crock is currently grounded or swimming or if he is moving upwards (jumping up usually)
        if (grounded || PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming || velocity.y >= 0)
            return;

        Vector3 sphereCastOrigin = transform.position + controller.center;
        float sphereCastRadius = controller.radius + controller.skinWidth;
        float sphereCastLength = controller.center.magnitude + groundCheckRadius;
        Ray sphereCastRay = new Ray(sphereCastOrigin, Vector3.down);
        RaycastHit hit;

        //checks if crock is touching ground but isn't grounded. RaycastHit gives more info than a simple sphere check
        if (Physics.SphereCast(sphereCastRay, sphereCastRadius, out hit, sphereCastLength, groundMask))
        {
            Vector3 pushDir = hit.normal + (ROOT2 * Vector3.down);
            controller.Move(pushDir * edgeCorrectionSpeed * Time.deltaTime);
        }
    }

    void Move(float _maxSpeed, float _turnSpeed, bool canMove)
    {
        functionalMaxSpeed = Mathf.Lerp(functionalMaxSpeed, _maxSpeed, 0.1f);

        //speeds up turning radius depending on speed and also if the stick movements are small
        //float actualTurnSpeed = _turnSpeed / speed

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horz, 0f, vert);
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

        //draws movement away from walls if currently over one
        /*
        if (currentPolygon.normal != Vector3.zero && currentPolygon.transform.tag == "Wall")
        {
            edgeCorrectionSpeed = 1000f;
            Vector3 move = currentPolygon.normal;
            move.y = 0;
            move.Normalize();

            velocity += move * speed * Vector3.Angle(move, moveDir) / 180f;
        } */
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

        if (jumping)
            return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2f * slopeRayMultiplier, groundMask))
        {

            if (hit.normal != Vector3.up)
            {
                //if player is charging, multiply the slope force by A LOT
                float chargeMult = 1f;
                if (GetComponent<Charge>().GetCharging())
                    chargeMult = 10f;

                controller.Move(Vector3.down * controller.height / 2f * slopeForceMultiplier * chargeMult * Time.deltaTime);
            }
        }
    }

    void ApplyGravity()
    {
        if (grounded && ungroundTimerUp && GetComponent<Attack>().attackDone
                                        && PlayerManager.Instance.currentState != PlayerManager.PlayerState.sliding
                                        && PlayerManager.Instance.currentState != PlayerManager.PlayerState.hurt)
            velocity.y = 0;

        float finalGravMult = gravityMult;
        if (velocity.y > 0)
            finalGravMult *= upGravMult;
        else if (velocity.y < 0)
            finalGravMult *= downGravMult;

        velocity += Physics.gravity * Time.deltaTime * finalGravMult;

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
        grounded = ungroundTimerUp &&
                   Physics.SphereCast(transform.position + controller.center, groundCheckRadius, Vector3.down, out hit,
                                      (controller.height / 2f) + .05f, groundMask);

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

    void CheckCrouch()
    {
        crouching = Input.GetAxis("Crouch") > 0 && grounded && PlayerManager.Instance.currentState != PlayerManager.PlayerState.transformed;

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
            model.forward = Vector3.Cross(transform.right, model.up);
        }

        //animation variable connection
        anim.SetBool("Crouching", crouching);

        //changes Player State
        if (crouching && !crouchingLastFrame)
        {
            crouchPressed = true;

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

        if (crouchPressed && Input.GetAxis("Crouch") == 0)
            crouchPressed = false;
    }

    void CheckSwimming()
    {
        if(Physics.CheckSphere(waterCheck.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide))
        {
            //first time collision
            if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
            {
                SetControllerDimensions(swimmingCollider);
                swim.EnterWater(transform.rotation.eulerAngles.y, velocity);
                controller.Move(waterCheck.position - transform.position - (Vector3.up * (waterCheckRadius - 0.12f)));
            }
            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging)
                GetComponent<Charge>().StopCharge();

            PlayerManager.Instance.currentState = PlayerManager.PlayerState.swimming;
            anim.SetBool("Swimming", true);

            //this block finds and sets the bubblecollider water volume to the current one (water volumes should never overlap so there shouldn't be conflicts)
            //bubble collider makes bubble particles disappear when leaving the water
            Collider[] cols = Physics.OverlapSphere(waterCheck.position, waterCheckRadius);
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
                PlayerManager.Instance.canMove = false;

                model.up = currentPolygon.normal;
                model.forward = Vector3.Cross(transform.right, model.up);
            }
            else
            {
                sliding = false;
                if (slidingLastFrame)
                {
                    PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                    sliding = false;
                    PlayerManager.Instance.canMove = true;
                    model.up = Vector3.up;
                    model.forward = Vector3.Cross(transform.right, model.up);
                }
            }
        }

        anim.SetBool("Sliding", sliding);
        
    }

    void Slide()
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 direction = Quaternion.Euler(0, cam.eulerAngles.y, 0) * new Vector3(horz, 0f, vert);

        Vector3 move = currentPolygon.normal;
        move.y = 0;
        move.Normalize();

        move += (direction * 1f);

        move *= slideSpeed;
        velocity += move;

        speed = Mathf.Clamp(velocity.magnitude, 0, functionalMaxSpeed);

        targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothing, 1f);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") &&
            ((PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming && grounded) ||
            (PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming && swim.IsPaddling())) &&
            !jumping)
        {
            if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming)
            {
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
                SetControllerDimensions(standingCollider);
            }

            if(PlayerManager.Instance.currentState == PlayerManager.PlayerState.ladder)
            {
                anim.SetBool("ClimbingLadder", false);
                GetComponent<LadderClimb>().SetRegrabTimer(.35f);
                PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
            }

            if (crouching && speed != 0)
                return;
            if (crouching && floorAbove)
                return;

            if (crouching)
                hiJumping = true;

            GetComponent<Idle>().StopIdleFull();

            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y * gravityMult);
            jumping = true;

            ungroundTimerUp = false;
            StartCoroutine("UngroundTimer");
        }

        //variable jump height only when in normal state
        if (!GetComponent<Charge>().GetCharging() && !crouching && Input.GetAxis("Jump") > 0)
        {
            upGravMult = oldUpGravMult * .45f; //if button is held, up gravity is much less meaning the jump is much higher
        } else if (Input.GetAxis("Jump") == 0)
        {
            upGravMult = oldUpGravMult;
        }
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

    void SetControllerDimensions(Vector2 stuff)
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
}
