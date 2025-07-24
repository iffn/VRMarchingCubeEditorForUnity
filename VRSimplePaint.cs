using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSimplePaint : PlaymodeEditor
{
    [SerializeField] private InputActionProperty editAction;
    [SerializeField] private InputActionProperty scaleToolOnY;
    [SerializeField] private InputActionProperty increasePlayerSizeButton;
    [SerializeField] private InputActionProperty decreasePlayerSizeButton;
    [SerializeField] private InputActionProperty subtractButton;
    [SerializeField] CharacterController linkedCharacterController;

    float defaultHeight;
    float defaultRadius;
    Vector3 defaultCenter;

    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float scaleThreshold = 0.01f;
    [SerializeField] Transform toolOrigin;

    float CurrentScale
    {
        get
        {
            return linkedCharacterController.transform.localScale.x;
        }
    }

    void OnEnable()
    {
        editAction.action.Enable();
        scaleToolOnY.action.Enable();
        increasePlayerSizeButton.action.Enable();
        decreasePlayerSizeButton.action.Enable();
        subtractButton.action.Enable();
    }

    void OnDisable()
    {
        editAction.action.Disable();
        scaleToolOnY.action.Disable();
        increasePlayerSizeButton.action.Disable();
        decreasePlayerSizeButton.action.Disable();
        subtractButton.action.Disable();
    }
    void Start()
    {
        InitializeController();

        initializeVREditor();

        placeableByClick.transform.gameObject.SetActive(true);
    }

    void Update()
    {
        placeableByClick.transform.position = toolOrigin.position;

        if (editAction.action.IsPressed())
        {
            BaseModificationTools.IVoxelModifier modifier = subtractButton.action.IsPressed() ?
                new BaseModificationTools.SubtractShapeModifier() : new BaseModificationTools.AddShapeModifier();

            linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
        }

        float scaleValue = scaleToolOnY.action.ReadValue<Vector2>().y;

        if(Mathf.Abs(scaleValue) > scaleThreshold)
        {
            placeableByClick.transform.localScale *= 1 + scaleValue * scaleSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.KeypadPlus) || increasePlayerSizeButton.action.IsPressed())
        {
            scalePlayer(CurrentScale * (1 + 0.25f * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.KeypadMinus) || decreasePlayerSizeButton.action.IsPressed())
        {
            scalePlayer(CurrentScale * (1 - 0.2f * Time.deltaTime));
        }
    }

    void initializeVREditor()
    {
        defaultHeight = linkedCharacterController.height;
        defaultRadius = linkedCharacterController.radius;
        defaultCenter = linkedCharacterController.center;
    }

    void scalePlayer(float scale)
    {
        float scaleFactor = scale / CurrentScale; //Needs to be done before changing it

        linkedCharacterController.transform.localScale = scale * Vector3.one;

        linkedCharacterController.height = scale * defaultHeight;
        linkedCharacterController.radius = scale * defaultRadius;
        linkedCharacterController.center = scale * defaultCenter;

        placeableByClick.transform.localScale *= scaleFactor;
    }
}
