using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEditorInternal;
using PropellerCap.EditorUsability;

public class PrefabCreationTool : EditorWindow
{
    //FBX transformation
    bool _resetFBXPosition = true;
    bool _resetFBXScale = true;
    bool _hasModelsContainer = true;

    //Components to add
    bool _componentsAreValid = false;
    public List<UnityEngine.Object> componentsToAdd = new List<UnityEngine.Object>();

    //Prefabs creation
    GameObject newPrefabInCreation;
    bool conversionIsRunning = false;
    float progressValue = 0f;
    float total = 0f;
    List<GameObject> _toDelete = new List<GameObject>();

    //Creation of the folders
    public Dictionary<string, bool> foldersToCreate = new Dictionary<string, bool>();
    
    //Labels
    public List<string> customLabelsToAdd = new List<string>();
    public List<string> automaticLabelsToAdd = new List<string>();
    string _selectionParentPath = "";
    SerializedObject _customLabelsSO;
    SerializedObject _automaticLabelsSO;
    SerializedProperty _customLabelsProperty;
    SerializedProperty _automaticLabelsProperty;
    ReorderableList _customLabelsList;
    ReorderableList _automaticLabelsList;

    //Button warnings
    float _warningShowTime = 2f; //Seconds
    float _warningEndTimeApplyLabels = 0f;
    float _warningEndTimeFoldersCreation = 0f;
    float _warningEndTimePrefabsCreation = 0f;

    Vector2 _scrollPos = new Vector2();

    [MenuItem("Propeller Cap/Tools/Prefab Creation Tool")]
    public static void ShowWindow()
    {
        PrefabCreationTool window = (PrefabCreationTool)EditorWindow.GetWindow(typeof(PrefabCreationTool));
        window.Show();
    }

    private void Awake()
    {
        //Here we open the tool for the first time so we populate the list of folders to generate
        foldersToCreate.Add("Textures", false);
        foldersToCreate.Add("Prefabs", false);
        foldersToCreate.Add("Materials", false);
        foldersToCreate.Add("Scripts", false);
        foldersToCreate.Add("SFX", false);
        foldersToCreate.Add("VFX", false);
        foldersToCreate.Add("Animations", false);

        //Here we generate the list every time the window opens

        //Create the custom labels list
        _customLabelsSO = new SerializedObject(this);
        _customLabelsProperty = _customLabelsSO.FindProperty(nameof(customLabelsToAdd));
        _customLabelsList = new ReorderableList(_customLabelsSO, _customLabelsProperty)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true,
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Custom Labels"),
            drawElementCallback = (rect, index, focused, active) =>
            {
                customLabelsToAdd[index] = EditorGUI.TextField(rect, customLabelsToAdd[index]);
            }
        };

        //Create the automatic labels list
        _automaticLabelsSO = new SerializedObject(this);
        _automaticLabelsProperty = _automaticLabelsSO.FindProperty(nameof(automaticLabelsToAdd));
        _automaticLabelsList = new ReorderableList(_automaticLabelsSO, _automaticLabelsProperty)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true,
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Automatic Labels"),
            drawElementCallback = (rect, index, focused, active) =>
            {
                automaticLabelsToAdd[index] = EditorGUI.TextField(rect, automaticLabelsToAdd[index]);
            }
        };
    }

    private void OnGUI()
    {
        //TODO Make sure to check if the selection is within a Models folder
        //TODO add a check for if something is within the Core folder

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        //***************************************************************************************
        //Draw the button to generate the prefabs from the selected FBXs, and draw a progress bar
        //***************************************************************************************

        #region Draw the prefab button and progress bar

        GUILayout.Space(10);

        if (conversionIsRunning == false)
        {
            if (_toDelete.Count > 0)
            {
                foreach (GameObject o in _toDelete)
                {
                    DestroyImmediate(o);
                }
                _toDelete.Clear();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(position.width / 6f);
            GUIContent content = new GUIContent("Apply All", "Apply all the settings indicated bellow.");
            if (GUILayout.Button(content, GUILayout.MaxWidth(4f * position.width / 6f), GUILayout.Height(30)))
            {
                if (_HasSelectedOnlyFBXs())
                {
                    conversionIsRunning = true;
                    RunTask();
                } 
                else
                {
                    _warningEndTimePrefabsCreation = (float)EditorApplication.timeSinceStartup + _warningShowTime;
                }
            }
            GUILayout.EndHorizontal();

            if ((float)EditorApplication.timeSinceStartup < _warningEndTimePrefabsCreation)
                EditorGUILayout.HelpBox("Selection must contain only .fbx files to create prefabs.", MessageType.Warning);
        }
        else
        {
            EditorGUI.ProgressBar(new Rect(10, 10, position.width - 20, 30), progressValue / total, "Progress");
            GUILayout.Space(32);
        }

        #endregion

        //********************************************
        //List of scripts to add to the created prefab
        //********************************************

        #region Display the list of components to add

        GUILayout.Space(10);

        //Draw the list of components
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_customLabelsSO.FindProperty(nameof(componentsToAdd)));
        _customLabelsSO.ApplyModifiedProperties();

        //And draw a button to clear the list
        if (GUILayout.Button("Clear", GUILayout.Height(50), GUILayout.Width(50)))
        {
            componentsToAdd.Clear();
        }
        GUILayout.EndHorizontal();

        //Show warning message if one of the scripts is not valid
        if (componentsToAdd.Count > 0)
        {
            string nonMonoBehaviourFileName = "";
            _componentsAreValid = _ComponentsListContainsOnlyMonoBehaviours(out nonMonoBehaviourFileName);
            if (_componentsAreValid == false)
            {
                EditorGUILayout.HelpBox("Components must inherit from MonoBehaviour. " +
                    "The component \"" + nonMonoBehaviourFileName + "\" does not inherit from MonoBehaviour.", MessageType.Warning);
            }
        }
        else
        {
            _componentsAreValid = true;
        }

        #endregion

        //************************************************************************
        //Draw the options for the modifications to the FBX fils inside the prefab
        //************************************************************************

        #region Prefab generation settings

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Prefab creation options :", EditorStyles.boldLabel);
        _hasModelsContainer = EditorGUILayout.Toggle("Add models container", _hasModelsContainer);
        _resetFBXPosition = EditorGUILayout.Toggle("Reset FBX Position", _resetFBXPosition);
        _resetFBXScale = EditorGUILayout.Toggle("Reset FBX Scale", _resetFBXScale);

        #endregion

        //*******************************************************
        //Draw the buttons for the additional folders to generate
        //*******************************************************

        #region Additional folders to generate

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Additional folders to generate :", EditorStyles.boldLabel);
        
        Dictionary<string, bool> newValuesForFolders = new Dictionary<string, bool>();
        bool isDrawingTheSecondOne = false;
        foreach (KeyValuePair<string, bool> item in foldersToCreate)
        {
            if (item.Key == "Prefabs") continue;

            //Draw the name of the folders
            if (isDrawingTheSecondOne == false) GUILayout.BeginHorizontal();
            
            if (item.Value)
            {
                if (GUILayout.Button(item.Key, EditorStyles.miniButton, GUILayout.MaxWidth(position.width / 2f)))
                {
                    newValuesForFolders.Add(item.Key, !item.Value);
                }
            }
            else
            {
                if (GUILayout.Button(item.Key, EditorStyles.toolbarButton, GUILayout.MaxWidth(position.width / 2f)))
                {
                    newValuesForFolders.Add(item.Key, !item.Value);
                }
            }

            if (isDrawingTheSecondOne) GUILayout.EndHorizontal();
            
            isDrawingTheSecondOne = !isDrawingTheSecondOne;
        }

        if (isDrawingTheSecondOne) GUILayout.EndHorizontal();

        foreach (KeyValuePair<string, bool> item in newValuesForFolders)
        {
            foldersToCreate[item.Key] = item.Value;
        }

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width / 6f);
        if (GUILayout.Button("Generate folders Only", GUILayout.MaxWidth(4f * position.width / 6f), GUILayout.Height(25)))
        {
            if (_SuccessfullyCreatedFoldersInSelection() == false)
                _warningEndTimeFoldersCreation = (float)EditorApplication.timeSinceStartup + _warningShowTime;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        if ((float)EditorApplication.timeSinceStartup < _warningEndTimeFoldersCreation)
            EditorGUILayout.HelpBox("Must select a folder to generate folders.", MessageType.Warning);

        #endregion

        //*******************************************************
        //Draw the list of labels to add to the generated prefabs
        //*******************************************************

        #region Labels to add to the generated prefabs

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Labels to apply :", EditorStyles.boldLabel);

        //Draw button to apply only labels to selection
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width / 6f);
        if (GUILayout.Button("Apply labels only", GUILayout.MaxWidth(4f * position.width / 6f), GUILayout.Height(25)))
        {
            if (_SuccessfullyAppliedLabelsOnSelection() == false)
                _warningEndTimeApplyLabels = (float)EditorApplication.timeSinceStartup + _warningShowTime;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        if ((float)EditorApplication.timeSinceStartup < _warningEndTimeApplyLabels)
            EditorGUILayout.HelpBox("Labels can only be applied on prefabs", MessageType.Warning);

        //Draw the list of custom labels
        _customLabelsSO.Update();
        _customLabelsList.DoLayoutList();
        _customLabelsSO.ApplyModifiedProperties();

        //Draw the list of automatic labels
        _automaticLabelsSO.Update();
        _automaticLabelsList.DoLayoutList();
        _automaticLabelsSO.ApplyModifiedProperties();

        //Button to regenerate the list of labels
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width / 6f);
        if (GUILayout.Button("Regenerate Labels List", GUILayout.MaxWidth(4f * position.width / 6f)) || SelectionHasChanged())
        {
            _RegenerateLabelsList();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        #endregion

        EditorGUILayout.EndScrollView();
    }

    private bool _ComponentsListContainsOnlyMonoBehaviours(out string nonMonoBehaviourFileName)
    {
        foreach (UnityEngine.Object item in componentsToAdd)
        {
            if (item == null) continue;

            string path = AssetDatabase.GetAssetPath(item);
            string extension = Path.GetExtension(path);
            if (extension != ".cs")
            {
                nonMonoBehaviourFileName = item.name;
                return false;
            }

            Type assetType = Type.GetType(item.name + ",Assembly-CSharp");
            if (assetType.IsSubclassOf(typeof(MonoBehaviour)) == false)
            {
                nonMonoBehaviourFileName = item.name;
                return false;
            }
        }

        nonMonoBehaviourFileName = "";
        return true;
    }

    private bool _SuccessfullyCreatedFoldersInSelection()
    {
        List<string> targetPaths = new List<string>();

        foreach (UnityEngine.Object item in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(item);
            string extension = Path.GetExtension(path);
            if (extension != "")
            {
                if (extension == ".fbx")
                {
                    //Get the asset folder path which should be the grand parent of the FBX file
                    path = Directory.GetParent(path).FullName;
                    path = Directory.GetParent(path).FullName;
                    targetPaths.Add(path);
                }
            }
            else
                targetPaths.Add(path);
        }

        foreach (string path in targetPaths)
        {
            _GenerateFoldersInDirectory(path);
        }

        return true;
    }

    private void _GenerateFoldersInDirectory(string parentDirectoryPath)
    {
        string prefabsFolder = parentDirectoryPath + "\\Prefabs";
        if (Directory.Exists(prefabsFolder) == false)
            Directory.CreateDirectory(prefabsFolder);

        string modelsFolder = parentDirectoryPath + "\\Models";
        if (Directory.Exists(modelsFolder) == false)
            Directory.CreateDirectory(modelsFolder);

        foreach (KeyValuePair<string, bool> item in foldersToCreate)
        {
            if (item.Value == false) continue;

            string texturesFolder = parentDirectoryPath + "\\" + item.Key;
            if (Directory.Exists(texturesFolder) == false)
                Directory.CreateDirectory(texturesFolder);
        }

        AssetDatabase.Refresh();
    }

    private bool _SuccessfullyAppliedLabelsOnSelection()
    {
        if (Selection.gameObjects.Length == 0)
            return false;

        foreach (GameObject item in Selection.gameObjects)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Path.GetExtension(path) != ".prefab")
            {
                return false;
            }
        }

        foreach (GameObject item in Selection.gameObjects)
        {
            string path = AssetDatabase.GetAssetPath(item.GetInstanceID());
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            _ApplyLabelsToObject(asset);
        }

        return true;
    }

    private void _ApplyLabelsToObject(UnityEngine.Object targetObject)
    {
        int arrayLength = customLabelsToAdd.Count + automaticLabelsToAdd.Count;
        string[] labelsArray = new string[arrayLength];

        //Add the labels to the array
        int index = 0;
        foreach (string label in customLabelsToAdd)
        {
            labelsArray[index] = label;
            index++;
        }
        foreach (string label in automaticLabelsToAdd)
        {
            labelsArray[index] = label;
            index++;
        }

        //Add labels from he array to the object
        AssetDatabase.SetLabels(targetObject, labelsArray);
    }

    private bool SelectionHasChanged()
    {
        if (Selection.objects.Length == 0)
            return false;

        string fbxAssetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        string modelsPath = Directory.GetParent(fbxAssetPath).FullName;
        if (modelsPath != _selectionParentPath)
        {
            _selectionParentPath = modelsPath;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void _RegenerateLabelsList()
    {
        if (Selection.gameObjects.Length == 0) return;

        automaticLabelsToAdd.Clear();
        //Fetch all the upper folder names and add them to the list
        string fbxAssetPath = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);
        string modelsFolderPath = Directory.GetParent(fbxAssetPath).FullName;
        string assetFolder = Directory.GetParent(modelsFolderPath).FullName;
        string assetFolderName = new DirectoryInfo(assetFolder).Name;
        automaticLabelsToAdd.Add(assetFolderName);

        string iterativePath = assetFolder;
        string iterativeName = assetFolderName;
        int iteration = 0;
        while(iterativeName != "Core" && iteration < 50 && iterativeName != "Assets")
        {
            string upperFolderPath = Directory.GetParent(iterativePath).FullName;
            string upperFolderName = new DirectoryInfo(upperFolderPath).Name;

            if (upperFolderName != "Core" && upperFolderName != "Assets")
                automaticLabelsToAdd.Add(upperFolderName);

            iterativePath = upperFolderPath;
            iterativeName = upperFolderName;
            iteration++;
        }
    }

    private bool _HasSelectedOnlyFBXs()
    {
        foreach (UnityEngine.Object item in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(item);
            if (Path.GetExtension(path) != ".fbx")
            {
                return false;
            }
        }
        return true;
    }

    private void _AddComponentsToObject(GameObject newPrefabInCreation)
    {
        foreach (UnityEngine.Object item in componentsToAdd)
        {
            newPrefabInCreation.AddComponent(Type.GetType(item.name + ",Assembly-CSharp"));
        }
    }

    private string _GetNameForNewPrefab(GameObject target)
    {
        var nameInfo = new NamingInfo(target.name);
        return "Prefab_" + nameInfo.GetNameWithOutPrefix();
    }

    private async void RunTask()
    {
        Task task = ConvertFBXs();
        await task;
    }
    private async Task ConvertFBXs()
    {
        total = (float)Selection.gameObjects.Length;
        progressValue = 0f;
        foreach (GameObject target in Selection.gameObjects)
        {
            //Create the prefab object and the FBX holder
            newPrefabInCreation = new GameObject();
            newPrefabInCreation.name = _GetNameForNewPrefab(target);
            GameObject prefabChild = new GameObject();
            prefabChild.name = "Models";
            prefabChild.transform.SetParent(newPrefabInCreation.transform);

#if UNITY_EDITOR
            //Spawn the FBX inside the FBX holder
            GameObject fbxObject = (GameObject)PrefabUtility.InstantiatePrefab(target);
            fbxObject.transform.SetParent(prefabChild.transform);
            fbxObject.name = target.name;

            //Apply changes to the FBX transform if needed
            if (_resetFBXPosition)
                fbxObject.transform.position = Vector3.zero;
            if (_resetFBXScale)
                fbxObject.transform.localScale = Vector3.one;
#endif

            //Add the components that are required to the object newPrefabInCreation
            if (_componentsAreValid)
                _AddComponentsToObject(newPrefabInCreation);

            //Keep track of the objects we instantiated to delete them later
            _toDelete.Add(newPrefabInCreation);

            //Get the path to the asset folder
            string fbxPath = AssetDatabase.GetAssetPath(target);
            string modelsFolder = Directory.GetParent(fbxPath).FullName;
            string assetFolder = Directory.GetParent(modelsFolder).FullName;

            //Create the different folders inside the asset folder
            _GenerateFoldersInDirectory(assetFolder);
            string newAssetPath = assetFolder + "\\Prefabs\\" + newPrefabInCreation.name + ".prefab";

            //Check if there is already a prefab with the same name
            if (File.Exists(newAssetPath))
            {
                Debug.LogWarning("There is already a prefab called " + newPrefabInCreation.name + ". The prefab will not be overwritten.");
                continue;
            }

            //Save the prefab as an asset
            PrefabUtility.SaveAsPrefabAsset(newPrefabInCreation, newAssetPath);

            //Add the labels to the prefab
            string projectRelativePath = "Assets" + newAssetPath.Substring(Application.dataPath.Length);
            UnityEngine.Object targetForLabels = AssetDatabase.LoadMainAssetAtPath(projectRelativePath);
            _ApplyLabelsToObject(targetForLabels);

            progressValue += 1f;
            await Task.Delay(1);
        }

        AssetDatabase.Refresh();
        Repaint();
        conversionIsRunning = false;
    }
}
