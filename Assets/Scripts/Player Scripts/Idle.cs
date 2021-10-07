using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonoBehaviour
{
    Animator anim;

    private float startingIdleTimer = 8f; //for debug only, can be removed in release

    private float idleTimer;
    private bool idling = false;
    private bool sleeping = false;
    private int prevIdle = -1;

    private int idleCount = 0; //the number of times idles have been triggered. This determines sleeping

    public ParticleSystem sleepBubblePart;
    public ParticleSystem wakeUpPart;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        idleTimer = startingIdleTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
        {
            if (!idling && idleCount <= 3)
                idleTimer -= Time.deltaTime;
            if (idleTimer <= 0 && !idling)
            {
                if (idleCount < 3)
                    PlayIdle();
                else if (!sleeping)
                {
                    GetComponentInChildren<Blink>().CloseEyes();
                    sleepBubblePart.Play();
                    sleeping = true;
                    idleCount++;
                    anim.SetInteger("IdleCounter", idleCount);

                    GetComponent<ChangeModel>().ChangeModelTo(1);
                }
            }
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

        idleCount++;
        anim.SetInteger("IdleCounter", idleCount);
    }

    //used on its own when the idle is stopped only by the animation finishing
    public void StopIdle()
    {
        if (PlayerManager.Instance.currentState == PlayerManager.PlayerState.normal)
        {
            sleepBubblePart.Stop();
            sleepBubblePart.Clear();
            idling = false;
            sleeping = false;
            anim.ResetTrigger("Idle");
            idleTimer = startingIdleTimer;
            GetComponent<ChangeModel>().ChangeModelTo(0);
        }
    }

    //used only when player starts moving in some way
    public void StopIdleFull()
    {
        if (sleeping)
        {
            wakeUpPart.Play();
            GetComponentInChildren<Blink>().OpenEyes();
        }
        idleCount = 0;
        anim.SetInteger("IdleCounter", idleCount);
        StopIdle();
    }

    public void Increment()
    {
        idleCount++;
        anim.SetInteger("IdleCounter", idleCount);
    }
}
