using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class AssignAutomaticLabelsOption : MonoBehaviour
    {
        [MenuItem("Assets/Propeller Cap/Assign Automatic Labels", false, 19)]
        public static void AssignAutomatcLabelsToSelection()
        {
            foreach (UnityEngine.Object item in Selection.objects)
            {
                //We skip folders
                if (Utils.GetAssetFileExtension(item) == "")
                    continue;

                string[] labelsToAssign = Utils.GetLabelsListForAssetInCore(item).ToArray();

                AssetDatabase.SetLabels(item, labelsToAssign);
            }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Propeller Cap/Assign Automatic Labels", true, 19)]
        public static bool CheckSelection()
        {
            LabelsSettings settings = Utils.FindQOLSettings().labelSettings;

            //If the selection only contains a folder the option should not show
            if (Selection.objects.Length == 1 && Utils.GetAssetFileExtension(Selection.objects[0]) == "")
                return false;

            //If the selection contains an element which is not in the core folder then the option should not show
            bool entireSelectionIsInCore = true;
            bool selectionAsstesHaveValidExtension = true;
            foreach (UnityEngine.Object item in Selection.objects)
            {
                //We skip folders
                if (Utils.GetAssetFileExtension(item) == "")
                    continue;

                if (Utils.AssetExistsWithinCoreFolder(item) == false)
                    entireSelectionIsInCore = false;

                if (false == settings.IsValidExtension(Utils.GetAssetFileExtension(item)))
                    selectionAsstesHaveValidExtension = false;
            }

            return entireSelectionIsInCore && selectionAsstesHaveValidExtension;
        }
    }
}