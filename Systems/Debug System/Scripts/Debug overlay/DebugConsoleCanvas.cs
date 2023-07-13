using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    public class DebugConsoleCanvas : MonoBehaviour
    {
        [SerializeField] CommandsCanvasDisplay _commandsDisplay;
        [SerializeField] FramePerSecondsCanvasDisplay _fpsDisplay;
        [SerializeField] LogsCanvasDisplay _logsDisplay;

        DebugOverlay _console;
        FrameRateProcessor _fpsProcessor;

        public void InitializeDebugConsoleCanvas(DebugOverlay console, FrameRateProcessor fpsProcessor)
        {
            _console = console;
            _fpsProcessor = fpsProcessor;

            _commandsDisplay.InitializeCanvasDisplay(_console);
            _fpsDisplay.InitializeCanvasDisplay(_fpsProcessor);
            _logsDisplay.InitializeCanvasDisplay(_console);
        }

        public void FocusCommandInputField()
        {
            _commandsDisplay.FocusCommandInputField();
            _console.blockActivationKey = true;
        }

        public void UpdateCanvasInformation()
        {
            _fpsDisplay.UpdateCanvasInformation();
        }

        #region Logs

        public void AddNewLog(string logText)
        {
            _logsDisplay.AddNewLog(new LogLine(logText));
        }

        public void WriteLogs()
        {
            _logsDisplay.WriteLogs();
        }

        public void PauseLogsStream()
        {
            _logsDisplay.PauseLogsStream();
        }

        public void ResumeLogsStream()
        {
            _logsDisplay.ResumeLogsStream();
        }

        #endregion Logs

        public void OnDebugConsoleShow()
        {
            _commandsDisplay.OnDebugConsoleShow();
            _logsDisplay.OnDebugConsoleShow();
            _fpsDisplay.OnDebugConsoleShow();
        }

        public void OnDebugConsoleHide()
        {
            _commandsDisplay.OnDebugConsoleHide();
            _fpsDisplay.OnDebugConsoleHide();
            _logsDisplay.OnDebugConsoleHide();
        }

    }
}