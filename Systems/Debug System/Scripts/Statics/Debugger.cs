using PropellerCap.QA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public static class Debugger
    {
        public static DebuggerManager manager { get; private set; }
        public static void Init(DebuggerManager debuggerManager)
        {
            manager = debuggerManager;
        }

        #region Timers
        public static void StartTimer(string identifier)
        {
            manager.StartTimer(identifier);
        }
        public static float StopTimer(string identifier)
        {
            return manager.StopTimer(identifier);
        }
        public static float ReadTimer(string identifier)
        {
            return manager.ReadTimer(identifier);
        }
        #endregion

        //----------------------------
        //Logs
        //----------------------------

        #region Logs
        public static void Log(string message, DebugType debugType = DebugType.Unassigned)
        {
            manager.Log(message, debugType);
        }

        public static void LogInit(string message)
        {
            manager.Log(message, DebugType.Initialization);
        }

        public static void LogLocalMAction(string message)
        {
            manager.Log(message, DebugType.LocalManagerAction);
        }

        public static void LogError(string message)
        {
            manager.Log(message, DebugType.Error);
        }
        public static void LogWarning(string message)
        {
            manager.Log(message, DebugType.Warning);
        }

        public static void LogSceneLoading(string message)
        {
            manager.Log(message, DebugType.SceneLoading);
        }
        public static void LogHook(string message)
        {
            manager.Log(message, DebugType.Hook);
        }
        public static void LogFlare(string message)
        {
            manager.Log(message, DebugType.Flare);
        }
        public static void LogEvent(string message)
        {
            manager.Log(message, DebugType.Event);
        }
        public static void LogState(string message)
        {
            manager.Log(message, DebugType.State);
        }
        public static void LogMyBehaviour(string message)
        {
            manager.Log(message, DebugType.MyBehaviour);
        }
        #endregion Logs

        //----------------------------
        //Commands
        //----------------------------

        #region Commands
        public static void AddCommand(IConsoleCommand consoleCommand)
        {
            manager.AddCommand(consoleCommand);
        }
        public static void ProcessCommand(string rawCommandText)
        {
            manager.ProcessCommand(rawCommandText);
        }
        #endregion Commands
    }
}
