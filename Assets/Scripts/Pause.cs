using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool activatePause = true;

    bool canPause = true;
    bool pressed = false;
    bool paused = false;

    float oldTimeScale = 1f;

    public Animator fadeAnim;
    public Animator menuAnim;
    public GameObject wealth;
    Animator wealthAnim;
    WealthCounter wealthCounter;

    // Start is called before the first frame update
    void Start()
    {
        wealthAnim = wealth.GetComponent<Animator>();
        wealthCounter = wealth.GetComponent<WealthCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        canPause = CheckCanPause();

        if (Input.GetButton("Pause") && canPause)
        {
            canPause = false;
            pressed = true;

            if (paused)
                UnPauseGame();
            else
                PauseGame();
        }

        if (!Input.GetButton("Pause") && pressed)
            pressed = false;

        fadeAnim.SetBool("Paused", paused);
        menuAnim.SetBool("Paused", paused);
            
    }
    bool CheckCanPause()
    {
        bool check = true;
        if (pressed)
            check = false;
        return check && activatePause;
    }
    void PauseGame()
    {
        paused = true;
        PlayerManager.Instance.paused = true;

        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;

        PlayerManager.Instance.canMove = false;

        wealthAnim.ResetTrigger("Enter");
        wealthAnim.ResetTrigger("Leave");
        wealthAnim.SetTrigger("Enter");
        wealthCounter.isVisible = true;
        wealthCounter.visibleTimer = 3f; //this must be any value above 0 so WealthCounter doesn't immediately hide it again
    }

    void UnPauseGame()
    {
        paused = false;
        PlayerManager.Instance.paused = false;

        Time.timeScale = oldTimeScale;

        PlayerManager.Instance.canMove = true;

        wealthAnim.ResetTrigger("Enter");
        wealthAnim.ResetTrigger("Leave");
        wealthAnim.SetTrigger("Leave");
        wealthCounter.isVisible = false;
        wealthCounter.visibleTimer = 0;
    }

    public static void SetPause(bool value)
    {
        activatePause = value;
    }
}
