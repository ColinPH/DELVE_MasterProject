using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Settings/Automated Build Settings", order = 1)]
public class AutomatedBuildSettings : ScriptableObject
{
    [Tooltip("The name of the scene file that should have index 0 in the build's scenes order.")]
    public string rootSceneName = "Scene_Root";
    [Tooltip("The name of the executable file of the game. The one to double click to launch the game,")]
    public string executableName = "Default Name";
#if UNITY_EDITOR
    public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
    public BuildOptions buildOptions = BuildOptions.None;
#endif
    [Tooltip("When the build is done, opens the folder that contains the build in an explorer window.")]
    public bool openDestinationFolder = true;
    [TextArea, Tooltip("The path, within the project's Assets folder, of the folder containing all the scenes to use in the build." +
        " Scenes can be put inside subfolders. To get the path of a folder, right click and select \"Copy Path\", then paste it in here.")]
    public string buildScenesFolderPath = "Assets/Scenes/Build Scenes";
    [TextArea, Tooltip("The path on the hard drive where the build should be saved.")]
    public string buildExportFolderPath = "";
    [Header("Automated Builder")]
    [Tooltip("If an automated builder is used, some settings will be overwritten.")]
    public bool isUsingAutomatedBuilder = false;
    [TextArea, Tooltip("The path to the config file to communicate to the Automated Builder.")]
    public string buildConfigFilePath = "";
}
