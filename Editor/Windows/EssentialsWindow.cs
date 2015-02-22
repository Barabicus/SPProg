using UnityEngine;
using System.Collections;
using UnityEditor;

public class EssentialsWindow : EditorWindow
{

    [MenuItem("Window/Essential GameObjects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EssentialsWindow));
    }

    void OnGUI()
    {
    }

}
