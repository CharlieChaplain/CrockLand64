﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleTrapped_Logic : Enemy
{
    bool freed = false;

    public SkinnedMeshRenderer model;
    public Mesh unboundMesh;
    public ParticleSystem ropePart;
    public ParticleSystem dirtPart;

    BoxCollider col;
    SphereCollider bounceCol;

    public Animator moleHillAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        controller = GetComponent<CharacterController>();
        col = GetComponent<BoxCollider>();
        bounceCol = GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    protected override void Update()
    {
    }

    void Freed()
    {
        freed = true;
        anim.SetTrigger("BreakFree");
        moleHillAnim.SetTrigger("BreakFree");

        model.sharedMesh = unboundMesh;
        ropePart.Play();

        StartCoroutine("Leave");
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

            if (health <= 0 && !freed)
            {
                Freed();
            }
        }
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(3.15f);

        dirtPart.Play();

        float timer = 3f;
        //moves the colliders down every frame til all of them and the model are below the ground
        Vector3 increment = Vector3.up * 0.02f;
        while (timer > 0)
        {
            controller.center -= increment;
            col.center -= increment;
            bounceCol.center -= increment;

            timer -= Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
