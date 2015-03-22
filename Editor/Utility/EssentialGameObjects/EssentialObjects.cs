using UnityEngine;
using System.Collections;
using UnityEditor;

public class EssentialObjects : ScriptableObject
{
    public Player player;
    public RTSCamera camera;
    public SpellList spellList;
    public GameplayGUI gameplayGUI;

    [MenuItem("Assets/Create/Essential Objects")]
    public static void CreateAsset()
    {
        CustomAssetUtility.CreateAsset<EssentialObjects>();
    }

}
