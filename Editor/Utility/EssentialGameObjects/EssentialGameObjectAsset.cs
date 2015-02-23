using UnityEngine;
using UnityEditor;
using System;

public class EssentialGameObjectAsset {

    [MenuItem("Assets/Create/Essential Objects")]
    public static void CreateAsset()
    {
        CustomAssetUtility.CreateAsset<EssentialObjects>();
    }
}
