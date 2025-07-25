using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleReferenceOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> referenceObjects;

    void Start()
    {
        foreach(GameObject referenceObject in referenceObjects)
        {
            referenceObject.SetActive(gameObject.activeSelf);
        }
    }

    void OnEnable()
    {
        foreach (GameObject referenceObject in referenceObjects)
        {
            referenceObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        foreach (GameObject referenceObject in referenceObjects)
        {
            referenceObject.SetActive(false);
        }
    }
}
