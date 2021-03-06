using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public static bool activatePause = true;
    _Controls controls;

    protected bool canPause = true;
    protected bool pressed = false;
    protected bool paused = false;
    protected bool oldCanMove; //what playermanager.canmove was before the pause. Prevents gaining control during cutscenes and stuff just by pausing

    float oldTimeScale = 1f;

    public Animator fadeAnim;

    protected Menu currentMenu;

    public List<Menu> allMenus;

    public AudioSource pauseMusic;

    public PlaySound menuOpenSound;
    public PlaySound menuCloseSound;

    private void Awake()
    {
        controls = InputManager.controls;
    }

    private void OnEnable()
    {
        StartCoroutine(EnableControls());
    }
    IEnumerator EnableControls()
    {
        yield return new WaitForEndOfFrame();

        controls = InputManager.controls;

        // UI subscriptions------------------------------------------------------
        controls.EditableControls.Move.performed += PauseCursorMovement;
        controls.EditableControls.Move.canceled += PauseCursorMovement;
        controls.EditableControls.Pause.started += PauseListener;
        controls.EditableControls.Confirm.started += PauseConfirm;
        controls.EditableControls.Cancel.started += PauseCancel;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentMenu = allMenus[0];

        foreach(Menu menu in allMenus)
        {
            menu.pause = this;
        }

        activatePause = true;
    }

    // Update is called once per frame
    void Update()
    {
        pauseMusic.volume = SoundManager.Instance.musicVolume;

        canPause = CheckCanPause();

        fadeAnim.SetBool("Paused", paused);
    }

    public void PauseListener(InputAction.CallbackContext obj)
    {
        if (!canPause)
            return;
        if (paused)
            UnPauseGame();
        else
            PauseGame();
    }

    //these three pass down the unity event that gets called during input to the current menu so it can perform it's logic.
    public void PauseCursorMovement(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        if (input == Vector2.zero)
            pressed = false;
        if (!paused || pressed)
            return;
        if (input != Vector2.zero)
            pressed = true;
        currentMenu.CursorMovement(input);
    }
    public void PauseConfirm(InputAction.CallbackContext obj)
    {
        currentMenu.Confirm();
    }
    public void PauseCancel(InputAction.CallbackContext obj)
    {
        currentMenu.Cancel();
    }

    //this method is extra complication now but is possibly futureproofing. Add more conditionals here if needed
    bool CheckCanPause()
    {
        return activatePause;
    }
    void PauseGame()
    {
        paused = true;
        PlayerManager.Instance.paused = true;
        oldCanMove = PlayerManager.Instance.canMove;
        PlayerManager.Instance.canMove = false;

        currentMenu = allMenus[0];
        currentMenu.cursor.ResetMenu();

        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;

        SoundManager.Instance.music.Pause();
        pauseMusic.PlayDelayed(0.2f);

        menuOpenSound.Play(CameraManager.Instance.sceneCam.transform);

        currentMenu.Enter();
    }

    public void UnPauseGame()
    {
        paused = false;
        PlayerManager.Instance.paused = false;
        PlayerManager.Instance.canMove = oldCanMove;

        Time.timeScale = oldTimeScale;

        SoundManager.Instance.music.UnPause();
        pauseMusic.Stop();

        menuCloseSound.Play(CameraManager.Instance.sceneCam.transform);

        currentMenu.Leave();
    }
    public static void SetPause(bool value)
    {
        activatePause = value;
    }
    /// <summary>
    /// Changes the current menu displayed based on a given index
    /// </summary>
    /// <param name="index">0 = main pause menu; 1 = options root menu; 2 = treasure root menu; 3 = audio menu</param>
    public void ChangeMenu(int index)
    {
        currentMenu.Leave();
        currentMenu = allMenus[index];
        currentMenu.Enter();
    }

    //checks if currentMenu is the one indicated by the input
    public bool CheckMenu(int i)
    {
        return currentMenu == allMenus[i];
    }

    public Menu GetCurrentMenu()
    {
        return currentMenu;
    }
}
