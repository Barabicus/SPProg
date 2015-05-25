﻿using UnityEngine;
using UnityEditor;
using System.IO;

public static class CustomAssetUtility
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Hit Text Properties")]
    public static void CreateTextProperties()
    {
        CustomAssetUtility.CreateAsset<HitTextProperties>();
    }

    [MenuItem("Assets/Create/Essential Objects")]
    public static void CreateEssentialObjects()
    {
        CustomAssetUtility.CreateAsset<EssentialObjects>();
    }

    [MenuItem("Assets/Create/Spell List Info")]
    public static void CreateSpellListInfo()
    {
        CustomAssetUtility.CreateAsset<SpellListInfo>();
    }

    [MenuItem("Assets/Create/GameConfig Info")]
    public static void CreateGameConfigInfo()
    {
        CustomAssetUtility.CreateAsset<GameConfigInfo>();
    }
}