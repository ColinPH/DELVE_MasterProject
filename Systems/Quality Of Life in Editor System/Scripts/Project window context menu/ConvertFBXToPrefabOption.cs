using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class ConvertFBXToPrefabOption
    {
        private static List<string> prefabPaths;

        [MenuItem("Assets/Propeller Cap/Convert FBX to Prefab", false, 19)]
        public static void ConvertFBXToPrefab()
        {
            prefabPaths = new List<string>();

            foreach (Object item in Selection.objects)
            {
                string fbxPath = Utils.GetAssetPathProjectRelative(item);
                GameObject prefabInScene = Utils.CreatePrefabObjectInHierarchy((GameObject)item);

                prefabPaths.Add(Utils.CreatePrefabInAssets(prefabInScene, Path.GetDirectoryName(fbxPath)));
            }

            //Add a delay to give Unity the time to create the prefab
            WaitAndMovePrefab();
        }

        [MenuItem("Assets/Propeller Cap/Convert FBX to Prefab", true, 19)]
        public static bool CheckSelection()
        {
            //Check that the selection is indeed an FBX
            bool selectionIsOnlyFBXs = true;

            foreach (Object item in Selection.objects)
            {
                string extension = Utils.GetAssetFileExtension(item);
                if (extension != ".fbx" && extension != "") //Directories are fine
                    selectionIsOnlyFBXs = false;
            }

            //If there is only one element in the selection and it's a folder, then we display FALSE
            if (Selection.objects.Length == 1 && Utils.GetAssetFileExtension(Selection.objects[0]) == "")
                return false;

            return selectionIsOnlyFBXs;
        }


        static async void WaitAndMovePrefab()
        {
            Task task = _A_WaitAndMovePrefab();
            await task;
        }

        private static async Task _A_WaitAndMovePrefab()
        {
            await Task.Delay(100);

            //Check if we are in an Asset folder
            string potentialAssetPath = "";
            string existingPrefabsFolderPath = "";
            bool isWithinAssetFolder = Utils.IsWithinAssetFolder(prefabPaths[0], out potentialAssetPath);
            
            if (isWithinAssetFolder)
            {
                //Test if Asset folder has a prefab folder
                if (false == Utils.ContainsTemplateFolderNamed("Prefabs", potentialAssetPath, out existingPrefabsFolderPath, true))
                {
                    //There was no folder called Prefabs but it has now been created
                }
            }
            else
            {
                return;
            }

            await Task.Delay(100);

            AssetDatabase.Refresh();

            //Move all the prefabs to the prefabs folder because we are in an Asset folder
            foreach (string item in prefabPaths)
            {
                AssetDatabase.MoveAsset(item, Utils.GetPathRelativeToProjectFolder(existingPrefabsFolderPath + "\\" + Path.GetFileName(item)));
            }
        }
    }
}