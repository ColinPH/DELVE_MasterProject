using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PropellerCap.QA
{
    public class CommandsCanvasDisplay : MonoBehaviour
    {
        [SerializeField] TMP_InputField _commandsInputField;

        DebugOverlay _console;

        public void InitializeCanvasDisplay(DebugOverlay console)
        {
            _console = console;
        }

        public void FocusCommandInputField()
        {
            _commandsInputField.Select();
        }

        public void OnDebugConsoleShow()
        {

            //Hook the input field for the commands and blocking the activation key from disabling the console
            _commandsInputField.onSubmit.AddListener((rawCommandText) =>
            {
                if (rawCommandText != "")
                {
                    Debugger.ProcessCommand(rawCommandText);
                    _commandsInputField.text = "";
                }
                _console.blockActivationKey = false;
            });
            _commandsInputField.onDeselect.AddListener((rawCommandText) =>
            {
                _console.blockActivationKey = false;
            });

        }

        public void OnDebugConsoleHide()
        {
            _commandsInputField.onSubmit.RemoveAllListeners();
            _commandsInputField.onDeselect.RemoveAllListeners();
        }
    }
}