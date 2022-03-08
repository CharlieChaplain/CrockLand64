using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//same as optionsmenu, but changes the flags to change menus because the title screen menu is slightly different
public class OptionsMenuTitle : OptionsMenu
{

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
        if (Input.GetButtonDown("Submit"))
        {
            switch (cursor.currentOption)
            {
                case 0: //Display
                    break;
                case 1: //Audio
                    pause.ChangeMenu(2);
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

        if (Input.GetButtonDown("Cancel"))
            pause.ChangeMenu(0);
    }

    public override void Enter()
    {
        base.Enter();
        optionsMenuAnim.SetBool("Show", true);
    }
    public override void Leave()
    {
        optionsMenuAnim.SetBool("Show", false);
    }
}
