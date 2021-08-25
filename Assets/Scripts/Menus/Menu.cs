using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Pause pause;

    public MenuCursor cursor;
    public int numOptions;

    protected bool pressed; //prevents holding the button to scroll every frame

    public List<Image> optionImages;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        cursor.ResetMenu();
        RecolorOptions();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!PlayerManager.Instance.paused)
            return;
        if (!pressed)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                pressed = true;
                cursor.MoveUp();
                RecolorOptions();
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                pressed = true;
                cursor.MoveDown();
                RecolorOptions();
            }
        }

        if ((Input.GetAxisRaw("Vertical") == 0))
            pressed = false;
    }

    protected virtual void RecolorOptions()
    {
        for(int i = 0; i < optionImages.Count; i++)
        {
            if (i == cursor.currentOption)
                optionImages[i].color = Color.white;
            else
                optionImages[i].color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
        }
    }

    public virtual void Logic()
    {

    }

    public virtual void Enter()
    {
        cursor.ResetMenu();
        RecolorOptions();
    }
    public virtual void Leave()
    {

    }
}