using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxInfo : MonoBehaviour
{
    public float damage;
    public bool on = false;

    public bool isKiller; //determines if this attack is one of crock's killing moves

    public static void ToggleHurtBox(GameObject hurtbox, bool toggle)
    {
        hurtbox.GetComponent<HurtBoxInfo>().on = toggle;
        hurtbox.GetComponent<Collider>().enabled = toggle;
    }
}
