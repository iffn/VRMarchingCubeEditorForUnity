using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "VR SaveData", menuName = "VREditor/ScriptableObjectSaveData")]
public class TransferAsset : ScriptableObject
{
    public List<int> placedObjectIndexes;
    public List<Vector3> placedObjectPositions;
    public List<Quaternion> placedObjectRotations;
    public List<Vector3> placedObjectScales;

    public List<Vector3> movedObjectPositions;
    public List<Quaternion> movedObjectRotations;
    public List<Vector3> movedObjectScales;

    public List<int> removedObjectIndexes;
    public List<List<StaticEditorFlags>> wasSetToStatic;

    public void ClearTransferDataAndSaveAsset()
    {
        placedObjectIndexes.Clear();
        placedObjectPositions.Clear();
        placedObjectRotations.Clear();
        placedObjectScales.Clear();

        movedObjectPositions.Clear();
        movedObjectRotations.Clear();
        movedObjectScales.Clear();

        removedObjectIndexes.Clear();
        wasSetToStatic.Clear();

        SaveAssetInEditor();
    }

    //public void StoreObjects(PlaceableObject[] moveableObjects, List<int> placedObjectIndexes, List<PlaceableObject> placedObjects)
    public void StoreObjects(
        List<PlaceableObject> placeableObjectsByIndex,
        List<PlaceableObject> initialMoveableObjects,
        List<PlaceableObject> addedObjects,
        List<PlaceableObject> removedExistingObjects
        )
    {
        // Moved objects
        for(int i = 0; i < initialMoveableObjects.Count; i++)
        {
            movedObjectPositions.Add(initialMoveableObjects[i].transform.position);
            movedObjectRotations.Add(initialMoveableObjects[i].transform.rotation);
            movedObjectScales.Add(initialMoveableObjects[i].transform.localScale);
        }

        // Placed objects
        for(int i = 0; i < addedObjects.Count; i++)
        {
            placedObjectIndexes.Add(addedObjects[i].placingIndex);
            placedObjectPositions.Add(addedObjects[i].transform.position);
            placedObjectRotations.Add(addedObjects[i].transform.rotation);
            placedObjectScales.Add(addedObjects[i].transform.localScale);
        }

        for(int i = 0; i<removedExistingObjects.Count; i++)
        {
            removedObjectIndexes.Add(removedExistingObjects[i].placingIndex);
        }

        SaveAssetInEditor();
    }

    public void RestoreObjects(
        List<PlaceableObject> placeablePrefabs,
        List<PlaceableObject> placeableObjectsByIndex,
        List<PlaceableObject> initialMoveableObjects,
        List<PlaceableObject> addedObjects,
        List<PlaceableObject> removedExistingObjects
        )
    {
        // Moved objects
        for(int i = 0; i < initialMoveableObjects.Count; i++)
        {
            initialMoveableObjects[i].transform.position = movedObjectPositions[i];
            initialMoveableObjects[i].transform.rotation = movedObjectRotations[i];
            initialMoveableObjects[i].transform.localScale = movedObjectScales[i];
        }

        // Placed objects
        for(int i = 0; i < placedObjectIndexes.Count; i++)
        {
            PlaceableObject placedObject = Instantiate(placeablePrefabs[placedObjectIndexes[i]]);
            placedObject.transform.position = placedObjectPositions[i];
            placedObject.transform.rotation = placedObjectRotations[i];
            placedObject.transform.localScale = placedObjectScales[i];
        }

        // Removed objects
        for (int i = 0; i < removedObjectIndexes.Count; i++)
        {
            if (removedObjectIndexes[i] < placeablePrefabs.Count)
            {
                // Marked as prefab
                GameObject objectToMark = placeablePrefabs[i].gameObject;
                objectToMark.SetActive(false);
                objectToMark.name = "[Deleted but marked as prefab] " + objectToMark.name;
            }
            else
            {
                // Other object
                GameObject objectToRemove = initialMoveableObjects[placeablePrefabs.Count + removedObjectIndexes[i]].gameObject;
                objectToRemove.SetActive(false);
                objectToRemove.name = "[Should be removed] " + objectToRemove.name;
            }
        }

        ClearTransferDataAndSaveAsset();
    }

    public void SaveAssetInEditor()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
