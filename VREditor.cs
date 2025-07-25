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
    
    [Header("Settings")]
    [SerializeField] InputActionProperty increasePlayerSizeButton;
    [SerializeField] InputActionProperty decreasePlayerSizeButton;
    [SerializeField] InputActionProperty leftHandScaleActivator;
    [SerializeField] InputActionProperty rightHandScaleActivator;

    [Header("Prefab")]
    [SerializeField] VRMarchingCubeEditor linkedVRMarchingCubeEditor;
    [SerializeField] PlayerController linkedPlayerController;

    void OnEnable()
    {
        increasePlayerSizeButton.action.Enable();
        decreasePlayerSizeButton.action.Enable();
        leftHandScaleActivator.action.Enable();
        rightHandScaleActivator.action.Enable();
    }

    void OnDisable()
    {
        increasePlayerSizeButton.action.Disable();
        decreasePlayerSizeButton.action.Disable();
        leftHandScaleActivator.action.Disable();
        rightHandScaleActivator.action.Disable();
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
    }

    void Update()
    {
        
    }
}
