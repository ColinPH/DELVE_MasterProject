using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap.QA
{
    public enum SpecialKey
    {
        None,
        Shift,
        Ctrl
    }

    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField] SpecialKey _specialKey1 = SpecialKey.None;
        [SerializeField] SpecialKey _specialKey2 = SpecialKey.None;
        [SerializeField] Key _activationKey = Key.D;
        [SerializeField] Key _commandsFieldFocusKey = Key.Enter;
        [SerializeField] Key _unlockMouseKey = Key.O;
        [SerializeField] bool _destroyAndCreateCanvasObj = false;
        [SerializeField] bool _showConsoleOnAwake = false;
        [SerializeField] GameObject _debugConsoleCanvasPrefab;
        [Range(1, 60), Tooltip("Times per seconds the UI elements are updated")]
        [SerializeField] int infoDisplayFrequency = 20;

        bool _debugConsoleIsActive = false;
        [HideInInspector] public bool blockActivationKey = false;
        //Variables for refreshing the UI elements
        float _lastUpdateTime = 0f;
        float _updateWaitingTime = 0f;

        DebugConsoleCanvas _consoleCanvas { get; set; }
        FrameRateProcessor fpsProcessor { get; set; }

        bool isPrinting = false;

        private void Awake()
        {
            fpsProcessor = new FrameRateProcessor();

            if (_showConsoleOnAwake)
            {
                Debug.Log("Spawn overlay");
                _ChangeConsoleState();
            }
        }

        private void Update()
        {
            _ProcessInput();

            if (_debugConsoleIsActive == false) return;


            fpsProcessor.ProcessFramesData();
            _UpdateConsoleCanvasElements();
        }

        private void LateUpdate()
        {
            if (_debugConsoleIsActive == false) return;

            _consoleCanvas.WriteLogs();
        }

        #region Logs

        public void WriteLog(string message)
        {
            if (_debugConsoleIsActive == false) return;
            _consoleCanvas.AddNewLog(message);
        }
        public void PauseLogsStream()
        {
            _consoleCanvas.PauseLogsStream();
        }
        public void ResumeLogsStream()
        {
            _consoleCanvas.ResumeLogsStream();
        }

        #endregion


        //*******************************************************************************
        //Processes input, shows and hides the debug console, updates the canvas elements
        //*******************************************************************************

        #region Process input and display information

        private void _ProcessInput()
        {
            if (Keyboard.current[_activationKey].wasPressedThisFrame && blockActivationKey == false)
            {
                bool specialKeyIsPressed_1 = _SpecialKeyIsPressed(_specialKey1);
                bool specialKeyIsPressed_2 = _SpecialKeyIsPressed(_specialKey2);
                if (specialKeyIsPressed_1 && specialKeyIsPressed_2)
                {
                    _ChangeConsoleState();
                }
            }

            if (_debugConsoleIsActive)
            {
                if (Keyboard.current[_commandsFieldFocusKey].wasPressedThisFrame)
                    _consoleCanvas.FocusCommandInputField();
            }

            if (Keyboard.current[_unlockMouseKey].wasPressedThisFrame)
            {
                //Activate the mouse
                if (Inputs.mouseIsActive)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Inputs.mouseIsActive = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Inputs.mouseIsActive = true;
                }
            }
        }

        public void _ChangeConsoleState()
        {
            if (_debugConsoleIsActive)
            {
                HideConsole(_destroyAndCreateCanvasObj);
            }
            else
            {
                ShowConsole(false);
            }
        }

        public void ShowConsole(bool focusCommandsInputField = true)
        {
            _debugConsoleIsActive = true;

            fpsProcessor.OnDebugConsoleShow();

            if (_consoleCanvas == null)
            {
                //Instantiate the canvas
                Debug.Log("Instantiating the logger window");
                _consoleCanvas = Instantiate(_debugConsoleCanvasPrefab).GetComponent<DebugConsoleCanvas>();
                _consoleCanvas.InitializeDebugConsoleCanvas(this, fpsProcessor);
            }

            _consoleCanvas.gameObject.SetActive(true);
            _consoleCanvas.OnDebugConsoleShow();
            if (focusCommandsInputField)
                _consoleCanvas.FocusCommandInputField();
        }

        public void HideConsole(bool destroyObject = false)
        {
            _debugConsoleIsActive = false;

            fpsProcessor.OnDebugConsoleHide();

            if (_consoleCanvas != null)
            {
                //Destroy the canvas
                _consoleCanvas.OnDebugConsoleHide();

                _consoleCanvas.gameObject.SetActive(false);

                if (destroyObject)
                {
                    Destroy(_consoleCanvas.gameObject);
                    _consoleCanvas = null;
                }
            }
        }

        private void _UpdateConsoleCanvasElements()
        {
            _updateWaitingTime = 1f / (float)infoDisplayFrequency;
            if (Time.realtimeSinceStartup >= _lastUpdateTime + _updateWaitingTime)
            {
                _consoleCanvas.UpdateCanvasInformation();
                _lastUpdateTime = Time.realtimeSinceStartup;
            }
        }

        private bool _SpecialKeyIsPressed(SpecialKey specialKey)
        {
            switch (specialKey)
            {
                case SpecialKey.None:
                    return true;
                case SpecialKey.Shift:
                    return Keyboard.current[Key.LeftShift].isPressed || Keyboard.current[Key.RightShift].isPressed;
                case SpecialKey.Ctrl:
                    return Keyboard.current[Key.LeftCtrl].isPressed || Keyboard.current[Key.RightCtrl].isPressed;
            }
            return false;
        }

        #endregion
    }
}
