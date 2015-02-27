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
        Find();
    }

    void Find()
    {
        EssentialObjects[] e = Resources.FindObjectsOfTypeAll<EssentialObjects>();

        if (e.Length > 0)
            ess = e[0];
        else
        {
            Debug.Log("Essential Objects could not be found!");
            return;
        }

    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        ess = EditorGUILayout.ObjectField(ess, typeof(EssentialObjects)) as EssentialObjects;
        if (GUILayout.Button(new GUIContent("Find")))
        {
            Find();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        if (ess == null)
            GUI.backgroundColor = Color.red;
        else
            GUI.backgroundColor = Color.green;
        if (GUILayout.Button(new GUIContent("Create Assets")) && ess != null)
        {
            if (!LoadObjects())
                return;

            gameplayGUI = PrefabUtility.InstantiatePrefab(gameplayGUI) as GameplayGUI;
            rtsCamera = PrefabUtility.InstantiatePrefab(rtsCamera) as RTSCamera;
            player = PrefabUtility.InstantiatePrefab(player) as Player;
            spellList = PrefabUtility.InstantiatePrefab(spellList) as SpellList;

            gameplayGUI.player = player;
            rtsCamera.followTarget = player.transform;
            Debug.Log("Created");
        }

        GUILayout.EndVertical();
    }

    private bool LoadObjects()
    {
        bool loaded = true;
        player = ess.player;
        gameplayGUI = ess.gameplayGUI;
        rtsCamera = ess.camera;
        spellList = ess.spellList;

        if (player == null)
        {
            Debug.Log("Player was null");
            loaded = false;
        }

        if (gameplayGUI == null)
        {
            Debug.Log("GameplayGUI was null");
            loaded = false;
        }

        if (rtsCamera == null)
        {
            Debug.Log("RtsCamera was null");
            loaded = false;
        }

        if (spellList == null)
        {
            Debug.Log("SpellList was null");
            loaded = false;
        }

        return loaded;
    }

}
