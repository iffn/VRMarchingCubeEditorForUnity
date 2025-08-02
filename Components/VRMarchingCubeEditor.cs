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
    [SerializeField] OptionSelector shapeSelector;

    [SerializeField] AnimationCurve paintCurve;
    [SerializeField] byte clearColor = 0;
    [SerializeField] byte grassColor = 200;
    [SerializeField] byte pathColor = 255;

    PlaceableByClickHandler placeableByClickHandler;

    EditShape PlaceableByClick => placeableByClickHandler.SelectedShape.AsEditShape;

    Transform toolOrigin;
    Tools currentTool = Tools.addAndRemove;

    public List<Transform> IncrementalScalingObjects
    {
        get
        {
            List<Transform> returnLIst = new ();

            List<IPlaceableByClick> editShapes = placeableByClickHandler.EditShapes;

            foreach (IPlaceableByClick shape in editShapes)
            {
                returnLIst.Add(shape.AsEditShape.transform);
            }

            return returnLIst;
        }
    }

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

        if(PlaceableByClick)
            PlaceableByClick.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        editAction.action.Disable();
        scaleToolOnY.action.Disable();
        subtractButton.action.Disable();

        try
        {
            PlaceableByClick.gameObject.SetActive(false);
        }
        catch (System.Exception)
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
        placeableByClickHandler.SelectedShape.AsEditShape.transform.SetPositionAndRotation(toolOrigin.position, toolOrigin.rotation);

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
            placeableByClickHandler.SelectedShape.AsEditShape.transform.localScale *= 1 + scaleValue * scaleSpeed * Time.deltaTime;
        }
    }

    public void SaveMarchingCubeData()
    {
        SaveData();
    }

    public void Setup(
        Transform toolOrigin,
        MarchingCubesController linkedMarchingCubeController
        )
    {
        this.toolOrigin = toolOrigin;
        this.linkedMarchingCubeController = linkedMarchingCubeController;

        InitializeController();

        List<string> toolNames = new List<string>(System.Enum.GetNames(typeof(Tools)));

        toolSelector.Setup(this, toolNames, false, (int)currentTool);

        placeableByClickHandler = new PlaceableByClickHandler(linkedMarchingCubeController);

        shapeSelector.Setup(this, new List<string>(placeableByClickHandler.EditShapeNames), false, 0);

        PlaceableByClick.gameObject.SetActive(enabled);
    }

    public void SelectOption(OptionSelector selector, int optionIndex)
    {
        if(selector == toolSelector)
        {
            currentTool = (Tools)optionIndex;
        }
        else if (selector == shapeSelector)
        {
            placeableByClickHandler.SelectShape(optionIndex);
        }
    }

    void AddAndRemoveUpdate()
    {
        if (editAction.action.IsPressed())
        {
            BaseModificationTools.IVoxelModifier modifier = subtractButton.action.IsPressed() ?
            new BaseModificationTools.SubtractShapeModifier() : new BaseModificationTools.AddShapeModifier();

            linkedMarchingCubeController.ModificationManager.ModifyData(PlaceableByClick, modifier);
        }
    }

    void PaintAlpha(byte color)
    {
        if (editAction.action.IsPressed())
        {
            BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.ChangeColorModifier(new Color32(0, 0, 0, grassColor), paintCurve, false, false, false, true);

            linkedMarchingCubeController.ModificationManager.ModifyData(PlaceableByClick, modifier);
        }
    }
}
