using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureSlot : MonoBehaviour
{
    public Vector2 menuPos;
    float offset;

    GameObject treasureImage;

    float timer;
    bool unlocked = false;
    public Sprite unlockedTreasure;

    // Start is called before the first frame update
    void Start()
    {
        menuPos.x = (GetComponent<RectTransform>().localPosition.x / 32f) + 4f;
        menuPos.y = 4f - ((GetComponent<RectTransform>().localPosition.y / 32f) + 2f);

        offset = menuPos.x - menuPos.y;
        treasureImage = transform.Find("treasureImage").gameObject;

        timer = Random.Range(10f, 30f);

        if (TreasureMaster.Instance.QueryTreasure((int)menuPos.x, (int)menuPos.y))
        {
            treasureImage.GetComponent<Image>().sprite = unlockedTreasure;
            treasureImage.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            treasureImage.GetComponent<Image>().color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        timer -= Time.unscaledDeltaTime;

        if(timer <= 0)
        {
            if(Random.value <= 0.2f && !unlocked)
                StartCoroutine("Wobble");
            timer = Random.Range(10f, 30f);
        }
    }

    public void CollectTreasure()
    {
        unlocked = true;
        treasureImage.GetComponent<Image>().sprite = unlockedTreasure;
        treasureImage.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        treasureImage.GetComponent<Image>().color = Color.white;
    }

    private void Move()
    {
        Vector3 movingPos = new Vector3(0, Mathf.Sin((Time.unscaledTime - (offset / 10f)) * 3f) - 1f, -.1f);
        treasureImage.GetComponent<RectTransform>().localPosition = movingPos;
    }

    IEnumerator Wobble()
    {
        for(float i = 0; i < 2; i += 0.2f)
        {
            treasureImage.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, (Mathf.Sin(i * Mathf.PI) - 1f) * 10f);

            yield return null;
        }

        treasureImage.GetComponent<RectTransform>().localRotation = Quaternion.identity;
    }
}
