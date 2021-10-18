using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    public int numOptions; //the number of options on the current menu
    public int currentOption; //the option the cursor currently sits on

    protected RectTransform rectTransform;
    Vector2 PosAtZeroY; //where the pointer is at when its y value is not altered.

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        PosAtZeroY = new Vector2(rectTransform.localPosition.x, 0);
    }

    public virtual void ResetMenu()
    {
        currentOption = 0;

        float yPos = (((numOptions - 1) / 2f) * 40f) + 5f;

        rectTransform.localPosition = new Vector3(PosAtZeroY.x, yPos, 0);
    }

    //moves the cursor down one option
    public virtual void MoveDown()
    {
        if(currentOption + 1 >= numOptions)
        {
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
    public virtual void MoveUp()
    {
        if (currentOption -1 < 0)
        {
        }
        else
        {
            currentOption--;
            float yPos = (((numOptions - 1) / 2f) * 40f) + 5f - (40f * currentOption);
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(PosAtZeroY.x, yPos, 0));
        }
    }

    protected virtual IEnumerator MoveCursor(Vector3 targetPos)
    {
        Vector3 initPos = rectTransform.localPosition;
        float seconds = 0.2f;
        for(float f = 0; f < seconds; f+= Time.unscaledDeltaTime)
        {
            rectTransform.localPosition = Vector3.Lerp(initPos, targetPos, Mathf.SmoothStep(0, 1f, f / seconds));
            yield return null;
        }

        rectTransform.localPosition = targetPos;
    }
}
