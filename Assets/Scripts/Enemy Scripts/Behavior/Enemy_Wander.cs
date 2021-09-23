using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Wander : MonoBehaviour
{
    public float speed;
    public float homeRadius;
    public Vector3 home;

    float timer;

    Vector3 currentDir;

    private void Start()
    {
        home = transform.position;
        timer = Random.Range(3f, 5f);
    }

    public Vector3 Wander()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = Random.Range(3f, 5f);

            if(Random.value > 0.25f)
            {
                currentDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                currentDir.Normalize();
            } else
            {
                currentDir = Vector3.zero;
            }

            if(Vector3.Distance(transform.position, home) > homeRadius)
            {
                currentDir = (home - transform.position).normalized;
            }
        }

        return currentDir * speed;
    }
}
