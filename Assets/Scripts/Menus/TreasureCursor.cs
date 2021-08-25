using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCursor : MenuCursor
{
    public Vector2 currentSlot;

    float width;

    public RectTransform leftBracket;
    public RectTransform rightBracket;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ResetMenu();
    }

    public override void ResetMenu()
    {
        currentSlot = Vector2.zero;
        rectTransform.localPosition = newPos();

        leftBracket.localPosition = new Vector3(-12f, 0, 0);
        rightBracket.localPosition = new Vector3(12f, 0, 0);
    }

    //moves the cursor down one option
    public override void MoveDown()
    {
        if (currentSlot.y == 4)
        {
            //transitioning to the back button
            currentSlot.y += 1f;
            width = 100f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(0, -102f, -.2f));
        }
        else if (currentSlot.y == 5)
        {
        }
        else
        {
            currentSlot.y += 1f;
            width = 24f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
        }
    }

    //moves the cursor up one option
    public override void MoveUp()
    {
        if (currentSlot.y == 0)
        {
        }
        else
        {
            currentSlot.y -= 1f;
            width = 24f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
        }
    }

    public void MoveLeft()
    {
        if (currentSlot.x == 0)
        {
        }
        else if (currentSlot.y < 5)
        {
            currentSlot.x -= 1f;
            width = 24f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
        }
    }
    public void MoveRight()
    {
        if (currentSlot.x == 8)
        {
        }
        else if(currentSlot.y < 5)
        {
            currentSlot.x += 1f;
            width = 24f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
        }
    }

    Vector3 newPos()
    {
        Vector2 correctedCurrentSlot = new Vector2(currentSlot.x, -currentSlot.y);
        Vector2 unityNeedsMoreVector3Constructors = (correctedCurrentSlot + new Vector2(-4f, 2f)) * 32f;
        return new Vector3(unityNeedsMoreVector3Constructors.x, unityNeedsMoreVector3Constructors.y, -0.2f);
    }

    protected override IEnumerator MoveCursor(Vector3 targetPos)
    {
        Vector3 initPos = rectTransform.localPosition;
        Vector3 bracketInitPos = rightBracket.localPosition;
        int numFrames = 10;
        for (int i = 0; i < numFrames; i++)
        {
            rectTransform.localPosition = Vector3.Lerp(initPos, targetPos, Mathf.SmoothStep(0, 1f, i / (float)numFrames));

            leftBracket.localPosition = Vector3.Lerp(-bracketInitPos, new Vector3(-width / 2f, 0, 0), Mathf.SmoothStep(0, 1f, i / (float)numFrames));
            rightBracket.localPosition = Vector3.Lerp(bracketInitPos, new Vector3(width / 2f, 0, 0), Mathf.SmoothStep(0, 1f, i / (float)numFrames));

            yield return null;
        }

        rectTransform.localPosition = targetPos;
    }
}
