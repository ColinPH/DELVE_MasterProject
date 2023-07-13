using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif 
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PropellerCap
{
    public class SceneLoader : ManagerBase
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] string _rootSceneName = "Scene_Root";
        [SerializeField] LoadableGroup _referenceMandatoryScenes;
        [SerializeField] LoadableGroup _referenceSpecialScenes;
        [Header("Default loading settings")]
        [SerializeField] LevelLoadingInfo _defaultLevelLoadingSettings;
        [SerializeField] CorridorLoadingInfo _defaultCorridorLoadingSettings;
        [SerializeField] UniqueSceneLoadingInfo _defaultUniqueLoadingSettings;
        [SerializeField] TutorialLoadingInfo _defaultTutorialLoadingSettings;
        [Header("Default unloading settings")]
        [SerializeField] LevelUnloadingInfo _defaultLevelUnloadingSettings;
        [SerializeField] CorridorUnloadingInfo _defaultCorridorUnloadingSettings;
        [SerializeField] UniqueSceneUnloadingInfo _defaultUniqueUnloadingSettings;
        [SerializeField] TutorialUnloadingInfo _defaultTutorialUnloadingSettings;

        public string rootSceneName => _rootSceneName;
        public SceneGroup lastLoadedSceneGroup => _lastLoadedSceneGroup;
        public object lastSceneIdentifier => _lastSceneIdentifier;

        private IEnumerator _loadingRoutine;
        private IEnumerator _unloadingRoutine;

        //Mandatory scene families
        private LoadableGroup _mandatoryScenes;
        //Disposable scene families
        private LoadableGroup _specialScenes;

        //Keep track of the last loaded scenes
        SceneGroup _lastLoadedSceneGroup;
        /// <summary> What allows the identification of the scene within its group, examples : Level1, Level2, Corridor4, Tutorial2, MainMenu, Credits... </summary>
        object _lastSceneIdentifier;

        //Validation checks set in initialization
        bool _sceneFamiliesAreValid = false;
        bool _scenesAreInBuildSettings = false;
        bool _scriptableObjectsHaveBeenInstantiated = false;

        //Contains all scenes that are currently Loaded
        Dictionary<LoadableObject, LoadedSceneInfo> _activeLoadableObjects = new Dictionary<LoadableObject, LoadedSceneInfo>();

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in Scene Loader.");
            Managers.sceneLoader = this;
        }

        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Scene Loader.");
            _sceneFamiliesAreValid = _CheckSceneFamilies();
#if UNITY_EDITOR
            _scenesAreInBuildSettings = _PutScenesInBuildSettings();
#endif

            _scriptableObjectsHaveBeenInstantiated = _InstantiateFamilies();

#if UNITY_EDITOR            
            //Add the scenes that are already loaded in the editor to the loaded scenes list
            Dictionary<LoadableObject, List<string>> existingLoadableObjects = _ProcessOpenedEditorScenes();

            //Check that the already loaded scenes belong to only 1 loadableObject (besides root).
            //This will otherwise create a conflict with the _Co_StartGameLoopFromLoadedSceneInEditor in GameManager because of the alreadyLoaedSceneGroup and identifier (only one loadable is supported)
            if (existingLoadableObjects.Count > 1)
                Debug.Log($"Not all scenes opened in the editor belong to the same LoadbleObject. This can cause problems. One of the faulty scenes is : {existingLoadableObjects.Last().Value[0]}.");
#endif
        }

        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Scene Loader.");
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<SceneLoader>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }

#if UNITY_EDITOR
        /// <summary> Looks at all the existing scenes and adds them to the loaded scenes list as if they had been loaded through the scene loader. </summary>
        private Dictionary<LoadableObject, List<string>> _ProcessOpenedEditorScenes()
        {
            Dictionary<LoadableObject, List<string>> toReturn = new Dictionary<LoadableObject, List<string>>();
            Debugger.LogSceneLoading("Processing editor scenes.");
            // Get a list of all loaded scenes
            Scene[] loadedScenes = new Scene[SceneManager.loadedSceneCount];
            for (int i = 0; i < loadedScenes.Length; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }
            Debugger.LogSceneLoading("Scenes count: " + loadedScenes.Length);

            //Get all the scene Objects assigned to the SceneLoader
            Dictionary<LoadableObject, SceneGroup> assignedLoadables = _GetAllAssignedScenes();

            //Iterate through the list of loaded scenes
            foreach (Scene scene in loadedScenes)
            {
                SceneAsset loadedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                Debug.LogWarning("Scene name : " + scene.name);
                //Check all loadables to see if they contain a scene asset that matches the current loaded scene
                foreach (KeyValuePair<LoadableObject, SceneGroup> loadable in assignedLoadables)
                {
                    foreach (UnityEngine.Object sceneObj in loadable.Key.GetAllSceneAssets())
                    {
                        string scenePath = AssetDatabase.GetAssetPath(sceneObj);
                        //Get the scene asset assigned to the loadable
                        SceneAsset assignedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                        //Check if that scene asset matches the one already loaded in the editor
                        if (loadedSceneAsset.name == assignedScene.name)
                        {
                            //Found a scene that matches the loaded scene
                            
                            //Save the loadable for checking if there is more than 1 loaded in editor
                            if (toReturn.ContainsKey(loadable.Key))
                                toReturn[loadable.Key].Add(loadedSceneAsset.name);
                            else
                                toReturn.Add(loadable.Key, new List<string> { loadedSceneAsset.name });

                            //Keep track of the loaded scene
                            _AddSceneNameToActiveLoadableObjects(loadable.Key, loadedSceneAsset.name, loadable.Value);

                            //Pretend the editor scene has been loaded from the SceneLoader to allow for the StartGame process to work (in GameManager)
                            _lastLoadedSceneGroup = loadable.Value;
                            _lastSceneIdentifier = loadable.Key.SceneIdentifier;
                        }
                    }
                }
            }
            return toReturn;
        }
#endif
        #endregion Initialization

        //--------------------

        //--------------------

        #region For GameManager
        /// <summary> Load the root scene if not already loaded. </summary>
        public IEnumerator Co_LoadRootScene(LoadSceneMode loadMode)
        {
            Debugger.LogSceneLoading("Loading Root scene with load mode : " + loadMode);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == _rootSceneName)
                {
                    Debugger.LogError("Trying to load the root scene while there is already one in the scene.");
                    yield break;
                }
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_rootSceneName, loadMode);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        /// <summary> This method is for the game manager at the beginning of the game. </summary>
        public IEnumerator Co_LoadMandatoryScenes()
        {
            if (_sceneFamiliesAreValid == false)
                yield break;

            Debugger.LogSceneLoading("Started loading : mandatory scenes.");
            Debugger.StartTimer("Family");

            foreach (LoadableObject item in _mandatoryScenes.GetLoadableObjects())
            {
                yield return StartCoroutine(_Co_LoadScenesWithinLoadable(item, SceneGroup.Mandatory, null, SceneTargets.All));
            }

            //Log the time it took
            Debugger.LogSceneLoading("Done loading : mandatory scenes." +
                " after " + Debugger.StopTimer("Family").ToString("0.00"));
        }        
        
        /// <summary> Unloads all the scenes and then reloads the Root scene non additively. </summary>
        public IEnumerator Co_ReloadGame()
        {
            yield return StartCoroutine(_Co_UnloadAll());

            yield return StartCoroutine(Co_LoadRootScene(LoadSceneMode.Single));
        }
        public LoadableObject GetLoadable(UniqueScene uniqueScene)
        {
            foreach (UniqueSceneObject item in _specialScenes.GetLoadableObjects())
            {
                if (item.SceneType == uniqueScene)
                    return item;
            }
            Debug.LogError($"The loadable family \"{_specialScenes.GroupName}\" does not contain a unique loadable of type {uniqueScene}. Check the SO and add the loadable to the family.");
            return null;
        }
        #endregion

        //--------------------

        //--------------------

        #region Public scene loading methods
        public void LoadLevelSection(string levelSectionName, LoadableObject loadableParent)
        {
            _loadingRoutine = Co_LoadLevelSection(levelSectionName, loadableParent);
            StartCoroutine(_loadingRoutine);
        }

        public IEnumerator Co_LoadLevelSection(string levelSectionName, LoadableObject loadableParent)
        {
            //Find the loadable
            LoadableObject loadable = null;
            LoadedSceneInfo info = null;
            foreach (var item in _activeLoadableObjects)
            {
                //if (item.Key.GetOptionalSceneNames().Contains(optionalLevelName))
                if (item.Key.GetAllSceneNames().Contains(levelSectionName))
                {
                    loadable = item.Key;
                    info = item.Value;
                    break;
                }
            }

            if (loadable == null)
            {
                Debugger.LogWarning($"The loadable object \"{loadableParent.LoadableName}\" does not contain an optional scene named \"{levelSectionName}\". Make sure it is assigned in the SO and that the list of scene names are matching the SceneAsset lists.");
                yield break;
            }

            //If the level is laready loaded we do not load it again
            if (info.ContainsSceneNamed(levelSectionName))
            {
                Debugger.LogWarning($"Trying to load \"{levelSectionName}\" but it is already loaded.");
                yield break;
            }

            //Load the optional level
            yield return StartCoroutine(_Co_LoadNewScene(levelSectionName, loadable, LoadSceneMode.Additive, SceneGroup.Unassigned));

            //TODO add the load information here
            Managers.gameManager.PostSceneLoadingProcess(null);
        }

        public IEnumerator Co_Load(SceneLoadPair sceneToLoad)
        {
            yield return StartCoroutine(_Co_LoadScenesWithinLoadable(sceneToLoad.loadable, SceneGroup.Unassigned, sceneToLoad.loadSettings, sceneToLoad.sceneTargets));
        }
        public IEnumerator Co_Load(SceneLoadPair sceneToLoad, bool unloadAllDisposable)
        {
            //Fetch the disposable scenes before we load the new ones
            List<LoadedSceneInfo> loadedDisposables = new List<LoadedSceneInfo>();
            if (unloadAllDisposable)
                loadedDisposables = _GetDisposableScenesWithin(_activeLoadableObjects.Values.ToList());
            
            //Load the new scenes
            yield return StartCoroutine(_Co_LoadScenesWithinLoadable(sceneToLoad.loadable, SceneGroup.Unassigned, sceneToLoad.loadSettings, sceneToLoad.sceneTargets));
            
            //Unload the scenes that were already there
            if (unloadAllDisposable)
                yield return StartCoroutine(_Co_UnloadDisposable(loadedDisposables));
        }

        public IEnumerator Co_Load(SceneLoadPair sceneToLoad, params SceneLoadPair[] scenesToUnload)
        {
            yield return StartCoroutine(_Co_LoadScenesWithinLoadable(sceneToLoad.loadable, SceneGroup.Unassigned, sceneToLoad.loadSettings, sceneToLoad.sceneTargets));
            foreach (SceneLoadPair item in scenesToUnload)
            {
                yield return StartCoroutine(Co_Unload(item));
            }
        }
        #endregion Public scene loading methods

        //--------------------

        //--------------------

        #region Private scene loading methods

        private IEnumerator _Co_LoadScenesWithinLoadable(LoadableObject loadable, SceneGroup sceneGroup, object loadingInfo, SceneTargets sceneTargets)
        {
            if (sceneGroup == SceneGroup.Unassigned)
                sceneGroup = loadable.SceneGroup;

            //Do not fire events for loading mandatory scenes, other sceens should not react to the mandatory ones because they are supposed to already be there
            if (sceneGroup != SceneGroup.Mandatory)
                Managers.eventManager.FireMainGameEvent(GameEvent.PreSceneLoad, this, sceneGroup, loadable.SceneIdentifier);

            List<string> scenes;
            if (sceneTargets == SceneTargets.All)
                scenes = loadable.GetAllSceneNames();
            else
                scenes = loadable.GetMainSceneNames();

            Debugger.LogSceneLoading("Started loading : " + loadable.LoadableName);
            Debugger.StartTimer("Loadable");
            foreach (string item in scenes)
            {
                yield return StartCoroutine(_Co_LoadNewScene(item, loadable, LoadSceneMode.Additive, sceneGroup));
            }
            Debugger.LogSceneLoading("Done loading : " + loadable.LoadableName +
                " after " + Debugger.StopTimer("Loadable").ToString("0.00"));

            _lastSceneIdentifier = loadable.SceneIdentifier;
            _lastLoadedSceneGroup = sceneGroup;

            //Again not firing event if it is a mandatory scene
            if (sceneGroup != SceneGroup.Mandatory)
                Managers.eventManager.FireMainGameEvent(GameEvent.SceneLoaded, this, sceneGroup, loadable.SceneIdentifier);           
        }

        IEnumerator _Co_LoadNewScene(string sceneName, LoadableObject loadable, LoadSceneMode mode, SceneGroup sceneGroup)
        {
            //If the scene is aleady loaded we do not load it again, already loaded scenes in editor have been processed in the SceneLoader MyAwake method
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                    yield break;
            }

            //Add the loadable to keep track of it            
            _AddSceneNameToActiveLoadableObjects(loadable, sceneName, sceneGroup);

            Debugger.LogSceneLoading("Started loading sceneAsset : " + sceneName);
            Debugger.StartTimer("Load Scene");

            //Wait for he scene to load
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Debugger.LogSceneLoading("Done loading sceneAsset : " + sceneName +
                " after " + Debugger.StopTimer("Load Scene").ToString("0.00") + " seconds.");
        }

        /// <summary> For custom inspector use. Makes it possible to load scenes into the editor. </summary>
        private void _LoadScenesInEditor(string sceneName)
        {
#if UNITY_EDITOR
            //If the editor is not in playmode, scene loading needs to be done differently
            //Some scenes might already be added to the scene by the developper
            //This doesn't or shouldn't happen in standalone

            if (Application.isPlaying)
                return;

            //If the scene is aleady loaded we do not load it again
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                    return;
            }

            //Add the scene to the editor
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (scenePath.Contains(sceneName))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
            }
#endif
        }

        #endregion

        //--------------------

        //--------------------

        #region Public scene unloading methods
        public void UnloadLevelSection(string levelSectionName, LoadableObject loadableParent)
        {
            _unloadingRoutine = Co_UnloadLevelSection(levelSectionName, loadableParent);
            StartCoroutine(_unloadingRoutine);
        }

        public IEnumerator Co_UnloadLevelSection(string levelSectionName, LoadableObject loadableParent)
        {
            //Find the loadable
            LoadableObject loadable = null;
            LoadedSceneInfo info = null;
            foreach (var item in _activeLoadableObjects)
            {
                //if (item.Key.GetOptionalSceneNames().Contains(optionalLevelName))
                if (item.Key.GetAllSceneNames().Contains(levelSectionName))
                {
                    loadable = item.Key;
                    info = item.Value;
                    break;
                }
            }

            if (loadable == null)
            {
                Debugger.LogWarning($"The loadable object \"{loadableParent.LoadableName}\" does not contain an optional scene named \"{levelSectionName}\". Make sure it is assigned in the SO and that the list of scene names are matching the SceneAsset lists.");
                yield break;
            }

            //If the level is laready loaded we do not load it again
            if (info.ContainsSceneNamed(levelSectionName) == false)
            {
                Debugger.LogWarning($"Trying to unload \"{levelSectionName}\" but it is already unloaded.");
                yield break;
            }

            //Load the optional level
            yield return StartCoroutine(_Co_UnloadScene(levelSectionName, loadable));
        }

        public IEnumerator Co_Unload(SceneLoadPair sceneToUnload)
        {
            //Debug.Log($"pair {sceneToUnload == null}");
            //Debug.Log($"loadable {sceneToUnload.loadable == null}");
            //Debug.Log($"unloadSettings {sceneToUnload.unloadSettings == null}");

            yield return StartCoroutine(_Co_UnloadScenesWithinLoadable(sceneToUnload.loadable, sceneToUnload.unloadSettings));
        }
        #endregion

        //--------------------

        //--------------------

        #region Private scene unloading methods
        private IEnumerator _Co_UnloadScenesWithinLoadable(LoadableObject loadableToUnload, object unloadingInfo)
        {
            SceneGroup sceneGroup = loadableToUnload.SceneGroup;

            //Do not fire events for unloading mandatory scenes, other sceens should not react to the mandatory ones because they are the first and lsat scenes to exist
            if (sceneGroup != SceneGroup.Mandatory)
                Managers.eventManager.FireMainGameEvent(GameEvent.PreSceneUnload, this, sceneGroup, loadableToUnload.SceneIdentifier);

            Debugger.LogSceneLoading("Started unloading : " + loadableToUnload.LoadableName);
            Debugger.StartTimer("Unloadable");
            //Unload all the scenes that are inside the active loadable
            foreach (string item in new List<string>(_activeLoadableObjects[loadableToUnload].loadedSceneNames))
            {
                yield return StartCoroutine(_Co_UnloadScene(item, loadableToUnload));
            }
            Debugger.LogSceneLoading("Done unloading : " + loadableToUnload.LoadableName +
                " after " + Debugger.StopTimer("Unloadable").ToString("0.00"));

            //Again not firing event if it is a mandatory scene
            if (sceneGroup != SceneGroup.Mandatory)
                Managers.eventManager.FireMainGameEvent(GameEvent.SceneUnloaded, this, sceneGroup, loadableToUnload.SceneIdentifier);
        }

        private IEnumerator _Co_UnloadScene(string sceneName, LoadableObject loadable)
        {
            Debugger.LogSceneLoading("Started unloading sceneAsset : " + sceneName);
            Debugger.StartTimer("Unload Scene");

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }

            //Remove the loadable from the active list
            _RemoveSceneNameFromActiveLoadableObjects(loadable, sceneName);

            Debugger.LogSceneLoading("Done unloading sceneAsset : " + sceneName +
                " after " + Debugger.StopTimer("Unload Scene").ToString("0.00") + " seconds.");
        }

        private IEnumerator _Co_UnloadDisposable(List<LoadedSceneInfo> scenesToUnload)
        {
            List<LoadedSceneInfo> loadedScenes = _GetDisposableScenesWithin(scenesToUnload);
            Debugger.LogSceneLoading($"Started to unload the disposable scenes. {loadedScenes.Count} scenes to unload.");
            foreach (LoadedSceneInfo item in loadedScenes)
            {
                if (item.sceneGroup == SceneGroup.Level)
                    yield return StartCoroutine(Co_Unload(new SceneLoadPair(item.loadable, null, _defaultLevelUnloadingSettings, SceneTargets.All)));
                else if (item.sceneGroup == SceneGroup.Corridor)
                    yield return StartCoroutine(Co_Unload(new SceneLoadPair(item.loadable, null, _defaultCorridorUnloadingSettings, SceneTargets.All)));
                else if (item.sceneGroup == SceneGroup.UniqueScene)
                    yield return StartCoroutine(Co_Unload(new SceneLoadPair(item.loadable, null, _defaultUniqueUnloadingSettings, SceneTargets.All)));
                else if (item.sceneGroup == SceneGroup.Tutorial)
                    yield return StartCoroutine(Co_Unload(new SceneLoadPair(item.loadable, null, _defaultTutorialUnloadingSettings, SceneTargets.All)));
            }
            Debugger.LogSceneLoading("Done unloading the disposable scenes.");
        }

        /// <summary> Unloads all Mandatory and Disposable scenes. Disposable scenes will be unloaded first. </summary>
        private IEnumerator _Co_UnloadAll()
        {
            Debugger.LogSceneLoading("Started unloading all scenes.");
            //Unload the disposable scenes first
            yield return StartCoroutine(_Co_UnloadDisposable(_activeLoadableObjects.Values.ToList()));

            //Unload the mandatory scenes
            foreach (LoadedSceneInfo item in new List<LoadedSceneInfo>(_activeLoadableObjects.Values))
            {
                if (item.disposable == false)
                {
                    yield return StartCoroutine(Co_Unload(new SceneLoadPair(item.loadable, null, _defaultUniqueUnloadingSettings, SceneTargets.All)));
                }
            }

            //There should be no active loadables anymore
            if (_activeLoadableObjects.Count > 0)
            {
                string remainingLoadables = "\n";
                foreach (var item in _activeLoadableObjects.Keys)
                {
                    remainingLoadables += item.LoadableName + "\n";
                }
                Debug.LogError("The dictionary of active loadableObjects should be empty. Loadable must not have been removed when unloading." +
                    " Remaining loadables are : " + remainingLoadables);
            }

            Debugger.LogSceneLoading("Done unloading all scenes");
        }
        #endregion

        //--------------------

        //--------------------

        #region Scenes to build settings and SceneFamily checks
        private bool _CheckSceneFamilies()
        {
            bool familiesAreValid = true;
            string wrongFamilyName = "";
            string wrongAssetName = "";

            if (_referenceMandatoryScenes == null || _referenceMandatoryScenes.GroupIsValid(out wrongAssetName) == false)
            {
                wrongFamilyName = nameof(_referenceMandatoryScenes);
                familiesAreValid = false;
            }
            /*if (_referenceTutorial == null || _referenceTutorial.FamilyIsValid(out wrongAssetName) == false)
            {
                wrongFamilyName = nameof(_referenceTutorial);
                familiesAreValid = false;
            }
            if (_referenceCorridors == null || _referenceCorridors.FamilyIsValid(out wrongAssetName) == false)
            {
                wrongFamilyName = nameof(_referenceCorridors);
                familiesAreValid = false;
            }
            if (_referenceLevels == null || _referenceLevels.FamilyIsValid(out wrongAssetName) == false)
            {
                wrongFamilyName = nameof(_referenceLevels);
                familiesAreValid = false;
            }*/
            if (_referenceSpecialScenes == null || _referenceSpecialScenes.GroupIsValid(out wrongAssetName) == false)
            {
                wrongFamilyName = nameof(_referenceSpecialScenes);
                familiesAreValid = false;
            }

            if (familiesAreValid == false && wrongAssetName != "")
                Debugger.Log("The " + wrongFamilyName + " has an invalid asset: " + wrongAssetName +
                    " Asset needs to be fixed for scenes to load.");
            else if (familiesAreValid == false)
                Debugger.Log(wrongFamilyName + " not assigned in the " + nameof(SceneLoader) +
                    " Assign a scene family for scenes to load.", DebugType.Error);

            return familiesAreValid;
        }

#if UNITY_EDITOR
        private bool _PutScenesInBuildSettings()
        {
            Debugger.LogInit("Adding missing scenes to build settings.");
            _AddSceneFamilyToBuildSettings(_referenceMandatoryScenes);
            _AddSceneFamilyToBuildSettings(_referenceSpecialScenes);
            _AddSceneFamilyToBuildSettings(Managers.runManager.TutorialRunCorridorGroup);
            _AddSceneFamilyToBuildSettings(Managers.runManager.TutorialRunLevelGroup);
            _AddSceneFamilyToBuildSettings(Managers.runManager.DefaultRunCorridorGroup);
            _AddSceneFamilyToBuildSettings(Managers.runManager.DefaultRunLevelGroup);
            return true;
        }

        private void _AddSceneFamilyToBuildSettings(LoadableGroup family)
        {
            Dictionary<string, EditorBuildSettingsScene> newScenes = new Dictionary<string, EditorBuildSettingsScene>();

            foreach (UnityEngine.Object obj in family.GetAllScenesAssets())
            {
                SceneAsset sceneAsset = obj as SceneAsset;
                if (sceneAsset != null)
                {
                    string path = AssetDatabase.GetAssetPath(sceneAsset);
                    var buildSettingsScene = new EditorBuildSettingsScene(path, true);

                    //Check that we don't add twice the same scene
                    if (newScenes.ContainsKey(path) == false)
                    {
                        //Check that the scene isn't already in the build settings
                        if (_IsSceneInBuildSettings(path) == false)
                            newScenes.Add(path, buildSettingsScene);
                    }
                }
            }

            //Add to the existing scenes
            List<EditorBuildSettingsScene> newBuildScenes = EditorBuildSettings.scenes.ToList();
            newBuildScenes.AddRange(newScenes.Values);
            EditorBuildSettings.scenes = newBuildScenes.ToArray();
        }
        private bool _IsSceneInBuildSettings(string scenePath)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == scenePath)
                {
                    return true;
                }
            }

            return false;
        }
#endif


        private bool _InstantiateFamilies()
        {
            _mandatoryScenes = Instantiate(_referenceMandatoryScenes);
            /*_tutorial = Instantiate(_referenceTutorial);
            _corridors = Instantiate(_referenceCorridors);
            _levels = Instantiate(_referenceLevels);*/
            _specialScenes = Instantiate(_referenceSpecialScenes);
            return true;
        }
        #endregion

        //--------------------

        //--------------------

        #region Other

        private void _AddSceneNameToActiveLoadableObjects(LoadableObject loadable, string nameToAdd, SceneGroup sceneGroup)
        {
            if (_activeLoadableObjects.ContainsKey(loadable))
                _activeLoadableObjects[loadable].AddSceneNamed(nameToAdd);
            else
            {
                var sceneInfo = new LoadedSceneInfo(loadable, sceneGroup, nameToAdd);
                _activeLoadableObjects.Add(loadable, sceneInfo);
            }
        }

        private List<LoadedSceneInfo> _GetDisposableScenesWithin(List<LoadedSceneInfo> loadedScenes)
        {
            List<LoadedSceneInfo> toReturn = new List<LoadedSceneInfo>();
            foreach (LoadedSceneInfo item in loadedScenes)
            {
                if (item.disposable)
                    toReturn.Add(item);
            }
            return toReturn;
        }

        private void _RemoveSceneNameFromActiveLoadableObjects(LoadableObject loadable, string nameToRemove)
        {
            if (_activeLoadableObjects.ContainsKey(loadable))
            {
                _activeLoadableObjects[loadable].RemoveSceneNamed(nameToRemove);
                //If there are no more loaded sceneAssets tied to this loadable we remove it
                if (_activeLoadableObjects[loadable].HasLoadedScenes() == false)
                    _activeLoadableObjects.Remove(loadable);
            }
            else
                Debug.LogError($"Trying to remove the scene \"{nameToRemove}\" from the \"{loadable.LoadableName}\" loadable. However it was not active.");
        }

        private Dictionary<LoadableObject, SceneGroup> _GetAllAssignedScenes()
        {
            Dictionary<LoadableObject, SceneGroup> assignedScenes = new Dictionary<LoadableObject, SceneGroup>();

            foreach (LoadableObject item in _mandatoryScenes.GetLoadableObjects())
            {
                assignedScenes.Add(item, SceneGroup.Mandatory);
            }
            /*foreach (LoadableObject item in _tutorial.GetLoadables())
            {
                assignedScenes.Add(item, SceneGroup.Level);
            }
            foreach (LoadableObject item in _corridors.GetLoadables())
            {
                assignedScenes.Add(item, SceneGroup.Corridor);
            }
            foreach (LoadableObject item in _levels.GetLoadables())
            {
                assignedScenes.Add(item, SceneGroup.Level);
            }*/
            foreach (LoadableObject item in _specialScenes.GetLoadableObjects())
            {
                assignedScenes.Add(item, SceneGroup.UniqueScene);
            }

            return assignedScenes;
        }
        #endregion
    }
}
