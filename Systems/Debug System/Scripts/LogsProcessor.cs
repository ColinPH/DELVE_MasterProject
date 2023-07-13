using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    public class LogsProcessor
    {
        private DebuggerManager _debuggerManager { get; set; }
        private DebugOverlay _debugOverlay { get; set; }
        public LogsProcessor(DebuggerManager debuggerManager, DebugOverlay debugOverlay)
        {
            _debuggerManager = debuggerManager;
            _debugOverlay = debugOverlay;
        }

        public void ProcessLog(string message, DebugType debugType)
        {
            string logText = _GetLogPrefix(debugType) + message;

            if (_DebugTypeIsActive(debugType))
            {
                //Log the message in the unity console
                _LogInUnityConsole(logText, debugType);

                //Log the message in the Debug overlay
                _debugOverlay.WriteLog(_ApplyRichTextColourTo(logText, debugType));
            }

            //Write the log in the logs file
            //TODO write the log to the log file
        }

        private string _ApplyRichTextColourTo(string logText, DebugType debugType)
        {
            switch (debugType)
            {
                case DebugType.Error:
                    return "<color=red>" + logText + "</color>";
                case DebugType.Warning:
                    return "<color=yellow>" + logText + "</color>";
                default:
                    return "<color=white>" + logText + "</color>";
            }
        }

        private void _LogInUnityConsole(string message, DebugType debugType)
        {
            switch (debugType)
            {
                case DebugType.Error:
                    Debug.LogError(message);
                    break;
                case DebugType.Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        private string _GetLogPrefix(DebugType debugType)
        {
            switch (debugType)
            {
                case DebugType.Unassigned:
                    return "Log: ";
                case DebugType.Initialization:
                    return "Init: ";
                case DebugType.Error:
                    return "Error: ";
                case DebugType.SceneLoading:
                    return "SceneLoad: ";
                case DebugType.LocalManagerAction:
                    return "LocalManagerAction: ";
                case DebugType.Warning:
                    return "Warning: ";
                case DebugType.Hook:
                    return "Hook: ";
                case DebugType.Flare:
                    return "Flare: ";
                case DebugType.Event:
                    return "Event: ";
                case DebugType.State:
                    return "State: ";
                case DebugType.MyBehaviour:
                    return "MyBehaviour: ";
                default:
                    return "Missing: ";
            }
        }
        private bool _DebugTypeIsActive(DebugType debugType)
        {
            switch (debugType)
            {
                case DebugType.Unassigned:
                    return true;
                case DebugType.Initialization:
                    return _debuggerManager._debugInitialization;
                case DebugType.Error:
                    return true;
                case DebugType.SceneLoading:
                    return _debuggerManager._debugSceneLoading;
                case DebugType.LocalManagerAction:
                    return _debuggerManager._debugLocalManagerActions;
                case DebugType.Warning:
                    return true;
                case DebugType.Hook:
                    return _debuggerManager._debugHookActions;
                case DebugType.Flare:
                    return _debuggerManager._debugFlareActions;
                case DebugType.Event:
                    return _debuggerManager._debugEvents;
                case DebugType.State:
                    return _debuggerManager._debugStates;
                case DebugType.MyBehaviour:
                    return _debuggerManager._debugMyBehaviour;
                default:
                    Debug.LogWarning("Debug type not implemented : " + debugType.ToString());
                    return true;
            }
        }
    }
}
