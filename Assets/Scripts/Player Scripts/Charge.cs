﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public float chargeSpeed;
    public float turnSpeed = 1f;
    public float chargeLength = 1f;
    private float turnSmoothing;
    private float targetAngle = 0;

    private CharacterController controller;
    private Animator anim;
    private PlayerMove playerMove;
    private float oldMaxSpeed;

    private float chargeTimer = 0f;

    private bool charging = false; //keeps track of if the player is currently charging
    private bool pressed = false; //keeps track if the button was already pressed this charge (prevents holding the button)

    public TrailRenderer trail;

    private float oldJumpHeight; //used to store the old jump height

    public Transform chargeCheck;
    public float chargeCheckRadius;
    public LayerMask chargeCheckMask;

    public PlaySound chargeSound;
    public GameObject chargeEffect;
    public ParticleSystem chargeShockwave;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerMove = GetComponent<PlayerMove>();

        oldMaxSpeed = playerMove.maxSpeed;
        oldJumpHeight = playerMove.jumpHeight;
    }

    private void Update()
    {
        anim.SetBool("Charging", charging);

        if(playerMove.GetGrounded() && Input.GetAxis("Charge") > 0 && !charging && !pressed && PlayerManager.Instance.canMove &&
            PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
        {
            StartCharge();
        }
        //counts down and eventually ends the charge after an amount of time
        if(charging)
        {
            if(playerMove.GetGrounded() && !playerMove.GetJumping())
                chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0 && playerMove.GetGrounded())
            {
                StopCharge();
            }
                
        }

        if(Input.GetAxis("Charge") == 0)
        {
            pressed = false;
        }

        chargeEffect.SetActive(playerMove.GetGrounded() && charging);
    }

    private void LateUpdate()
    {
        if (charging)
        {
            if (CheckCollisions())
            {
                Vector3 newV = (-transform.forward + (transform.up * 10f)).normalized * 10f;
                playerMove.SetSpeed(-12f);
                playerMove.SetVelocity(newV);
                StopCharge();
                StartCoroutine("Bounce");
                return;
            }
        }
    }

    public Vector3 ChargeMove(Transform cam)
    {
        float angle = 0f;

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horz, 0f, vert);

        if(direction != Vector3.zero)
        {
            //smooths player rotation and faces the player the correct direction based off of camera position
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothing, turnSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        }
        //breaks free if crock collides with anything 
        /*
        if (CheckCollisions())
        {
            Vector3 newV = (-transform.forward + (transform.up * 10f)).normalized * 10f;
            playerMove.SetSpeed(-12f);
            playerMove.SetVelocity(newV);
            StopCharge();
            StartCoroutine("Bounce");
            return;
        }*/

        Vector3 velocity = transform.forward * chargeSpeed;
        playerMove.SetSpeed(chargeSpeed);
        //playerMove.SetSpeed(chargeSpeed); //done so playerMove knows how fast crock is going incase charge is interrupted

        //set face direction
        PlayerManager.Instance.faceDir = transform.forward;

        playerMove.SetAngleToTarget();

        playerMove.SlopeCorrection();

        return velocity;
    }

    void StartCharge()
    {
        HurtBoxInfo.ToggleHurtBox(GetComponent<Attack>().ChargeCollider, true);

        pressed = true;
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.charging;
        chargeTimer = chargeLength;
        charging = true;
        trail.enabled = true;

        oldJumpHeight = playerMove.jumpHeight;
        playerMove.jumpHeight += 1.9f;

        oldMaxSpeed = playerMove.maxSpeed;
        playerMove.maxSpeed = chargeSpeed;

        chargeSound.Play(transform);
        chargeShockwave.Play();
    }
    public void StopCharge()
    {
        HurtBoxInfo.ToggleHurtBox(GetComponent<Attack>().ChargeCollider, false);

        charging = false;
        anim.SetBool("Charging", false);
        trail.enabled = false;
        trail.Clear();
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;

        playerMove.jumpHeight = oldJumpHeight;

        playerMove.maxSpeed = oldMaxSpeed;
    }

    //checks any sideways collisions to bounce back
    bool CheckCollisions()
    {
        /* debug
        for(int i = 0; i < 30; i++)
        {
            Vector3 newDir = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)) * Vector3.up;
            newDir *= chargeCheckRadius;
            Debug.DrawLine(chargeCheck.position, chargeCheck.position + newDir, Color.red);
        }*/
        return Physics.CheckSphere(chargeCheck.position, chargeCheckRadius, chargeCheckMask, QueryTriggerInteraction.Ignore);
    }

    //this coroutine merely times out a charge if it goes uninterrupted
    IEnumerator chargeTimesUp()
    {
        while(chargeTimer > 0)
        {
            chargeTimer -= Time.deltaTime;
            yield return null;
        }
        StopCharge();
    }

    public bool GetCharging()
    {
        return charging;
    }

    IEnumerator Bounce()
    {
        GetComponent<PlayerMove>().Unground();
        anim.SetTrigger("Bounce");
        for(float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            yield return null;
        }
        anim.ResetTrigger("Bounce");
    }
}
