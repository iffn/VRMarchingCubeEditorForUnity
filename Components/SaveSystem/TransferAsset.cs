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

    public void StoreObjects(PlaceableObject[] moveableObjects, List<int> placedObjectIndexes, List<PlaceableObject> placedObjects)
    {
        // Moved objects
        for(int i = 0; i < moveableObjects.Length; i++)
        {
            movedObjectPositions.Add(moveableObjects[i].transform.position);
            movedObjectRotations.Add(moveableObjects[i].transform.rotation);
            movedObjectScales.Add(moveableObjects[i].transform.localScale);
        }

        // Placed objects
        this.placedObjectIndexes = placedObjectIndexes;

        for(int i = 0; i < placedObjects.Count; i++)
        {
            placedObjectPositions.Add(placedObjects[i].transform.position);
            placedObjectRotations.Add(placedObjects[i].transform.rotation);
            placedObjectScales.Add(placedObjects[i].transform.localScale);
        }

        SaveAssetInEditor();
    }

    public void RestoreObjects(PlaceableObject[] moveableObjects, List<PlaceableObject> placeablePrefabs)
    {
        for(int i = 0; i < moveableObjects.Length; i++)
        {
            if (i < movedObjectPositions.Count)
            {
                moveableObjects[i].transform.position = movedObjectPositions[i];
                moveableObjects[i].transform.rotation = movedObjectRotations[i];
                moveableObjects[i].transform.localScale = movedObjectScales[i];
            }
        }

        for(int i = 0; i < placedObjectIndexes.Count; i++)
        {
            if (placedObjectIndexes[i] < placeablePrefabs.Count)
            {
                PlaceableObject placedObject = Instantiate(placeablePrefabs[placedObjectIndexes[i]]);
                placedObject.transform.position = placedObjectPositions[i];
                placedObject.transform.rotation = placedObjectRotations[i];
                placedObject.transform.localScale = placedObjectScales[i];
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

    public void ClearTransferDataAndSaveAsset()
    {
        placedObjectIndexes.Clear();
        placedObjectPositions.Clear();
        placedObjectRotations.Clear();
        placedObjectScales.Clear();
        movedObjectPositions.Clear();
        movedObjectRotations.Clear();
        movedObjectScales.Clear();

        SaveAssetInEditor();
    }
}
