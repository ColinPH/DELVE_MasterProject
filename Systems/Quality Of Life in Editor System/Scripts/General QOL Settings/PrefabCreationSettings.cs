using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PrefabCreationSettings
{
    [Header("Empty Prefab Options")]
    [Tooltip("If TRUE, the generated empty prefab will be selected and the rename process will be started (same as pressing f2).")]
    public bool renamePrefabOnCreation = true;
    [Tooltip("Default name of the empty prefab.")]
    public string defaultPrefabName = "Prefab_Default Prefab Name";
    [Header("General Prefabs Options")]
    [Tooltip("If TRUE, an empty object will created inside the prefab object to contain the FBX object.")]
    public bool addFBXParentObject = true;
    [Tooltip("The name of the objects, inside the prefab object, that contains the FBX object.")]
    public string fbxParentName = "Models";
    [Tooltip("If TRUE, will set the position of the FBX object's transform to 0,0,0 after instantiation.")]
    public bool resetFBXPosition = true; 
    [Tooltip("If TRUE, will set the scale of the FBX object's transform to 0,0,0 after instantiation.")]
    public bool resetFBXScale = true;
    [Tooltip("List of names of the objects to instantiate in the prefab object.")]
    public List<string> defaultPrefabsChildObjects = new List<string>() { 
        "Models",
        "Colliders"
    };
    [Tooltip("Default components to add to the instantiated prefab.")]
    public List<UnityEngine.Object> defaultComponents = new List<UnityEngine.Object>();

    /// <summary>
    /// Called when the scriptable object that contains these settings is Enabled
    /// </summary>
    public void OnContainerEnable()
    {
        
    }
}
