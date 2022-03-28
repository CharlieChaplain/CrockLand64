using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleFileImages : MonoBehaviour
{
    public GameObject newFile;
    public GameObject gameData;
    public List<RectTransform> allImages;
    public List<Vector2> allInitPos;

    public bool newGame;
    public TextMeshProUGUI newGameText;

    private void Start()
    {
        allInitPos.Clear();
        foreach(RectTransform t in allImages)
        {
            allInitPos.Add(t.anchoredPosition);
        }

        newFile.SetActive(newGame);
        gameData.SetActive(!newGame);
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

    public void EraseFile()
    {
        newGame = true;
        StartCoroutine("Explode");
        StartCoroutine("GrowText", 24f);
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

    IEnumerator Explode()
    {
        Vector2[] velocities = new Vector2[allImages.Count];
        Vector2 grav = new Vector2(0, -0.4f);
        for(int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 5f));
        }

        for (float f = 0; f < 2f; f += Time.deltaTime)
        {
            for (int i = 0; i < velocities.Length; i++)
            {
                allImages[i].anchoredPosition += velocities[i];
                velocities[i] += grav;
            }
            yield return null;
        }
    }

    IEnumerator GrowText(float pt)
    {
        newGameText.fontSize = 0.5f;
        newFile.SetActive(true);
        Debug.Log(newGameText);
        float timer = 1f;
        for(float f = 0; f < timer; f += Time.deltaTime)
        {
            newGameText.fontSize = Mathf.Lerp(0.5f, pt, f / timer);
            yield return null;
        }
        newGameText.fontSize = pt;
        gameData.SetActive(false);
    }
}
