using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public enum ActionEventType { Started = 0, Performed = 1, Canceled = 2, };
    public enum ActionControlType { Vector2 = 0, Button = 1, }

    [Serializable]
    public struct PlayerInputSamples
    {
        public List<PlayerInputSample_Vector2> vector2Samples;
        public List<PlayerInputSample_Button> buttonSamples;
        public List<PlayerCursorSample> cameraSamples;

        public PlayerInputSamples(
            List<PlayerInputSample_Vector2> vector2Samples, 
            List<PlayerInputSample_Button> buttonSamples,
            List<PlayerCursorSample> cameraSamples)
        {
            this.vector2Samples = vector2Samples;
            this.buttonSamples = buttonSamples;
            this.cameraSamples = cameraSamples;
        }
    }

    [Serializable]
    public struct PlayerCursorSample
    {
        public float sampleTime;
        float x;
        float y;

        public PlayerCursorSample(float sampleTime, Vector2 position)
        {
            this.sampleTime = sampleTime;
            x = position.x;
            y = position.y;
        }

        public Vector2 position => new Vector2(x, y);
    }

    [Serializable]
    public struct PlayerInputSample_Vector2
    {
        public InputType inputType;
        public ActionEventType actionEventType;
        public float castingTime;
        float x;
        float y;
        public PlayerInputSample_Vector2(InputType inputType, ActionEventType actionEventType, float castingTime, Vector2 sampleValue)
        {
            this.inputType = inputType;
            this.castingTime = castingTime;
            this.x = sampleValue.x;
            this.y = sampleValue.y;
            this.actionEventType = actionEventType;
        }

        public Vector2 sampleValue => new Vector2(x, y);
    }

    [Serializable]
    public struct PlayerInputSample_Button
    {
        public InputType inputType;
        public ActionEventType actionEventType;
        public float castingTime;
        public PlayerInputSample_Button(InputType inputType, ActionEventType actionEventType, float castingTime)
        {
            this.inputType = inputType;
            this.castingTime = castingTime;
            this.actionEventType = actionEventType;
        }
    }
}
