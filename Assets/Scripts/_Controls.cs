//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/_Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @_Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @_Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""_Controls"",
    ""maps"": [
        {
            ""name"": ""EditableControls"",
            ""id"": ""435cbb5e-7964-4aeb-875e-87ce2875d3e2"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""7f5f62b4-3577-4554-9e2e-c5f69ca3b40f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LookMovement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""576f505c-b064-4809-9202-6b50a5262dc5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""InvertVector2(invertX=false),StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ec4b0e2f-ba7e-4551-8871-e8a0572ab188"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""be47249c-e95d-44a0-8cdf-02a69425802c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PunchHeld"",
                    ""type"": ""Button"",
                    ""id"": ""5172f321-af58-4218-aa68-66804822aef4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.8)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Charge"",
                    ""type"": ""Button"",
                    ""id"": ""94ff0bcd-9d13-4301-bd4e-ea64c24b29bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""6ed74863-54cf-4444-b330-d12be56ff2d1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""67bae93a-b54b-480e-8cb3-bb66be8d2f4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""74233f3a-edd3-4ec2-94fe-618dd4dee4a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""6bbba83c-20fe-4089-b655-410ccbebf702"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""9cf494a7-da20-4dc0-b733-eff128b1d160"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a9478f58-9cba-4a33-ad6e-dc1a3097d657"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8431bd90-5e89-4212-8962-d78c8dcfb2b6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""40386df5-58fd-48f2-b999-eb2b1b0ca889"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""64a5bc42-a1a7-4cc4-a6aa-0ea77bcb817f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8100fed0-e83d-4ebe-b7a9-faa90095b297"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49001f09-f74f-419b-81c6-a69ef225c2ef"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d3da09a-2b62-4282-9b72-6613cbbfb0a8"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4b52fb7-ba58-4303-9ddc-4099c1fd8fe5"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb6c4563-6b24-457a-b192-f5fafb2ff128"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c315fce0-5641-4e2c-9c17-f7e409ae7166"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c227e3be-e513-4242-aadc-5a61c2a53184"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a10af779-88ba-4960-bbb3-9884a5d900d2"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7139768-c91a-4df5-a8ed-83a726cfd943"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""188c7167-bd69-45e0-8a09-0678650ea93e"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""734181f5-7b81-409a-84da-3668b07fb024"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8306e69-916e-42b2-bb61-2eb39cc2ea33"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5be86494-c6f9-46fc-8713-cdd41fd798f4"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed6dda65-ee0a-4ea2-88e2-f5272be8bba5"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1ccf3c7-77b2-4d5d-b36b-4ca0f71e6e3f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e834a483-9a53-4d9f-804a-7396f83d4b29"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b49b2d8a-025a-460e-bae9-513f156e2978"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b3f88b1-40c8-4243-8d48-4a364f33ea93"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PunchHeld"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""407dcd3e-066b-49c3-9bd4-421a03921b96"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PunchHeld"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // EditableControls
        m_EditableControls = asset.FindActionMap("EditableControls", throwIfNotFound: true);
        m_EditableControls_Move = m_EditableControls.FindAction("Move", throwIfNotFound: true);
        m_EditableControls_LookMovement = m_EditableControls.FindAction("LookMovement", throwIfNotFound: true);
        m_EditableControls_Jump = m_EditableControls.FindAction("Jump", throwIfNotFound: true);
        m_EditableControls_Punch = m_EditableControls.FindAction("Punch", throwIfNotFound: true);
        m_EditableControls_PunchHeld = m_EditableControls.FindAction("PunchHeld", throwIfNotFound: true);
        m_EditableControls_Charge = m_EditableControls.FindAction("Charge", throwIfNotFound: true);
        m_EditableControls_Crouch = m_EditableControls.FindAction("Crouch", throwIfNotFound: true);
        m_EditableControls_Pause = m_EditableControls.FindAction("Pause", throwIfNotFound: true);
        m_EditableControls_Confirm = m_EditableControls.FindAction("Confirm", throwIfNotFound: true);
        m_EditableControls_Cancel = m_EditableControls.FindAction("Cancel", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // EditableControls
    private readonly InputActionMap m_EditableControls;
    private IEditableControlsActions m_EditableControlsActionsCallbackInterface;
    private readonly InputAction m_EditableControls_Move;
    private readonly InputAction m_EditableControls_LookMovement;
    private readonly InputAction m_EditableControls_Jump;
    private readonly InputAction m_EditableControls_Punch;
    private readonly InputAction m_EditableControls_PunchHeld;
    private readonly InputAction m_EditableControls_Charge;
    private readonly InputAction m_EditableControls_Crouch;
    private readonly InputAction m_EditableControls_Pause;
    private readonly InputAction m_EditableControls_Confirm;
    private readonly InputAction m_EditableControls_Cancel;
    public struct EditableControlsActions
    {
        private @_Controls m_Wrapper;
        public EditableControlsActions(@_Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_EditableControls_Move;
        public InputAction @LookMovement => m_Wrapper.m_EditableControls_LookMovement;
        public InputAction @Jump => m_Wrapper.m_EditableControls_Jump;
        public InputAction @Punch => m_Wrapper.m_EditableControls_Punch;
        public InputAction @PunchHeld => m_Wrapper.m_EditableControls_PunchHeld;
        public InputAction @Charge => m_Wrapper.m_EditableControls_Charge;
        public InputAction @Crouch => m_Wrapper.m_EditableControls_Crouch;
        public InputAction @Pause => m_Wrapper.m_EditableControls_Pause;
        public InputAction @Confirm => m_Wrapper.m_EditableControls_Confirm;
        public InputAction @Cancel => m_Wrapper.m_EditableControls_Cancel;
        public InputActionMap Get() { return m_Wrapper.m_EditableControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(EditableControlsActions set) { return set.Get(); }
        public void SetCallbacks(IEditableControlsActions instance)
        {
            if (m_Wrapper.m_EditableControlsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnMove;
                @LookMovement.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnLookMovement;
                @LookMovement.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnLookMovement;
                @LookMovement.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnLookMovement;
                @Jump.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnJump;
                @Punch.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunch;
                @Punch.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunch;
                @Punch.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunch;
                @PunchHeld.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunchHeld;
                @PunchHeld.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunchHeld;
                @PunchHeld.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPunchHeld;
                @Charge.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCharge;
                @Charge.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCharge;
                @Charge.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCharge;
                @Crouch.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCrouch;
                @Pause.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnPause;
                @Confirm.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnConfirm;
                @Confirm.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnConfirm;
                @Confirm.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnConfirm;
                @Cancel.started -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_EditableControlsActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_EditableControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @LookMovement.started += instance.OnLookMovement;
                @LookMovement.performed += instance.OnLookMovement;
                @LookMovement.canceled += instance.OnLookMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Punch.started += instance.OnPunch;
                @Punch.performed += instance.OnPunch;
                @Punch.canceled += instance.OnPunch;
                @PunchHeld.started += instance.OnPunchHeld;
                @PunchHeld.performed += instance.OnPunchHeld;
                @PunchHeld.canceled += instance.OnPunchHeld;
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Confirm.started += instance.OnConfirm;
                @Confirm.performed += instance.OnConfirm;
                @Confirm.canceled += instance.OnConfirm;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }
        }
    }
    public EditableControlsActions @EditableControls => new EditableControlsActions(this);
    public interface IEditableControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLookMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnPunch(InputAction.CallbackContext context);
        void OnPunchHeld(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnConfirm(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
}
