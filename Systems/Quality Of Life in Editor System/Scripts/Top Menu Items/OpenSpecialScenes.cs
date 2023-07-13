using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class OpenSpecialScenes
    {
        //Root
        [MenuItem("Propeller Cap/Open Scene/Root", false, 0)]
        public static void OpenRoot()
        {
            _OpenSceneInEditor("Scene_Root");
        }

        //Features Playground
        [MenuItem("Propeller Cap/Open Scene/Features Playground", false, 0)]
        public static void OpenFeaturesPlayground()
        {
            _OpenSceneInEditor("Scene_Features Playground", "Scene_Ambience");
        }

        //Ambience
        [MenuItem("Propeller Cap/Open Scene/Ambience", false, 0)]
        public static void OpenAmbience()
        {
            _OpenSceneInEditor("Scene_Ambience");
        }

        //Main menu
        [MenuItem("Propeller Cap/Open Scene/Main Menu", false, 50)]
        public static void OpenMainMenu()
        {
            _OpenSceneInEditor("Scene_Main Menu");
        }

        //Head Up Display
        [MenuItem("Propeller Cap/Open Scene/Head Up Display", false, 50)]
        public static void OpenHeadUpDisplay()
        {
            _OpenSceneInEditor("Scene_Head Up Display");
        }

        //Corridor
        [MenuItem("Propeller Cap/Open Scene/Corridor", false, 100)]
        public static void OpenCorridor()
        {
            _OpenSceneInEditor("Scene_Corridor_1", "Scene_Ambience");
        }

        //Hub
        [MenuItem("Propeller Cap/Open Scene/Hub", false, 100)]
        public static void OpenHub()
        {
            _OpenSceneInEditor("Scene_Hub", "Scene_Ambience");
        }

        //Tutorial
        [MenuItem("Propeller Cap/Open Scene/Tutorial", false, 100)]
        public static void OpenTutorial()
        {
            _OpenSceneInEditor("Scene_Tutorial", "Scene_Ambience", "Scene_Hub Loader");
        }

        //Level Gauntlet
        [MenuItem("Propeller Cap/Open Scene/Level Gauntlet", false, 150)]
        public static void OpenLevelGauntlet()
        {
            _OpenSceneInEditor("Scene_Level_Gauntlet", "Scene_Ambience");
        }

        //Level 1
        [MenuItem("Propeller Cap/Open Scene/Level 1", false, 150)]
        public static void OpenLevel1()
        {
            _OpenSceneInEditor("Scene_Level_1", "Scene_Ambience");
        }

        //Level Library
        [MenuItem("Propeller Cap/Open Scene/Level Library", false, 150)]
        public static void OpenLevelLibrary()
        {
            _OpenSceneInEditor("Scene_Level_Library", "Scene_Ambience");
        }

        //Level Bridge
        [MenuItem("Propeller Cap/Open Scene/Level Bridge", false, 150)]
        public static void OpenLevelBridge()
        {
            _OpenSceneInEditor("Scene_Level_Bridge", "Scene_Ambience");
        }

        private static void _OpenSceneInEditor(string sceneName, params string[] additionalScenes)
        {
            // Save the current state of the project
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();

            string assetPath = _GetScenePath(sceneName);

            if (assetPath == "")
                return;

            //Load the root scene
            EditorSceneManager.OpenScene(assetPath);

            if (additionalScenes.Length == 0)
                return;

            foreach (var item in additionalScenes)
            {
                EditorSceneManager.OpenScene(_GetScenePath(item), OpenSceneMode.Additive);
            }
        }

        private static string _GetScenePath(string sceneName)
        {
            // Search for Scene_Root in the assets
            string[] guids = AssetDatabase.FindAssets("t:Scene " + sceneName);

            if (guids.Length == 0)
            {
                Debug.Log($"Tried to load the scene named \"{sceneName}\" but there is no asset with that name.");
                return "";
            }

            foreach (var item in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path).name == sceneName)
                    return path;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return assetPath;
        }
    }
}
