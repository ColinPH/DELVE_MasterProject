using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using PropellerCap;
//using UnityEditor.Build.Content;
#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

#if UNITY_EDITOR
public class AutomatedBuildStarter
{
    /// <summary>
    /// This method is also called by the AutomatedBuilder application.
    /// DO NOT CHANGE unless it is also changed in the application.
    /// </summary>
    [MenuItem("Propeller Cap/Build Machine/Start Automatic Build")]
    public static void StartAutomatedBuild()
    {
        //*************************************************************
        //Find the build settings asset and make sure there is only one
        //*************************************************************

        #region 

        string[] results;
        results = AssetDatabase.FindAssets("t:" + typeof(AutomatedBuildSettings));
        if (results.Length > 1)
        {
            Debug.LogError("There are multiple assets of type " + typeof(AutomatedBuildSettings) + ". Make sure that there is only one.");
            return;
        }

        string settingsPath = "";
        foreach (string guid in results)
        {
            settingsPath = AssetDatabase.GUIDToAssetPath(guid);
            break;
        }

        AutomatedBuildSettings buildSettings = (AutomatedBuildSettings)AssetDatabase.LoadAssetAtPath(settingsPath, typeof(AutomatedBuildSettings));
        
        if (buildSettings == null)
        {
            Debug.LogError("There is no asset of type " + typeof(AutomatedBuildSettings) + " in the project. Make sure to " +
                "create one using Rightclick/Scriptable Objects/Settings/Automated Build Settings.");
            return;
        }

        #endregion

        //****************************************************************************************
        //Extract and check the information from the build settings, and prepare the export folder
        //****************************************************************************************

        #region 

        string scenesFolderPath = buildSettings.buildScenesFolderPath;
        string exportFolderPath = buildSettings.buildExportFolderPath;
        string buildName = buildSettings.executableName;
        string projectCodeName = Application.productName;
        string gameVersion = Application.version;

        BuildTarget buildTarget = buildSettings.buildTarget;
        BuildOptions buildOptions = buildSettings.buildOptions;

        BuildConfig buildConfig = new BuildConfig();
        
        if (buildSettings.isUsingAutomatedBuilder)
        {
            //Read the build config file to retrieve the information from the builder
            var jsonText = File.ReadAllText(buildSettings.buildConfigFilePath);
            buildConfig = JsonConvert.DeserializeObject<BuildConfig>(jsonText);

            scenesFolderPath = buildConfig.scenesToBuildFolderPath;
            exportFolderPath = buildConfig.buildsExportFolderPath;
            buildName = buildConfig.executableName;
            projectCodeName = buildConfig.projectCodeName;
            gameVersion = buildConfig.gameVersion;
            PlayerSettings.bundleVersion = buildConfig.gameVersion;
        }
        else
        {
            //Check that the paths from the build settings are valid
            if (Directory.Exists(scenesFolderPath) == false)
            {
                Debug.LogError("The " + nameof(scenesFolderPath) + " in the " + typeof(AutomatedBuildSettings) + " is not valid.");
                return;
            }
            if (Directory.Exists(exportFolderPath) == false)
            {
                Debug.LogError("The " + nameof(exportFolderPath) + " in the " + typeof(AutomatedBuildSettings) + " is not valid.");
                return;
            }
        }

        //Prepare the build folder
        string date = DateTime.Now.ToString("d");
        date = date.Replace('/', ' ');
        string buildFolderName = projectCodeName + "_" + gameVersion + "_" + date;
        exportFolderPath += "\\" + buildFolderName;

        //Create a new folder for the build
        Directory.CreateDirectory(exportFolderPath);
;
        #endregion

        //*******************************************************************************
        //Fetch the scenes path in asset folder and prepare the list with Root at index 0
        //*******************************************************************************

        #region

        //The scene paths need to be relative to the assets folder path, start with Assets/...
        List<string> scenesForBuild = _GetScenePathsFrom(scenesFolderPath);

        //Make sure there is a scene called Scene_Root
        bool hasRootScene = false;
        int rootSceneIndex = 0;
        foreach (string level in scenesForBuild)
        {
            if (level.Contains("\\" + buildSettings.rootSceneName + ".unity"))
            {
                hasRootScene = true;
                rootSceneIndex = scenesForBuild.IndexOf(level);
            }
        }

        if (hasRootScene == false)
        {
            Debug.LogError("There is no scene called Scene_Root, make sure to create such scene in the folder that contains the scenes to build.");
            return;
        }

        //Make sure the root scene is first in the list
        string rootSceneName = scenesForBuild[rootSceneIndex];
        scenesForBuild.RemoveAt(rootSceneIndex);
        scenesForBuild.Insert(0, rootSceneName);

        #endregion

        //*****************************************
        //Build the game and open the export folder
        //*****************************************

        #region

        string executablePath = exportFolderPath + "\\" + buildName + ".exe";
        BuildReport report = BuildPipeline.BuildPlayer(scenesForBuild.ToArray(), executablePath, buildTarget, buildOptions);
        
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            return;
        }

        if (summary.result == BuildResult.Succeeded)
        {
            //If the build succeeds we add 1 to the version
            string[] splitted = Application.version.Split('.');
            int testBuildNumber = int.Parse(splitted[2]);
            testBuildNumber++;
            string newVersion = splitted[0] + "." + splitted[1] + "." + testBuildNumber.ToString();
            
            PlayerSettings.bundleVersion = newVersion;

            float megaBytes = summary.totalSize / 1000000; //1 million
            string suffix = " MB";
            if (megaBytes > 1000)
            {
                megaBytes = megaBytes / 1000;
                suffix = " GB";
            }
            string buildSize = megaBytes.ToString("0") + suffix;

            string buildTime = summary.totalTime.Hours.ToString("00") + ":" + summary.totalTime.Minutes.ToString("00") + ":" + summary.totalTime.Seconds.ToString("00");

            Debug.Log("Build succeeded: build size is " + buildSize + ", and build time is " + buildTime + " seconds.");

            //If we are in an automated builder save the data to the buildConfig file
            buildConfig.buildTime = buildTime;
            buildConfig.buildSize = (int)megaBytes;
            buildConfig.buildsExportFolderPath = exportFolderPath;
            buildConfig.latestBuildExecutablePath = executablePath;
        }

        if (buildSettings.openDestinationFolder && buildSettings.isUsingAutomatedBuilder == false)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/open," + exportFolderPath);
        }

        if (buildSettings.isUsingAutomatedBuilder)
        {
            if (buildSettings.isUsingAutomatedBuilder)
            {
                //Save the data to the build config
                var json = JsonConvert.SerializeObject(buildConfig, Formatting.Indented);
                File.WriteAllText(buildSettings.buildConfigFilePath, json);
            }

            /*if (buildConfig.launchAppAfterBuild)
            {
                System.Diagnostics.Process.Start(executablePath);
            }*/
        }

        #endregion
    }

    private static List<string> _GetScenePathsFrom(string scenesFolderPath)
    {
        List<string> toReturn = new List<string>();
        DirectoryInfo dirInfo = new DirectoryInfo(scenesFolderPath);
        foreach (FileInfo file in dirInfo.GetFiles("*.unity"))
        {
            string projectRelativePath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
            toReturn.Add(projectRelativePath);
        }
        //Check the sub directories
        foreach (DirectoryInfo dir in dirInfo.GetDirectories())
        {
            toReturn.AddRange(_GetScenePathsFrom(dir.FullName));
        }

        return toReturn;
    }
}
#endif
