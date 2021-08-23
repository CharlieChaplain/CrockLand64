using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public LayerMask ladderMask;
    public float climbSpeed;
    public float climbAccel;

    float regrabTimer = 0; //a timer that counts down after leaving a ladder to prevent immediate regrabbing
    CharacterController controller;
    Animator anim;

    Vector3 velocity;

    GameObject currentLadder;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (regrabTimer > 0)
            regrabTimer -= Time.deltaTime;
    }

    public void LadderMove()
    {
        RaycastHit hit;
        Vector3 rayDir = currentLadder.transform.position - transform.position;
        rayDir.y = 0;
        Ray ladderRay = new Ray(transform.position, rayDir);
        Debug.DrawRay(transform.position, rayDir, Color.red, 5f);
        Vector3 newForward = transform.forward;

        if (Physics.Raycast(ladderRay, out hit, 5f, ladderMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("LADDER");
            newForward = -hit.normal;
        }
        transform.forward = newForward;

        if (Input.GetAxis("Vertical") != 0)
            velocity += transform.up * Input.GetAxis("Vertical") * climbAccel;
        else if (velocity.magnitude > 0.2f)
            velocity /= climbAccel;
        else
            velocity = Vector3.zero;

        velocity = Vector3.ClampMagnitude(velocity, climbSpeed);

        controller.Move(velocity * Time.deltaTime);

        anim.SetFloat("YSpeed", velocity.y);
    }
    void EndClimb(Vector3 exitVelocity)
    {
        anim.SetBool("ClimbingLadder", false);
        anim.SetTrigger("Jump");
        GetComponent<PlayerMove>().SetVelocity(exitVelocity);
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;
        regrabTimer = .35f;
    }

    public void SetRegrabTimer(float time)
    {
        regrabTimer = time;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 13 && //13 = ladder
           regrabTimer <= 0 &&
           (PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal ||
           PlayerManager.Instance.currentState == PlayerManager.PlayerState.charging))
        {
            currentLadder = other.gameObject;

            PlayerManager.Instance.currentState = PlayerManager.PlayerState.ladder;

            velocity = Vector3.zero;

            RaycastHit hit;
            Vector3 rayDir = currentLadder.transform.position - transform.position;
            rayDir.y = 0;
            Ray ladderRay = new Ray(transform.position, rayDir);
            if (Physics.Raycast(ladderRay, out hit, 5f, ladderMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 newPos = other.transform.position + hit.normal * (controller.radius + controller.skinWidth) * 1.6f;
                newPos.y = transform.position.y;

                controller.enabled = false;
                Debug.Log(newPos);
                transform.position = newPos;
                controller.enabled = true;
            }

            anim.SetBool("ClimbingLadder", true);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.ladder)
        {
            if (other.tag == "LadderTop")
            {
                Debug.Log("top");

                EndClimb(Vector3.up * 20f);
            }
            else if (velocity.y < 0 && other.tag == "LadderBot")
            {
                Debug.Log("bottom");
                transform.forward = -transform.forward;
                EndClimb(transform.forward * 10f);
            }
        }
    }
}
