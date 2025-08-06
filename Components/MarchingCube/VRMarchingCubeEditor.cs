using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VRMarchingCubeEditor : PlaymodeEditor, OptionUser
{
    [SerializeField] InputActionProperty editAction;
    [SerializeField] InputActionProperty scaleToolOnY;
    [SerializeField] InputActionProperty subtractButton;
    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float scaleThreshold = 0.01f;
    [SerializeField] OptionSelector toolSelector;
    [SerializeField] OptionSelector shapeSelector;
    [SerializeField] OptionSelector colorSelector;
    [SerializeField] GameObject colorUITitle;

    [SerializeField] AnimationCurve paintCurve;
    [SerializeField] byte clearColor = 0;
    [SerializeField] byte grassColor = 200;
    [SerializeField] byte pathColor = 255;
    [SerializeField] Toggle SaveOnExitPlaymodeToggle;
    [SerializeField] bool saveOnExitPlaymodeDefault = true;

    MarchingCubesController linkedMarchingCubeController;
    protected override MarchingCubesController LinkedMarchingCubeController => linkedMarchingCubeController;

    List<PaintOption> paintOptions;
    PlaceableByClickHandler placeableByClickHandler;
    Transform toolOrigin;
    Tools currentTool = Tools.AddAndRemove;
    int currentColor = 0;

    public bool SaveOnExitPlaymode
    {
        get
        {
            return SaveOnExitPlaymodeToggle.isOn;
        }
    }
    EditShape PlaceableByClick => placeableByClickHandler.SelectedShape.AsEditShape;


    public List<Transform> IncrementalScalingObjects
    {
        get
        {
            List<Transform> returnLIst = new();

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
        AddAndRemove,
        Smooth,
        Bridge,
        Paint
    }

    void OnEnable()
    {
        if (PlaceableByClick)
            PlaceableByClick.gameObject.SetActive(true);
    }

    void OnDisable()
    {
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
            case Tools.AddAndRemove:
                AddAndRemoveUpdate();
                break;
            case Tools.Smooth:
                break;
            case Tools.Bridge:
                break;
            case Tools.Paint:
                Paint();
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

    public void LoadMarchingCubeDataOnceSetupComplete()
    {
        LoadData();
    }

    public void Setup(
        Transform toolOrigin,
        MarchingCubesController linkedMarchingCubeController,
        List<PaintOption> paintOptions
        )
    {
        this.toolOrigin = toolOrigin;
        this.linkedMarchingCubeController = linkedMarchingCubeController;
        this.paintOptions = paintOptions;

        InitializeController();

        List<string> toolNames = new List<string>(System.Enum.GetNames(typeof(Tools)));

        toolSelector.Setup(this, toolNames, false, (int)currentTool);

        placeableByClickHandler = new PlaceableByClickHandler(linkedMarchingCubeController);

        shapeSelector.Setup(this, new List<string>(placeableByClickHandler.EditShapeNames), false, 0);

        PlaceableByClick.gameObject.SetActive(enabled);

        SaveOnExitPlaymodeToggle.SetIsOnWithoutNotify(saveOnExitPlaymodeDefault);

        List<string> colorNames = new();

        for (int i = 0; i < paintOptions.Count; i++)
        {
            colorNames.Add(paintOptions[i].name);
        }

        colorSelector.Setup(this, colorNames, false, (int)currentColor);

        SetPaintUI();
    }

    public void SelectOption(OptionSelector selector, int optionIndex)
    {
        if (selector == toolSelector)
        {
            currentTool = (Tools)optionIndex;

            SetPaintUI();
        }
        else if (selector == shapeSelector)
        {
            placeableByClickHandler.SelectShape(optionIndex);
        }
        else if (selector == colorSelector)
        {
            currentColor = optionIndex;
        }
    }

    void AddAndRemoveUpdate()
    {
        if (editAction.action.IsPressed())
        {
            BaseModificationTools.IVoxelModifier modifier = subtractButton.action.IsPressed() ?
            new BaseModificationTools.SubtractShapeModifier() : new BaseModificationTools.AddShapeModifier();

            LinkedMarchingCubeController.ModificationManager.ModifyData(PlaceableByClick, modifier);
        }
    }

    void SetPaintUI()
    {
        bool nowPaint = currentTool == Tools.Paint;
        colorSelector.gameObject.SetActive(nowPaint);
        colorUITitle.SetActive(nowPaint);
    }

    void Paint()
    {
        if (editAction.action.IsPressed())
        {
            PaintOption selectedColor = paintOptions[currentColor];

            BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.ChangeColorModifier(selectedColor.Color, paintCurve, selectedColor.paintRed, selectedColor.paintGreen, selectedColor.paintBlue, selectedColor.paintAlpha);

            LinkedMarchingCubeController.ModificationManager.ModifyData(PlaceableByClick, modifier);
        }
    }
}
