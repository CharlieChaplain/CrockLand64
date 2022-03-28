using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Pursue : MonoBehaviour
{
    public float speed;
    public GameObject target;
    public float chaseRadius; //how far away the enemy will start pursuing the target

    public Vector3 Pursue()
    {
        return (target.transform.position - transform.position).normalized;
    }
}
