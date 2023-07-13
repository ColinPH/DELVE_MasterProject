using UnityEngine;

namespace PropellerCap
{
    public static class Managers
    {
        static DebuggerManager _debuggerManager;
        public static DebuggerManager debuggerManager
        {
            get
            {
                if (_debuggerManager == null)
                    LogError(nameof(DebuggerManager).ToString());
                return _debuggerManager;
            }
            set { _debuggerManager = value; }
        }

        //--------- Initialization

        static InitializationManager _initializationManager;
        public static InitializationManager initializationManager
        {
            get
            {
                if (_initializationManager == null)
                    LogError(nameof(InitializationManager).ToString());
                return _initializationManager;
            }
            set { _initializationManager = value; }
        }

        //--------- Game

        static GameManager _gameManager;
        public static GameManager gameManager
        {
            get
            {
                if (_gameManager == null)
                    LogError(nameof(GameManager).ToString());
                return _gameManager;
            }
            set { _gameManager = value; }
        }

        //--------- Event

        static EventManager _eventManager;
        public static EventManager eventManager
        {
            get
            {
                if (_eventManager == null)
                    LogError(nameof(EventManager).ToString());
                return _eventManager;
            }
            set { _eventManager = value; }
        }

        //--------- Scene

        static SceneLoader _sceneLoader;
        public static SceneLoader sceneLoader
        {
            get
            {
                if (_sceneLoader == null)
                    LogError(nameof(SoundManager).ToString());
                return _sceneLoader;
            }
            set { _sceneLoader = value; }
        }

        //--------- Sound

        static SoundManager _soundManager;
        public static SoundManager soundManager
        {
            get
            {
                if (_soundManager == null)
                    LogError(nameof(SoundManager).ToString());
                return _soundManager;
            }
            set { _soundManager = value; }
        }

        //--------- Sanity

        static SanityManager _sanityManager;
        public static SanityManager sanityManager
        {
            get
            {
                if (_sanityManager == null)
                    LogError(nameof(SanityManager).ToString());
                return _sanityManager;
            }
            set { _sanityManager = value; }
        }

        //--------- Totem

        static TotemManager _totemManager;
        public static TotemManager totemManager
        {
            get
            {
                if (_totemManager == null)
                    LogError(nameof(TotemManager).ToString());
                return _totemManager;
            }
            set { _totemManager = value; }
        }

        //--------- Run

        static RunManager _runManager;
        public static RunManager runManager
        {
            get
            {
                if (_runManager == null)
                    LogError(nameof(RunManager).ToString());
                return _runManager;
            }
            set { _runManager = value; }
        }

        //--------- Player

        static PlayerManager _playerManager;
        public static PlayerManager playerManager
        {
            get
            {
                if (_playerManager == null)
                    LogError(nameof(PlayerManager).ToString());
                return _playerManager;
            }
            set { _playerManager = value; }
        }

        //--------- UI

        static UIManager _UIManager;
        public static UIManager uiManager
        {
            get
            {
                if (_UIManager == null)
                    LogError(nameof(UIManager).ToString());
                return _UIManager;
            }
            set { _UIManager = value; }
        }

        //--------- Localization

        static LocalizationManager _localizationManager;
        public static LocalizationManager localizationManager
        {
            get
            {
                if (_localizationManager == null)
                    LogError(nameof(LocalizationManager).ToString());
                return _localizationManager;
            }
            set { _localizationManager = value; }
        }

        //--------- Metrics

        static MetricsManager _metricsManager;
        public static MetricsManager metricsManager
        {
            get
            {
                if (_metricsManager == null)
                    LogError(nameof(MetricsManager).ToString());
                return _metricsManager;
            }
            set { _metricsManager = value; }
        }

        //--------- Metrics

        static SaveManager _saveManager;
        public static SaveManager saveManager
        {
            get
            {
                if (_saveManager == null)
                    LogError(nameof(SaveManager).ToString());
                return _saveManager;
            }
            set { _saveManager = value; }
        }

        private static void LogError(string managerToAccess)
        {
            if (Application.isEditor) return;
            Debug.LogError(
                "Trying to access the " + managerToAccess + " through the static"
                + nameof(Managers) + " class."
                + " But it has not been set yet. Are you sure it is assigning itself in Init() ?");
        }
    }
}