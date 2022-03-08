using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyCursor : MenuCursor
{
    int index = 0;
    public List<Vector2> positions;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ResetMenu(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ResetMenu()
    {
        index = 0;
        rectTransform.anchoredPosition = new Vector2(positions[0].x, positions[0].y);
    }

    public void ResetMenu(int mode)
    {
        //sets default index to 1 (no) for copy and erase modes, but 0 (yes) for play mode
        switch (mode)
        {
            case 1:
                index = 1;
                break;
            case 2:
                index = 1;
                break;
            default:
                index = 0;
                break;
        }
        rectTransform.anchoredPosition = new Vector2(positions[index].x, positions[index].y);
    }

    public void MoveLeft()
    {
        if (index == 0)
        {
        }
        else
        {
            index -= 1;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }

    }
    public void MoveRight()
    {
        if (index == 1)
        {
        }
        else
        {
            index += 1;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    public int GetIndex()
    {
        return index;
    }

    Vector3 newPos()
    {
        return new Vector3(positions[index].x, positions[index].y, -0.2f);
    }
}
