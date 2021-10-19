using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{
    Animator anim;
    Material mat;
    ParticleSystem part;
    bool pressed;

    public TargetTarget tTarget;
    public Texture[] texs;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        part = GetComponentInChildren<ParticleSystem>();
        pressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!pressed && other.gameObject.layer == 10) //layer 10 = enemy
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.GetHeavyThrown())
                Pressed();
        }
    }

    void Pressed()
    {
        anim.SetTrigger("Push");
        pressed = true;
        mat.mainTexture = texs[1];
        part.Play();

        tTarget.GetComponent<TargetTarget>().Trigger();
    }
}
