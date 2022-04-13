using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static _Controls controls;

    public static event Action rebindComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            controls = GetComponent<_InputControl>().GetControl();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool keyboardInput = true;

    public void OnSceneLoad()
    {
        //controls = GetComponent<_InputControl>().GetControl();
    }

    public static void StartRebind(string actionName, int bindIndex, GameObject verifyMenu, bool keyboard)
    {
        InputAction action = controls.asset.FindAction(actionName);
        if (action == null || bindIndex >= action.bindings.Count)
        {
            Debug.Log("action/binding not found");
        }

        if (action.bindings[bindIndex].isComposite)
        {
            var firstIndex = bindIndex + 1;
            if (firstIndex < action.bindings.Count && action.bindings[firstIndex].isPartOfComposite)
                DoRebind(action, firstIndex, verifyMenu, true, keyboard);

        }
        else
        {
            DoRebind(action, bindIndex, verifyMenu, false, keyboard);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindIndex, GameObject verifyMenu, bool allComposite, bool keyboard)
    {
        if (actionToRebind == null || bindIndex < 0)
            return;


        //brings up the verify menu
        verifyMenu.GetComponent<Animator>().SetBool("Visible", true);
        TextMeshProUGUI status = verifyMenu.GetComponentInChildren<TextMeshProUGUI>();

        // If it's a part binding, get the name of the part to display instead of the root action
        var partName = actionToRebind.name;
        if (actionToRebind.bindings[bindIndex].isPartOfComposite)
            partName = actionToRebind.bindings[bindIndex].name;

        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindIndex);


        status.text = "Press a " + rebind.expectedControlType + " for " + partName;

        rebind.OnComplete(operation =>
        {
            verifyMenu.GetComponent<Animator>().SetBool("Visible", false);

            actionToRebind.Enable();
            operation.Dispose();

            //if the bind is part of a composite, then start it again for the next part
            if (allComposite)
            {
                var nextIndex = bindIndex + 1;
                if (nextIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextIndex].isPartOfComposite)
                    DoRebind(actionToRebind, nextIndex, verifyMenu, true, keyboard);
            }

            //saves the binding once it's been rebound
            SaveBindingOverride(actionToRebind);

            //if the action being rebound is punching, also rebinds the punch hold action
            if (actionToRebind.name == "Punch")
            {
                InputAction punchHeldAction = controls.asset.FindAction("PunchHeld");
                punchHeldAction.ApplyBindingOverride(bindIndex, actionToRebind.bindings[bindIndex].effectivePath);
                SaveBindingOverride(punchHeldAction);
            }

            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();
            verifyMenu.GetComponent<Animator>().SetBool("Visible", false);
        });

        rebind.WithControlsExcluding("Mouse");
        if (keyboard)
            rebind.WithControlsExcluding("<Gamepad>");
        else
            rebind.WithControlsExcluding("<Keyboard>");

        rebind.Start();
    }

    public static string GetControlName(string actionName, int binding)
    {
        return controls.asset.FindAction(actionName).GetBindingDisplayString(binding);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for(int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        InputAction action = controls.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }

    public static void ResetBinding(string actionName, int bindIndex)
    {
        InputAction action = controls.asset.FindAction(actionName);

        if (action.bindings[bindIndex].isComposite)
        {
            for(int i = bindIndex; i < action.bindings.Count && (action.bindings[i].isComposite || action.bindings[i].isPartOfComposite); i++)
                action.RemoveBindingOverride(i);
        } else
            action.RemoveBindingOverride(bindIndex);
        SaveBindingOverride(action);
    }
}
