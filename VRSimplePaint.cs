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
    [SerializeField] private InputActionProperty scaleActionOnY;

    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float scaleThreshold = 0.01f;
    [SerializeField] Transform toolOrigin;

    void OnEnable()
    {
        editAction.action.Enable();
    }

    void OnDisable()
    {
        editAction.action.Disable();
    }
    void Start()
    {
        InitializeController();
    }

    void Update()
    {
        placeableByClick.transform.position = toolOrigin.position;

        if (editAction.action.IsPressed())
        {
            BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.AddShapeModifier();
            linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
        }

        float scaleValue = scaleActionOnY.action.ReadValue<Vector2>().y;

        if(Mathf.Abs(scaleValue) > scaleThreshold)
        {
            placeableByClick.transform.localScale *= 1 + scaleValue * scaleSpeed * Time.deltaTime;
        }
    }
}
