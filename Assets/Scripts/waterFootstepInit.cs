using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterFootstepInit : MonoBehaviour
{
    public ParticleSystem splash;
    public void Init(Collider col)
    {
        splash.trigger.SetCollider(0, col);
    }
}
