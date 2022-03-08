using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrolling : MonoBehaviour
{
    public float speed;

    public GameObject scrollingElement;
    float newPos;
    Vector2 elementSize;

    GameObject currentElement;
    GameObject nextElement;

    Vector2 currentInitPos;
    Vector2 nextInitPos;

    // Start is called before the first frame update
    void Start()
    {
        newPos = -Mathf.Sign(speed);

        elementSize = scrollingElement.GetComponent<RectTransform>().sizeDelta;

        currentElement = scrollingElement;

        nextElement = Instantiate(scrollingElement, transform);
        nextElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newPos * elementSize.y);

        currentInitPos = currentElement.GetComponent<RectTransform>().anchoredPosition;
        nextInitPos = nextElement.GetComponent<RectTransform>().anchoredPosition;

        StartCoroutine("Scroll");
    }

    IEnumerator Scroll()
    {
        while (true)
        {
            float timer = 0;
            while (Mathf.Abs(timer) < elementSize.y)
            {
                Vector2 nextPosition = new Vector2(0, timer);
                currentElement.GetComponent<RectTransform>().anchoredPosition = currentInitPos + nextPosition;
                nextElement.GetComponent<RectTransform>().anchoredPosition = nextInitPos + nextPosition;
                timer += Time.deltaTime * speed;
                yield return null;
            }
            yield return null;
        }
    }
}
