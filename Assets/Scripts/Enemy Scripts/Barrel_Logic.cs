using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Logic : Enemy
{
    public ParticleSystem[] dieParts;

    bool destroyed = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Move();

        //modelObj.GetComponent<MeshCollider>().enabled = !carried || !(thrown && !grounded);
        modelObj.GetComponent<MeshCollider>().enabled = grounded;

        felled = true;
    }

    void Move()
    {
        CheckGrounded();

        if (grounded)
            velocity = Vector3.zero;

        if (!carried && !destroyed)
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

            StartCoroutine(Die());

            destroyed = true;
        }
    }

    IEnumerator Die()
    {
        for (float t = 0; t < .05f; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1.02f, 0.98f, 1.02f));
            yield return null;
        }

        foreach(ParticleSystem parts in dieParts)
        {
            parts.GetComponent<ParticleSystem>().Play();
        }

        ToHideOnDeath.SetActive(false);
        GetComponent<CharacterController>().enabled = false;

        Destroy(gameObject, 2f);
    }
}
