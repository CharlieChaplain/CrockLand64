using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLogic : MonoBehaviour
{
    public Animator anim;

    float timer;
    bool rustle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9) //9 = player
        {
            Debug.Log("Leaves");

            if(other.GetComponent<PlayerMove>().GetVelocity().y < 0)
            {
                rustle = true;
                anim.SetBool("Rustle", rustle);
                timer = 0.2f;
            }
        }
    }

    private void Update()
    {
        //this timer setup will make sure if the rustle animation is occuring, it won't queue up another one later when it shouldn't play.
        if (timer > 0)
            timer -= Time.deltaTime;
        if(timer <= 0 && rustle)
        {
            rustle = false;
            anim.SetBool("Rustle", rustle);
        }
    }
}
