using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsMenu : Menu
{
    public Animator controlsMenuAnim;
    public bool keyboard; //true if it changes keyboard controls, false if it changes controller controls
    public ControlsCursor ccursor;

    public RectTransform movingMenu;

    public List<string> controlNames;

    int movingMenuIndex = 0; //the index of the option currently at the top of the moving menu

    public GameObject verifyMenu;
    private TextMeshProUGUI verifyText;
    private int bindingIndex; //what gets passed in to determine which binding to access. 0 = keyboard, 1 = controller. This is set up in the controls asset

    private void OnEnable()
    {
        InputManager.rebindComplete += RebindComplete;

        for(int i = 0; i < controlNames.Count; i++)
        {
            InputManager.LoadBindingOverride(controlNames[i]);
        }
    }
    private void OnDisable()
    {
        InputManager.rebindComplete -= RebindComplete;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        ccursor.ResetMenu();
        RecolorOptions();
        pause = GameObject.Find("PauseDirector").GetComponent<Pause>();
        active = false;
        verifyText = verifyMenu.GetComponentInChildren<TextMeshProUGUI>();

        if (keyboard)
            bindingIndex = 0;
        else
            bindingIndex = 1;

        UpdateBindUI();
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

        //back
        if(ccursor.currentOption == numOptions - 2)
            pause.ChangeMenu(4);
        //reset defaults
        else if(ccursor.currentOption == numOptions - 1)
        {
            for(int i = 0; i < controlNames.Count; i++)
            {
                InputManager.ResetBinding(controlNames[i], bindingIndex);
                UpdateBindUI();
            }
        }

        //all rebindings
        else if (keyboard)
        {
            active = false;
            switch (ccursor.currentOption)
            {
                case 0: //move
                    InputManager.StartRebind("Move", 0, verifyMenu, true);
                    break;
                case 1: //jump
                    InputManager.StartRebind("Jump", 0, verifyMenu, true);
                    break;
                case 2: //punch
                    InputManager.StartRebind("Punch", 0, verifyMenu, true);
                    break;
                case 3: //crouch
                    InputManager.StartRebind("Crouch", 0, verifyMenu, true);
                    break;
                case 4: //charge
                    InputManager.StartRebind("Charge", 0, verifyMenu, true);
                    break;
                case 5: //pause
                    InputManager.StartRebind("Pause", 0, verifyMenu, true);
                    break;
                case 6: //confirm
                    InputManager.StartRebind("Confirm", 0, verifyMenu, true);
                    break;
                case 7: //cancel
                    InputManager.StartRebind("Cancel", 0, verifyMenu, true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            active = false;
            switch (ccursor.currentOption)
            {
                case 0: //jump
                    InputManager.StartRebind("Jump", 0, verifyMenu, false);
                    break;
                case 1: //punch
                    InputManager.StartRebind("Punch", 0, verifyMenu, false);
                    break;
                case 2: //crouch
                    InputManager.StartRebind("Crouch", 0, verifyMenu, false);
                    break;
                case 3: //charge
                    InputManager.StartRebind("Charge", 0, verifyMenu, false);
                    break;
                case 4: //pause
                    InputManager.StartRebind("Pause", 0, verifyMenu, false);
                    break;
                case 5: //confirm
                    InputManager.StartRebind("Confirm", 0, verifyMenu, false);
                    break;
                case 6: //cancel
                    InputManager.StartRebind("Cancel", 0, verifyMenu, false);
                    break;
                default:
                    break;
            }
        }
    }
    public override void Cancel()
    {
        if (!active)
            return;
        pause.ChangeMenu(4);
    }

    protected override void RecolorOptions()
    {
        if(ccursor.currentOption == numOptions - 2 || ccursor.currentOption == numOptions - 1) //deals with the two bottom buttons
        {
            for (int i = 0; i < optionImages.Count; i++)
            {
                optionImages[i].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
            }

            optionImages[ccursor.currentOption + numOptions - 2].color = Color.white;
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
        //keyboard menu has 1 more option than gamepad menu does
        if (keyboard)
        {
            if (ccursor.currentOption >= 8)
            {
                return;
            }
            else
            {
                if ((dir < 0 && ccursor.currentOption == 3 && movingMenuIndex == 4) || //moving back up the menu
                    (dir < 0 && ccursor.currentOption == 2 && movingMenuIndex == 3) ||
                    (dir < 0 && ccursor.currentOption == 1 && movingMenuIndex == 2) ||
                    (dir < 0 && ccursor.currentOption == 0 && movingMenuIndex == 1) ||

                    (dir > 0 && ccursor.currentOption == 4 && movingMenuIndex == 0) || //moving down the menu
                    (dir > 0 && ccursor.currentOption == 5 && movingMenuIndex == 1) ||
                    (dir > 0 && ccursor.currentOption == 6 && movingMenuIndex == 2) ||
                    (dir > 0 && ccursor.currentOption == 7 && movingMenuIndex == 3))
                    StartCoroutine("MoveOptionsCo", dir);
            }
        }
        else
        {
            if (ccursor.currentOption >= 7)
            {
                return;
            }
            else
            {
                if ((dir < 0 && ccursor.currentOption == 2 && movingMenuIndex == 3) || //moving back up the menu
                    (dir < 0 && ccursor.currentOption == 1 && movingMenuIndex == 2) ||
                    (dir < 0 && ccursor.currentOption == 0 && movingMenuIndex == 1) ||

                    (dir > 0 && ccursor.currentOption == 4 && movingMenuIndex == 0) || //moving down the menu
                    (dir > 0 && ccursor.currentOption == 5 && movingMenuIndex == 1) ||
                    (dir > 0 && ccursor.currentOption == 6 && movingMenuIndex == 2))
                    StartCoroutine("MoveOptionsCo", dir);
            }
        }
    }

    void RebindComplete()
    {
        UpdateBindUI();
        active = true;
    }

    void UpdateBindUI()
    {
        for(int i = 0; i < numOptions - 2; i++)
        {
            string actionName = controlNames[i];
            optionImages[(i * 2) + 1].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.GetControlName(actionName, bindingIndex);
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
