using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRMarchingCubeEditor : PlaymodeEditor, OptionUser
{
    [SerializeField] InputActionProperty editAction;
    [SerializeField] InputActionProperty scaleToolOnY;
    [SerializeField] InputActionProperty subtractButton;
    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float scaleThreshold = 0.01f;
    [SerializeField] OptionSelector toolSelector;

    [SerializeField] AnimationCurve paintCurve;
    [SerializeField] byte clearColor = 0;
    [SerializeField] byte grassColor = 200;
    [SerializeField] byte pathColor = 255;


    Transform toolOrigin;
    Tools currentTool = Tools.addAndRemove;

    enum Tools
    {
        addAndRemove,
        smooth,
        bridge,
        paintGrass,
        paintPath,
        paintClear
    }

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

        try
        {
            placeableByClick.gameObject.SetActive(false);
        }
        catch(System.Exception e)
        {
            // Ignore, gets thrown when the object is destroyed when exiting play mode
        }
    }

    void Start()
    {
        // Use Setup instead
    }

    void Update()
    {
        placeableByClick.transform.position = toolOrigin.position;

        switch (currentTool)
        {
            case Tools.addAndRemove:
                AddAndRemoveUpdate();
                break;
            case Tools.smooth:
                break;
            case Tools.bridge:
                break;
            case Tools.paintGrass:
                PaintAlpha(grassColor);
                break;
            case Tools.paintPath:
                PaintAlpha(pathColor);
                break;
            case Tools.paintClear:
                PaintAlpha(clearColor);
                break;
            default:
                break;
        }

        float scaleValue = scaleToolOnY.action.ReadValue<Vector2>().y;

        if (Mathf.Abs(scaleValue) > scaleThreshold)
        {
            placeableByClick.transform.localScale *= 1 + scaleValue * scaleSpeed * Time.deltaTime;
        }
    }

    public void SaveMarchingCubeData()
    {
        SaveData();
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

        toolSelector.Setup(this, (int)currentTool);
    }

    public void SelectOption(OptionSelector selector, int optionIndex)
    {
        if(selector == toolSelector)
        {
            currentTool = (Tools)optionIndex;
        }
    }

    void AddAndRemoveUpdate()
    {
        BaseModificationTools.IVoxelModifier modifier = subtractButton.action.IsPressed() ?
            new BaseModificationTools.SubtractShapeModifier() : new BaseModificationTools.AddShapeModifier();

        linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
    }

    void PaintAlpha(byte color)
    {
        BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.ChangeColorModifier(new Color32(0, 0, 0, grassColor), paintCurve, false, false, false, true);

        linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
    }
}
