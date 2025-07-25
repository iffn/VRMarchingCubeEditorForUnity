using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRMarchingCubeEditor : PlaymodeEditor
{
    [SerializeField] InputActionProperty editAction;
    [SerializeField] InputActionProperty scaleToolOnY;
    [SerializeField] InputActionProperty subtractButton;
    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float scaleThreshold = 0.01f;

    Transform toolOrigin;

    void OnEnable()
    {
        editAction.action.Enable();
        scaleToolOnY.action.Enable();
        subtractButton.action.Enable();

        if(placeableByClick)
            placeableByClick.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        editAction.action.Disable();
        scaleToolOnY.action.Disable();
        subtractButton.action.Disable();

        placeableByClick.gameObject.SetActive(false);
    }

    void Start()
    {
        // Use Setup instead
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

        if (Mathf.Abs(scaleValue) > scaleThreshold)
        {
            placeableByClick.transform.localScale *= 1 + scaleValue * scaleSpeed * Time.deltaTime;
        }
    }

    public void Setup(
        Transform toolOrigin,
        MarchingCubesController linkedMarchingCubeController,
        EditShape placeableByClick
        )
    {
        this.toolOrigin = toolOrigin;
        this.linkedMarchingCubeController = linkedMarchingCubeController;
        this.placeableByClick = placeableByClick;

        placeableByClick.gameObject.SetActive(enabled);

        InitializeController();
    }
}
