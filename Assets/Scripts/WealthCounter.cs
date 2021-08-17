using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WealthCounter : MonoBehaviour
{
    public List<TextMeshProUGUI> counterText;
    Animator anim;

    public bool isVisible = false;
    public float visibleTimer = 0;

    int wealthToAdd;

    bool coroutOn = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        PopulateText();

        CheckWealth();
    }

    void CheckWealth()
    {
        if (PlayerManager.Instance.displayWealth != PlayerManager.Instance.wealth)
        {
            DisplayWealth();
        }

        if (visibleTimer > 0)
        {
            visibleTimer -= Time.deltaTime;
        }

        if (isVisible && visibleTimer <= 0)
        {
            HideWealth();
        }
    }

    void PopulateText()
    {
        string number = PlayerManager.Instance.displayWealth.ToString();

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

    public void DisplayWealth()
    {
        if (!isVisible)
        {
            anim.SetTrigger("Enter");
            isVisible = true;
        }
        if (!coroutOn)
            StartCoroutine("AddWealth");

        visibleTimer = 3f;
    }

    public void HideWealth()
    {
        anim.SetTrigger("Leave");
        isVisible = false;
    }

    //jumpstarts adding money to your total wealth counter
    IEnumerator AddWealth()
    {
        coroutOn = true;
        wealthToAdd = PlayerManager.Instance.wealth - PlayerManager.Instance.displayWealth;

        while (wealthToAdd != 0)
        {
            wealthToAdd = PlayerManager.Instance.wealth - PlayerManager.Instance.displayWealth;

            //the time until this coroutine loops again. If wealthToAdd is a larger number, the increment will be up to 10x faster
            float time = 0.1f / (((Mathf.Clamp(Mathf.Abs(wealthToAdd), 1f, 100f) - 1f) / 10f) + 1f);

            int increment = 0;
            if (wealthToAdd > 0)
                increment = 1;
            else if (wealthToAdd < 0)
                increment = -1;

            PlayerManager.Instance.displayWealth += increment;

            yield return new WaitForSeconds(time);
        }

        coroutOn = false;
    }
}
