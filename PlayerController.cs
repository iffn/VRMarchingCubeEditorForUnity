using iffnsStuff.MarchingCubeEditor.EditTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] InputActionProperty increasePlayerSizeButton;
    [SerializeField] InputActionProperty decreasePlayerSizeButton;
    [SerializeField] InputActionProperty leftHandScaleActivator;
    [SerializeField] InputActionProperty rightHandScaleActivator;

    [SerializeField] Transform scaleIndicator;
    [SerializeField] TMPro.TextMeshPro scaleText;

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

    float CurrentScale => linkedCharacterController.transform.localScale.x;
    Vector3 ControllerCenter => 0.5f * (leftHandController.transform.position + rightHandController.transform.position);
    float HandDistance => (leftHandController.transform.position - rightHandController.transform.position).magnitude;

    // Start is called before the first frame update
    void Start()
    {
        // Use Setup instead
    }

    // Update is called once per frame
    void Update()
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

    public void Setup(
        CharacterController linkedCharacterController,
        Transform leftHandController,
        Transform rightHandController
        )
    {
        this.linkedCharacterController = linkedCharacterController;
        this.leftHandController = leftHandController;
        this.rightHandController = rightHandController;

        scaleIndicator.gameObject.SetActive(false);

        defaultHeight = linkedCharacterController.height;
        defaultRadius = linkedCharacterController.radius;
        defaultCenter = linkedCharacterController.center;
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
