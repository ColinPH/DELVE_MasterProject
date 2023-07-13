using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class CreateAssetFolderWithTemplateOption
    {
        [MenuItem("Assets/Propeller Cap/Create Asset Folder with Template", false, 19)]
        public static void AssignSelectedTexturesToMaterial()
        {
            Object selectedObj = Selection.objects[0];

            string selectedAssetPath = Utils.GetAssetPathProjectRelative(selectedObj);

            //Create a new folder to put the template folders in
            selectedAssetPath += "\\" + "Temporary Folder Name";
            DirectoryInfo assetFolder = Directory.CreateDirectory(selectedAssetPath);

            //Add the template folders
            Utils.CreateAssetFoldersTemplateAtPath(assetFolder.FullName);

            //Select the asset folder to rename it
            Selection.objects = new Object[] { Utils.GetAssetAtPath(selectedAssetPath) };

            //Start the rename process
            StartRenameDelay();

            //Make sure the changes are saved
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Propeller Cap/Create Asset Folder with Template", true, 19)]
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
            //Start the rename action to rename the selection
            Event renameEvent = Event.KeyboardEvent("f2");
            EditorWindow.focusedWindow.SendEvent(renameEvent);
        }
    }
}