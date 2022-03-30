using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Player Unity Events---------------------------------------------------------------------
[Serializable]
public class MoveInputEvent : UnityEvent<float, float> { }
[Serializable]
public class JumpInputEvent : UnityEvent<bool> { }
[Serializable]
public class PunchInputEvent : UnityEvent<bool> { }
[Serializable]
public class PunchHeldEvent : UnityEvent<bool> { }
[Serializable]
public class CrouchInputEvent : UnityEvent<bool> { }
[Serializable]
public class ChargeInputEvent : UnityEvent<bool> { }

// UI Unity Events--------------------------------------------------------------------------
[Serializable]
public class PauseInputEvent : UnityEvent<bool> { }
[Serializable]
public class ConfirmInputEvent : UnityEvent<bool> { }
[Serializable]
public class CancelInputEvent : UnityEvent<bool> { }

public class _InputControl : MonoBehaviour
{
    _Controls controls;
    public MoveInputEvent moveInputEvent;
    public JumpInputEvent jumpInputEvent;
    public PunchInputEvent punchInputEvent; //for initial press
    public PunchHeldEvent punchHeldEvent; //for holding punch, to facilitate charged throws
    public CrouchInputEvent crouchInputEvent;
    public ChargeInputEvent chargeInputEvent;

    public PauseInputEvent pauseInputEvent;
    public ConfirmInputEvent confirmInputEvent;
    public CancelInputEvent cancelInputEvent;

    private void Awake()
    {
        controls = new _Controls();
    }

    private void OnEnable()
    {
        controls.EditableControls.Enable();

        // player subscriptions--------------------------------------------------
        controls.EditableControls.Movement.performed += OnMove;
        controls.EditableControls.Movement.canceled += OnMove;

        controls.EditableControls.Jump.started += OnJump;
        controls.EditableControls.Jump.canceled += OnJump;

        controls.EditableControls.Punch.started += OnPunch;
        controls.EditableControls.Punch.performed += OnPunchHeld;
        controls.EditableControls.Punch.canceled += OnPunchHeld;

        controls.EditableControls.Crouch.performed += OnCrouch;
        controls.EditableControls.Crouch.canceled += OnCrouch;

        controls.EditableControls.Charge.started += OnCharge;

        // UI subscriptions------------------------------------------------------
        controls.EditableControls.Pause.started += OnPause;
        controls.EditableControls.Submit.started += OnConfirm;
        controls.EditableControls.Cancel.started += OnCancel;
    }

    private void OnDisable()
    {
        controls.EditableControls.Disable();
    }

    public _Controls GetControl()
    {
        return controls;
    }

    // Player methods---------------------------------------------------------------------
    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        moveInputEvent.Invoke(input.x, input.y);
    }
    private void OnJump(InputAction.CallbackContext obj)
    {
        //used to turn off and on lower gravity if jump is held
        if (obj.started)
            jumpInputEvent.Invoke(true);
        else if (obj.canceled)
            jumpInputEvent.Invoke(false);
    }

    private void OnPunch(InputAction.CallbackContext obj)
    {
        punchInputEvent.Invoke(obj.performed);
    }
    private void OnPunchHeld(InputAction.CallbackContext obj)
    {
        punchHeldEvent.Invoke(obj.performed);
    }

    private void OnCrouch(InputAction.CallbackContext obj)
    {
        crouchInputEvent.Invoke(obj.performed);
    }

    private void OnCharge(InputAction.CallbackContext obj)
    {
        chargeInputEvent.Invoke(obj.started);
    }

    // UI methods---------------------------------------------------------------------
    private void OnPause(InputAction.CallbackContext obj)
    {
        pauseInputEvent.Invoke(obj.started);
    }
    private void OnConfirm(InputAction.CallbackContext obj)
    {
        confirmInputEvent.Invoke(obj.started);
    }
    private void OnCancel(InputAction.CallbackContext obj)
    {
        cancelInputEvent.Invoke(obj.started);
    }

}
