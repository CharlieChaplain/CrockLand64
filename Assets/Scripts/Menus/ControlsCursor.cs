using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCursor : MenuCursor
{
    ControlsMenu parentMenu;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentMenu = GetComponentInParent<ControlsMenu>();
        PosAtZeroY = new Vector2(rectTransform.localPosition.x, 0);

        ResetMenu();
    }

    public override void ResetMenu()
    {
        //base.ResetMenu();
    }

    //moves the cursor down one option
    public override void MoveDown()
    {
        if (currentOption >= 7)
        {
        }
        else if (currentOption == 6) //handles moving the cursor down and over for the final option
        {
            currentOption++;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(-185, -90, 0));
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
        else
        {
            currentOption++;
            StopCoroutine("MoveCursor");
            StartCoroutine("DelayCursorMove");
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    //moves the cursor up one option
    public override void MoveUp()
    {
        if (currentOption <= 0)
        {
        }
        else if (currentOption == 8)
        {
            currentOption -= 2;
            StopCoroutine("MoveCursor");
            StartCoroutine("DelayCursorMove");
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
        else
        {
            currentOption--;
            StopCoroutine("MoveCursor");
            StartCoroutine("DelayCursorMove");
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    public void MoveLeft()
    {
        if(currentOption == 8)
        {
            currentOption--;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(-180, -90, 0));
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    public void MoveRight()
    {
        if (currentOption == 7)
        {
            currentOption++;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", new Vector3(0, -90, 0));
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    IEnumerator DelayCursorMove()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine("MoveCursor", parentMenu.CalculatePointerPos());
    }
}
