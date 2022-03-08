using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VerifyMenu : Menu
{
    public Menu parentMenu;
    public VerifyCursor vCursor;

    public List<TextMeshProUGUI> optionTexts;
    public List<Sprite> backgrounds;
    Image background;

    Animator anim;

    [TextArea(3, 20)]
    public List<string> verifyText;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    protected override void Start()
    {
        pause = GameObject.Find("PauseDirector").GetComponent<Pause>();
        background = GetComponent<Image>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!active)
            return;

        if (!pressed)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                pressed = true;
                vCursor.MoveRight();
                RecolorOptions();
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                pressed = true;
                vCursor.MoveLeft();
                RecolorOptions();
            }
        }

        if ((Input.GetAxisRaw("Horizontal") == 0))
            pressed = false;

        Logic();
    }


    public override void Logic()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if(vCursor.GetIndex() == 0) //yes
            {
                parentMenu.AcceptVerification();
            }
            Leave();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Leave();
        }
    }

    protected override void RecolorOptions()
    {
        foreach (TextMeshProUGUI text in optionTexts)
            text.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        optionTexts[vCursor.GetIndex()].color = Color.white;
    }

    public void Enter(int mode)
    {
        text.text = verifyText[mode];
        background.sprite = backgrounds[mode];
        vCursor.ResetMenu(mode);
        RecolorOptions();
        active = true;
        anim.SetBool("Visible", true);
        parentMenu.SetActive(false);
    }
    public override void Leave()
    {
        active = false;
        anim.SetBool("Visible", false);
        StartCoroutine("DelayParentActivation");
    }

    //done so going back to parent menu doesn't automatically reactivate verify menu due to button presses on the same frame
    IEnumerator DelayParentActivation()
    {
        yield return new WaitForEndOfFrame();
        parentMenu.SetActive(true);
    }
}
