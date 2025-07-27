using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] InputActionProperty leftPickupInput;
    [SerializeField] InputActionProperty rightPickupInput;

    Transform leftHandController;
    Transform rightHandController;


    [Header("Detection")]
    [SerializeField] LayerMask placeableLayer;
    [SerializeField] float pickupRadius = 0.01f;

    // Internal hand state struct
    private struct HandState
    {
        public PlaceableObject held;
        public Vector3 offsetPos;
        public Quaternion offsetRot;
    }

    private HandState leftHand;
    private HandState rightHand;

    void Start()
    {

    }

    public GameObject debugObject;

    void Update()
    {
        HandleHand(leftPickupInput, leftHandController, ref leftHand);
        HandleHand(rightPickupInput, rightHandController, ref rightHand);

        debugObject = leftHand.held != null ? leftHand.held.gameObject : null;
    }

    public void Setup(Transform leftHandController, Transform rightHandController)
    {
        this.leftHandController = leftHandController;
        this.rightHandController = rightHandController;

        // Initialize hand states
        leftHand = new HandState { held = null };
        rightHand = new HandState { held = null };

        // Enable input actions
        leftPickupInput.action.Enable();
        rightPickupInput.action.Enable();
    }

    void HandleHand(InputActionProperty input, Transform handTransform, ref HandState handState)
    {
        Vector3 origin = handTransform.position;

        if (input.action.WasPressedThisFrame())
        {
            Collider[] hits = Physics.OverlapSphere(origin, pickupRadius, placeableLayer, QueryTriggerInteraction.Collide);
            foreach (Collider hit in hits)
            {
                PlaceableObject placeable = hit.GetComponentInParent<PlaceableObject>();
                if (placeable != null)
                {
                    handState.held = placeable;

                    // Store relative offset
                    handState.offsetPos = Quaternion.Inverse(handTransform.rotation) *
                                          (placeable.transform.position - origin);

                    handState.offsetRot = Quaternion.Inverse(handTransform.rotation) *
                                          placeable.transform.rotation;
                    break;
                }
            }
        }

        if (input.action.IsPressed() && handState.held != null)
        {
            handState.held.transform.position = origin + handTransform.rotation * handState.offsetPos;

            if (handState.held.RemainVertical)
            {
                Vector3 flatForward = handTransform.forward;
                flatForward.y = 0;
                flatForward.Normalize();

                if (flatForward == Vector3.zero)
                    flatForward = Vector3.forward;

                Quaternion flatRotation = Quaternion.LookRotation(flatForward, Vector3.up);

                handState.held.transform.rotation = flatRotation * handState.offsetRot;
            }
            else
            {
                handState.held.transform.rotation = handTransform.rotation * handState.offsetRot;
            }
        }

        if (input.action.WasReleasedThisFrame() && handState.held != null)
        {
            handState.held = null;
        }
    }
}
