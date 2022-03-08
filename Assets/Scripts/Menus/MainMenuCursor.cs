using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursor : MenuCursor
{
    public Vector2 index;

    public List<Vector2> positions;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ResetMenu();
    }

    public override void ResetMenu()
    {
        index = new Vector2(0, 0);
        rectTransform.localPosition = positions[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //moves the cursor down one option
    public override void MoveDown()
    {
        if (index.y == 5)
        {
        }
        else
        {
            index.y += 1f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    //moves the cursor up one option
    public override void MoveUp()
    {
        if (index.y == 0)
        {
        }
        else
        {
            index.y -= 1f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    public void MoveLeft()
    {
        if (index.x == 0)
        {
        }
        else if (index.y < 5)
        {
            index.x -= 1f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            if(index.y > 2)
                menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }

    }
    public void MoveRight()
    {
        if (index.x == 1)
        {
        }
        else if (index.y < 5)
        {
            index.x += 1f;
            StopCoroutine("MoveCursor");
            StartCoroutine("MoveCursor", newPos());
            if (index.y > 2)
                menuPipSound.Play(CameraManager.Instance.sceneCam.transform.position);
        }
    }

    Vector3 newPos()
    {
        int i = GetIndex();
        return new Vector3(positions[i].x, positions[i].y, -0.2f);
    }

    public int GetIndex()
    {
        //this converts the 2d coordinates of the cursor to the proper 1d list to access the proper position;
        int i = (int)index.y;
        if (index.y >= 3)
            i += (int)index.x;
        if (index.y >= 4)
            i++;

        return i;
    }
}
