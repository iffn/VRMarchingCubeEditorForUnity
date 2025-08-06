using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleReferenceOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> referenceObjects;

    /*
    void Start()
    {
        foreach(GameObject referenceObject in referenceObjects)
        {
            referenceObject.SetActive(gameObject.activeSelf);
        }
    }
    */

    void OnEnable()
    {
        foreach (GameObject referenceObject in referenceObjects)
        {
            if(referenceObject == null)
            {
                Debug.LogWarning($"Reference object is null in {gameObject.name}");
                continue;
            }

            try
            {
                referenceObject.SetActive(true);
            }
            catch(System.Exception e)
            {
                Debug.LogWarning($"Error enabling reference object {referenceObject.name}: {e.Message}");
                // Ignore, gets thrown when the object is destroyed when exiting play mode
            }

        }
    }
    /*
    void OnDisable()
    {
        foreach (GameObject referenceObject in referenceObjects)
        {
            try
            {
                referenceObject.SetActive(false);
            }
            catch
            {
                // Ignore, gets thrown when the object is destroyed when exiting play mode
            }
        }
    }
    */
}
