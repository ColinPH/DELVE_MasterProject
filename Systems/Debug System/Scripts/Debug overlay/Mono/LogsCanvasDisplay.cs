using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Linq;

namespace PropellerCap.QA
{
    public class LogsCanvasDisplay : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] int _maxAmountOfLogs = 100;
        [Header("Prefabs")]
        [SerializeField] RectTransform logsContainer;
        [SerializeField] TMP_Text _logsText;
        [SerializeField] Toggle _logsStreamControl;

        DebugOverlay _console;
        StringBuilder _stringBuilder;

        bool _isStreamingLogs = true;
        int _amountLogs = 0;
        int _amountLogsToRemove = 0;

        /// <summary> These are all the logs that have been written already. </summary>
        List<LogLine> _logs = new List<LogLine>();
        /// <summary> This contains all the logs that need to be added at the end of the frame. </summary>
        List<LogLine> _awaitingLogs = new List<LogLine>();

        public void InitializeCanvasDisplay(DebugOverlay console)
        {
            _console = console;
            _stringBuilder = new StringBuilder();

            _logsStreamControl.onValueChanged.AddListener((isActive) => 
            {
                if (isActive)
                    ResumeLogsStream();
                else
                    PauseLogsStream();
            });
        }

        public void OnDebugConsoleShow()
        {

        }

        public void OnDebugConsoleHide()
        {

        }

        /// <summary> Stops the logs from being added/removed. They will stil be added to the logs file. </summary>
        public void PauseLogsStream()
        {
            _isStreamingLogs = false;
        }
        public void ResumeLogsStream()
        {
            if (_isStreamingLogs)
                return;

            _isStreamingLogs = true;

            //Remove all existing logs
            _logs.Clear();
            _awaitingLogs.Clear();
            _stringBuilder.Clear();
            _logsText.text = "";
        }

        /// <summary> Write all the logs from the awaiting logs list. </summary>
        public void WriteLogs()
        {
            if (_awaitingLogs.Count == 0 || _isStreamingLogs == false)
                return;

            //Remove the amount of logs to remove
            _RemoveLogFromList(_amountLogsToRemove);

            //Add the new logs from the awaiting list
            foreach (LogLine item in _awaitingLogs)
            {
                _stringBuilder.Append(item.GetLogText() + "\n");
                _logs.Add(item);
            }
            
            _logsText.SetText(_stringBuilder.ToString());

            //Update the size of the content to have the scrollbar usable
            float preferredHeight = _logsText.preferredHeight;
            logsContainer.sizeDelta = new Vector2(logsContainer.sizeDelta.x, preferredHeight);

            _awaitingLogs.Clear();
        }

        public void AddNewLog(LogLine logLine)
        {
            if (_isStreamingLogs == false)
                return;

            _awaitingLogs.Add(logLine);
            _amountLogs += 1;// logLine.thickness;

            Debug.Log($"Amount logs is ({_amountLogs}) and max logs is ({_maxAmountOfLogs})");

            if (_amountLogs > _maxAmountOfLogs)
                _amountLogsToRemove = _amountLogs - _maxAmountOfLogs;
        }

        void _RemoveLogFromList(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _stringBuilder.Remove(0, _logs[i].GetLogLength());//Add 2 because of the \n after each log
            }
            _logs.RemoveRange(0, amount);
            _amountLogsToRemove = 0;
            _amountLogs -= amount;
        }
    }
}