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
    }

    public void ChargeMove(Transform cam)
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
        controller.Move(transform.forward * chargeSpeed * Time.deltaTime);
        playerMove.SetSpeed(chargeSpeed); //done so playerMove knows how fast crock is going incase charge is interrupted

        //set face direction
        PlayerManager.Instance.faceDir = transform.forward;

        playerMove.SetAngleToTarget();

        if (CheckCollisions())
        {
            Vector3 newV = (-transform.forward + (transform.up * 10f)).normalized * 10f;
            playerMove.SetSpeed(-12f);
            playerMove.SetVelocity(newV);
            StopCharge();
            StartCoroutine("Bounce");
        }

        playerMove.SlopeCorrection();
    }

    void StartCharge()
    {
        pressed = true;
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.charging;
        chargeTimer = chargeLength;
        charging = true;
        trail.enabled = true;

        oldJumpHeight = playerMove.jumpHeight;
        playerMove.jumpHeight += 1.9f;

        oldMaxSpeed = playerMove.maxSpeed;
        playerMove.maxSpeed = chargeSpeed;
    }
    public void StopCharge()
    {
        charging = false;
        trail.enabled = false;
        trail.Clear();
        PlayerManager.Instance.currentState = PlayerManager.PlayerState.normal;

        playerMove.jumpHeight = oldJumpHeight;

        playerMove.maxSpeed = oldMaxSpeed;
    }

    //checks any sideways collisions to bounce back
    bool CheckCollisions()
    {
        return Physics.CheckSphere(chargeCheck.position, chargeCheckRadius, chargeCheckMask);
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
        anim.SetTrigger("Bounce");
        PlayerManager.Instance.canMove = false;
        while (!playerMove.GetGrounded())
            yield return null;
        PlayerManager.Instance.canMove = true;
    }
}