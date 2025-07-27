using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRAxisDebugging : MonoBehaviour
{
    public InputActionProperty inputAction;
    private InputActionProperty previousInputAction;

    public bool IsPressed;
    public Vector2 ReadValueVector2;

    void OnEnable()
    {
        if (inputAction.action != null)
        {
            inputAction.action.Enable();
            previousInputAction = inputAction;
        }
    }

    void OnDisable()
    {
        inputAction.action?.Disable();
        previousInputAction.action?.Disable();
    }

    void Update()
    {
        if (inputAction != previousInputAction)
        {
            previousInputAction.action?.Disable();
            previousInputAction = inputAction;
            inputAction.action?.Enable();
        }

        if (inputAction.action != null)
        {
            IsPressed = inputAction.action.IsPressed();

            if (inputAction.action.activeControl?.valueType == typeof(Vector2))
                ReadValueVector2 = inputAction.action.ReadValue<Vector2>();
            else
                ReadValueVector2 = Vector2.zero;
        }
    }
}
