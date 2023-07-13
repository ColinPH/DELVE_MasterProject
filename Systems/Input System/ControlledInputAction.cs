using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    /// <summary> Same as Unity's InputAction class, it just gives us another layer of control. </summary>
    public class ControlledInputAction
    {
        InputAction _action;
        PlayerInputController _controller;
        ActionControlType _controlType;
        bool inputIsActive = true;
        bool _isSimulateInput = false;

        public delegate void OnActionEvent(ControlledInputContext context);
        public OnActionEvent started { get; set; }
        public OnActionEvent performed { get; set; }
        public OnActionEvent canceled { get; set; }

        public delegate void OnInputDisabled();
        public OnInputDisabled onInputDisabled { get; set; }
        public OnInputDisabled onInputEnabled { get; set; }

        public ActionControlType controlType => _controlType;

        public ControlledInputAction(InputAction action, PlayerInputController controller, ActionControlType controlType)
        {
            _controller = controller;
            _action = action;
            _controlType = controlType;

            action.started += OnActionStarted;
            action.performed += OnActionPerformed;
            action.canceled += OnActionCanceled;

            _controller.OnInputStateChange += SetInputIsActiveTo;
        }
        public ControlledInputAction(PlayerInputController controller, ActionControlType controlType)
        {
            _controller = controller;
            _controlType = controlType;
            _isSimulateInput = true;
        }

        public void DeregisterFromInput()
        {
            _action.started -= OnActionStarted;
            _action.performed -= OnActionPerformed;
            _action.canceled -= OnActionCanceled;

            _controller.OnInputStateChange -= SetInputIsActiveTo;
        }

        public void SetInputIsActiveTo(bool newState)
        {
            inputIsActive = newState;

            if (newState)
                onInputEnabled?.Invoke();
            else
                onInputDisabled?.Invoke();
        }

        void OnActionStarted(InputAction.CallbackContext context)
        {
            if (inputIsActive)
                started?.Invoke(new ControlledInputContext(context));
        }
        void OnActionPerformed(InputAction.CallbackContext context)
        {
            if (inputIsActive)
                performed?.Invoke(new ControlledInputContext(context));
        }

        void OnActionCanceled(InputAction.CallbackContext context)
        {
            if (inputIsActive)
                canceled?.Invoke(new ControlledInputContext(context));
        }

        public void SimulateActionStarted(ControlledInputContext controlledContext)
        {
            if (_isSimulateInput && inputIsActive)
                started?.Invoke(controlledContext);
        }
        public void SimulateActionPerformed(ControlledInputContext controlledContext)
        {
            if (_isSimulateInput && inputIsActive)
                performed?.Invoke(controlledContext);
        }
        public void SimulateActionCanceled(ControlledInputContext controlledContext)
        {
            if (_isSimulateInput && inputIsActive)
                canceled?.Invoke(controlledContext);
        }

        public bool isPressed => _action.IsPressed();
        public bool wasPressedThisFrame => _action.WasPressedThisFrame();
        public bool wasReleasedThisFrame => _action.WasReleasedThisFrame();
    }
}