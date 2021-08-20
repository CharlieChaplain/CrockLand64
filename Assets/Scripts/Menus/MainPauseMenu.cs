﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPauseMenu : Menu
{
    public Animator mainMenuAnim;
    public GameObject wealth;
    Animator wealthAnim;
    WealthCounter wealthCounter;

    public Animator molesAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        wealthAnim = wealth.GetComponent<Animator>();
        wealthCounter = wealth.GetComponent<WealthCounter>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Logic()
    {
        if (Input.GetButtonDown("Submit"))
        {
            switch (cursor.currentOption)
            {
                case 0:
                    pause.ChangeMenu(1);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        mainMenuAnim.SetBool("Show", true);

        molesAnim.SetBool("Visible", true);

        wealthAnim.ResetTrigger("Enter");
        wealthAnim.ResetTrigger("Leave");
        wealthAnim.SetTrigger("Enter");
        wealthCounter.isVisible = true;
        wealthCounter.visibleTimer = 3f; //this must be any value above 0 so WealthCounter doesn't immediately hide it again
    }
    public override void Leave()
    {
        mainMenuAnim.SetBool("Show", false);

        molesAnim.SetBool("Visible", false);

        wealthAnim.ResetTrigger("Enter");
        wealthAnim.ResetTrigger("Leave");
        wealthAnim.SetTrigger("Leave");
        wealthCounter.isVisible = false;
        wealthCounter.visibleTimer = 0;
    }
}
