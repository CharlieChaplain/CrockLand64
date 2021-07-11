using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key_Logic : Enemy
{
    GameObject crock;

    Vector3 intendedDir;

    Vector3 home;
    public float homeRadius;

    public float speed;
    float initSpeed;
    public float turnSpeed;
    public float accel;

    float wanderTimer;

    public float fleeRadius;

    public bool fleeing = false;

    float hurtTimer = 0;
    float standupTimer = 0;

    public GameObject headTrigger;

    public enum TreasureColor
    {
        red,
        blue,
        green,
        yellow,
        purple
    };

    public TreasureColor keyColor;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        crock = GameObject.Find("Crock");
        wanderTimer = Random.Range(1f, 3f);
        controller = GetComponent<CharacterController>();
        home = transform.position;

        initSpeed = speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (hurtTimer > 0)
            hurtTimer -= Time.deltaTime;
        else
            Move();

        anim.SetBool("Carried", carried);

        StandUp();
    }

    void Move()
    {
        if (!carried)
        {
            headTrigger.SetActive(true);

            if (!felled && !thrown)
            {
                if ((transform.position - crock.transform.position).magnitude < fleeRadius)
                {
                    fleeing = true;
                    Flee();
                }
                else
                {
                    fleeing = false;
                    Wander();
                }

                velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel) * speed;
                //velocity = Vector3.Lerp(velocity, intendedDir, turnSpeed * Time.deltaTime);

                Vector3 XYDir = velocity;
                XYDir.y = 0;

                if (XYDir != Vector3.zero)
                {
                    transform.forward = XYDir;
                }
                anim.SetFloat("Speed", XYDir.magnitude);
            }

            CheckGrounded();
            if (grounded)
            {
                velocity.y = 0;
                if (thrown)
                {
                    home = transform.position;
                    thrown = false;
                }

                if (felled)
                    velocity = Vector3.zero;
            }

            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            headTrigger.SetActive(false);
        }
    }

    void Wander()
    {
        speed = initSpeed;

        wanderTimer -= Time.deltaTime;

        if(wanderTimer < 0)
        {
            wanderTimer = Random.Range(1f, 3f);
            float decision = Random.value;

            if((home - transform.position).magnitude > homeRadius)
            {
                intendedDir = home - transform.position;
            }
            else if (decision < 0.5f)
            {
                intendedDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            } else
            {
                intendedDir = Vector3.zero;
            }
        }

        if(intendedDir != Vector3.zero)
            intendedDir.Normalize();
    }

    void Flee()
    {
        speed = initSpeed * 2f;

        //will find a direction to run that is away from Crock. If they approach the edge of their home they'll desire to run back home more
        Vector3 toHomeDir = home - transform.position;
        float ratio = toHomeDir.magnitude / homeRadius;
        Vector3 fleeDir = transform.position - crock.transform.position;

        intendedDir = Vector3.Lerp(fleeDir, toHomeDir, ratio);

        intendedDir.Normalize();
    }

    void StandUp()
    {
        if (carried)
        {
            standupTimer = 0;
        }

        if(felled && !carried)
        {
            standupTimer += Time.deltaTime;

            if (standupTimer >= 4f && standupTimer < 5f)
                anim.SetFloat("StandUp", 2f);
            else if (standupTimer >= 5f)
            {
                anim.SetFloat("StandUp", 3f);
                health = maxHealth;
            }
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
                standupTimer = 0;
                anim.SetFloat("StandUp", 1f);
            }
            else
            {
                anim.SetTrigger("Hit");
                hurtTimer = 1f;
                velocity = Vector3.zero;
                intendedDir = Vector3.zero;
            }
        }
    }
}
