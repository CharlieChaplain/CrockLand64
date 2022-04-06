using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsRootMenu : Menu
{
    public Animator menuAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Logic()
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        menuAnim.SetBool("Show", true);
    }
    public override void Leave()
    {
        base.Leave();
        menuAnim.SetBool("Show", false);
    }

    public override void Confirm()
    {
        if (!active)
            return;
        switch (cursor.currentOption)
        {
            case 0: //Keyboard
                pause.ChangeMenu(5);
                break;
            case 1: //Controller
                pause.ChangeMenu(6);
                break;
            case 3: //Back to Options
                pause.ChangeMenu(1);
                break;
            default:
                break;
        }
    }
    public override void Cancel()
    {
        if (!active)
            return;
        pause.ChangeMenu(1);
    }
}
