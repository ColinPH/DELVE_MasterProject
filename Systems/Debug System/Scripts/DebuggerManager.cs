using System;
using PropellerCap.QA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class DebuggerManager : ManagerBase
    {
        public static DebuggerManager Instance { get; private set; }

        //[SerializeField] GameObject _debugOverlayPrefab;
        [SerializeField] GameObject _debugOverlayObject;
        [Header("Debug log options")]
        public bool _applyTryCatchOnMyBehaviour = true;
        [Header("Debug log options")]
        //TODO put all these debug settings in a proper settings class
        public bool _debugInitialization = true;
        public bool _debugSceneLoading = true;
        public bool _debugLocalManagerActions = true;
        public bool _debugHookActions = true;
        public bool _debugFlareActions = true;
        public bool _debugEvents = true;
        public bool _debugStates = true;
        public bool _debugMyBehaviour = true;
        [Header("Commands")]
        [SerializeField] CommandsBundle _defaultCommandBundle;
        Dictionary<string, float> _timers = new Dictionary<string, float>();

        CommandProcessor _commandsProcessor { get; set; }
        LogsProcessor _logsProcessor { get; set; }
        DebugOverlay _debugOverlay { get; set; }

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
            Managers.debuggerManager = this;
            Debugger.Init(this);
            
            //Find or create the debug overlay
            _debugOverlay = _debugOverlayObject.GetComponent<DebugOverlay>();

            _logsProcessor = new LogsProcessor(this, _debugOverlay); //This needs to be before logging anything

            Debugger.LogInit("Init in Debugger Manager");

            //Initialize the command processor
            if (_defaultCommandBundle != null)
                _commandsProcessor = new CommandProcessor(_defaultCommandBundle);
            else
                _commandsProcessor = new CommandProcessor();

            //Register to receive the logs from the application
            Application.logMessageReceived += HandleApplicationLogMessage;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Debugger Manager");
        }

        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Debugger manager");
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<DebuggerManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion Initialization

        //----------------------------
        //Timers
        //----------------------------

        #region Timers
        public void StartTimer(string identifier)
        {
            _timers.Add(identifier, Time.realtimeSinceStartup);
        }
        public float StopTimer(string identifier)
        {
            if (_CheckTimerIdentifier(identifier) == false)
                return -1f;

            float time = Time.realtimeSinceStartup - _timers[identifier];
            _timers.Remove(identifier);
            return time;
        }
        public float ReadTimer(string identifier)
        {
            if (_CheckTimerIdentifier(identifier) == false)
                return -1f;

            return Time.realtimeSinceStartup - _timers[identifier];
        }
        private bool _CheckTimerIdentifier(string identifier)
        {
            bool identifierIsValid = _timers.ContainsKey(identifier);
            if (identifierIsValid == false)
                Debug.LogError("\"" + identifier + "\" is not valid as an identifier for the debugger timers." +
                    " Check the STartTimer and StopTimer function calls.");
            return identifierIsValid;
        }
        #endregion

        //----------------------------
        //Logs
        //----------------------------

        #region Logs
        private void HandleApplicationLogMessage(string logMessage, string stackTrace, LogType logType)
        {
            if (logType== LogType.Error)
            {
                _debugOverlay.WriteLog("<color=red>" + logMessage + "</color>");
                _debugOverlay.WriteLog("<color=red>" + stackTrace + "</color>");
            }            
        }
        public void Log(string message, DebugType debugType)
        {
            _logsProcessor.ProcessLog(message, debugType);
        }
        #endregion Logs

        //----------------------------
        //Commands
        //----------------------------

        #region Commands
        public void ProcessCommand(string rawCommandText)
        {
            _commandsProcessor.ProcessCommand(rawCommandText, this);
        }
        public void AddCommand(IConsoleCommand commandToAdd)
        {
            _commandsProcessor.AddCommand(commandToAdd);
        }
        #endregion Commands
    }
}
