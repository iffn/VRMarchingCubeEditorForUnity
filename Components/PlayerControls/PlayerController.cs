using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InputActionProperty increasePlayerSizeButton;
    [SerializeField] InputActionProperty decreasePlayerSizeButton;
    [SerializeField] InputActionProperty leftHandScaleActivator;
    [SerializeField] InputActionProperty rightHandScaleActivator;

    [SerializeField] Transform scaleIndicator;
    [SerializeField] TMPro.TextMeshPro scaleText;
    [SerializeField] LayerMask walkIncludeLayers;
    [SerializeField] LayerMask walkExcludeLayers;
    [SerializeField] LayerMask scalingGhostIncludeLayers;
    [SerializeField] LayerMask scalingGhostExcludeLayers;

    CharacterController linkedCharacterController;
    Transform leftHandController;
    Transform rightHandController;
    float defaultHeight;
    Vector3 defaultCenter;
    float defaultRadius;
    Vector3 initialCenter;
    bool scalingActive;
    float initialHandDistance;
    float initialScale;
    DynamicMoveProvider linkedMoveProvider;
    moveOptions moveOption = moveOptions.scalingGhost;

    float CurrentScale => linkedCharacterController.transform.localScale.x;
    Vector3 ControllerCenter => 0.5f * (leftHandController.transform.position + rightHandController.transform.position);
    float HandDistance => (leftHandController.transform.position - rightHandController.transform.position).magnitude;

    public enum moveOptions
    {
        walk,
        scalingGhost
    }

    // Start is called before the first frame update
    void Start()
    {
        // Use Setup instead
    }

    // Update is called once per frame
    void Update()
    {
        switch (moveOption)
        {
            case moveOptions.walk:
                break;
            case moveOptions.scalingGhost:
                ScalingGhostUpdate();
                break;
            default:
                break;
        }
    }

    public void Setup(
        CharacterController linkedCharacterController,
        Transform leftHandController,
        Transform rightHandController,
        DynamicMoveProvider linkedMoveProvider
        )
    {
        this.linkedCharacterController = linkedCharacterController;
        this.leftHandController = leftHandController;
        this.rightHandController = rightHandController;
        this.linkedMoveProvider = linkedMoveProvider;

        scaleIndicator.gameObject.SetActive(false);

        defaultHeight = linkedCharacterController.height;
        defaultRadius = linkedCharacterController.radius;
        defaultCenter = linkedCharacterController.center;
    }

    public void SetMoveOption(moveOptions newMoveOption)
    {
        switch (newMoveOption)
        {
            case moveOptions.walk:
                linkedCharacterController.includeLayers = walkIncludeLayers;
                linkedCharacterController.excludeLayers = walkExcludeLayers;
                linkedMoveProvider.useGravity = true;
                break;
            case moveOptions.scalingGhost:
                linkedCharacterController.includeLayers = scalingGhostIncludeLayers;
                linkedCharacterController.excludeLayers = scalingGhostExcludeLayers;
                linkedMoveProvider.useGravity = false;
                break;
            default:
                break;
        }

        moveOption = newMoveOption;
    }

    void ScalingGhostUpdate()
    {
        if (Input.GetKey(KeyCode.KeypadPlus) || increasePlayerSizeButton.action.IsPressed())
        {
            ScalePlayer(CurrentScale * (1 + 0.25f * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.KeypadMinus) || decreasePlayerSizeButton.action.IsPressed())
        {
            ScalePlayer(CurrentScale * (1 - 0.2f * Time.deltaTime));
        }

        HandleScale();
    }

    void ScalePlayer(float scale)
    {
        float scaleFactor = scale / CurrentScale; //Needs to be done before changing it

        linkedCharacterController.transform.localScale = scale * Vector3.one;

        linkedCharacterController.height = scale * defaultHeight;
        linkedCharacterController.radius = scale * defaultRadius;
        linkedCharacterController.center = scale * defaultCenter;

        //placeableByClick.transform.localScale *= scaleFactor;
    }

    void HandleScale()
    {
        if (leftHandScaleActivator.action.IsPressed() && rightHandScaleActivator.action.IsPressed())
        {
            if (!scalingActive)
            {
                initialCenter = ControllerCenter;
                scalingActive = true;
                initialHandDistance = HandDistance;
                initialScale = CurrentScale;
                scaleIndicator.gameObject.SetActive(true);
            }

            float newHandDistance = HandDistance;

            float scaleFactor = (initialHandDistance * CurrentScale) / newHandDistance;

            float newScale = scaleFactor * initialScale;

            ScalePlayer(newScale);

            Vector3 newHandCenter = ControllerCenter;

            Vector3 offset = initialCenter - newHandCenter;

            // ToDo: Move player and collider
            linkedCharacterController.transform.position += offset;

            scaleIndicator.position = newHandCenter;
            scaleIndicator.LookAt(rightHandController.position, Vector3.up);
            scaleIndicator.localScale = newHandDistance * 0.6f * Vector3.one;
            scaleText.text = newScale.ToString("G4");
        }
        else
        {
            if (scalingActive)
            {
                scalingActive = false;
                scaleIndicator.gameObject.SetActive(false);
            }
        }
    }
}
