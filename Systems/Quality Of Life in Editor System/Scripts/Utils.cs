using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public static class Utils
    {
        public static QOLSettings FindQOLSettings()
        {
            QOLSettings settings = (QOLSettings)GetAssetOfType<QOLSettings>();
            if (settings == null)
                throw new Exception("There is no asset of type " + typeof(QOLSettings) + 
                    " available in the project. " +
                    "Make sure to create one under Scriptable Objects/Settings/Quality Of Life Settings.");
            
            return settings;
        }

        #region Find assets of a certain type

        public static UnityEngine.Object GetAssetOfType<T>()
        {
            string[] results;
            results = AssetDatabase.FindAssets("t:" + typeof(T));

            if (results.Length > 1)
                Debug.LogWarning("There are multiple assets of type " + typeof(T) + ", only the first has been returned. Consider using " + nameof(GetAssetsOfType) + " instead.");

            string settingsPath = "";
            foreach (string guid in results)
            {
                settingsPath = AssetDatabase.GUIDToAssetPath(guid);
                break;
            }

            return AssetDatabase.LoadAssetAtPath(settingsPath, typeof(T));
        }

        public static List<UnityEngine.Object> GetAssetsOfType<T>()
        {
            string[] results;
            results = AssetDatabase.FindAssets("t:" + typeof(T));

            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
            foreach (string guid in results)
            {
                string settingsPath = AssetDatabase.GUIDToAssetPath(guid);
                assets.Add(AssetDatabase.LoadAssetAtPath(settingsPath, typeof(T)));
            }

            return assets;
        }

        public static UnityEngine.Object GetAssetAtPath(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        }

        #endregion

        #region Manage Asset folders

        /// <summary>
        /// Returns TRUE if the given path leads within an asset folder. OUTs the path of the asset folder its in.
        /// </summary>
        public static bool IsWithinAssetFolder(string assetPath, out string assetFolderPath)
        {
            //An asset folder is when it already contains directories with names from the template.
            //Then we take the parent directory as the Asset folder

            AssetFoldersTemplateSettings templateSettings = FindQOLSettings().assetFoldersTemplateSettings;

            string potentialTemplateName = Path.GetFileNameWithoutExtension(assetPath);
            string potentialTemplateNamePath = assetPath;
            bool hasFoundAssetFolder = false;

            //Amount of parent folders to check from the selection
            int depth = templateSettings.templateNameQueryDepth;
            for (int i = 0; i < depth; i++)
            {
                if (templateSettings.folderNamesInTemplate.Contains(potentialTemplateName))
                {
                    //Here we are a file that is within an asset folder
                    hasFoundAssetFolder = true;
                    break;
                }

                potentialTemplateNamePath = Directory.GetParent(potentialTemplateNamePath).ToString();
                potentialTemplateName = Path.GetFileNameWithoutExtension(potentialTemplateNamePath);
            }

            if (hasFoundAssetFolder)
                assetFolderPath = Directory.GetParent(potentialTemplateNamePath).ToString();
            else
                assetFolderPath = assetPath;

            return hasFoundAssetFolder;
        }

        public static void CreateAssetFoldersTemplateAtPath(string parentPath)
        {
            _CreateAssetFoldersTemplate(parentPath);
        }

        public static void CreateAssetFoldersTemplateAsChildOf(UnityEngine.Object parentAsset)
        {
            string parentPath = AssetDatabase.GetAssetPath(parentAsset);
            _CreateAssetFoldersTemplate(parentPath);
        }

        private static void _CreateAssetFoldersTemplate(string parentDirectoryPath)
        {
            AssetFoldersTemplateSettings templateSettings = FindQOLSettings().assetFoldersTemplateSettings;

            foreach (string item in templateSettings.folderNamesInTemplate)
            {
                string folderPath = parentDirectoryPath + "\\" + item;
                if (Directory.Exists(folderPath) == false)
                    Directory.CreateDirectory(folderPath);
            }
            AssetDatabase.Refresh();
        }

        public static bool ContainsTemplateFolderNamed(string foldername, string targetPath, out string existingFolderPath, bool createIfNonExistant = false)
        {
            string potentialFolderPath = targetPath + "\\" + foldername;
            if (Directory.Exists(potentialFolderPath))
            {
                //Here the folder exists
                existingFolderPath = potentialFolderPath;
                return true;
            }

            if (createIfNonExistant == false)
            {
                existingFolderPath = targetPath;
                return false;
            }

            //Here we create the folder
            DirectoryInfo folderInfo = Directory.CreateDirectory(potentialFolderPath);
            existingFolderPath = folderInfo.FullName;
            return false;
        }

        #endregion

        #region Assets and files paths

        /// <summary>
        /// Returns the path on hard drive of the folder containing the given asset.
        /// </summary>
        public static string GetFileParentFolderFullPath(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            return Directory.GetParent(assetPath).ToString();
        }

        public static string GetPathRelativeToProjectFolder(string fullPath)
        {
            //If the first folder is already "Assets" then we don't do anything
            if (fullPath.Substring(0, 6) == "Assets")
            {
                return fullPath;
            }
            else
            {
                return "Assets" + fullPath.Substring(Application.dataPath.Length);
            }
        }

        public static string GetAssetPathProjectRelative(UnityEngine.Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
        }

        public static string GetAssetPathFull(UnityEngine.Object asset)
        {
            return Path.GetFullPath(AssetDatabase.GetAssetPath(asset));
        }

        public static bool AssetExistsWithinCoreFolder(UnityEngine.Object asset)
        {
            string assetPath = Utils.GetAssetPathProjectRelative(asset);
            return AssetExistsWithinCoreFolder(assetPath);
        }

        public static bool AssetExistsWithinCoreFolder(string targetAssetPath)
        {
            AssetFoldersTemplateSettings templateSettings = FindQOLSettings().assetFoldersTemplateSettings;

            //Fetch all the upper folder names and add them to the list
            string iterativePath = targetAssetPath;
            string iterativeName = new DirectoryInfo(targetAssetPath).Name;
            int iteration = 0;
            while (iterativeName != templateSettings.coreFolderName && iteration < 25 && iterativeName != "Assets")
            {
                string upperFolderPath = Directory.GetParent(iterativePath).FullName;
                string upperFolderName = new DirectoryInfo(upperFolderPath).Name;

                if (upperFolderName == templateSettings.coreFolderName)
                    return true;

                iterativePath = upperFolderPath;
                iterativeName = upperFolderName;
                iteration++;
            }
            return false;
        }

        #endregion

        #region Prefab Creation

        public static GameObject CreatePrefabObjectInHierarchy(GameObject fbxObject = null)
        {
            PrefabCreationSettings creationSettings = FindQOLSettings().prefabCreationSettings;
            NamingConventionsSettings namingSettings = FindQOLSettings().namingConventionsSettings;

            //Create the prefab and name it
            GameObject newPrefabInCreation = new GameObject();
            if (fbxObject == null)
                newPrefabInCreation.name = creationSettings.defaultPrefabName;
            else
            {
                NamingInfo nameInfo = new NamingInfo(fbxObject.name);
                string newName = 
                    namingSettings.GetAssetPrefixFromType(AssetType.PrefabAsset) +
                    namingSettings.textSplitter +
                    nameInfo.GetNameWithOutPrefix();
                newPrefabInCreation.name = newName;
            }

            //Make sure the transform values are initialized
            newPrefabInCreation.transform.position = Vector3.zero;
            newPrefabInCreation.transform.rotation = Quaternion.identity;
            newPrefabInCreation.transform.localScale = Vector3.one;

            //Create the fbx parent
            List<string> childrenNames = new List<string>(creationSettings.defaultPrefabsChildObjects);
            if (creationSettings.addFBXParentObject)
            {
                string fbxParentName = creationSettings.fbxParentName;
                if (childrenNames.Contains(fbxParentName) == false)
                    childrenNames.Add(fbxParentName);
            }

            Transform fbxHolder = newPrefabInCreation.transform;

            //Create the default children of a prefab
            foreach (string item in childrenNames)
            {
                GameObject prefabChild = new GameObject();
                prefabChild.name = item;

                //Make sure the transform values are reset
                prefabChild.transform.position = Vector3.zero;
                prefabChild.transform.rotation = Quaternion.identity;
                prefabChild.transform.localScale = Vector3.one;

                prefabChild.transform.SetParent(newPrefabInCreation.transform);

                //Save the FBX folder to instantiate the FBX in it
                if (item == creationSettings.fbxParentName)
                {
                    if (creationSettings.addFBXParentObject)
                        fbxHolder = prefabChild.transform;
                }
            }

            //Instantiate the FBX in the designated holder
            if (fbxObject != null)
            {
                //Spawn the FBX inside the FBX holder
                GameObject newFBXObject = (GameObject)PrefabUtility.InstantiatePrefab(fbxObject);
                newFBXObject.transform.SetParent(fbxHolder);

                //Apply changes to the FBX transform if needed
                if (creationSettings.resetFBXPosition)
                    newFBXObject.transform.position = Vector3.zero;
                if (creationSettings.resetFBXScale)
                    newFBXObject.transform.localScale = Vector3.one;
            }

            //Add the default Components to the prefab
            foreach (UnityEngine.Object item in creationSettings.defaultComponents)
            {
                newPrefabInCreation.AddComponent(Type.GetType(item.name + ",Assembly-CSharp"));
            }

            return newPrefabInCreation;
        }

        public static string CreatePrefabInAssets(GameObject targetObject, string parentFolderPath)
        {
            //Create prefab out of GameObject
            string newPrefabPath = parentFolderPath + "\\" + targetObject.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(targetObject, newPrefabPath);

            //Delete the prefab instantiated in the scene
            UnityEngine.Object.DestroyImmediate(targetObject);

            return newPrefabPath;
        }

        #endregion

        #region Labels

        public static List<string> GetLabelsListForAssetInCore(UnityEngine.Object targetAsset)
        {
            string assetPath = GetAssetPathProjectRelative(targetAsset);
            return GetLabelsListForAssetInCore(assetPath);
        }

        public static List<string> GetLabelsListForAssetInCore(string targetAssetPath)
        {
            AssetFoldersTemplateSettings templateSettings = FindQOLSettings().assetFoldersTemplateSettings;

            List<string> labelsToReturn = new List<string>();

            //Fetch all the upper folder names and add them to the list
            string modelsFolderPath = Directory.GetParent(targetAssetPath).FullName;
            string assetFolder = Directory.GetParent(modelsFolderPath).FullName;
            string assetFolderName = new DirectoryInfo(assetFolder).Name;
            labelsToReturn.Add(assetFolderName);

            string iterativePath = assetFolder;
            string iterativeName = assetFolderName;
            int iteration = 0;
            while (iterativeName != templateSettings.coreFolderName && iteration < 50 && iterativeName != "Assets")
            {
                string upperFolderPath = Directory.GetParent(iterativePath).FullName;
                string upperFolderName = new DirectoryInfo(upperFolderPath).Name;

                if (upperFolderName != templateSettings.coreFolderName && upperFolderName != "Assets")
                    labelsToReturn.Add(upperFolderName);

                iterativePath = upperFolderPath;
                iterativeName = upperFolderName;
                iteration++;
            }
            return labelsToReturn;
        }

        #endregion

        public static TextureTypes GetTextureTypeFromFileName(string fileName)
        {
            NamingConventionsSettings settings = FindQOLSettings().namingConventionsSettings;
            string suffix = GetFileNameSuffix(fileName);
            return settings.GetTextureTypeFromSuffix(suffix);
        }

        public static string GetFileNameSuffix(string fileName)
        {
            NamingConventionsSettings settings = FindQOLSettings().namingConventionsSettings;
            NamingInfo nameInfo = new NamingInfo(fileName, settings.textSplitter);
            return nameInfo.suffix;
        }

        public static string GetFileNamePrefix(string fileName)
        {
            NamingConventionsSettings settings = FindQOLSettings().namingConventionsSettings;
            NamingInfo nameInfo = new NamingInfo(fileName);
            return nameInfo.prefix;
        }

        public static string GetAssetFileExtension(UnityEngine.Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return Path.GetExtension(path);
        }

        public static string ChangeAssetFileNamePrefix(string fileName, AssetType newPrefixType)
        {
            NamingConventionsSettings settings = FindQOLSettings().namingConventionsSettings;
            NamingInfo nameInfo = new NamingInfo(fileName);
            nameInfo.prefix = settings.GetAssetPrefixFromType(newPrefixType);
            return nameInfo.GetFullName();
        }

        public static string GetDefaultMaterialShaderTextureName(TextureTypes targetType)
        {
            switch (targetType)
            {
                case TextureTypes.NonExistant:
                    return "";
                case TextureTypes.Albedo:
                    return "_MainTex";
                case TextureTypes.Metallic:
                    return "_MetallicGlossMap";
                case TextureTypes.Normal:
                    return "_BumpMap";
                case TextureTypes.Height:
                    return "_ParallaxMap";
                case TextureTypes.Occlusion:
                    return "_OcclusionMap";
                case TextureTypes.Emission:
                    return "_EmissionMap";
                case TextureTypes.DetailMask:
                    return "_DetailMask";
                default:
                    Debug.LogError("Texture type " + targetType.ToString() + " not implemented.");
                    return "";
            }
        }
    }
}