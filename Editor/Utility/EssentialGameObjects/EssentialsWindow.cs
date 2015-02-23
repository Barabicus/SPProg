using UnityEngine;
using System.Collections;
using UnityEditor;

public class EssentialsWindow : EditorWindow
{

    Player player;
    GameplayGUI gameplayGUI;
    RTSCamera rtsCamera;
    SpellList spellList;
    EssentialObjects ess;

    [MenuItem("Window/Essential GameObjects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EssentialsWindow));
    }

    public void OnEnable()
    {

        EssentialObjects[] e = Resources.FindObjectsOfTypeAll<EssentialObjects>();

        if (e.Length > 0)
            ess = e[0];
        else
        {
            Debug.LogError("Error Essential Objects could not be found!");
        }

        player = ess.player;
        gameplayGUI = ess.gameplayGUI;
        rtsCamera = ess.camera;
        spellList = ess.spellList;
    }

    void OnGUI()
    {
        ess = EditorGUILayout.ObjectField(ess, typeof(EssentialObjects)) as EssentialObjects;
        if (GUILayout.Button(new GUIContent("Create Assets")) && player != null)
        {
            gameplayGUI = PrefabUtility.InstantiatePrefab(gameplayGUI) as GameplayGUI;
            rtsCamera = PrefabUtility.InstantiatePrefab(rtsCamera) as RTSCamera;
            player = PrefabUtility.InstantiatePrefab(player) as Player;
            spellList = PrefabUtility.InstantiatePrefab(spellList) as SpellList;

            gameplayGUI.player = player;
            rtsCamera.followTarget = player.transform;
        }
    }

}
