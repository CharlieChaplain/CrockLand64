﻿using System.Collections;
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

    private Swim swim;

    private bool grounded;
    private bool ungroundTimerUp = true; //short timer that prevents player from being grounded again immediately after jumping
    private bool jumping = false;
    private bool hiJumping = false;
    private RaycastHit currentPolygon;

    public float jumpHeight;
    private float oldJumpHeight;

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

    Vector3 hitInfoPoint;

    private void Start()
    {
        functionalMaxSpeed = maxSpeed;
        decel = accel;
        oldJumpHeight = jumpHeight;
        oldDownGravMult = downGravMult;
        oldUpGravMult = upGravMult;
        oldMaxSpeed = maxSpeed;
        oldECSpeed = edgeCorrectionSpeed;

        swim = GetComponent<Swim>();
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
            default:
                break;

        }
        if (canMove)
        {
            Jump();
        }

        if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
        {
            GetComponent<Attack>().AttackLogic();

            if (canMove)
            {

                CheckCrouch();

                anim.SetFloat("Speed", speed);
            }

            ApplyGravity();

            if (speed > 0)
                GetComponent<Idle>().StopIdle();


            slidingLastFrame = sliding;
            crouchingLastFrame = crouching;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 10) //10 = enemy
        {
            //if you land on an enemy you get bounced. Doesn't work as of now
            if (!grounded)
            {
                Debug.Log("boing");
                velocity = new Vector3(velocity.x, 10f, velocity.z);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(hitInfoPoint, 0.1f);
    }

    private void LateUpdate()
    {
        if(PlayerManager.Instance.currentState != PlayerManager.PlayerState.swimming)
        {
            bool normalMove = PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal;
            if (grounded && normalMove && speed > maxSpeed / 2f)
                TwistWaist();

            //tries to fix unity's garbo default character controller when dealing with edges. Still needs some work
            Vector3 sphereCenter = transform.position + (Vector3.up * (controller.radius - controller.skinWidth));
            float sphereRadius = controller.radius - controller.skinWidth + 0.1f;
            //Collider[] cols = Physics.OverlapSphere(sphereCenter, sphereRadius, groundMask);
            Collider[] cols = Physics.OverlapSphere(sphereCenter, sphereRadius);
            if (!grounded && cols.Length != 0)
            {
                // THIS ASSUMES THE FIRST HIT COLLIDER IS THE ONE YOU WANT TO SLIDE OFF OF
                Vector3 relativeHitPoint = transform.position - cols[0].ClosestPoint(sphereCenter);

                hitInfoPoint = cols[0].ClosestPoint(sphereCenter);

                if (relativeHitPoint.magnitude > noSlipDistance)
                {
                    Vector3 edgeFallMovement = relativeHitPoint.normalized;
                    edgeFallMovement.y = 0;
                    Vector3 raycastStart = transform.position + (edgeFallMovement * 1f);
                    Vector3 newPos = edgeFallMovement;

                    hitInfoPoint = raycastStart;

                    RaycastHit hit;
                    if(Physics.Raycast(raycastStart, Vector3.down, out hit, 5f, groundMask))
                    {
                        if (hit.transform.gameObject.CompareTag("Wall"))
                        {
                            newPos = hit.point - transform.position;
                        }
                    }

                    controller.Move(newPos * edgeCorrectionSpeed * Time.deltaTime);


                    if (cols[0].transform.gameObject.CompareTag("Wall"))
                    {
                        controller.Move(newPos * edgeCorrectionSpeed * Time.deltaTime);

                        //calcs dot product between edgefallmove and velocity, if velocity is up against the wall, it's stronger
                        //float dotProduct = Mathf.Max(-Vector3.Dot(velocity.normalized, edgeFallMovement), 0);
                        float dotProduct = Mathf.Max(-Vector3.Dot(inputVector, edgeFallMovement), 0);

                        Vector3 test = velocity;
                        test.y = 0;

                        Vector3 dampenVelocity = edgeFallMovement * dotProduct * inputVector.magnitude * velDampenMult;

                        //velocity += (dampenVelocity);
                    }
                }
            }
        }

        //last thing in the frame is to move the character
        controller.Move(velocity * Time.deltaTime);
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

        //speeds up turning radius depending on speed and also if the stick movements are small
        //float actualTurnSpeed = _turnSpeed / speed * (Mathf.Clamp(direction.sqrMagnitude, 0.01f, 1.0f));
        float actualTurnSpeed = Mathf.Max(_turnSpeed * (speed / _maxSpeed), 0.01f);

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
            float decelVal = decel * .6f;
            
            //--------old way was to just clamp it. New way is to add value to negative speed til it equalizes at 0--------
            //speed = Mathf.Clamp(speed, 0, functionalMaxSpeed);
            if(speed <= 0 && speed > -1f) //this just squashes speed when it gets too small
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
        if (grounded && ungroundTimerUp && PlayerManager.Instance.currentState != PlayerManager.PlayerState.sliding)
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

        //animation variable connection
        anim.SetBool("Grounded", grounded);

        //always resets jump height and down grav mult when crock lands from a jump.
        if (jumping && grounded)
        {
            jumping = false;
            hiJumping = false;

            jumpHeight = oldJumpHeight;
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
        if (grounded)
        {
            //edgeCorrectionSpeed = oldECSpeed;
        }
    }

    void CheckCrouch()
    {
        crouching = Input.GetAxis("Crouch") > 0 && grounded;

        //keeps crock crouching if he's under a low ceiling
        floorAbove = Physics.Raycast(transform.position + (Vector3.up * controller.height), Vector3.up, (controller.height / 2f) + 0.1f);

        if (crouchingLastFrame && floorAbove)
        {
            crouching = true;
        }

        //things to do every tick when crouching
        if (crouching)
        {
            GetComponent<Idle>().StopIdle();
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

            if (crouching && speed != 0)
                return;
            if (crouching && floorAbove)
                return;

            if (crouching)
                hiJumping = true;

            GetComponent<Idle>().StopIdle();

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
        //Using angle and target angle rotated vectors
        Vector3 currentDir = transform.forward;
        Vector3 targetDir = Quaternion.Euler(new Vector3(0f, (angle - targetAngle) % 360f, 0f)) * transform.forward;

        float targetAngleToLean = Vector3.SignedAngle(currentDir, targetDir, Vector3.up);

        if (Time.timeScale < 1f)
                targetAngleToLean = angleToLean;

        angleToLean = Mathf.Clamp(Mathf.Lerp(targetAngleToLean, angleToLean, .9f), -30f, 30f);

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

    //this method can be called by other scripts (like Charge) to set angle equal to target angle. This mitigates glitches with leaning
    public void SetAngleToTarget()
    {
        angle = targetAngle;
        angleToLean = 0;
    }

    IEnumerator UngroundTimer()
    {
        yield return new WaitForSeconds(0.1f);
        ungroundTimerUp = true;
    }
}