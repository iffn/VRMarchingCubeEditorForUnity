using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaceableObject))]
public class PlaceableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        PlaceableObject obj = (PlaceableObject)target;

        // Add a button below the default inspector
        if (GUILayout.Button("Fit Box Collider To Mesh Bounds"))
        {
            obj.FitBoxColliderToMeshBounds();
        }
    }
}
