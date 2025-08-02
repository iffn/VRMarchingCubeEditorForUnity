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
    [SerializeField] CharacterController linkedCharacterController;
    [SerializeField] Transform toolOrigin;
    [SerializeField] Transform leftHandController;
    [SerializeField] Transform rightHandController;
    [SerializeField] DynamicMoveProvider linkedMoveProvider;
    [SerializeField] List<PlaceableObject> placeablePrefabs;

    [Header("Prefab")]
    [SerializeField] VRMarchingCubeEditor linkedVRMarchingCubeEditor;
    [SerializeField] PlayerController linkedPlayerController;
    [SerializeField] ObjectPlacement linkedObjectPlacement;
    [SerializeField] Transform handMenu;

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

        linkedPlayerController.Setup(
            linkedCharacterController,
            leftHandController,
            rightHandController,
            linkedMoveProvider
        );

        linkedObjectPlacement.Setup(
            leftHandController,
            rightHandController,
            placeablePrefabs
        );

        inVR = FindObjectOfType<XRDeviceSimulator>(false) == null;
    }

    void Update()
    {
        if (inVR)
            UpdateHandMenuPosition();
        
    }

    void UpdateHandMenuPosition()
    {
        handMenu.position = rightHandController.position;
        handMenu.rotation = rightHandController.rotation;
    }
}
