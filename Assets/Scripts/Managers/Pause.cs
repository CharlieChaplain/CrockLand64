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

    Menu currentMenu;

    public List<Menu> allMenus;

    // Start is called before the first frame update
    void Start()
    {
        currentMenu = allMenus[0];
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

        if (paused && currentMenu == allMenus[0] && Input.GetButtonDown("Cancel"))
            UnPauseGame();

        if (!Input.GetButton("Pause") && pressed)
            pressed = false;

        fadeAnim.SetBool("Paused", paused);

        if (paused)
        {
            currentMenu.Logic();
        }
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

        currentMenu = allMenus[0];
        currentMenu.cursor.ResetMenu();

        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;

        PlayerManager.Instance.canMove = false;

        currentMenu.Enter();
    }

    void UnPauseGame()
    {
        paused = false;
        PlayerManager.Instance.paused = false;

        Time.timeScale = oldTimeScale;

        PlayerManager.Instance.canMove = true;

        currentMenu.Leave();
    }

    public static void SetPause(bool value)
    {
        activatePause = value;
    }
    /// <summary>
    /// Changes the current menu displayed based on a given index
    /// </summary>
    /// <param name="index">0 = main pause menu; 1 = options root menu</param>
    public void ChangeMenu(int index)
    {
        currentMenu.Leave();
        currentMenu = allMenus[index];
        currentMenu.Enter();
    }
}
