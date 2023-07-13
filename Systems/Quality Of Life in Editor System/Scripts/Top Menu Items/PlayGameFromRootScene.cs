using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace PropellerCap.EditorUsability
{
    public class PlayGameFromRootScene
    {
        [MenuItem("Propeller Cap/Play From Root")]
        public static void PlayFromRoot()
        {
            // Save the current state of the project
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveOpenScenes();

            // Search for Scene_Root in the assets
            
            string[] guids = AssetDatabase.FindAssets(Utils.FindQOLSettings().namingConventionsSettings.rootSceneName);
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (guids.Length > 1)
            {
                string extraScenesPaths = "";
                for (int i = 1; i < guids.Length; i++)
                {
                    extraScenesPaths += AssetDatabase.GUIDToAssetPath(guids[i]) + "\n";
                }
                Debug.LogError($"There seems to be more than 1 scene named Scene_Root. Make sure there is only one. The loaded root scene might not be correct. Here is a list of the extra scenes paths: \n");
            }
            //Load the root scene
            EditorSceneManager.OpenScene(assetPath);

            // Enter playmode
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
}