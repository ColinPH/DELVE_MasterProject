using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class RunManager : ManagerBase
    {
        public static RunManager Instance { get; private set; }

        [SerializeField] RunObject _referenceTutorialRun;
        [SerializeField] RunObject _referenceDefaultRun;
        [SerializeField] SoundClip _playerHubRespawnSound;

        RunObject _instantiatedTutorialRun;
        RunObject _instantiatedDefaultRun;

        IEnumerator _runStartRoutine;
        IEnumerator _loadNextRunStepRoutine;
        IEnumerator _unloadPreviousRunStepRoutine;
        IEnumerator _runStopRoutine;

        RunObject _activeRunObject;

        SceneLoadPair _currentRoom;
        SceneLoadPair _previousRoom;

        #region Public accessors and Events
        //Accessors
        public LoadableGroup TutorialRunLevelGroup => _referenceTutorialRun.levelsFamily;
        public LoadableGroup TutorialRunCorridorGroup => _referenceTutorialRun.corridorsFamily;
        public LoadableGroup DefaultRunLevelGroup => _referenceDefaultRun.levelsFamily;
        public LoadableGroup DefaultRunCorridorGroup => _referenceDefaultRun.corridorsFamily;
        public RunObject ActiveRunObject => _activeRunObject;
        //Events
        public delegate void RunStartHandler(RunObject activeRunObject);
        /// <summary> Delegate called when a run has been started. Invoked before the GameEvent.RunStart in EventManager, after the initialization of the chosen RunObject, and before the loading of the scenes. </summary>
        public RunStartHandler OnRunStart { get; set; }
        public delegate void NextLevelLoadHandler(SceneLoadPair nextScenePair);
        /// <summary> Delegate called before loading the scenes of the next level. </summary>
        public NextLevelLoadHandler OnPreNextLevelLoad { get; set; }
        /// <summary> Delegate called after loading the scenes of the next level. </summary>
        public NextLevelLoadHandler OnPostNextLevelLoad { get; set; }
        #endregion Public accessors and Events

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
            Debugger.LogInit("Init in Run Manager");
            Managers.runManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Run Manager");
        }

        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Run Manager");
            _instantiatedTutorialRun = Instantiate(_referenceTutorialRun);
            _instantiatedDefaultRun = Instantiate(_referenceDefaultRun);
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<RunManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion

        



        #region Run controls

        public void StartTutorialRunFromIntroScene()
        {
            StartRun(_instantiatedTutorialRun, true, true);
        }

        public void StartRun(bool unloadDisposableScenes = false, bool hasFadeFromBlack = false)
        {
            StartRun(_instantiatedDefaultRun, unloadDisposableScenes, hasFadeFromBlack);
        }

        /// <summary> Should be called from the first scene of the game. </summary>
        public void StartRun(RunObject runObject, bool unloadDisposableScenes = false, bool hasFadeFromBlack = false)
        {
            _runStartRoutine = Co_StartRun(runObject, unloadDisposableScenes, hasFadeFromBlack);
            StartCoroutine(_runStartRoutine);
        }

        public void LoadNextCorridor(CorridorLoadingInfo loadingInfo)
        {
            //Debug.LogWarning("Load next corridor");
            _loadNextRunStepRoutine = Co_LoadNextCorridor(loadingInfo);
            StartCoroutine(_loadNextRunStepRoutine);
        }
        public void LoadNextLevel(LevelLoadingInfo loadingInfo)
        {
            //Debug.Log("Load next level");
            _loadNextRunStepRoutine = Co_LoadNextLevel(loadingInfo);
            StartCoroutine(_loadNextRunStepRoutine);
        }

        public void UnloadPreviousCorridor()
        {
            //Debug.Log("Unload previous corridor");
            _unloadPreviousRunStepRoutine = Co_UnloadPreviousCorridor();
            StartCoroutine(_unloadPreviousRunStepRoutine);
        }
        public void UnloadPreviousLevel()
        {
            //Debug.Log("Unload previous level");
            _unloadPreviousRunStepRoutine = Co_UnloadPreviousLevel();
            StartCoroutine(_unloadPreviousRunStepRoutine);
        }

        public void CompleteRun()
        {
            _runStopRoutine = _Co_RunCompleted();
            StartCoroutine(_runStopRoutine);
        }

        public void FailRun()
        {
            _activeRunObject = null;
            Debugger.Log("Run failed");
            _runStopRoutine = _Co_RunFailed();
            StartCoroutine(_runStopRoutine);
        }
        #endregion Run controls

        public IEnumerator Co_StartRun(RunObject runObject, bool unloadDisposableScenes, bool hasFadeFromBlack = false)
        {
            _activeRunObject = runObject;

            Metrics.manager.CloseRunData();
            Metrics.manager.OpenRunData(runObject.RunName);

            //Initialize the run object
            _activeRunObject.Init();

            OnRunStart?.Invoke(_activeRunObject);
            Managers.eventManager.FireGameEvent(GameEvent.RunStart, this);

            if (hasFadeFromBlack)
                yield return StartCoroutine(HUD.blackFader.Co_FadeToBlack());

            object loadingInfo = null;
            _previousRoom = new SceneLoadPair(Managers.sceneLoader.GetLoadable(UniqueScene.Hub), null, null, SceneTargets.All);
            _currentRoom = _GenerateSceneLoadPair(_activeRunObject.GetNextLevel(), loadingInfo, SceneTargets.All);
            
            //Pause the initialization of new objects, this is to have full control. It has the flaw to also include the runtime instantiations of other already existing objets
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = false;
            
            yield return StartCoroutine(Managers.sceneLoader.Co_Load(_currentRoom, unloadDisposableScenes));
            if (unloadDisposableScenes)
                _previousRoom = null;
            
            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            Managers.gameManager.PostSceneLoadingProcess(loadingInfo);

            //Resume the initialization of new objects
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = true;

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);

            if (hasFadeFromBlack) 
                yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());
        }

        private IEnumerator _Co_RunFailed()
        {
            yield return StartCoroutine(Managers.playerManager.Co_KillPlayer());

            //Unload the previous room if it has not already be done
            if (_previousRoom != null)
                yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_previousRoom));

            //Unload the current room we are in
            yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_currentRoom));

            //Load the hub
            UniqueSceneLoadingInfo hubLoadingInfo = null;
            SceneLoadPair pair = new SceneLoadPair(Managers.sceneLoader.GetLoadable(UniqueScene.Hub), hubLoadingInfo, null, SceneTargets.All);

            //Pause the initialization of new objects, this is to have full control. It has the flaw to also include the runtime instantiations of other already existing objets
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = false;
            
            yield return StartCoroutine(Managers.sceneLoader.Co_Load(pair, true));

            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            Managers.gameManager.PostSceneLoadingProcess(hubLoadingInfo);

            //Resume the initialization of new objects
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = true;

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);

            Sound.PlaySound(_playerHubRespawnSound, gameObject);

            yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());
        }

        private IEnumerator _Co_RunCompleted()
        {
            Debugger.Log("Run completed");

            yield return StartCoroutine(HUD.blackFader.Co_FadeToBlack());

            _activeRunObject = null;
            //Fire the Run Completed event
            Managers.eventManager.FireGameEvent(GameEvent.RunCompleted, this);

            //Unload the previous room if it has not already be done
            if (_previousRoom != null)
                yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_previousRoom));

            //Unload the current room we are in
            yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_currentRoom));

            //Load the Credits scene
            UniqueSceneLoadingInfo creditsLoadingInfo = null;
            SceneLoadPair pair = new SceneLoadPair(Managers.sceneLoader.GetLoadable(UniqueScene.Credits), creditsLoadingInfo, null, SceneTargets.All);

            //Pause the initialization of new objects, this is to have full control. It has the flaw to also include the runtime instantiations of other already existing objets
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = false;

            yield return StartCoroutine(Managers.sceneLoader.Co_Load(pair, true));

            Destroy(FindObjectOfType<PlayerCharacter>().gameObject);

            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            Managers.gameManager.PostSceneLoadingProcess(creditsLoadingInfo);

            //Resume the initialization of new objects
            Managers.gameManager._initializeNewlyInstantiatedWorldObjects = true;

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);

            yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());
        }

        public IEnumerator Co_LoadNextCorridor(CorridorLoadingInfo loadingInfo)
        {
            _previousRoom = _currentRoom;
            _currentRoom = _GenerateSceneLoadPair(_activeRunObject.GetNextCorridor(), loadingInfo, SceneTargets.Main);

            yield return StartCoroutine(Managers.sceneLoader.Co_Load(_currentRoom));

            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            Managers.gameManager.PostSceneLoadingProcess(loadingInfo);

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);
        }

        public IEnumerator Co_LoadNextLevel(LevelLoadingInfo loadingInfo)
        {
            _previousRoom = _currentRoom;
            _currentRoom = _GenerateSceneLoadPair(_activeRunObject.GetNextLevel(), loadingInfo, SceneTargets.Main);

            //Fire events
            OnPreNextLevelLoad?.Invoke(_currentRoom);

            yield return StartCoroutine(Managers.sceneLoader.Co_Load(_currentRoom, false));

            OnPostNextLevelLoad?.Invoke(_currentRoom);

            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            Managers.gameManager.PostSceneLoadingProcess(loadingInfo);

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);
        }

        public IEnumerator Co_UnloadPreviousCorridor()
        {
            yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_previousRoom));
            _previousRoom = null;
        }
        public IEnumerator Co_UnloadPreviousLevel()
        {
            yield return StartCoroutine(Managers.sceneLoader.Co_Unload(_previousRoom));
            _previousRoom = null;
        }

        private SceneLoadPair _GenerateSceneLoadPair(LoadableObject loadable, object loadingInfo, SceneTargets sceneTargets)
        {
            return new SceneLoadPair(loadable, loadingInfo, null, sceneTargets);
        }

    }
}
