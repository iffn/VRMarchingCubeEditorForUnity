using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "VR SaveData", menuName = "VREditor/ScriptableObjectSaveData")]
public class TransferAsset : ScriptableObject
{
    public List<int> placedObjectIndex;
    public List<Vector3> placedObjectPosition;
    public List<Quaternion> placedObjectRotation;
    public List<Vector3> placedObjectScale;

    public List<Vector3> movedObjectPosition;
    public List<Quaternion> movedObjectRotation;
    public List<Vector3> movedObjectScale;

    public void StoreObjects(PlaceableObject[] moveableObjects)
    {
        for(int i = 0; i < moveableObjects.Length; i++)
        {
            movedObjectPosition.Add(moveableObjects[i].transform.position);
            movedObjectRotation.Add(moveableObjects[i].transform.rotation);
            movedObjectScale.Add(moveableObjects[i].transform.localScale);
        }

        SaveAssetInEditor();
    }

    public void RestoreObjects(PlaceableObject[] moveableObjects)
    {
        for(int i = 0; i < moveableObjects.Length; i++)
        {
            if (i < movedObjectPosition.Count)
            {
                moveableObjects[i].transform.position = movedObjectPosition[i];
                moveableObjects[i].transform.rotation = movedObjectRotation[i];
                moveableObjects[i].transform.localScale = movedObjectScale[i];
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
        placedObjectIndex.Clear();
        placedObjectPosition.Clear();
        placedObjectRotation.Clear();
        placedObjectScale.Clear();
        movedObjectPosition.Clear();
        movedObjectRotation.Clear();
        movedObjectScale.Clear();

        SaveAssetInEditor();
    }
}
