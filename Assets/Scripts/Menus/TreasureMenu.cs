using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMenu : Menu
{
    public Animator treasureAnim;

    public TreasureCursor tCursor;

    // Start is called before the first frame update
    protected override void Start()
    {
        tCursor.ResetMenu();
        treasureAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!PlayerManager.Instance.paused || !pause.CheckMenu(2))
            return;
        if (!pressed)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                pressed = true;
                tCursor.MoveUp();
                RecolorOptions();
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                pressed = true;
                tCursor.MoveDown();
                RecolorOptions();
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                pressed = true;
                tCursor.MoveRight();
                RecolorOptions();
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                pressed = true;
                tCursor.MoveLeft();
                RecolorOptions();
            }
        }

        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
            pressed = false;
    }

    public override void Logic()
    {
        if (Input.GetButtonDown("Submit"))
        {
            // pull up info on treasure here
            if(tCursor.currentSlot.y == 5f)
            {
                pause.ChangeMenu(0);
            }
        }

        if(Input.GetButtonDown("Cancel"))
            pause.ChangeMenu(0);
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
    }
    public override void Leave()
    {
        treasureAnim.SetBool("Visible", false);
    }
}
