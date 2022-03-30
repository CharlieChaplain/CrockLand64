using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : Menu
{
    public Animator optionsMenuAnim;

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
        optionsMenuAnim.SetBool("Show", true);
    }
    public override void Leave()
    {
        base.Leave();
        optionsMenuAnim.SetBool("Show", false);
    }

    public override void Confirm()
    {
        if (!active)
            return;
        switch (cursor.currentOption)
        {
            case 0: //Display
                break;
            case 1: //Audio
                pause.ChangeMenu(3);
                break;
            case 2: //Controls
                break;
            case 3: //Back
                pause.ChangeMenu(0);
                break;
            default:
                break;
        }
    }
    public override void Cancel()
    {
        if (!active)
            return;
        pause.ChangeMenu(0);
    }
}
