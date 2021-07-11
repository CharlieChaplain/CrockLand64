using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy_Logic : Enemy
{
    public Transform bodyPos;

    private Camera cam;
    float tolerance = 50f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Move();

        if(felled && !IsInView())
        {
            felled = false;
            health = maxHealth;
            anim.SetTrigger("StandUp");
        }
    }

    void Move()
    {
        CheckGrounded();

        if (grounded)
            velocity = Vector3.zero;

        if (!carried)
        {
            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int layerNumber = 9; //layer 9 = player
        if (other.gameObject.layer == layerNumber && other.CompareTag("HurtBox"))
        {
            PlaySound hurtSound = null;
            if (other.gameObject.layer == 9) //layer 9 = player
                hurtSound = PlayerManager.Instance.currentHitSound;

            Hurt(other.GetComponent<HurtBoxInfo>().damage, other.transform.position, hurtSound);

            if (health <= 0)
            {
                anim.SetTrigger("Fall");
                felled = true;
            } else
            {
                anim.SetTrigger("Hit");

                StopCoroutine("wobbleScale");
                StartCoroutine("wobbleScale");
            }
        }
    }

    public void MovePos()
    {
        transform.position = bodyPos.position;
    }

    bool IsInView()
    {
        Vector3 pos = cam.WorldToScreenPoint(transform.position);
        if((pos.x >= 0 - tolerance && pos.x <= Screen.width + tolerance) && (pos.y >= 0 - tolerance && pos.y <= Screen.height + tolerance))
        {
            return true;
        }
        return false;

    }
}
