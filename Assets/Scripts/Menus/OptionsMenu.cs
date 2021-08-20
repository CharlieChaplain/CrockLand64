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
        if (Input.GetButtonDown("Submit"))
        {
            switch (cursor.currentOption)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
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
