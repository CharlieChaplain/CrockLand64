using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    protected float health;

    public bool gravAffected;
    public float gravMult = 1f;

    public Transform groundCheck;
    public float groundCheckRadius;
    protected bool grounded;
    public LayerMask groundMask;

    protected bool felled;
    protected bool carried; //when the enemy is being carried
    protected bool thrown; //after enemy is thrown and before it hits the ground
    protected Vector3 velocity;

    public Animator anim;
    protected CharacterController controller;

    public GameObject hurtPart; //the particle to play when enemy is struck

    public GameObject modelObj;

    protected float standupTimer = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        health = maxHealth;
        felled = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected void Die()
    {
    }

    public bool IsFelled()
    {
        return felled;
    }

    public void Throw(Vector3 force)
    {
        velocity = force;
        StartCoroutine("heavyThrow");
    }

    protected void StandUp()
    {
        if (carried)
        {
            standupTimer = 0;
        }

        if (felled && !carried)
        {
            standupTimer += Time.deltaTime;

            if (standupTimer >= 4f && standupTimer < 5f)
                anim.SetFloat("StandUp", 2f);
            else if (standupTimer >= 5f)
            {
                anim.SetFloat("StandUp", 3f);
                health = maxHealth;
                felled = false;
            }
        }
    }

    protected void ApplyGravity()
    {
        if(gravAffected)
            velocity += Physics.gravity * gravMult;
    }

    protected void Hurt(float damage, Vector3 assailantPos, PlaySound hurtSound)
    {
        health -= damage;
        if (hurtPart != null)
        {
            Vector3 charCenter = transform.position + GetComponent<CharacterController>().center;
            //Vector3 hitpoint = charCenter + assailantPos - charCenter;
            Vector3 hitpoint = charCenter;
            GameObject _hurtPart = GameObject.Instantiate(hurtPart, hitpoint, Quaternion.identity);
            Destroy(_hurtPart, 1f);
        }
        if (hurtSound != null)
        {
            hurtSound.Play(transform.position + GetComponent<CharacterController>().center);
        }
        StartCoroutine("pauseTime");
    }

    protected virtual void CheckGrounded()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    public void SetCarried(bool _carried)
    {
        carried = _carried;
    }
    public void SetThrown(bool _thrown)
    {
        thrown = _thrown;
    }
    public void SetFelled(bool _felled)
    {
        felled = _felled;
    }

    //this coroutine disables gravity for the enemy for a time proportionate to the heavy throw chargeup.
    protected IEnumerator heavyThrow()
    {
        gravAffected = false;
        float time = Mathf.Max((velocity.magnitude - 25f), 0) / 250f;
        yield return new WaitForSeconds(time);
        gravAffected = true;
    }

    protected IEnumerator pauseTime()
    {
        yield return new WaitForSecondsRealtime(0.06f);
        float currentTimeScale = Time.timeScale;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = currentTimeScale;
    }

    protected IEnumerator wobbleScale() //will scale the enemy in a sinusoidal manner 
    {
        Vector3 scale = new Vector3(1f, 1f, 1f);
        float timer = 0f;
        while(timer <= 1f)
        {
            //float horzScale = 1f + (Mathf.Sin(timer * Mathf.PI) / (1f + timer));
            float horzScale = 1f + (Mathf.Sin((timer + .5f) * Mathf.PI * 2.5f) * 0.5f / (1f + (timer * 10f)));
            scale.x = horzScale;
            scale.z = horzScale;
            //scale.y = 1f + (Mathf.Cos(timer * Mathf.PI) / (1f + timer));
            scale.y = 1f + (Mathf.Cos((timer + .5f) * Mathf.PI * 2.5f) * 0.5f / (1f + (timer * 10f)));

            modelObj.transform.localScale = scale;

            timer += Time.deltaTime;

            yield return null;
        }

        modelObj.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
