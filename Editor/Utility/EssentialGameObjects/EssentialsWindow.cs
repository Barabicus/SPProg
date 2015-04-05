using UnityEngine;
using System.Collections;
using UnityEditor;

public class EssentialsWindow : EditorWindow
{

    PlayerController player;
    GameplayGUI gameplayGUI;
    RTSCamera rtsCamera;
    GameMainController mainGameController;
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
        var pe = Resources.LoadAll<EssentialObjects>("Utility/EssentialObjects");

        if (pe.Length > 0)
            ess = pe[0];
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
            player = PrefabUtility.InstantiatePrefab(player) as PlayerController;
            mainGameController = PrefabUtility.InstantiatePrefab(mainGameController) as GameMainController;

            var gameMainReferences = mainGameController.GetComponentInChildren<GameMainReferences>();
            // Setup references
            gameMainReferences.Player = player;
            gameMainReferences.RTSCamera = rtsCamera;
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
        mainGameController = ess.mainGameController;

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

        if (mainGameController == null)
        {
            Debug.Log("Game Main Controller was nulll");
            loaded = false;
        }

        return loaded;
    }

}
