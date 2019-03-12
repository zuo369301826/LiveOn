using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoadWay
{
    [MenuItem("Tool/AssetBundleMode")]
    public static void AssetBundleLoad()
    {
#if UNITY_STANDALONE_WIN
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "AssetBundle");
#endif
#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "AssetBundle");
#endif

    }

    [MenuItem("Tool/EditorMode")]
    public static void AssetDatabaseLoad()
    {
#if UNITY_STANDALONE_WIN
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "AssetDatabase");
#endif
#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "AssetDatabase");
#endif

    }
}
