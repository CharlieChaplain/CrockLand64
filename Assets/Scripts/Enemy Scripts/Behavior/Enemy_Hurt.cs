using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// put this script on the object that has the hurtboxInfo on it.
/// </summary>
public class Enemy_Hurt : MonoBehaviour
{
    HurtBoxInfo hurtBox;
    public int damageVariance;

    private void Start()
    {
        hurtBox = GetComponent<HurtBoxInfo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) //9 = player
        {
            other.GetComponent<Hurt>().HurtPlayer((int)hurtBox.damage, damageVariance, transform.position);
        }
    }
}
