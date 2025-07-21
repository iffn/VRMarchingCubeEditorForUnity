using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSimplePaint : MonoBehaviour
{
    [SerializeField] private InputActionProperty editAction;

    [SerializeField] MarchingCubesController linkedMarchingCubeController;
    [SerializeField] EditShape placeableByClick;
    [SerializeField] float scaleSpeed = 1f;
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
    }

    void HandleEditing()
    {
        RayHitResult result = RaycastToCenter(true);

        if (result != RayHitResult.None)
        {
            placeableByClick.gameObject.SetActive(true);
            placeableByClick.transform.position = result.point;

            if (Input.GetMouseButtonDown(0))
            {
                BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.AddShapeModifier();
                linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
            }
            if (Input.GetMouseButtonDown(1))
            {
                BaseModificationTools.IVoxelModifier modifier = new BaseModificationTools.SubtractShapeModifier();
                linkedMarchingCubeController.ModificationManager.ModifyData(placeableByClick, modifier);
            }

            float scaleAxis = Input.GetAxis("Mouse ScrollWheel");

            placeableByClick.transform.localScale *= (1 - scaleAxis * scaleSpeed);
        }
        else
        {
            placeableByClick.gameObject.SetActive(false);
        }
    }

    void HandleSaving()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            ScriptableObjectSaveData saveData = linkedMarchingCubeController.linkedSaveData;

            VoxelData[,,] voxelDataReference = linkedMarchingCubeController.VoxelDataReference;

            saveData.SaveData(voxelDataReference);

#if UNITY_EDITOR
            EditorUtility.SetDirty(saveData);
            AssetDatabase.SaveAssets();
#endif
        }
    }

    void InitializeController()
    {
        linkedMarchingCubeController.ClearAllViews();
        linkedMarchingCubeController.Initialize(1, 1, 1, true, false);
        LoadData();
    }

    void LoadData()
    {
        if (linkedMarchingCubeController.linkedSaveData == null)
            return;

        linkedMarchingCubeController.SaveAndLoadManager.LoadGridData(linkedMarchingCubeController.linkedSaveData);
    }

    RayHitResult RaycastToCenter(bool detectBoundingBox = true)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore)) //~0 = layer mask for all layers
            return new RayHitResult(hitInfo.point, hitInfo.normal);

        if (!detectBoundingBox)
            return RayHitResult.None;

        Vector3 areaPosition = linkedMarchingCubeController.transform.position;
        Vector3Int areaSize = linkedMarchingCubeController.MaxGrid;
        Bounds bounds = new Bounds(areaPosition + areaSize / 2, areaSize);

        (Vector3, Vector3)? result = bounds.GetIntersectRayPoints(ray);
        if (result != null)
            return new RayHitResult(result.Value.Item2, bounds.GetNormalToSurface(result.Value.Item2));

        // Both normal Raycast and Bounds intersection did not succeed 
        return RayHitResult.None;
    }
}
