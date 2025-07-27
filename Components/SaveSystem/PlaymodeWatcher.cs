#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeWatcher
{
    public static bool SaveMarchingCubeData = true;
    public static bool SaveNewObjects = true;
    public static bool SaveMovedObjects = true;

    static PlayModeWatcher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                OnBeforeEnterPlayMode();
                break;

            case PlayModeStateChange.EnteredPlayMode:
                OnAfterEnterPlayMode();
                break;

            case PlayModeStateChange.ExitingPlayMode:
                OnBeforeExitPlayMode();
                break;

            case PlayModeStateChange.EnteredEditMode:
                OnAfterExitPlayMode();
                break;
        }
    }

    private static void OnBeforeEnterPlayMode()
    {
        
    }

    private static void OnAfterEnterPlayMode()
    {

    }

    private static void OnBeforeExitPlayMode()
    {
        // Capture data before it's lost
        if (SaveMarchingCubeData)
        {
            Debug.Log("Saving marching cube data");

            foreach (VRMarchingCubeEditor editor in Object.FindObjectsOfType<VRMarchingCubeEditor>())
            {
                editor.SaveMarchingCubeData();
            }
        }
    }

    private static void OnAfterExitPlayMode()
    {

    }
}
#endif
