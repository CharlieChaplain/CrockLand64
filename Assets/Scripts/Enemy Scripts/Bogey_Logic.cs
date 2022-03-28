using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bogey_Logic : Enemy
{
    float hurtTimer = 0;
    public bool attacking = false;

    Enemy_Pursue pursue;
    Enemy_Wander wander;

    public float turnSpeed;
    public float accel;

    public GameObject attackHurtBox;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        pursue = GetComponent<Enemy_Pursue>();
        wander = GetComponent<Enemy_Wander>();

        pursue.target = PlayerManager.Instance.player;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (hurtTimer > 0)
            hurtTimer -= Time.deltaTime;
        else if (attacking)
            AttackMovement();
        else
        {
            Move();
            AttackLogic();
        }

    }

    private void Move()
    {
        if (!felled)
        {
            // LOGIC TO DECIDE MOVEMENT DIRECTION, REPLACE WITH RELEVANT LOGIC ----------------------------------------
            Vector3 intendedDir = Vector3.zero;
            Vector3 XYDir = Vector3.zero; ;
            if ((pursue.target.transform.position - transform.position).magnitude < pursue.chaseRadius)
            {
                intendedDir = pursue.Pursue();
                velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel);
                XYDir = velocity;
                velocity *= pursue.speed;
            }
            else
            {
                intendedDir = wander.Wander();
                if (intendedDir == Vector3.zero)
                    velocity = intendedDir;
                else
                {
                    velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel);
                    XYDir = velocity;
                    velocity *= wander.speed;
                }
            }
            XYDir.y = 0;
            //prevents direction confusion if 
            if (XYDir != Vector3.zero)
            {
                transform.forward = XYDir;
            }
            // END MOVEMENT LOGIC -------------------------------------------------------------------------------------
        }

        //final move of character controller
        controller.Move(velocity * Time.deltaTime);
    }

    void AttackLogic()
    {
        if (felled)
            return;

        Vector3 dir = pursue.target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle < 20f && dir.magnitude < 2.2f)
        {
            attacking = true;
            anim.SetTrigger("Attack");
        }
    }

    void AttackMovement()
    {
        Vector3 faceDir = Vector3.RotateTowards(transform.forward, pursue.Pursue(), turnSpeed * 1.5f, accel);

        faceDir.y = 0;
        //prevents direction confusion if 
        if (faceDir != Vector3.zero)
        {
            transform.forward = faceDir;
        }
    }

    public void ToggleHurtBox(bool toggle)
    {
        HurtBoxInfo.ToggleHurtBox(attackHurtBox, toggle);
    }
}
