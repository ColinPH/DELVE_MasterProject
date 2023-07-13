using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public class DataReader : EditorWindow
    {
        string _datasetFilesExtension = ".bin";
        Color _defaultGUIColour = new Color(38f / 255f, 38f / 255f, 38f / 255f);
        Color _selectedGUIColour = new Color(55f / 255f, 74f / 255f, 60f / 255f);
        Color _notVisualizedGUIColour = new Color(74f / 255f, 55f / 255f, 55f / 255f);
        Color _defaultVariantGUIColour = new Color(45f / 255f, 45f / 255f, 45f / 255f);
        public List<DatasetItem> datasetItems = new List<DatasetItem>();
        /// <summary> Datasets to remove at the end of the OnGUI, from the X buttons. </summary>
        List<DatasetItem> _datasetToRemove = new List<DatasetItem>();
        SerializedObject _customLabelsSO;

        //controls that apply to all datasets of the list
        bool _applyAll_ShowDeathPositions = false;
        bool _applyAll_ShowInteractionPositions = false;
        bool _applyAll_ShowPath = true;

        //Values used for simulation
        string _playerDummyPrefabAssetName = "Prefab_Player Dummy";
        GameObject _dummyPrefab;
        List<DatasetInstance> _activeSimulations = new List<DatasetInstance>();

        //Controls what information to show
        bool _showPlaythroughDates = true;

        //Errors that prevent the data reader from working, will be shown instead of the regular UI
        string _errorMessage = "";
        bool _hasErrorMessage = false;

        [MenuItem("Propeller Cap/Tools/DataReader")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(DataReader));
            window.titleContent = new GUIContent("Data Reader");
        }

        private void OnEnable()
        {
            _customLabelsSO = new SerializedObject(this);
            _FetchExistingDatasetInstances();
            _CheckForLostDatasets();
            _dummyPrefab = _FindPlayerDummyPrefab();
        }

        private void OnGUI()
        {
            _customLabelsSO.Update();

            GUILayout.Space(10);

            if (_hasErrorMessage)
            {
                string message = $"Some errors revent the {nameof(DataReader)} from initializing successfully. " +
                    $"Fix the folowing error and open the {nameof(DataReader)} again:\n\n{_errorMessage}";
                EditorGUILayout.HelpBox(message, MessageType.Error);
                _customLabelsSO.ApplyModifiedProperties();
                return;
            }

            //Display main controls
            _DrawMainControlButtons();

            GUILayout.Space(10);

            //Display the titles for each property in the list
            _DrawListTitles();

            _DrawHorizontalSeparationLine();

            //Draw the buttons to control all options
            _DrawControlAllOptions();

            _DrawHorizontalSeparationLine();

            //Display the list of dataset options
            foreach (DatasetItem item in datasetItems)
            {
                Color backgroundColor = _defaultGUIColour;
                if (datasetItems.IndexOf(item) % 2 == 0)
                    backgroundColor = _defaultVariantGUIColour;

                if (item.datasetInstance != null)
                {
                    if (Selection.gameObjects.Contains(item.datasetInstance.gameObject))
                        backgroundColor = _selectedGUIColour;
                }

                if (item.hasBeenVisualized == false)
                    backgroundColor = _notVisualizedGUIColour;

                _DrawDatasetInfo(item, backgroundColor);
                GUILayout.Space(4);
            }

            //Remove the datasets that could have been removed using the X button
            foreach (var item in _datasetToRemove)
            {
                datasetItems.Remove(item);
            }

            _customLabelsSO.ApplyModifiedProperties();
        }

        private void Update()
        {
            if (_activeSimulations.Count>  0)
            {
                List<DatasetInstance> simulationsToStop = new List<DatasetInstance>();
                foreach (DatasetInstance item in _activeSimulations)
                {
                    item.ProgressInSimulation();
                    if (item.dataset.simulationProgress == 1f)
                        simulationsToStop.Add(item);
                }

                foreach (DatasetInstance item in simulationsToStop)
                {
                    _StopSimulation(item.dataset);
                }
            }
        }


        private GameObject _FindPlayerDummyPrefab()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + _playerDummyPrefabAssetName);

            if (guids.Length == 0)
            {
                _errorMessage = $"There seems to be no prefab named \"{_playerDummyPrefabAssetName}\" in the assets folder.";
                _hasErrorMessage = true;
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private void _LogVisualizationMissing(string goalToAchieve)
        {
            Debug.LogError($"Can not {goalToAchieve} because the data has not been visualized.");
        }

        //******************************************************
        //Different methods to draw the interface, used in OnGUI
        //******************************************************

        #region Methods to draw the interface
        private void _DrawMainControlButtons()
        {
            GUILayout.BeginHorizontal();
            
            GUIStyle buttonStyle = GUI.skin.button;
            buttonStyle.wordWrap = true;
            if (GUILayout.Button("Add selected Datasets", buttonStyle, GUILayout.Height(40), GUILayout.MinWidth(90)))
            {
                if (_SelectionIsValid())
                {
                    _AddSelectionToDataset();
                }
            }
            if (GUILayout.Button("Visualize Data", buttonStyle, GUILayout.Height(40), GUILayout.MinWidth(70)))
            {
                _VisualizeData();
            }
            if (GUILayout.Button("Hide renderers", buttonStyle, GUILayout.Height(40), GUILayout.MinWidth(70)))
            {
                _HideRenderers();
            }
            if (GUILayout.Button("Remove renderers", buttonStyle, GUILayout.Height(40), GUILayout.MinWidth(70)))
            {
                _RemoveRenderers();
            }
            if (GUILayout.Button("Update Reader", buttonStyle, GUILayout.Height(40), GUILayout.MinWidth(100), GUILayout.ExpandWidth(true)))
            {
                _UpdateReader();
            }
            GUILayout.EndHorizontal();
        }

        private static void _DrawHorizontalSeparationLine()
        {
            // Define the line texture
            Texture2D lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, Color.grey);
            lineTexture.Apply();

            // Draw the line
            GUILayout.Box(GUIContent.none, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), lineTexture);
        }

        private void _DrawListTitles()
        {
            GUIStyle boldLabelStyle = new GUIStyle(GUI.skin.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;
            boldLabelStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level Name", boldLabelStyle, GUILayout.MinWidth(80), GUILayout.MaxWidth(120));
            GUILayout.Label("Path Color", boldLabelStyle, GUILayout.MinWidth(70), GUILayout.MaxWidth(70));
            boldLabelStyle.wordWrap = true;
            GUILayout.Label("Death", boldLabelStyle, GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
            GUILayout.Label("Inter.", boldLabelStyle, GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
            boldLabelStyle.wordWrap = false;
            GUILayout.Label("Remove Path", boldLabelStyle, GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
            GUILayout.EndHorizontal();
        }

        private void _DrawControlAllOptions()
        {
            GUIStyle boldLabelStyle = new GUIStyle(GUI.skin.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;
            boldLabelStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal();
            //Title
            GUILayout.Label("", boldLabelStyle, GUILayout.MinWidth(80), GUILayout.MaxWidth(120));
            GUILayout.Label("", GUILayout.MinWidth(70), GUILayout.MaxWidth(70));
            //Toggle to show the death positions
            bool tempBool = GUILayout.Toggle(_applyAll_ShowDeathPositions, "", GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
            if (tempBool != _applyAll_ShowDeathPositions)
            {
                _applyAll_ShowDeathPositions = tempBool;
                foreach (var item in datasetItems)
                {
                    if (_applyAll_ShowDeathPositions)
                        _ShowPlayerDeath(item);
                    else
                        _HidePlayerDeath(item);
                }
            }

            //Toggle to show the interaction positions
            tempBool = GUILayout.Toggle(_applyAll_ShowInteractionPositions, "", GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
            if (tempBool != _applyAll_ShowInteractionPositions)
            {
                _applyAll_ShowInteractionPositions = tempBool;
                foreach (var item in datasetItems)
                {
                    if (_applyAll_ShowInteractionPositions)
                        _ShowPlayerInteractions(item);
                    else
                        _HidePlayerInteractions(item);
                }
            }

            //Show or Hide all item paths
            GUILayout.Label("", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
            string buttonText = _applyAll_ShowPath ? "Show" : "Hide";
            if (GUILayout.Button(buttonText, GUILayout.MinWidth(50), GUILayout.MaxWidth(50)))
            {
                foreach (var item in datasetItems)
                {
                    if (_applyAll_ShowPath) 
                        _ShowDataset(item);
                    else
                        _HideDataset(item);
                }
                _applyAll_ShowPath = !_applyAll_ShowPath;
            }

            GUILayout.EndHorizontal();
        }

        private void _DrawDatasetInfo(DatasetItem dataset, Color backgroundColor)
        {
            //The vertical group is for the background
            GUILayout.BeginVertical();
            Rect rect = GUILayoutUtility.GetRect(0, 0);
            float lineHeight = EditorGUIUtility.singleLineHeight + 5;
            if (dataset.isInspecting) lineHeight += 2 * 20 + 4;
            Rect boxRect = new Rect(rect.x + 2, rect.y, rect.width - 4, lineHeight);
            EditorGUI.DrawRect(boxRect, backgroundColor);

            GUILayout.BeginHorizontal();
            //Level name
            GUILayout.Label(dataset.levelName, GUILayout.MinWidth(80), GUILayout.MaxWidth(120));

            //Date of the playthrough
            if (_showPlaythroughDates)
                GUILayout.Label(dataset.dateTime.ToString("dd/MM/yy"), GUILayout.Width(60));

            //Color ofthe path
            dataset.startColor = EditorGUILayout.ColorField(dataset.startColor, GUILayout.Width(40));

            //Show player death toggle
            bool tempBool = GUILayout.Toggle(dataset.showDeaths, "", GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
            if (tempBool != dataset.showDeaths) //Value has been changed
            {
                if (dataset.showDeaths == false) //We have to do the inverse because we try to go to the new state
                    _ShowPlayerDeath(dataset);
                else
                    _HidePlayerDeath(dataset);
            }

            //Show player interaction toggle
            tempBool = GUILayout.Toggle(dataset.showInteractions, "", GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
            if (tempBool != dataset.showInteractions) //Value has been changed
            {
                if (dataset.showInteractions == false) //We have to do the inverse because we try to go to the new state
                    _ShowPlayerInteractions(dataset);
                else
                    _HidePlayerInteractions(dataset);
            }

            //Highlight the dataset by hiding others
            if (GUILayout.Button("Highlight", GUILayout.Width(65)))
            {
                _HighlightDataset(dataset);
            }

            //Control whether to show or hide the dataset
            if (dataset.isShown == false)
                GUI.backgroundColor = Color.grey;
            string buttonText = dataset.isShown ? "Shown" : "Hidden";
            if (GUILayout.Button(buttonText, GUILayout.Width(55)))
            {
                if (dataset.isShown == false) //We have to do the inverse because we try to go to the new state
                    _ShowDataset(dataset);
                else
                    _HideDataset(dataset);
            }
            GUI.backgroundColor = Color.white;

            //Inspect elements, showing the slider to control where to place the dummy
            if (dataset.isInspecting)
                GUI.backgroundColor = Color.grey;
            buttonText = dataset.isInspecting ? "Inspect" : "Inspect";
            if (GUILayout.Button(buttonText, GUILayout.Width(60)))
            {
                if (dataset.isInspecting == false) //We have to do the inverse because we try to go to the new state
                    _StartInspecting(dataset);
                else
                    _StopInspecting(dataset);
            }
            GUI.backgroundColor = Color.white;

            //Remove dataset from list
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _RemoveDatasetItem(dataset);
            }

            GUILayout.EndHorizontal();

            if (dataset.isInspecting)
            {
                //Draw the fields with the Go to and simulate buttons
                _DrawSimulationSlider(dataset);

                //Draw the fields to control the line renderer options
                _DrawLineRendererOptions(dataset);
            }

            //The vertical group is for the background
            GUILayout.EndVertical();
        }

        private void _DrawSimulationSlider(DatasetItem dataset)
        {
            GUILayout.BeginHorizontal();
            float newSimulationProgress = GUILayout.HorizontalSlider(dataset.simulationProgress, 0.0f, 1.0f, GUILayout.MinWidth(300), GUILayout.MaxWidth(340), GUILayout.Height(20));
            if (Mathf.Abs(newSimulationProgress - dataset.simulationProgress) >= 0.001)
            {
                dataset.simulationProgress = newSimulationProgress;
                _UpdateDummyProgression(dataset);
            }

            //Focus the scene camera on the dummy
            if (GUILayout.Button("Go to", GUILayout.Width(45)))
            {
                _FocusCameraOnDummy(dataset);
            }

            //Play or stop the movement of the dummy along the data
            string buttonText = dataset.isSimulating ? "Stop" : "Simulate";
            if (GUILayout.Button(buttonText, GUILayout.Width(70)))
            {
                if (dataset.isSimulating == false) //We have to do the inverse because we try to go to the new state
                    _StartSimulation(dataset);
                else
                    _StopSimulation(dataset);
            }
            GUILayout.EndHorizontal();
        }

        private void _DrawLineRendererOptions(DatasetItem dataset)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Width", GUILayout.Width(40));
            float lineWidth = dataset.lineRenderer.widthMultiplier;
            lineWidth = GUILayout.HorizontalSlider(lineWidth, 0.1f, 1.0f, GUILayout.Width(50), GUILayout.Height(20));
            dataset.lineRenderer.widthMultiplier = lineWidth;
            GUILayout.Label($"[{lineWidth.ToString("0.00")}]", GUILayout.Width(35));
            int totalPositions = dataset.totalPoints;
            int currentPoint = UnityEngine.Mathf.FloorToInt(dataset.simulationProgress * totalPositions);
            GUILayout.Label($"Points [{currentPoint}-{totalPositions}]", GUILayout.Width(130));
            if (GUILayout.Button("Simplify", GUILayout.Width(70)))
            {
                dataset.lineRenderer.Simplify(dataset.tolerance);
            }
            if (GUILayout.Button("Reload", GUILayout.Width(70)))
            {
                Debug.Log("Not implemented, reload the data from file.");
            }
            GUILayout.EndHorizontal();
        }

        #endregion Methods to draw the interface

        //*********************************************
        //Main control buttons at the top of the window
        //*********************************************

        #region General control methods

        /// <summary> Serach the scene for existing dataset instances that have already been made. And add them to the list. </summary>
        private void _FetchExistingDatasetInstances()
        {
            DatasetInstance[] datasetInstances = FindObjectsOfType<DatasetInstance>(true);
            foreach (DatasetInstance item in datasetInstances)
            {
                if (datasetItems.Contains(item.dataset) == false)
                    datasetItems.Add(item.dataset);
            }
        }

        /// <summary> Removes all datasets that do not have a gameObject to represent them in the scene. </summary>
        private void _CheckForLostDatasets()
        {
            List<DatasetItem> itemsToRemove = new List<DatasetItem>();
            foreach (DatasetItem item in datasetItems)
            {
                if (item.datasetInstance == null)
                    itemsToRemove.Add(item);
            }
            foreach (DatasetItem item in itemsToRemove)
            {
                datasetItems.Remove(item);
            }
        }

        private bool _SelectionIsValid()
        {
            if (Selection.objects.Length == 0)
            {
                Debug.LogError($"Trying to add datasets but selection is empty.");
                return false;
            }

            bool selectionIsValid = true;
            foreach (UnityEngine.Object item in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(item);
                string extension = Path.GetExtension(path);
                string fileName = Path.GetFileName(path);
                if (extension != _datasetFilesExtension)
                {
                    selectionIsValid = false;
                    Debug.LogError($"The following file in the selection is not a valid data file \"{fileName}\". The file needs to have a .bin extension.");
                    break;
                }
            }
            return selectionIsValid;
        }

        private void _AddSelectionToDataset()
        {
            List<UnityEngine.Object> existingSourceFiles = new List<UnityEngine.Object>();
            foreach (var item in datasetItems)
            {
                existingSourceFiles.Add(item.datasetAsset);
            }

            bool assignedAtLeastOne = false;
            foreach (UnityEngine.Object item in Selection.objects)
            {
                if (existingSourceFiles.Contains(item) == false)
                {
                    existingSourceFiles.Add(item);

                    //Create an object in the scene to represent the dataset
                    GameObject obj = new GameObject();
                    DatasetInstance dataInstance = obj.AddComponent<DatasetInstance>();
                    dataInstance.SetDataset(new DatasetItem(item));
                    obj.name = $"Dataset Visualizer_{dataInstance.dataset.levelName}_{dataInstance.dataset.dateTime.ToString("dd/MM/yy HH:mm:ss")}";
                    LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
                    //Setup the line renderer
                    lineRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));
                    if (lineRenderer.sharedMaterial == null)
                        Debug.Log("fvgsgvsv");
                    lineRenderer.sharedMaterial.color = dataInstance.dataset.startColor;
                    lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    lineRenderer.allowOcclusionWhenDynamic = false;
                    lineRenderer.shadowBias = 0;
                    lineRenderer.widthMultiplier = 0.1f;

                    dataInstance.dataset.lineRenderer = lineRenderer;
                    datasetItems.Add(dataInstance.dataset);
                    assignedAtLeastOne = true;
                }
            }

            if (assignedAtLeastOne == false)
                Debug.Log($"All datasets in the selection have already been added.");
        }
        
        private void _VisualizeData()
        {
            foreach (DatasetItem item in datasetItems)
            {
                if (item.hasBeenVisualized)
                    continue;

                item.ProcessPlayerDataSamples();
                item.lineRenderer.positionCount = item.totalPoints;
                item.lineRenderer.SetPositions(item.playerPositions.ToArray());
                item.hasBeenVisualized = true;
            }
        }

        private void _HideRenderers()
        {
            foreach (DatasetItem item in datasetItems)
            {
                if (item.isSimulating)
                    _StopSimulation(item);
                if (item.isShown)
                    _HideDataset(item);
                if (item.isInspecting)
                    _StopInspecting(item);
            }
        }

        private void _RemoveRenderers()
        {
            _HideRenderers();
            foreach (DatasetItem item in datasetItems)
            {
                if (item.datasetInstance != null)
                    DestroyImmediate(item.datasetInstance.gameObject);
            }
            datasetItems.Clear();
        }

        private void _UpdateReader()
        {
            datasetItems.Clear();
            _FetchExistingDatasetInstances();
        }

        private void _RemoveDatasetItem(DatasetItem dataset)
        {
            if (dataset.isSimulating)
                _StopSimulation(dataset);
            if (dataset.isShown)
                _HideDataset(dataset);
            if (dataset.isInspecting)
                _StopInspecting(dataset);

            DestroyImmediate(dataset.datasetInstance.gameObject);
            _datasetToRemove.Add(dataset);
        }

        #endregion General control methods

        //**********************************************
        //Methods to affect the paths shown in the scene
        //**********************************************

        #region Methods for paths in scene
        private void _HideDataset(DatasetItem dataset)
        {
            if (dataset.datasetInstance == null)
                return;

            if (dataset.isSimulating)
                _StopSimulation(dataset);
            if (dataset.isInspecting)
                _StopInspecting(dataset);

            dataset.datasetInstance.gameObject.SetActive(false);
            dataset.isShown = false;
        }

        private void _ShowDataset(DatasetItem dataset)
        {
            if (dataset.datasetInstance == null)
                return;

            dataset.datasetInstance.gameObject.SetActive(true);
            dataset.isShown = true;
        }

        private void _HighlightDataset(DatasetItem dataset)
        {
            foreach (var item in datasetItems)
            {
                if (item == dataset) 
                    continue;
                _HideDataset(item);
            }
            _ShowDataset(dataset);
        }

        private void _ShowPlayerInteractions(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                _LogVisualizationMissing("show player interaction markers");
                return;
            }

            dataset.showInteractions = true;
            dataset.datasetInstance.ProcessDataForInteractionGizmos();
        }

        private void _HidePlayerInteractions(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                _LogVisualizationMissing("hide player interaction markers");
                return;
            }
            dataset.showInteractions = false;
        }

        private void _ShowPlayerDeath(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                _LogVisualizationMissing("show player death marker");
                return;
            }
            
            dataset.showDeaths = true;
            dataset.datasetInstance.ProcessDataForDeathGizmos();
        }

        private void _HidePlayerDeath(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                _LogVisualizationMissing("hide player death marker");
                return;
            }

            dataset.showDeaths = false;
        }
        #endregion Methods for paths in scene

        //*********************************************************
        //Methods for the simulation of the dummies along the paths
        //*********************************************************

        #region Methods for simulation
        private void _StartInspecting(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                Debug.LogError("Can not inspect the element if the data has not been visualized.");
                return;
            }

            dataset.isInspecting = true;
            if (dataset.isShown == false)
                _ShowDataset(dataset);

            //Create the dummy if it is not already there and place it at the progression value on the path
            dataset.datasetInstance.CreateDummy(_dummyPrefab);
            dataset.datasetInstance.UpdateDummyPosition();
        }

        private void _StopInspecting(DatasetItem dataset)
        {
            dataset.isInspecting = false;
            dataset.datasetInstance.HideDummy();
        }

        private void _StartSimulation(DatasetItem dataset)
        {
            dataset.isSimulating = true;
            if (_activeSimulations.Contains(dataset.datasetInstance) == false)
                _activeSimulations.Add(dataset.datasetInstance);
        }

        /// <summary> Called when the simulation slider has moved. </summary>
        private void _UpdateDummyProgression(DatasetItem dataset)
        {
            if (dataset.hasBeenVisualized == false)
            {
                Debug.LogWarning("Can not update progression if the dataset has not been visualized.");
                return;
            }

            dataset.datasetInstance.UpdateDummyPosition();
        }

        private void _StopSimulation(DatasetItem dataset)
        {
            dataset.isSimulating = false;
            if (_activeSimulations.Contains(dataset.datasetInstance))
                _activeSimulations.Remove(dataset.datasetInstance);
        }

        private void _FocusCameraOnDummy(DatasetItem dataset)
        {
            UnityEngine.Object[] oldSelection = Selection.objects;
            Selection.activeGameObject = dataset.datasetInstance.DummyObj;
            SceneView.lastActiveSceneView.FrameSelected();
            Selection.objects = oldSelection;
        }
        #endregion Methods for simulation
    }
}
