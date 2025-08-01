using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour, OptionUser, IButtonListUser
{
    [SerializeField] InputActionProperty leftPickupInput;
    [SerializeField] InputActionProperty rightPickupInput;
    [SerializeField] TransferAsset objectTransferAsset;

    List<PlaceableObject> placeablePrefabs;

    [SerializeField] OptionSelector toolButtons;
    [SerializeField] ButtonList placeablePrefabButtons;

    [Header("Detection")]
    [SerializeField] LayerMask placeableLayer;
    [SerializeField] float pickupRadius = 0.01f;

    Transform leftHandController;
    Transform rightHandController;
    private HandState leftHand;
    private HandState rightHand;

    List<PlaceableObject> placeableObjectsByIndex;
    List<PlaceableObject> initialMoveableObjects;
    List<PlaceableObject> addedObjects;
    List<PlaceableObject> removedExistingObjects;

    //PlaceableObject[] moveableObjects;
    //List<int> placedObjectIndexes = new List<int>();
    //List<PlaceableObject> placedObjects = new List<PlaceableObject>();

    Tools currentTool = Tools.Move;

    enum Tools
    {
        Move,
        Duplicate,
        Delete
    }

    // Internal hand state struct
    private struct HandState
    {
        public PlaceableObject held;
        public Vector3 offsetPos;
        public Quaternion offsetRot;
    }

    void Start()
    {

    }

    void Update()
    {
        switch (currentTool)
        {
            case Tools.Move:
                HandleHandWhenMoving(leftPickupInput, leftHandController, ref leftHand);
                HandleHandWhenMoving(rightPickupInput, rightHandController, ref rightHand);
                break;
            case Tools.Duplicate:
                HandleHandWhenDuplicating(leftPickupInput, leftHandController);
                HandleHandWhenDuplicating(rightPickupInput, rightHandController);
                break;
            case Tools.Delete:
                HandleHandWhenDeleting(leftPickupInput, leftHandController);
                HandleHandWhenDeleting(rightPickupInput, rightHandController);
                break;
            default:
                break;
        }
    }

    public void Setup(Transform leftHandController, Transform rightHandController, List<PlaceableObject> placeablePrefabs)
    {
        this.leftHandController = leftHandController;
        this.rightHandController = rightHandController;
        this.placeablePrefabs = placeablePrefabs;

        // Initialize hand states
        leftHand = new HandState { held = null };
        rightHand = new HandState { held = null };

        // Enable input actions
        leftPickupInput.action.Enable();
        rightPickupInput.action.Enable();

        List<string> toolNames = new List<string>(System.Enum.GetNames(typeof(Tools)));

        toolButtons.Setup(this, toolNames, false, (int)currentTool);

        List<string> placeablePrefabNames = new List<string>();

        foreach(PlaceableObject pref in placeablePrefabs)
        {
            placeablePrefabNames.Add(pref.name);
        }

        placeablePrefabButtons.Setup(this, placeablePrefabNames);
    }

    public void GatherObjects()
    {
        placeableObjectsByIndex = new List<PlaceableObject>();
        addedObjects = new List<PlaceableObject>();
        removedExistingObjects = new List<PlaceableObject>();

        // Placeable prefabs
        for (int i = 0; i < placeablePrefabs.Count; i++)
        {
            placeablePrefabs[i].placingIndex = i;
            placeableObjectsByIndex.Add(placeablePrefabs[i]);
        }

        // Placeable objects in scene
        PlaceableObject[] allObjectsInScene = Object.FindObjectsOfType<PlaceableObject>();

        initialMoveableObjects = new List<PlaceableObject>(allObjectsInScene);

        for (int i = 0; i < allObjectsInScene.Length; i++)
        {
            if(placeablePrefabs.Contains(allObjectsInScene[i]))
                continue;

            allObjectsInScene[i].placingIndex = placeablePrefabs.Count + i;
            placeableObjectsByIndex.Add(allObjectsInScene[i]);
        }
    }

    public void StoreObjects()
    {
        objectTransferAsset.StoreObjects(
            placeableObjectsByIndex,
            initialMoveableObjects,
            addedObjects,
            removedExistingObjects);
    }

    public void RestoreObjects()
    {
        GatherObjects();

        objectTransferAsset.RestoreObjects(
            placeablePrefabs,
            placeableObjectsByIndex,
            initialMoveableObjects,
            addedObjects,
            removedExistingObjects);
    }

    public void SelectOption(OptionSelector selector, int optionIndex)
    {
        if(selector == toolButtons)
        {
            currentTool = (Tools)optionIndex;
        }
    }

    public void UseButton(ButtonList list, int index)
    {
        if(list == placeablePrefabButtons)
        {
            SpawnObject(index);
        }
    }

    void DuplicateObject(PlaceableObject baseObject)
    {
        PlaceableObject placedObject = Instantiate(baseObject);
        placedObject.placingIndex = baseObject.placingIndex;
        addedObjects.Add(placedObject);
        placedObject.transform.localPosition += 0.5f * placedObject.transform.localScale;
    }

    void SpawnObject(int objectID)
    {
        PlaceableObject baseObject = placeablePrefabs[objectID];

        PlaceableObject placedObject = Instantiate(baseObject);

        placedObject.placingIndex = baseObject.placingIndex;
        placedObject.gameObject.SetActive(true);
        addedObjects.Add(placedObject);

        placedObject.transform.position = rightHandController.position;
    }

    void DeleteObject(PlaceableObject objectToDelete)
    {
        if (objectToDelete == null)
            return;

        if(placeablePrefabs.Contains(objectToDelete))
        {
            objectToDelete.gameObject.SetActive(false);
        }
        else if (initialMoveableObjects.Contains(objectToDelete))
        {
            objectToDelete.gameObject.SetActive(false);
            removedExistingObjects.Add(objectToDelete);
        }

        if (addedObjects.Contains(objectToDelete))
        {
            addedObjects.Remove(objectToDelete);
            GameObject.Destroy(objectToDelete.gameObject);
        }
    }

    void HandleHandWhenMoving(InputActionProperty input, Transform handTransform, ref HandState handState)
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

    void HandleHandWhenDuplicating(InputActionProperty input, Transform handTransform)
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
                    DuplicateObject(placeable);
                    break;
                }
            }
        }
    }

    void HandleHandWhenDeleting(InputActionProperty input, Transform handTransform)
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
                    DeleteObject(placeable);
                    break;
                }
            }
        }
    }

    
}
