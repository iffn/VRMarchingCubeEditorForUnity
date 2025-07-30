using iffnsStuff.MarchingCubeEditor.Core;
using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VREditor : MonoBehaviour
{
    [Header("Scene specific")]
    [SerializeField] MarchingCubesController linkedMarchingCubesController;
    [SerializeField] CharacterController linkedCharacterController;
    [SerializeField] Transform toolOrigin;
    [SerializeField] Transform leftHandController;
    [SerializeField] Transform rightHandController;
    [SerializeField] List<PlaceableObject> placeablePrefabs;

    [Header("Prefab")]
    [SerializeField] VRMarchingCubeEditor linkedVRMarchingCubeEditor;
    [SerializeField] PlayerController linkedPlayerController;
    [SerializeField] ObjectPlacement linkedObjectPlacement;

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
            rightHandController
        );

        linkedObjectPlacement.Setup(
            leftHandController,
            rightHandController,
            placeablePrefabs
        );
    }

    void Update()
    {
        
    }
}
