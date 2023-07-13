using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] bool _simulate = false;
        [SerializeField] UnityEngine.Object _inputRecordings;
        [SerializeField] Transform _playerTransform;
        [SerializeField] Transform _cameraTransform;
        //TODO this information should be fetched in the input settings
        public string keyboardMouseControlSchemeName = "MouseKeyboard";
        public string gamepadControlSchemeName = "Controller";

        PlayerInput _playerInput;
        InputDevice _activeDevice;
        bool _isReadingInput = true;
        bool _isSimulatingInputs = false;

        PlayerInputSamplesReader _samplesReader;

        /// <summary> Whether the controller reads and processes the events from the input map. </summary>
        public bool isReadingInput { get => _isReadingInput; }
        /// <summary> Whether the input controller is firing the inputs from a data file. </summary>
        public bool isSimulatingInputs => _isSimulatingInputs;
        public delegate void InputStateChangeHandler(bool newState);
        /// <summary> Called when the input is enabled or disabled. </summary>
        public InputStateChangeHandler OnInputStateChange { get; set; }


        public InputDevice inputDevice { get => _activeDevice; }
        public delegate void InputDeviceChangeHandler(InputDevice newDevice);
        public InputDeviceChangeHandler OnInputDeviceChanged { get; set; }


        private void Awake()
        {
            if (_inputRecordings != null && Application.isEditor && _simulate)
            {
                _isSimulatingInputs = true;
                _samplesReader= new PlayerInputSamplesReader(_inputRecordings, this, _playerTransform, _cameraTransform);
                return;
            }

            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onControlsChanged += _OnDeviceChanged;

            _activeDevice = _GetCurrentDevice();
        }

        private void Update()
        {
            if (_isSimulatingInputs)
            {
                _samplesReader.ProcessSamples();
            }
        }

        private void _OnDeviceChanged(PlayerInput obj)
        {
            InputDevice newDevice = _GetCurrentDevice();
            _activeDevice = newDevice;
            OnInputDeviceChanged?.Invoke(newDevice);
        }

        public void StopSimulation()
        {
            _isSimulatingInputs = false;
        }

        /// <summary> Enables the inputs. Does not include the camera view. </summary>
        public void EnableInputs()
        {
            if (_isSimulatingInputs) return;

            OnInputStateChange?.Invoke(true);
            _isReadingInput = true;
        }

        /// <summary> Disables the inputs. Does not include the camera view. </summary>
        public void DisableInputs()
        {
            if (_isSimulatingInputs) return;

            OnInputStateChange?.Invoke(false);
            _isReadingInput = false;
        }

        /// <summary> Should be called after awake </summary>
        public ControlledInputAction GetActionOfType(InputType inputType)
        {
            if (_isSimulatingInputs)
            {
                return _samplesReader.GetActionOfType(inputType);
            }

            switch (inputType)
            {
                case InputType.Primary:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Primary)], this, GetActionControlType(inputType));
                case InputType.Secondary:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Secondary)], this, GetActionControlType(inputType));
                case InputType.Movement:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Movement)], this, GetActionControlType(inputType));
                case InputType.Rotation:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Rotation)], this, GetActionControlType(inputType));
                case InputType.Action1:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Action1)], this, GetActionControlType(inputType));
                case InputType.Action2:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Action2)], this, GetActionControlType(inputType));
                case InputType.Action3:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Action3)], this, GetActionControlType(inputType));
                case InputType.Action4:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Action4)], this, GetActionControlType(inputType));
                case InputType.Jump:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Jump)], this, GetActionControlType(inputType));
                case InputType.Pause:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.Pause)], this, GetActionControlType(inputType));
                case InputType.PrimaryAlternative:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.PrimaryAlternative)], this, GetActionControlType(inputType));
                case InputType.SecondaryAlternative:
                    return new ControlledInputAction(_playerInput.actions[nameof(InputMap.Level.SecondaryAlternative)], this, GetActionControlType(inputType));
                default:
                    Debug.LogError("InputType no implemented in switch statement : " + inputType);
                    return null;
            }
        }

        private InputDevice _GetCurrentDevice()
        {
            if (isSimulatingInputs)
            {
                return InputDevice.KeyboardMouse;
            }

            if (_playerInput.currentControlScheme == keyboardMouseControlSchemeName)
            {
                return InputDevice.KeyboardMouse;
            }
            else if (_playerInput.currentControlScheme == gamepadControlSchemeName)
            {
                return InputDevice.GamePad;
            }
            else
            {
                Debug.LogError("Control scheme names do not match. Defaulting to keyboard and mouse");
                return InputDevice.KeyboardMouse;
            }
        }

        public ActionControlType GetActionControlType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Primary:
                    return ActionControlType.Button;
                case InputType.Secondary:
                    return ActionControlType.Button;
                case InputType.Movement:
                    return ActionControlType.Vector2;
                case InputType.Rotation:
                    return ActionControlType.Vector2;
                case InputType.Action1:
                    return ActionControlType.Button;
                case InputType.Action2:
                    return ActionControlType.Button;
                case InputType.Action3:
                    return ActionControlType.Button;
                case InputType.Action4:
                    return ActionControlType.Button;
                case InputType.Jump:
                    return ActionControlType.Button;
                case InputType.Pause:
                    return ActionControlType.Button;
                case InputType.PrimaryAlternative:
                    return ActionControlType.Button;
                case InputType.SecondaryAlternative:
                    return ActionControlType.Button;
                default:
                    Debug.LogError("InputType no implemented in switch statement : " + inputType);
                    return ActionControlType.Button;
            }
        }
    }
}