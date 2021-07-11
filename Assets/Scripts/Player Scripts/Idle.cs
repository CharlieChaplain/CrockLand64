using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonoBehaviour
{
    Animator anim;

    private float startingIdleTimer = 8f; //for debug only, can be removed in release

    private float idleTimer;
    private bool idling = false;
    private int prevIdle = -1;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        idleTimer = startingIdleTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(!idling)
            idleTimer -= Time.deltaTime;
        if(idleTimer <= 0 && !idling)
        {
            PlayIdle();
        }
    }

    void PlayIdle()
    {
        idling = true;
        int idleNumber = Mathf.FloorToInt(Random.Range(0, 2.99f));
        //redoes random if same idle is selected to dissuade but not forbid repeats
        if(idleNumber == prevIdle)
            idleNumber = Mathf.FloorToInt(Random.Range(0, 2.99f));

        prevIdle = idleNumber;

        //used to force an idle
        //idleNumber = 0;

        anim.SetInteger("IdleNumber", idleNumber);
        anim.SetTrigger("Idle");
    }

    public void StopIdle()
    {
        idling = false;
        anim.ResetTrigger("Idle");
        idleTimer = startingIdleTimer;
    }
}
