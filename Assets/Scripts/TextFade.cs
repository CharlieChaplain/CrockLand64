using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{
    public float speed;

    TextMeshProUGUI text;
    Color initCol;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        initCol = text.color;
    }
    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        text.color = new Color(initCol.r, initCol.g, initCol.b, alpha);
    }
}
