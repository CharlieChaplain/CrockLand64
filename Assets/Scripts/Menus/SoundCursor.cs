using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCursor : MenuCursor
{

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        PosAtZeroY = new Vector2(rectTransform.localPosition.x, 0);

        ResetMenu();
    }

    public override void ResetMenu()
    {
        base.ResetMenu();
    }

    //moves the cursor down one option
    public override void MoveDown()
    {
        if (currentOption + 1 >= numOptions)
        {
        }
        else if (currentOption == 1) //handles moving the cursor down and over for the final option
        {
            currentOption++;
            float yPos = (((numOptions - 1) / 2f) * 40f) + 5f - (40f * currentOption);
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(-90, yPos, 0)); //-90 is HARDCODED and is how far over the cursor is when hovering the back button
        }
        else
        {
            currentOption++;
            float yPos = (((numOptions - 1) / 2f) * 40f) + 5f - (40f * currentOption);
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(PosAtZeroY.x, yPos, 0));
        }
    }

    //moves the cursor up one option
    public override void MoveUp()
    {
        if (currentOption - 1 < 0)
        {
        }
        else if (currentOption == 2)
        {
            currentOption--;
            float yPos = (((numOptions - 1) / 2f) * 40f) + 5f - (40f * currentOption);
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(-131, yPos, 0)); //-131 is HARDCODED and is how far over the cursor is when hovering the sliders
        }
        else
        {
            currentOption--;
            float yPos = (((numOptions - 1) / 2f) * 40f) + 5f - (40f * currentOption);
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(PosAtZeroY.x, yPos, 0));
        }
    }
}
