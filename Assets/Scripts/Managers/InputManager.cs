using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool keyboardInput = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckController())
        {
            keyboardInput = false;
        }
        else if (Input.anyKeyDown)
        {
            keyboardInput = true;
        }
    }

    bool CheckController()
    {
        bool joystick = false;
        if (Input.GetAxis("HorizontalLook") > 0)
            joystick = true;
        if (Input.GetAxis("VerticalLook") > 0)
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton4))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton5))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton6))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton8))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton10))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton11))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton12))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton13))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton14))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton15))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton16))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton17))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton18))
            joystick = true;
        if (Input.GetKeyDown(KeyCode.JoystickButton19))
            joystick = true;
        return joystick;
    }
}
