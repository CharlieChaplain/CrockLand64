using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMenu : Menu
{
    public Animator treasureAnim;

    public TreasureCursor tCursor;

    public PlaySound SOOpen;
    public PlaySound SOClose;

    // Start is called before the first frame update
    protected override void Start()
    {
        tCursor.ResetMenu();
        treasureAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public override void Logic()
    {
    }

    protected override void RecolorOptions()
    {
        if (tCursor.currentSlot.y == 5f)
            optionImages[0].color = Color.white;
        else
            optionImages[0].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
    }

    public override void Enter()
    {
        base.Enter();
        treasureAnim.SetBool("Visible", true);
        SOOpen.Play(CameraManager.Instance.sceneCam.transform.position);
    }
    public override void Leave()
    {
        base.Leave();
        treasureAnim.SetBool("Visible", false);
        SOClose.Play(CameraManager.Instance.sceneCam.transform.position);
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
                //vertical cursor movement
                if (input.y > 0)
                {
                    tCursor.MoveUp();
                    RecolorOptions();
                }
                else if (input.y < 0)
                {
                    tCursor.MoveDown();
                    RecolorOptions();
                }
            }
            else
            {
                //horizontal cursor movement
                if (input.x > 0)
                {
                    tCursor.MoveRight();
                    RecolorOptions();
                }
                else if (input.x < 0)
                {
                    tCursor.MoveLeft();
                    RecolorOptions();
                }
            }
        }
    }
    public override void Confirm()
    {
        if (!active)
            return;
        // pull up info on treasure here
        if (tCursor.currentSlot.y == 5f)
        {
            pause.ChangeMenu(0);
        }
    }
    public override void Cancel()
    {
        if (!active)
            return;
        pause.ChangeMenu(0);
    }
}
