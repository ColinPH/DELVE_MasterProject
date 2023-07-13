using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PropellerCap
{
    public class GameManager : ManagerBase
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] Key _combinationKey1 = Key.LeftShift;
        [SerializeField] Key _combinationKey2 = Key.LeftCtrl;
        [SerializeField] Key _quitGameKey = Key.Q;
        [SerializeField] Key _restartGameKey = Key.R;
        [Header("Show game introduction")]
        [SerializeField] bool _startFromIntro = true;
        [SerializeField] bool _startFromHub = false;

        [Header("/!\\ Runtime information /!\\")]
        [SerializeField] bool _loopIsActive = false;
        private List<ManagerBase> _managers = new List<ManagerBase>();
        [HideInInspector] public bool _initializeNewlyInstantiatedWorldObjects = true;

        //New entities for "My" initialization
        List<WorldObject> _newWorldObjects = new List<WorldObject>();
        List<LocalManager> _newLocalManagers = new List<LocalManager>();
        //Existing entities for "My" updates
        List<WorldObject> _worldObjects = new List<WorldObject>();
        List<LocalManager> _localManagers = new List<LocalManager>();

        public void SetManagers(List<ManagerBase> managers) => _managers = managers;

        public void StartGame()
        {
            Debugger.LogInit("Game started through the Game Manager.");

            //We started from root is the root scene was already in the scene and there was only 1 scene
            bool isStartingFromRootScene = false;
            bool rootSceneIsLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == Managers.sceneLoader.rootSceneName)
                {
                    rootSceneIsLoaded = true;
                    break;
                }
            }
            if (rootSceneIsLoaded && SceneManager.sceneCount == 1)
                isStartingFromRootScene = true;

            if (isStartingFromRootScene)
                StartCoroutine(_Co_StartGameLoopFromRoot());
            else
                StartCoroutine(_Co_StartGameLoopFromLoadedSceneInEditor(rootSceneIsLoaded));
        }


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
            Debugger.LogInit("Init in Game Manager");
            Managers.gameManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("Myawake in Game Manager");
            _SubscribeManagersToMainEvents();
        }

        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Game Manager");
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<GameManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion


        #region Managers controls

        /// <summary> Calls OnGamePreparationComplete on all managers. </summary>
        void _CallOnGameStartComplete()
        {
            foreach (var item in _managers)
            {
                item.OnGamePreparationComplete();
            }
        }

        void _SubscribeManagersToMainEvents()
        {
            
        }

        void RaiseOnNewLevelLoaded()
        {
            foreach (var item in _managers)
            {
                item.OnNewLevelLoaded();
            }
        }

        #endregion Managers controls


        #region Mono behaviours
        private void Update()
        {
            if (_loopIsActive)
            {
                if (_initializeNewlyInstantiatedWorldObjects)
                {
                    //Call MyAwake and MyStart on the new worldObjects and add them to the pool
                    MyBehaviourInitializationOnNewWorldObjects(false);
                }

                _CallUpdateLoop();
            }

            //Check for game control inputs
            _CheckGameControlInputs();
        }

        private void FixedUpdate()
        {
            if (_loopIsActive)
                _CallFixedUpdateLoop();
        }

        private void LateUpdate()
        {
            if (_loopIsActive)
                _CallLateUpdateLoop();
        }
        #endregion


        #region Game saves controls

        public void StartGameFromSave(int saveIndex)
        {
            StartCoroutine(_Co_StartGameFromSave());
        }

        private IEnumerator _Co_StartGameFromSave()
        {
            //TODO retrieve the hub loading information from the save file
            UniqueSceneLoadingInfo hubLoadingInfo = null;
            SceneLoadPair pair = new SceneLoadPair(Managers.sceneLoader.GetLoadable(UniqueScene.Hub), hubLoadingInfo, null, SceneTargets.All);
            
            //Pause the initialization of new objects, this is to have full control. It has the flaw to also include the runtime instantiations of other already existing objets
            _initializeNewlyInstantiatedWorldObjects = false; 

            yield return StartCoroutine(Managers.sceneLoader.Co_Load(pair, true));
            
            SceneGroup lastStartedScene = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            PostSceneLoadingProcess(null);

            //Resume the initialization of new objects
            _initializeNewlyInstantiatedWorldObjects = true; 

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastStartedScene, lastSceneIdentifier);

            yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());
        }

        #endregion Game saves controls


        #region Post scene loading process and WorldObject management
        public void PostSceneLoadingProcess(object loadSettings)
        {
            Debugger.LogMyBehaviour($"Applying the post scene loading process on worldObjects ({_newLocalManagers.Count}) and localManagers ({_newWorldObjects.Count}).");
            _PostSceneLoadingLocalManagerInitialization(_newWorldObjects, _newLocalManagers, loadSettings);
            
            _MyBehaviourInitialization(_newWorldObjects);
            _AddNewEntitiesToExistingOnes(true, true);
            _ClearNewEntities(true, true);
        }

        /// <summary> Calls MyAwake and MyStart on the new WorldObjects, then clears the list. </summary>
        public void MyBehaviourInitializationOnNewWorldObjects(bool logMyBehaviour = true)
        {
            if (_newWorldObjects.Count == 0)
                return;
            _MyBehaviourInitialization(_newWorldObjects, logMyBehaviour);
            _AddNewEntitiesToExistingOnes(true, false);
            _ClearNewEntities(true, false);
        }

        private void _PostSceneLoadingLocalManagerInitialization(List<WorldObject> newWorldObjects, List<LocalManager> newLocalManagers, object loadSettings)
        {
            //Call pre local manager initialized on WorldObjects
            Debugger.LogMyBehaviour($"Calling pre local manager initialization on {newWorldObjects.Count} newly loaded world objects.");
            _CallPreLocalManagerInitializationOnWorldObjects(newWorldObjects);

            //Initialize new local managers
            Debugger.LogMyBehaviour($"Initializing {newLocalManagers.Count} newly loaded local manager.");
            _CallLocalManagerInitializationOn(newLocalManagers, loadSettings);
        }

        private void _PreSceneUnloadingProcess()
        {
            //TODO implement this
        }

        /// <summary> Calls MyAwake and MyStart on the given WorldObjects. </summary>
        private void _MyBehaviourInitialization(List<WorldObject> newWorldObjects, bool logMyBehaviour = true)
        {
            //Call MyAwake on new WorldObjects
            if (logMyBehaviour)
                Debugger.LogMyBehaviour($"Calling MyAwake on {newWorldObjects.Count} newly loaded world objects.");
            _CallMyAwakeOnWorldObjects(newWorldObjects);

            //Call MyStart on new WorldObjects
            if (logMyBehaviour)
                Debugger.LogMyBehaviour($"Calling MyStart on {newWorldObjects.Count} newly loaded world objects.");
            _CallMyStartOnWorldObjects(newWorldObjects);
        }

        public void RegisterNewLocalManager(LocalManager newLocalManager)
        {
            if (_newLocalManagers.Contains(newLocalManager))
            {
                Debug.LogError($"The LocalManager \"{newLocalManager.LocalManagerName}\" has already been registered to the GameManager.");
                return;
            }
            Debugger.LogMyBehaviour($"Registering new LocalManager \"{newLocalManager.LocalManagerName}\"");
            _newLocalManagers.Add(newLocalManager);
        }
        public void DeregisterLocalManager(LocalManager newLocalManager)
        {
            Debugger.LogMyBehaviour($"Before deregistering \"{newLocalManager.LocalManagerName}\", the length of the local managers array is {_localManagers.Count}");
            if (_localManagers.Contains(newLocalManager) == false)
            {
                Debug.LogError($"Trying to remove the LocalManager \"{newLocalManager.LocalManagerName}\" from GameManager but is not registered.");
                return;
            }
            Debugger.LogMyBehaviour($"Deregistering LocalManager \"{newLocalManager.LocalManagerName}\"");
            _localManagers.Remove(newLocalManager);
        }


        public void RegisterNewWorldObject(WorldObject newWorldObject)
        {
            Debugger.LogMyBehaviour($"Registering new WorldObject \"{newWorldObject.worldName}\"");
            if (_newWorldObjects.Contains(newWorldObject))
            {
                Debug.LogError($"The WorldObject \"{newWorldObject.worldName}\" has already been registered to the GameManager.");
                return;
            }
            _newWorldObjects.Add(newWorldObject);
        }

        public void DeregisterWorldObject(WorldObject worldObjectToRemove)
        {
            if (_worldObjects.Contains(worldObjectToRemove) == false)
            {
                if (_newWorldObjects.Contains(worldObjectToRemove))
                {
                    _newWorldObjects.Remove(worldObjectToRemove);
                    return;
                }
                Debug.LogError($"Trying to remove the WorldObject \"{worldObjectToRemove.worldName}\" from GameManager but is not registered.");
                return;
            }
            _worldObjects.Remove(worldObjectToRemove);
        }

        private void _AddNewEntitiesToExistingOnes(bool worldObjects, bool localManagers)
        {
            Debugger.LogMyBehaviour($"Adding new entities to existing ones: worldObjects ({worldObjects}), localManagers ({localManagers}).");
            if (worldObjects)
                _worldObjects.AddRange(_newWorldObjects);
            if (localManagers)
                _localManagers.AddRange(_newLocalManagers);
        }
        private void _ClearNewEntities(bool worldObjects, bool localManagers)
        {
            Debugger.LogMyBehaviour($"Clearing new entities: worldObjects ({worldObjects}), localManagers ({localManagers}).");
            if (worldObjects)
                _newWorldObjects.Clear();
            if (localManagers)
                _newLocalManagers.Clear();
        }
        #endregion Post scene loading and WorldObject management


        #region Game loop starters
        private IEnumerator _Co_StartGameLoopFromRoot()
        {
            Debug.Log("Starting From Root scene");
            //Load the game mandatory scenes
            yield return StartCoroutine(Managers.sceneLoader.Co_LoadMandatoryScenes());

            //Initialize the local scene managers
            SceneGroup lastStartedSceneGroup = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            PostSceneLoadingProcess(null);

            //The game is ready to start now
            _CallOnGameStartComplete();

            //Load first scene because we started from root
            if (_StartFromIntroScene())
            {
                LoadableObject introLoadable = Managers.sceneLoader.GetLoadable(UniqueScene.Intro);
                yield return StartCoroutine(Managers.sceneLoader.Co_Load(new SceneLoadPair(introLoadable, null, null, SceneTargets.All)));
            }
            else
            {
                LoadableObject mainMenuLoadable = Managers.sceneLoader.GetLoadable(UniqueScene.MainMenu);
                yield return StartCoroutine(Managers.sceneLoader.Co_Load(new SceneLoadPair(mainMenuLoadable, null, null, SceneTargets.All)));

                yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());

            }

            PostSceneLoadingProcess(null);

            //The game run should be started by something inside the first scene to load

            _loopIsActive = true;
        }

        private IEnumerator _Co_StartGameLoopFromLoadedSceneInEditor(bool rootSceneIsLoaded)
        {
            //Load the root scene if not already there
            if (rootSceneIsLoaded == false)
                yield return StartCoroutine(Managers.sceneLoader.Co_LoadRootScene(LoadSceneMode.Additive));

            //Save the worldObjects and local managers to initialize them later (the ones from the mandatory scenes should be initialized first)
            List<WorldObject> editorLoadedWorldObjects = new List<WorldObject>(_newWorldObjects);
            List<LocalManager> editorLoadedLocalManagers = new List<LocalManager>(_newLocalManagers);
            //Clear the world objects and local managers lists
            _ClearNewEntities(true, true);
            //Also save the last started scene group and the last scene identifier
            SceneGroup alreadyLoadedSceneGroup = Managers.sceneLoader.lastLoadedSceneGroup;
            object alreadyLoadedSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;

            //Load the game mandatory scenes
            yield return StartCoroutine(Managers.sceneLoader.Co_LoadMandatoryScenes());

            //Call the initialization on local scene managers and registered WorldObjects
            SceneGroup lastLoadedSceneGroup = Managers.sceneLoader.lastLoadedSceneGroup;
            object lastSceneIdentifier = Managers.sceneLoader.lastSceneIdentifier;
            _PostSceneLoadingLocalManagerInitialization(_newWorldObjects, _newLocalManagers, null);
            MyBehaviourInitializationOnNewWorldObjects();

            //The game is ready to start now
            _CallOnGameStartComplete();

            Managers.eventManager.FireGameEvent(GameEvent.RunStart, this);

            //The scene has been loaded in the editor but we still fire the events as if it had been loaded
            Managers.eventManager.FireGameEvent(GameEvent.PreSceneLoad, this);
            Managers.eventManager.FireGameEvent(GameEvent.SceneLoaded, this);

            //Call initialization again but on the already loaded local managers and WorldObjects
            lastLoadedSceneGroup = alreadyLoadedSceneGroup;
            lastSceneIdentifier = alreadyLoadedSceneIdentifier;

            //Add the already existing local managers and world objects to the list of new objects
            _newWorldObjects.AddRange(editorLoadedWorldObjects);
            _newLocalManagers.AddRange(editorLoadedLocalManagers);
            _PostSceneLoadingLocalManagerInitialization(_newWorldObjects, _newLocalManagers, null);
            MyBehaviourInitializationOnNewWorldObjects();
            //Call initialization again for any object that could have been instantiated in the LocalManager initialization
            MyBehaviourInitializationOnNewWorldObjects();

            //SceneStart event
            Managers.eventManager.FireMainGameEvent(GameEvent.SceneStart, this, lastLoadedSceneGroup, lastSceneIdentifier);

            yield return StartCoroutine(HUD.blackFader.Co_FadeFromBlack());

            _loopIsActive = true;
        }
        #endregion Game loop starters

        //----------------------------------

        //----------------------------------

        #region WorldObjects calls for MyBehaviour
        private void _CallPreLocalManagerInitializationOnWorldObjects(List<WorldObject> newWorldObjects)
        {
            foreach (WorldObject item in newWorldObjects)
            {
                item.CallPreLocalManagerInitialization();
            }
        }

        private void _CallLocalManagerInitializationOn(List<LocalManager> newLocalManagers, object loadingSettings)
        {
            foreach (LocalManager localManager in newLocalManagers)
            {
                localManager.CallLocalManagerInitialization(loadingSettings);
            }
        }

        private void _CallMyAwakeOnWorldObjects(List<WorldObject> newWorldObjects)
        {
            foreach (WorldObject item in newWorldObjects)
            {
                item.CallMyAwake();
            }
        }

        private void _CallMyStartOnWorldObjects(List<WorldObject> newWorldObjects)
        {
            foreach (WorldObject item in newWorldObjects)
            {
                item.CallMyStart();
            }
        }

        private void _CallUpdateLoop()
        {
            foreach (WorldObject item in _worldObjects)
            {
                item.CallMyUpdate();
            }
        }

        private void _CallFixedUpdateLoop()
        {
            foreach (WorldObject item in _worldObjects)
            {
                item.CallMyFixedUpdate();
            }
        }

        private void _CallLateUpdateLoop()
        {
            foreach (WorldObject item in _worldObjects)
            {
                item.CallMyLateUpdate();
            }
        }
        #endregion WorldObjects calls for MyBehaviour


        #region Game controlling methods and inputs

        private void _CheckGameControlInputs()
        {
            if (Keyboard.current[_combinationKey1].isPressed
                && Keyboard.current[_combinationKey2].isPressed)
            {
                if (Keyboard.current[_quitGameKey].wasPressedThisFrame)
                    QuitGame();

                if (Keyboard.current[_restartGameKey].wasPressedThisFrame)
                    StartCoroutine(Co_RestartGame("Game restart shorcut buttons have bee pressed."));
            }
        }

        private bool _StartFromIntroScene()
        {
            //If the option in the inspector is activated
            if (_startFromIntro && Application.isEditor)
                return true;

            if (_startFromHub && Application.isEditor)
                return false;

            //If it is the first time the game is launched
            //TODO move this to the save manager
            if (PlayerPrefs.GetInt("StartWithMenu") == 0)
                return true;
            else
                return false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public IEnumerator Co_RestartGame(string reason)
        {
            Debugger.Log("Restarting game because: " + reason);

            _loopIsActive = false;

            yield return StartCoroutine(Managers.sceneLoader.Co_ReloadGame());

            StartGame();
        }
        #endregion
    } 
}