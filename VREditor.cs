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
    [SerializeField] EditShape placeableByClick;
    [SerializeField] CharacterController linkedCharacterController;
    [SerializeField] Transform toolOrigin;
    [SerializeField] Transform leftHandController;
    [SerializeField] Transform rightHandController;

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
            linkedMarchingCubesController,
            placeableByClick
        );

        linkedPlayerController.Setup(
            linkedCharacterController,
            leftHandController,
            rightHandController
        );

        linkedObjectPlacement.Setup(
            leftHandController,
            rightHandController
        );
    }

    void Update()
    {
        
    }
}
