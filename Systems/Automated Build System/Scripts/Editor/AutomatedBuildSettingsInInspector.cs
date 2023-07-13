using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutomatedBuildSettings))]
public class AutomatedBuildSettingsInInspector : Editor
{
    AutomatedBuildSettings buildSettings;

    //For checking if there are multiple instances of the build settings
    bool _hasMultipleInstancesInAssets = false;
    List<string> _instancesPaths = new List<string>();

    //For checking the paths indicated in the text fields
    bool _scenesFolderPathIsValid = false;
    bool _exportFolderPathIsValid = false;
    string _oldScenesFolderPath = "";
    string _oldExportFolderPath = "";

    private void OnEnable()
    {
        buildSettings = (AutomatedBuildSettings)target;

        //Check for other instances of the automated build settings
        _instancesPaths = new List<string>();
        string[] results;
        results = AssetDatabase.FindAssets("t:" + typeof(AutomatedBuildSettings));

        if (results.Length > 1)
        {
            _hasMultipleInstancesInAssets = true;
            foreach (string guid in results)
            {
                _instancesPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (buildSettings.buildScenesFolderPath != _oldScenesFolderPath || buildSettings.buildExportFolderPath != _oldExportFolderPath)
            _CheckBuildSettingsFolderPaths();

        if (_hasMultipleInstancesInAssets)
        {
            EditorGUILayout.HelpBox("There are multiple assets of type " + typeof(AutomatedBuildSettings) +
                ", make sure there is only one. All instances located at paths : " + _GetInstancesNamesAsText(), MessageType.Error);
        }

        if (_scenesFolderPathIsValid == false)
        {
            string reason = buildSettings.buildScenesFolderPath.Contains("\n") ? "Remove line break." : "";
            EditorGUILayout.HelpBox("The " + nameof(buildSettings.buildScenesFolderPath) + " is not valid. " + reason, MessageType.Warning);
        }

        if (_exportFolderPathIsValid == false)
        {
            string reason = buildSettings.buildExportFolderPath.Contains("\n") ? "Remove line break." : "";
            EditorGUILayout.HelpBox("The " + nameof(buildSettings.buildExportFolderPath) + " is not valid. " + reason, MessageType.Warning);
        }
    }

    private void _CheckBuildSettingsFolderPaths()
    {
        _scenesFolderPathIsValid = false;
        _exportFolderPathIsValid = false;

        //Check the paths in the text areas
        if (Directory.Exists(buildSettings.buildScenesFolderPath))
            _scenesFolderPathIsValid = true;
        
        if (Directory.Exists(buildSettings.buildExportFolderPath))
            _exportFolderPathIsValid = true;

        _oldScenesFolderPath = buildSettings.buildScenesFolderPath;
        _oldExportFolderPath = buildSettings.buildExportFolderPath;
    }

    private string _GetInstancesNamesAsText()
    {
        string toReturn = "";
        foreach (string instancePath in _instancesPaths)
        {
            toReturn += "\n" + instancePath;
        }
        return toReturn;
    }
}
