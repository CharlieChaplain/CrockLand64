using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFileImages : MonoBehaviour
{
    public List<RectTransform> allImages;

    public List<Vector2> allInitPos;

    private void Start()
    {
        allInitPos.Clear();
        foreach(RectTransform t in allImages)
        {
            allInitPos.Add(t.anchoredPosition);
        }
    }

    public void ResetPositions()
    {
        for(int i = 0; i < allImages.Count; i++)
        {
            allImages[i].anchoredPosition = allInitPos[i];
        }
    }

    public void StartTremble()
    {
        StopCoroutine("Tremble");
        StartCoroutine("Tremble");
    }

    public void StopTremble()
    {
        StopCoroutine("Tremble");
        ResetPositions();
    }

    IEnumerator Tremble()
    {
        float variance = 1f;
        while (true)
        {
            for (int i = 0; i < allImages.Count; i++)
            {
                Vector2 newPos = new Vector2(Random.Range(-variance, variance) + allInitPos[i].x,
                    Random.Range(-variance, variance) + allInitPos[i].y);
                allImages[i].anchoredPosition = newPos;
            }
            yield return null;
        }
    }
}
