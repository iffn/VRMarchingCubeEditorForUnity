using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerController : MonoBehaviour, OptionUser
{
    [SerializeField] InputActionProperty increasePlayerSizeButton;
    [SerializeField] InputActionProperty decreasePlayerSizeButton;
    [SerializeField] InputActionProperty leftHandScaleActivator;
    [SerializeField] InputActionProperty rightHandScaleActivator;
    [SerializeField] InputActionProperty leftTeleportButton;
    [SerializeField] InputActionProperty rightTeleportButton;

    [SerializeField] MoveOptions defaultMovementOption = MoveOptions.ScalingGhost;

    [SerializeField] Transform leftLineRenderer;
    [SerializeField] Transform rightLineRenderer;

    [SerializeField] Transform scaleIndicator;
    [SerializeField] TMPro.TextMeshPro scaleText;
    [SerializeField] OptionSelector movementStateSelector;
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
    Vector3 initialScaleCenterWorld;
    bool scalingActive;
    float initialHandDistancePlayerScale;
    float initialScale;
    List<Transform> directScalingObjects;
    List<Transform> incrementalScalingObjects;

    DynamicMoveProvider linkedMoveProvider;
    MoveOptions moveOption = MoveOptions.ScalingGhost;

    float CurrentPlayerScale => linkedCharacterController.transform.localScale.x;
    Vector3 ControllerCenterWorld => 0.5f * (leftHandController.transform.position + rightHandController.transform.position);
    float HandDistanceWorld => (leftHandController.transform.position - rightHandController.transform.position).magnitude;

    bool selectStartingPoint = false;

    public enum MoveOptions
    {
        Walk,
        ScalingGhost
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
            case MoveOptions.Walk:
                WalkingUpdate();
                break;
            case MoveOptions.ScalingGhost:
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
        DynamicMoveProvider linkedMoveProvider,
        List<Transform> directScalingObjects,
        List<Transform> incrementalScalingObjects
        )
    {
        this.linkedCharacterController = linkedCharacterController;
        this.leftHandController = leftHandController;
        this.rightHandController = rightHandController;
        this.linkedMoveProvider = linkedMoveProvider;
        this.directScalingObjects = directScalingObjects;
        this.incrementalScalingObjects = incrementalScalingObjects;

        scaleIndicator.gameObject.SetActive(false);

        defaultHeight = linkedCharacterController.height;
        defaultRadius = linkedCharacterController.radius;
        defaultCenter = linkedCharacterController.center;

        List<string> movementStateNames = new List<string>(System.Enum.GetNames(typeof(MoveOptions)));

        movementStateSelector.Setup(this, movementStateNames, false, (int)defaultMovementOption);

        SetMoveOption(defaultMovementOption);

        leftLineRenderer.gameObject.SetActive(false);
        rightLineRenderer.gameObject.SetActive(false);
    }

    public void SetMoveOption(MoveOptions newMoveOption)
    {
        switch (newMoveOption)
        {
            case MoveOptions.Walk:
                linkedCharacterController.includeLayers = walkIncludeLayers;
                linkedCharacterController.excludeLayers = walkExcludeLayers;
                linkedMoveProvider.useGravity = true;
                break;
            case MoveOptions.ScalingGhost:
                linkedCharacterController.includeLayers = scalingGhostIncludeLayers;
                linkedCharacterController.excludeLayers = scalingGhostExcludeLayers;
                linkedMoveProvider.useGravity = false;
                break;
            default:
                break;
        }

        moveOption = newMoveOption;
    }

    void WalkingUpdate()
    {
        if (selectStartingPoint)
        {
            ScalingGhostUpdate();

            HandleTeleportHandAndGetPosition(leftHandController, leftLineRenderer, leftTeleportButton);
            HandleTeleportHandAndGetPosition(rightHandController, rightLineRenderer, rightTeleportButton);
        }
        else
        {
            // No special movement 
        }
    }

    void HandleTeleportHandAndGetPosition(Transform controller, Transform lineRenderer, InputActionProperty teleportAction)
    {
        lineRenderer.SetPositionAndRotation(controller.position, controller.rotation);

        Ray ray = new Ray(controller.position, controller.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            lineRenderer.localScale = new Vector3(lineRenderer.localScale.x, lineRenderer.localScale.y, hit.distance);

            if (teleportAction.action.WasPerformedThisFrame())
            {
                // Teleport
                linkedCharacterController.transform.position = hit.point;
                selectStartingPoint = false;
                ScalePlayerAndScalingObjects(1);

                leftLineRenderer.gameObject.SetActive(false);
                rightLineRenderer.gameObject.SetActive(false);
            }
        }
    }

    void ScalingGhostUpdate()
    {
        if (Input.GetKey(KeyCode.KeypadPlus) || increasePlayerSizeButton.action.IsPressed())
        {
            ScalePlayerAndScalingObjects(CurrentPlayerScale * (1 + 0.25f * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.KeypadMinus) || decreasePlayerSizeButton.action.IsPressed())
        {
            ScalePlayerAndScalingObjects(CurrentPlayerScale * (1 - 0.2f * Time.deltaTime));
        }

        HandleScale();
    }

    void ScalePlayerAndScalingObjects(float scale)
    {
        float scaleFactor = scale / CurrentPlayerScale; //Needs to be done before changing it

        Vector3 directScaleVector = scale * Vector3.one;

        linkedCharacterController.transform.localScale = directScaleVector;

        linkedCharacterController.height = scale * defaultHeight;
        linkedCharacterController.radius = scale * defaultRadius;
        linkedCharacterController.center = scale * defaultCenter;

        foreach(Transform directScaler in directScalingObjects)
        {
            directScaler.localScale = directScaleVector;
        }

        foreach(Transform incrementalScaler in incrementalScalingObjects)
        {
            incrementalScaler.localScale *= scaleFactor;
        }
    }

    void HandleScale()
    {
        if (leftHandScaleActivator.action.IsPressed() && rightHandScaleActivator.action.IsPressed())
        {
            if (!scalingActive)
            {
                initialScaleCenterWorld = ControllerCenterWorld;
                scalingActive = true;
                initialHandDistancePlayerScale = HandDistanceWorld / CurrentPlayerScale;
                initialScale = CurrentPlayerScale;
                scaleIndicator.gameObject.SetActive(true);
            }

            float newHandDistancePlayerScale = HandDistanceWorld / CurrentPlayerScale;

            float newScale = initialHandDistancePlayerScale * initialScale / newHandDistancePlayerScale;

            ScalePlayerAndScalingObjects(newScale);

            Vector3 newHandCenterWorld = ControllerCenterWorld;

            Vector3 offsetWorld = initialScaleCenterWorld - newHandCenterWorld;

            linkedCharacterController.transform.position += offsetWorld;

            scaleIndicator.position = newHandCenterWorld;
            scaleIndicator.LookAt(rightHandController.position, Vector3.up);
            scaleIndicator.localScale = HandDistanceWorld * 0.6f * Vector3.one;
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

    public void SelectOption(OptionSelector selector, int optionIndex)
    {
        if(selector == movementStateSelector)
        {
            if(optionIndex == (int)MoveOptions.Walk)
            {
                selectStartingPoint = true;
                leftLineRenderer.gameObject.SetActive(true);
                rightLineRenderer.gameObject.SetActive(true);
            }

            moveOption = (MoveOptions)optionIndex;
        }
    }
}
