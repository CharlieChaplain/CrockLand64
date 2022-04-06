using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : Menu
{
    public Animator controlsMenuAnim;
    public bool keyboard; //true if it changes keyboard controls, false if it changes controller controls
    public ControlsCursor ccursor;

    public RectTransform movingMenu;

    int movingMenuIndex = 0; //the index of the option currently at the top of the moving menu

    // Start is called before the first frame update
    protected override void Start()
    {
        ccursor.ResetMenu();
        RecolorOptions();
        pause = GameObject.Find("PauseDirector").GetComponent<Pause>();
        active = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Logic()
    {
        
    }

    public override void CursorMovement(Vector2 input)
    {
        if (!PlayerManager.Instance.paused || !active)
        {
            return;
        }
        if (!pressed)
        {
            //checks which input is bigger, x or y
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                //handles moving up and down
                if (input.y > 0)
                {
                    ccursor.MoveUp();
                    RecolorOptions();
                    MoveOptions(-1);
                }
                else if (input.y < 0)
                {
                    ccursor.MoveDown();
                    RecolorOptions();
                    MoveOptions(1);
                }
            }
            else
            {
                //handles moving between the bottom options with horizontal input
                if (input.x > 0)
                {
                    ccursor.MoveRight();
                    RecolorOptions();
                }
                else if (input.x < 0)
                {
                    ccursor.MoveLeft();
                    RecolorOptions();
                }
            }
        }
    }

    public override void Enter()
    {
        ccursor.ResetMenu();
        RecolorOptions();
        movingMenuIndex = 0;
        movingMenu.localPosition = new Vector3(0,0,-0.8f);
        active = true;
        controlsMenuAnim.SetBool("Show", true);
    }
    public override void Leave()
    {
        base.Leave();
        controlsMenuAnim.SetBool("Show", false);
    }

    public override void Confirm()
    {
        if (!active)
            return;
        /*
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
        */
    }
    public override void Cancel()
    {
        if (!active)
            return;
        pause.ChangeMenu(4);
    }

    protected override void RecolorOptions()
    {
        if(ccursor.currentOption == 7 || ccursor.currentOption == 8) //deals with the two bottom buttons
        {
            for (int i = 0; i < optionImages.Count; i++)
            {
                optionImages[i].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
            }

            optionImages[ccursor.currentOption + 7].color = Color.white;
        } else //deals with any of the moving options
        {
            for (int i = 0; i < optionImages.Count; i+=2)
            {
                if (i / 2 == ccursor.currentOption)
                {
                    optionImages[i].color = Color.white;
                    optionImages[i+1].color = Color.white;
                }
                else
                {
                    optionImages[i].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
                    optionImages[i + 1].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
                }
            }
        }
    }

    public Vector3 CalculatePointerPos()
    {
        float height = 40f;
        float xPos = -131f;
        float initHeight = 85f;

        return new Vector3(xPos, initHeight - (height * (ccursor.currentOption - movingMenuIndex)), -1f);
    }

    void MoveOptions(int dir)
    {
        if(ccursor.currentOption >= 7)
        {
            return;
        }
        else
        {
            if((dir < 0 && ccursor.currentOption == 2 && movingMenuIndex == 3) || //moving back up the menu
                (dir < 0 && ccursor.currentOption == 1 && movingMenuIndex == 2) ||
                (dir < 0 && ccursor.currentOption == 0 && movingMenuIndex == 1) ||

                (dir > 0 && ccursor.currentOption == 4 && movingMenuIndex == 0) || //moving down the menu
                (dir > 0 && ccursor.currentOption == 5 && movingMenuIndex == 1) ||
                (dir > 0 && ccursor.currentOption == 6 && movingMenuIndex == 2))
                StartCoroutine("MoveOptionsCo", dir);
        }

    }

    IEnumerator MoveOptionsCo(int dir)
    {
        movingMenuIndex += dir;
        Vector3 initPos = movingMenu.localPosition;
        Vector3 moveVector = new Vector3(0, 40f, 0);
        Vector3 targetPos = initPos + (moveVector * dir);

        float seconds = 0.2f;
        for (float f = 0; f < seconds; f += Time.unscaledDeltaTime)
        {
            movingMenu.localPosition = Vector3.Lerp(initPos, targetPos, Mathf.SmoothStep(0, 1f, f / seconds));
            yield return null;
        }

        movingMenu.localPosition = targetPos;
    }
}
