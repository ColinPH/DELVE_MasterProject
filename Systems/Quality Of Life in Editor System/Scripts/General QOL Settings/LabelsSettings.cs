using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap.EditorUsability
{
    [Serializable]
    public class LabelsSettings
    {
        /*[Tooltip("Ignore the label changes on folders.")]
        public bool ignoreFolders = true;*/
        [Tooltip("Whether the labels can only be applied to specific extensions.")]
        public bool useSpecificExtensions = true;
        [Tooltip("The file extensions which can receive labels in the project. The .asset is for the scriptable objects.")]
        public List<string> validExtensions = new List<string>() { 
            ".prefab", //For prefabs
            ".asset" //For Scriptable Objects
        };

        /// <summary>
        /// Called when the scriptable object that contains these settings is Enabled
        /// </summary>
        public void OnContainerEnable()
        {

        }

        public bool IsValidExtension(string extension)
        {
            /*if (ignoreFolders == false && extension == "")
                return true; //Here the extension is a folder*/

            if (useSpecificExtensions == false)
                return true;

            return validExtensions.Contains(extension);
        }

    }
}