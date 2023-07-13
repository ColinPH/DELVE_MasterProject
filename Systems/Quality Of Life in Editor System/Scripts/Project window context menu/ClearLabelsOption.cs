using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class ClearLabelsOption
    {
        [MenuItem("Assets/Propeller Cap/Clear Labels", false, 19)]
        public static void ClearLabelsFromSelection()
        {
            LabelsSettings settings = Utils.FindQOLSettings().labelSettings;

            foreach (Object item in Selection.objects)
            {
                //Skip the folders
                if (/*settings.ignoreFolders && */Utils.GetAssetFileExtension(item) == "")
                    continue;

                /*if (settings.IsValidExtension(Utils.GetAssetFileExtension(item)))
                    */AssetDatabase.ClearLabels(item);
            }
        }

        [MenuItem("Assets/Propeller Cap/Clear Labels", true, 19)]
        public static bool CheckSelection()
        {
            /*LabelsSettings settings = Utils.FindQOLSettings().clearLabelsSettings;

            foreach (Object item in Selection.objects)
            {
                if (settings.ignoreFolders && Utils.GetAssetFileExtension(item) == "")
                    continue;

                //If one of the selected assets has a wrong extension, disable the button
                *//*if (settings.IsValidExtension(Utils.GetAssetFileExtension(item)) == false)
                    return false;*//*
            }*/

            return true;
        }
    }
}