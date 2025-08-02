using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] bool remainVertical = true;

    public bool RemainVertical
    {
        get { return remainVertical; }
    }

    public int placingIndex { get; set; } = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FitBoxColliderToMeshBounds()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("No MeshFilters found in children.");
            return;
        }

        Bounds combinedBounds = new Bounds();

        bool firstBounds = true;

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            // Get local bounds and transform to world space
            Bounds meshBounds = mf.sharedMesh.bounds;
            Vector3[] vertices = new Vector3[8];

            Vector3 center = meshBounds.center;
            Vector3 extents = meshBounds.extents;

            // Get 8 corners of the bounding box
            vertices[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
            vertices[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
            vertices[2] = center + new Vector3(-extents.x, extents.y, -extents.z);
            vertices[3] = center + new Vector3(-extents.x, extents.y, extents.z);
            vertices[4] = center + new Vector3(extents.x, -extents.y, -extents.z);
            vertices[5] = center + new Vector3(extents.x, -extents.y, extents.z);
            vertices[6] = center + new Vector3(extents.x, extents.y, -extents.z);
            vertices[7] = center + new Vector3(extents.x, extents.y, extents.z);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = mf.transform.TransformPoint(vertices[i]);
            }

            if (firstBounds)
            {
                combinedBounds = new Bounds(vertices[0], Vector3.zero);
                firstBounds = false;
            }

            foreach (var v in vertices)
            {
                combinedBounds.Encapsulate(v);
            }
        }

        // Ensure BoxCollider exists
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Convert world space bounds to local space
        boxCollider.center = transform.InverseTransformPoint(combinedBounds.center);
        Vector3 localSize = transform.InverseTransformVector(combinedBounds.max - combinedBounds.min);
        boxCollider.size = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), Mathf.Abs(localSize.z));
    }

}
