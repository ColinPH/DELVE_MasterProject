using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class CreateEmptyPrefabOption
    {
        [MenuItem("Assets/Propeller Cap/Create Empty Prefab", false, 19)]
        public static void AssignSelectedTexturesToMaterial()
        {
            Object selectedObj = Selection.objects[0];

            string assetPath = Utils.GetAssetPathProjectRelative(selectedObj);
            GameObject prefabInScene = Utils.CreatePrefabObjectInHierarchy();

            string prefabPath = Utils.CreatePrefabInAssets(prefabInScene, assetPath);
            UnityEngine.Object prefabInAssets = Utils.GetAssetAtPath(prefabPath);

            //Select the created prefab
            Selection.objects = new Object[] { prefabInAssets };

            //Add a delay to give Unity the time to create the prefab
            //Otherwise the original selection will be the rename target
            PrefabCreationSettings creationSettings = Utils.FindQOLSettings().prefabCreationSettings;
            if (creationSettings.renamePrefabOnCreation)
                StartRenameDelay();

            //Make sure the changes are saved
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Propeller Cap/Create Empty Prefab", true, 19)]
        public static bool CheckSelection()
        {
            //Can only be called on a folder
            return Utils.GetAssetFileExtension(Selection.objects[0]) == "";
        }

        static async void StartRenameDelay()
        {
            Task task = _WaitAndRename();
            await task;
        }

        private static async Task _WaitAndRename()
        {
            await Task.Delay(100);
            //Start the rename action to rename the created prefab
            var e = Event.KeyboardEvent("f2");
            EditorWindow.focusedWindow.SendEvent(e);
        }
    }
}