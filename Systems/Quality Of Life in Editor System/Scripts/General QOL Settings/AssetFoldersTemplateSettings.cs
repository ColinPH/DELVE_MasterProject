using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AssetFoldersTemplateSettings
{
    [Tooltip("The name of the folder that contains all the Asset folders.")]
    public string coreFolderName = "Core";
    [Tooltip("How many parents from the selection are checked to know if we are within an asset folder.")]
    public int templateNameQueryDepth = 4;
    [Tooltip("The names of the folders that should be within the asset folder template.")]
    public List<string> folderNamesInTemplate = new List<string>() {
        "Animations",
        "Prefabs",
        "Models",
        "Scripts",
        "Textures",
        "Materials",
        "SFX",
        "VFX"
    };

    /// <summary>
    /// Called when the scriptable object that contains these settings is Enabled
    /// </summary>
    public void OnContainerEnable()
    {
        
    }
}
