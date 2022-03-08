using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WealthCounterTitle : MonoBehaviour
{
    public List<TextMeshProUGUI> counterText;
    public int fileNum;

    // Start is called before the first frame update
    void Start()
    {
        LoadWealthInfo(fileNum);
    }

    // Update is called once per frame
    void Update()
    {
        PopulateText();
    }

    void PopulateText()
    {
        //number is set here
        string number = "30";

        while (number.Length != counterText.Count)
            number = "-" + number;

        for (int i = 0; i < counterText.Count; i++)
        {
            char digit = number[number.Length - i - 1];
            if (digit == '-')
                counterText[i].text = "";
            else
                counterText[i].text = digit.ToString();
        }
    }

    void LoadWealthInfo(int file)
    {
        //put loading code here
    }
}
