using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class VREditor : MonoBehaviour
{
    [Header("Scene specific")]
    [SerializeField] MarchingCubesController linkedMarchingCubesController;
    [SerializeField] List<PlaceableObject> placeablePrefabs;

    [Header("Prefab")]
    [SerializeField] DynamicMoveProvider linkedMoveProvider;
    [SerializeField] CharacterController linkedCharacterController;
    [SerializeField] Transform toolOrigin;
    [SerializeField] Transform leftHandController;
    [SerializeField] Transform rightHandController;
    [SerializeField] VRMarchingCubeEditor linkedVRMarchingCubeEditor;
    [SerializeField] PlayerController linkedPlayerController;
    [SerializeField] ObjectPlacement linkedObjectPlacement;
    [SerializeField] Transform handMenu;
    [SerializeField] List<Transform> directScalingObjects;
    [SerializeField] List<Transform> incrementalScalingObjects;
    [SerializeField] List<InputActionProperty> allInputs;

    public List<PlaceableObject> PlaceablePrefabs => placeablePrefabs;

    bool inVR = false;

    void OnEnable()
    {
        
    }

    void OnDisable()
    {

    }
    void Start()
    {
        linkedVRMarchingCubeEditor.Setup(
            toolOrigin,
            linkedMarchingCubesController
        );

        List<Transform> completeIncrementalScalingObjects = new(incrementalScalingObjects);

        completeIncrementalScalingObjects.AddRange(linkedVRMarchingCubeEditor.IncrementalScalingObjects);

        linkedPlayerController.Setup(
            linkedCharacterController,
            leftHandController,
            rightHandController,
            linkedMoveProvider,
            directScalingObjects,
            completeIncrementalScalingObjects
        );

        linkedObjectPlacement.Setup(
            leftHandController,
            rightHandController,
            placeablePrefabs
        );

        inVR = FindObjectOfType<XRDeviceSimulator>(false) == null;

        foreach(InputActionProperty input in allInputs) // ToDo: Auto add them from asset
        {
            input.action.Enable();
        }
    }

    void Update()
    {
        if (inVR)
            UpdateHandMenuPosition();
    }

    void UpdateHandMenuPosition()
    {
        handMenu.SetPositionAndRotation(rightHandController.position, rightHandController.rotation);
    }
}
