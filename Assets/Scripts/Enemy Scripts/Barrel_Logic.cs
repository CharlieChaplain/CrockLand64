using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Logic : Enemy
{
    public ParticleSystem[] dieParts;

    bool destroyed = false;

    public Transform waterCheck;
    public LayerMask waterMask;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(!destroyed)
            Move();

        //modelObj.GetComponent<MeshCollider>().enabled = !carried || !(thrown && !grounded);
        modelObj.GetComponent<MeshCollider>().enabled = grounded;

        felled = true;
    }

    void Move()
    {
        CheckGrounded();
        CheckWater();
        if (grounded)
            velocity = Vector3.zero;

        //will kill the enemy if it hits a wall while being heavy thrown.
        if (heavyThrown)
        {
            SphereCollider sphCol = GetComponent<SphereCollider>(); //this collider can be different for each enemy. If not, then move this method to base class
            if (Physics.CheckSphere(transform.position + sphCol.center, sphCol.radius, groundMask))
            {
                destroyed = true;
                Die();
            }
        }

        if (!carried && !destroyed)
        {
            ApplyGravity();
            controller.Move(velocity * Time.deltaTime);
        }
    }
    //checks if barrel is in water at the specified location (ie underwater), so it can be destroyed
    void CheckWater()
    {
        if (Physics.CheckSphere(waterCheck.position, 0.4f, waterMask, QueryTriggerInteraction.Collide))
        {
            destroyed = true;
            Die();
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
            hurtSound.Play(transform.position);

            Die();

            destroyed = true;
        }
    }

    void Die()
    {
        foreach(ParticleSystem parts in dieParts)
        {
            parts.GetComponent<ParticleSystem>().Play();
        }
        dieSound.Play(transform.position);
        ToHideOnDeath.SetActive(false);
        GetComponent<CharacterController>().enabled = false;

        Destroy(gameObject, 2f);
    }
}
