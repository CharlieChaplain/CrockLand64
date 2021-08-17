using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) //9 = player
        {
            other.GetComponent<Hurt>().HurtPlayer(10, 2, transform.position);
        }
    }
}
