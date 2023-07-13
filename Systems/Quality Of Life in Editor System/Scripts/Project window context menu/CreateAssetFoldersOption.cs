using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class CreateAssetFoldersOption
    {
        [MenuItem("Assets/Propeller Cap/Add Folders Template", false, 19)]
        public static void CreateAssetFolders()
        {
            Object selectedObj = Selection.objects[0];

            string assetPath = Utils.GetAssetPathProjectRelative(selectedObj);
            Utils.IsWithinAssetFolder(assetPath, out string assetFolderPath);
            Utils.CreateAssetFoldersTemplateAtPath(assetFolderPath);

            //Make sure the changes are saved
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Propeller Cap/Add Folders Template", true, 19)]
        public static bool CheckSelection()
        {
            //This could check if we are within the folder named Core.
            //But there is no need to limit this option to only the Core folder.

            //Can only be called on a folder
            return Utils.GetAssetFileExtension(Selection.objects[0]) == "";
        }
    }
}