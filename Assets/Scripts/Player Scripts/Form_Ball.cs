using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Form_Ball : MonoBehaviour
{
    public bool grounded;
    public bool speedLimit; //whether or not to apply a top speed
    public float jumpForce;//300
    public float speedMult;//30 //what the forces will be multiplied by before being added to rigidbody
    public float maxSpeed;//4
    public float masterGravMult; //how much gravity will be multiplied by when pulling this object down
    private float internalGravMult = 1f; //used in conjunction with jumping to affect gravity

    public LayerMask groundLayer;
    public float groundCheckRadius;

    private bool jumpPressed;

    private Transform cameraTrans; //the transform of the camera. Used to get forwards and rights

    new Rigidbody rigidbody;

    public AudioSource rollingSound;

    // Start is called before the first frame update
    void Start()
    {
        cameraTrans = CameraManager.Instance.sceneCam.gameObject.transform;
        rigidbody = GetComponent<Rigidbody>();
        jumpPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        SoundStuff();
    }

    private void FixedUpdate()
    {
        if (PlayerManager.Instance.canMove)
            Move();

        rigidbody.useGravity = false;
        rigidbody.AddForce(Physics.gravity * masterGravMult * internalGravMult * rigidbody.mass);
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speedMult;

        Vector3 previousVelocity = rigidbody.velocity;

        moveDirection = Quaternion.Euler(0, cameraTrans.rotation.eulerAngles.y, 0) * moveDirection;
        float groundedMult = 1f;
        if (!grounded)
            groundedMult = 0.2f;

        rigidbody.AddForce(moveDirection * groundedMult);

        //clamps to max speed
        if (speedLimit && rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);

        //slows down rigidbody if no input is being made <----------THIS IS DEALT WITH USING RIGIDBODY DRAG VALUES
        //if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0 && grounded)

        //will preserve the y velocity so it isn't affected by the clamping/slowdown
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, previousVelocity.y, rigidbody.velocity.z);

        //jump
        if(grounded && !jumpPressed && Input.GetAxis("Jump") > 0)
        {
            jumpPressed = true;
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }

        if (jumpPressed && Input.GetAxis("Jump") == 0)
            jumpPressed = false;

        if (jumpPressed && rigidbody.velocity.y > 0)
            internalGravMult = 0.5f;
        else
            internalGravMult = 1f;
    }

    void CheckGround()
    {
        Vector3 groundCheck = transform.position - (Vector3.up * 0.65f);
        if(Physics.CheckSphere(groundCheck, groundCheckRadius, groundLayer))
        {
            grounded = true;
        }  
        else
            grounded = false;
    }

    void SoundStuff()
    {
        float mag = rigidbody.velocity.magnitude;
        if (mag > 0.1f && grounded)
        {
            if (!rollingSound.isPlaying)
                rollingSound.Play();
            float fraction = 1f - ((maxSpeed - mag) / maxSpeed);
            float bounds = 0.3f;
            rollingSound.volume = fraction * SoundManager.Instance.soundEffectVolume;
            rollingSound.pitch = 1f - bounds * (fraction * bounds);
        }else
        {
            rollingSound.Pause();
        }
    }
}
