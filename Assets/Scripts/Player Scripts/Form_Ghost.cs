using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Form_Ghost : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float maxVertSpeed;
    public RuntimeAnimatorController animController;
    public ParticleSystem parts;

    public float riseSpeed;

    float flyTimer = 0;
    bool grounded;
    PlayerMove playerMove;

    public void GhostInit(PlayerMove pm)
    {
        flyTimer = 0;
        playerMove = pm;
    }

    public void GhostUpdate(bool _grounded)
    {
        grounded = _grounded;
    }

    public Vector3 VerticalMovement()
    {
        Vector3 movement = Vector3.zero;

        //only allows crock to fly for some number of seconds
        if (Input.GetButton("Jump") && flyTimer <= 3f)
        {
            if (grounded)
                playerMove.Unground();
            movement += Vector3.up * riseSpeed;
            flyTimer += Time.deltaTime;
        }

        //resets flytimer if button is unpressed and crock is grounded
        if(grounded && !Input.GetButton("Jump"))
        {
            flyTimer = 0;
        }
        return movement;
    }
}
