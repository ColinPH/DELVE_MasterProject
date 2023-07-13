using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder;
using static UnityEngine.Rendering.DebugUI.Table;

namespace PropellerCap
{
    public class FirstPersonViewController : MonoBehaviour
    {
        [SerializeField] Transform targetCamera;
        [Header("Keyboard Sensitivity")]
        [SerializeField] float mouseSensitivity = 2.0f;
        float smoothing = 0.8f;
        [Header("Gamepad Sensitivity")]
        public bool inverseVerticalMovement = true;
        [SerializeField] Vector2 gamepadSensitivity = new Vector2(100f, 100f);

        InputDevice _currentInputDevice = InputDevice.KeyboardMouse;

        Vector2 _joystickCoords = new Vector2();

        PlayerInputController _inputController;
        ControlledInputAction _rotationAction;
        Transform playerTransform;
        bool _simulationIsActive = false;

        private void Start()
        {
            playerTransform = gameObject.transform;
            _inputController = GetComponent<PlayerInputController>();
            if (_inputController.isSimulatingInputs)
            {
                _simulationIsActive = true;
                return;
            }

            _rotationAction = _inputController.GetActionOfType(InputType.Rotation);
            _rotationAction.performed += OnRotationInput;

            _currentInputDevice = _inputController.inputDevice;
            _inputController.OnInputDeviceChanged += _OnInputDeviceChanged;
        }

        void Update()
        {
            if (_simulationIsActive) return;

            if (_currentInputDevice == InputDevice.GamePad)
            {
                _RotateCameraAlongJoystick();
            }
            else if (_currentInputDevice == InputDevice.KeyboardMouse)
            {
                //_RotateCameraAlongJoystick();
                ControlCameraRotation();
            }
        }


        private void _OnInputDeviceChanged(InputDevice newDevice)
        {
            _currentInputDevice = newDevice;
        }


        private void _RotateCameraAlongJoystick()
        {
            Vector3 additionalRotation = new Vector3();

            float inverseFactor = (inverseVerticalMovement) ? -1f : 1f;

            //Rotate the player
            additionalRotation.y = _joystickCoords.x * gamepadSensitivity.x * Time.deltaTime;
            additionalRotation.x = _joystickCoords.y * gamepadSensitivity.y * Time.deltaTime * inverseFactor;

            Debug.Log($"total : {transform.eulerAngles + additionalRotation} angle : {transform.eulerAngles} addition : {additionalRotation}");

           

            #region GPT solution
            // Get the current rotation of the player object
            Quaternion currentRotation = playerTransform.transform.rotation;

            // Calculate the rotation around the y-axis
            float yRotation = additionalRotation.y;
            Quaternion yRotationQuaternion = Quaternion.AngleAxis(yRotation, Vector3.up);

            // Combine the y-axis rotation with the current rotation
            Quaternion newRotation = yRotationQuaternion * currentRotation;

            // Set the new rotation of the player object
            playerTransform.transform.rotation = newRotation;

            // Limit the rotation of the camera around the x-axis
            float xRotation = additionalRotation.x * -1f;
            float angle = targetCamera.transform.localEulerAngles.x - xRotation;
            float maxAngle = 85f;
            if (angle > 180)
            {
                angle -= 360;
            }
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
            targetCamera.transform.localEulerAngles = new Vector3(angle, 0, 0);

            #endregion
        }

        private void ControlCameraRotation()
        {
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            Vector2 mouseLook = Vector3.zero;
            Vector2 smoothVector = Vector3.zero;

            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(mouseSensitivity * smoothing, mouseSensitivity * smoothing));
            smoothVector.x = Mathf.Lerp(smoothVector.x, mouseDelta.x, 1f / smoothing);
            smoothVector.y = Mathf.Lerp(smoothVector.y, mouseDelta.y, 1f / smoothing);
            mouseLook += smoothVector;

            Vector3 additionalRotation = new Vector3(-mouseLook.y, mouseLook.x, 0);

            // Get the current rotation of the player object
            Quaternion currentRotation = playerTransform.transform.rotation;

            // Calculate the rotation around the y-axis
            float yRotation = additionalRotation.y;
            Quaternion yRotationQuaternion = Quaternion.AngleAxis(yRotation, Vector3.up);

            // Combine the y-axis rotation with the current rotation
            Quaternion newRotation = yRotationQuaternion * currentRotation;

            // Set the new rotation of the player object
            playerTransform.transform.rotation = newRotation;

            // Limit the rotation of the camera around the x-axis
            float xRotation = additionalRotation.x * -1f;
            float angle = targetCamera.transform.localEulerAngles.x - xRotation;
            float maxAngle = 85f;
            if (angle > 180)
            {
                angle -= 360;
            }
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
            targetCamera.transform.localEulerAngles = new Vector3(angle, 0, 0);

        }


        private void OnRotationInput(ControlledInputContext context)
        {
            _joystickCoords = context.GetVector2();
        }

    }
}
