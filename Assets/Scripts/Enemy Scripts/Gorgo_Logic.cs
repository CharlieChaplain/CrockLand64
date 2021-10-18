using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorgo_Logic : Enemy
{
    float hurtTimer = 0;

    Enemy_Pursue pursue;
    Enemy_Wander wander;

    public GameObject headTrigger;

    public float turnSpeed;
    public float accel;

    public bool attacking;

    public Texture[] texs;
    
    public GameObject auraPrefab;
    GameObject aura;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        pursue = GetComponent<Enemy_Pursue>();
        wander = GetComponent<Enemy_Wander>();

        pursue.target = PlayerManager.Instance.player;

        GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = texs[0];
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (hurtTimer > 0)
            hurtTimer -= Time.deltaTime;
        else if (attacking)
        {
            //call update function for aura object here
        }
        else if (!attacking)
        {
            Move();
            AttackLogic();
        }

        anim.SetBool("Carried", carried);
        StandUp();
    }

    //can copy and paste this Move function for basic enemies
    private void Move()
    {
        //if -- not being carried
        if (!carried)
        {
            headTrigger.SetActive(true);

            //if -- enemy is still walking around
            if (!felled && !thrown)
            {
                // LOGIC TO DECIDE MOVEMENT DIRECTION, REPLACE WITH RELEVANT LOGIC ----------------------------------------
                Vector3 intendedDir = Vector3.zero;
                if ((pursue.target.transform.position - transform.position).magnitude < pursue.chaseRadius)
                {
                    intendedDir = pursue.Pursue();
                    velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel) * pursue.speed;
                }
                else
                {
                    intendedDir = wander.Wander();
                    if (intendedDir == Vector3.zero)
                        velocity = intendedDir;
                    else
                        velocity = Vector3.RotateTowards(transform.forward, intendedDir, turnSpeed, accel) * wander.speed;
                }

                Vector3 XYDir = velocity;
                XYDir.y = 0;
                //prevents direction confusion if 
                if (XYDir != Vector3.zero)
                {
                    transform.forward = XYDir;
                }
                anim.SetFloat("Speed", XYDir.magnitude);
                // END MOVEMENT LOGIC -------------------------------------------------------------------------------------
            }
            //end if -- enemy is still walking around

            //checks for being grounded and applies gravity
            CheckGrounded();
            if (grounded)
            {
                velocity.y = 0;
                if (thrown)
                {
                    wander.home = transform.position;
                    thrown = false;
                }

                if (felled)
                    velocity = Vector3.zero;
            }

            ApplyGravity();

            //final move of character controller
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            headTrigger.SetActive(false);
        }
        //end if -- not being carried
    }

    //checks conditions and starts attack if they are met
    void AttackLogic()
    {
        if (felled)
            return;

        Vector3 dir = pursue.target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle < 20f && dir.magnitude < 4f)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            aura = Instantiate(auraPrefab, transform.position, Quaternion.identity);
            aura.GetComponent<EnemyAura_Logic>().OnSpawn();
        }
    }

    //deals with getting hit by crock
    private void OnTriggerEnter(Collider other)
    {
        int layerNumber = 9; //layer 9 = player
        if (other.gameObject.layer == layerNumber && other.CompareTag("HurtBox"))
        {
            HurtBoxInfo hurtBox = other.GetComponent<HurtBoxInfo>();

            PlaySound hurtSound = null;
            if (other.gameObject.layer == 9) //layer 9 = player
                hurtSound = PlayerManager.Instance.currentHitSound;

            Hurt(hurtBox.damage, other.transform.position, hurtSound);

            if (health <= 0)
            {
                felled = true;
                attacking = false;

                //will destroy the enemy if the hurtbox is of a killing move. otherwise, just knocks it down
                if (hurtBox.isKiller)
                {
                    StartCoroutine(Die());
                }
                else
                {
                    anim.SetTrigger("Fall");
                    standupTimer = 0;
                    anim.SetFloat("StandUp", 1f);
                }
            }
            else
            {
                anim.SetTrigger("Hit");
                StartCoroutine(ResetHit());
                hurtTimer = 1f;
                velocity = Vector3.zero;
            }
        }
        else if(other.gameObject.layer == 4) //water = 4
        {
            StartCoroutine(Die());
        }
    }

    public void ChangeTex(int index)
    {
        if (index < 0 || index >= texs.Length)
            return;
        GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = texs[index];
    }

    public void ActivateAura()
    {
        aura.GetComponent<EnemyAura_Logic>().Activate();
    }

    IEnumerator Attack()
    {
        for(float f = 0; f < .33f; f += Time.deltaTime)
        {
            controller.Move(transform.forward * 5f * Time.deltaTime);
            yield return null;
        }
    }

    //will reset the hit trigger if it doesn't happen immediately so the gorgo doesn't get hurt at a weird time
    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.05f);
        anim.ResetTrigger("Hit");
    }

    IEnumerator Die()
    {
        Destroy(aura);

        hurtTimer = 1000f; //will prevent movement

        for (float t = 0; t < .1f; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1.02f, 0.98f, 1.02f));
            yield return null;
        }
        diePart.GetComponent<ParticleSystem>().Play();
        ToHideOnDeath.SetActive(false);
        GetComponent<CharacterController>().enabled = false;

        Destroy(this.gameObject, 2f);
    }
}
