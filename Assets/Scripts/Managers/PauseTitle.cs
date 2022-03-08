using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTitle : Pause
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        canPause = CheckCanPause();

        if (Input.GetButton("Pause"))
        {
            canPause = false;
            pressed = true;

            if (!paused)
            {
                paused = true;
                PlayerManager.Instance.paused = true;
                currentMenu = allMenus[0];
                currentMenu.cursor.ResetMenu();

                currentMenu.Enter();
            } else
            {

            }
        }

        if (paused && currentMenu == allMenus[0] && Input.GetButtonDown("Cancel") && !pressed && currentMenu.GetComponent<MainMenu>().canReturn)
        {
            //go back to "press start"
            paused = false;
            PlayerManager.Instance.paused = false;
            currentMenu.Leave();
        }

        if (!Input.GetButton("Pause") && pressed)
            pressed = false;

        if (paused)
        {
            currentMenu.Logic();
        }
    }
    bool CheckCanPause()
    {
        bool check = true;
        if (pressed)
            check = false;
        return check && activatePause;
    }
}
