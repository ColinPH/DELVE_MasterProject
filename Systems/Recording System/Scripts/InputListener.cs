using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class InputListener
    {
        public ControlledInputAction controlledAction;
        public InputType inputType;
        PlayerInputRecorder inputRecoder;
        public InputListener(ControlledInputAction controlledAction, InputType inputType, PlayerInputRecorder inputRecorder)
        {
            this.controlledAction = controlledAction;
            this.inputType = inputType;
            this.inputRecoder = inputRecorder;
            controlledAction.started += _AddToStarted;
            controlledAction.performed += _AddToPerformed;
            controlledAction.canceled += _AddToCancelled;
        }

        private void _AddToStarted(ControlledInputContext context)
        {
            if (controlledAction.controlType == ActionControlType.Button)
                inputRecoder.AddButtonSamples(inputType, ActionEventType.Started, Time.time);
            else if (controlledAction.controlType == ActionControlType.Vector2)
                inputRecoder.AddVector2Samples(inputType, ActionEventType.Started, Time.time, context.GetVector2());
        }
        private void _AddToPerformed(ControlledInputContext context)
        {
            if (controlledAction.controlType == ActionControlType.Button)
                inputRecoder.AddButtonSamples(inputType, ActionEventType.Performed, Time.time);
            else if (controlledAction.controlType == ActionControlType.Vector2)
                inputRecoder.AddVector2Samples(inputType, ActionEventType.Performed, Time.time, context.GetVector2());
        }
        private void _AddToCancelled(ControlledInputContext context)
        {
            if (controlledAction.controlType == ActionControlType.Button)
                inputRecoder.AddButtonSamples(inputType, ActionEventType.Canceled, Time.time);
            else if (controlledAction.controlType == ActionControlType.Vector2)
                inputRecoder.AddVector2Samples(inputType, ActionEventType.Canceled, Time.time, context.GetVector2());
        }
    }
}
