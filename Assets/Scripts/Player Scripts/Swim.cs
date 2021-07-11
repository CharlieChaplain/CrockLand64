using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swim : MonoBehaviour
{
    Transform cam; //camera

    Vector3 targetVelocity;
    Vector3 velocity;

    private Vector3 targetAngleEulers;
    private Vector3 currentAngleEulers;

    public bool inWater;
    bool underwater;

    public Transform waterCheck;
    public float waterCheckRadius;
    public LayerMask waterMask;

    CharacterController controller;
    Animator anim;

    public float timeToTurn = 0.1f; //time for character to arrive at target angle
    public float turnSpeed; //how quickly angles change
    Vector3 timeToTurnRef;
    float strokeTimer;
    public float strokeSpeed;
    bool stroked = false;
    bool paddled = false;

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

    private enum WaterStates
    {
        paddling,
        diving
    }
    private WaterStates currentWaterState = WaterStates.paddling;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        cam = GetComponent<PlayerMove>().cam;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUnderwater();
    }
    public void EnterWater(float currentYRotation, Vector3 _velocity)
    {
        //currentAngleEulers = targetAngleEulers = Vector3.zero;
        //currentAngleEulers.y = currentYRotation;
        //targetAngleEulers.y = currentYRotation;

        currentWaterState = WaterStates.paddling;

        velocity = (_velocity * 0.5f) + (transform.forward * 2f);

        transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

        //large splash if crock is moving downwards quickly
        if(velocity.y < -2f)
        {
            enterWater.Play(transform.position);
            partBubblesBurst.Play();

            Vector3 splooshPos = transform.position;
            RaycastHit hit;

            //creates ray and then reverses the direction so that it'll see the top of the water from the outside
            Ray splooshRay = new Ray(transform.position, Vector3.up);
            splooshRay.origin = splooshRay.GetPoint(5f);
            splooshRay.direction *= -1f;

            if(Physics.Raycast(splooshRay, out hit, 5f, waterMask, QueryTriggerInteraction.Collide))
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
            partWake.Play();
        }
    }

    public void RootSwimMove()
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

        controller.Move(velocity * Time.deltaTime);
    }

    void SwimRotate()
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        //--------------INVERT VERT FOR INVERTED Y CONTROLS--------------
        Vector3 direction = new Vector3(vert, horz, 0);
        if (direction.magnitude > 1)
            direction.Normalize();

        targetAngleEulers += direction * turnSpeed;
        targetAngleEulers.x = Mathf.Clamp(targetAngleEulers.x, -90f, 90f);

        currentAngleEulers = Vector3.SmoothDamp(currentAngleEulers, targetAngleEulers, ref timeToTurnRef, timeToTurn);
        currentAngleEulers.x = Mathf.Clamp(currentAngleEulers.x, -90f, 90f);

        transform.rotation = Quaternion.Euler(currentAngleEulers);
    }

    void SwimMove()
    {
        float paddle = Input.GetAxis("Punch");
        float stroke = Input.GetAxis("Jump");

        if (stroke > 0 && strokeTimer <= 0 && !stroked)
        {
            //set regular velocity to stroke speed
            velocity = transform.forward * strokeSpeed;

            anim.SetTrigger("Stroke");
            underwaterSwoosh.Play(transform.position);

            stroked = true;
            strokeTimer = 1f;

            partBubblesBurst.Stop();
            partBubblesBurst.Play();
        }
        else if (paddle > 0)
        {
            //set targetvelocity to paddle speed
            targetVelocity = transform.forward * paddleSpeed;
            paddled = true;
        }
        else
        {
            //set targetveloctiy to zero
            targetVelocity = Vector3.zero;
        }

        if (stroke == 0)
        {
            stroked = false;
        }

        //this line makes crock always swim in his forward direction while diving. Gives more control over direction.
        velocity = transform.forward * velocity.magnitude;

        if (strokeTimer > 0)
            strokeTimer -= Time.deltaTime;
    }
    void Paddle(Transform cam)
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horz, 0f, vert);
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

        if (underwater)
            velocity.y += .2f;

        if (Input.GetAxis("Punch") == 0)
        {
            paddled = false;
        }

        if(velocity == Vector3.zero)
        {
            //emits less particles if staying still
            var partWakeEmission = partWake.emission;
            partWakeEmission.rateOverTime = 1f;
        }
        else
        {
            var partWakeEmission = partWake.emission;
            partWakeEmission.rateOverTime = 3f;

            transform.forward = velocity;
        }

        //enter dive state
        if (Input.GetAxis("Punch") > 0 && !paddled)
        {
            currentWaterState = WaterStates.diving;
            //StartCoroutine("DiveCo");
            Dive();
        }
    }

    void CheckUnderwater()
    {
        //initial check for being in water. This sphere is at the player origin
        if (Physics.CheckSphere(waterCheck.position, waterCheckRadius, waterMask, QueryTriggerInteraction.Collide))
        {
            //--------------------IN WATER------------------
            inWater = true;


            if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.swimming)
            {
                //secondary check to see if player is at the top of the water. This sphere is above the previous sphere
                if (!Physics.CheckSphere(waterCheck.position + (Vector3.up * 2f * waterCheckRadius) + (Vector3.up * 0f),
                            waterCheckRadius, waterMask, QueryTriggerInteraction.Collide))
                {
                    if (underwater && currentWaterState == WaterStates.diving)
                    {
                        currentWaterState = WaterStates.paddling;
                        //StartCoroutine("SurfaceCo");
                        Surface();
                    }

                    velocity.y = 0;

                    if (underwater == true)
                    {
                        partWake.Play();
                        partBubbles.Stop();
                    }

                    underwater = false;
                }
                else
                {
                    if(underwater == false)
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
    }

    public bool IsPaddling()
    {
        return !underwater;
    }

    void Dive()
    {
        anim.SetTrigger("Stroke");
        underwaterSwoosh.Play(transform.position);

        Vector3 direction = Quaternion.Euler(-30f, 0, 0) * transform.forward;
        velocity = strokeSpeed * direction;
        targetVelocity = velocity;

        transform.forward = velocity;

        //currentAngleEulers.y = transform.rotation.eulerAngles.y;
        //targetAngleEulers.y = transform.rotation.eulerAngles.y;

        currentAngleEulers = targetAngleEulers = transform.rotation.eulerAngles;
    }
    IEnumerator DiveCo()
    {
        PlayerManager.Instance.canMove = false;


        //Quaternion targetRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(30f, 0, 0) * transform.rotation;

        float rotateStep = 150f * Time.deltaTime;

        anim.SetTrigger("Stroke");


        for (float f = 0; f < 0.6f; f += Time.deltaTime)
        {
            yield return null;
            controller.Move(targetRotation * Vector3.forward * strokeSpeed * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateStep);
        }

        currentAngleEulers = transform.rotation.eulerAngles;
        targetAngleEulers = currentAngleEulers;

        PlayerManager.Instance.canMove = true;
    }

    void Surface()
    {
        anim.SetTrigger("Stroke");
        //underwaterSwoosh.Play(transform.position);
        surface.Play(transform.position);

        transform.rotation = Quaternion.Euler(0, currentAngleEulers.y, 0);

        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();
        targetVelocity = velocity = direction * paddleSpeed;

        //transform.forward = velocity;

        //Debug.Break();
    }

    IEnumerator SurfaceCo()
    {
        Debug.Log("hello");

        PlayerManager.Instance.canMove = false;

        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        float rotateStep = 250f * Time.deltaTime;

        anim.SetTrigger("Stroke");

        while (transform.rotation.eulerAngles.x > 0.01f || transform.rotation.eulerAngles.x < -0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateStep);
            yield return null;
        }

        velocity = Vector3.zero;

        PlayerManager.Instance.canMove = true;
    }
}
