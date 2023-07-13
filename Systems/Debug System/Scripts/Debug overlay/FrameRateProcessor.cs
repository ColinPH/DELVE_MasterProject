using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap.QA
{
    [Serializable]
    public class FrameRateProcessor
    {
        //FPS variables
        [SerializeField] int FPSAverageAmount = 20;
        public bool resetMinMaxAfterSeconds = true;
        [SerializeField] int minMaxResetAfter = 300;
        [SerializeField] int targetFPS = 60;
        [SerializeField] Color targetFPSColor = Color.green;
        [SerializeField] Color mediumFPSColor = new Color(255, 153, 0); //Orange
        [SerializeField] Color lowFPSColor = Color.red;

        [HideInInspector] public float latestDelta = 0f;
        [HideInInspector] public int latestFPS = 0;
        [HideInInspector] public int averageFPS = 0;
        [HideInInspector] public float lowestDelta = -1f;
        [HideInInspector] public float highestDelta = 0f;

        float[] _deltaTimes;
        int _deltasIndex = 0;
        int _minMaxIncrement = 0;

        public void ProcessFramesData()
        {
            //Average FPS
            latestDelta = Time.deltaTime;
            latestFPS = Mathf.FloorToInt(1f / latestDelta);
            _deltaTimes[_deltasIndex] = latestDelta;
            averageFPS = Mathf.FloorToInt(1f / _deltaTimes.Average());
            _deltasIndex++;

            if (_deltasIndex >= FPSAverageAmount) _deltasIndex = 0;

            //Min max ms
            if (latestDelta > highestDelta)
                highestDelta = latestDelta;
            if (latestDelta < lowestDelta || lowestDelta < 0f)
                lowestDelta = latestDelta;

            //Reset min max ms
            if (resetMinMaxAfterSeconds)
            {
                _minMaxIncrement++;
                if (_minMaxIncrement >= minMaxResetAfter)
                {
                    _minMaxIncrement = 0;
                    lowestDelta = -1f;
                    highestDelta = 0f;
                }
            }
        }

        public void ResetMinMaxMSValues()
        {
            _minMaxIncrement = 0;
            lowestDelta = -1f;
            highestDelta = 0f;

            ProcessFramesData();
        }

        public void OnDebugConsoleShow()
        {
            _deltaTimes = new float[FPSAverageAmount];
        }
        public void OnDebugConsoleHide()
        {

        }

        public Color GetMilliSecondsColor(float ms)
        {
            return _GetFPSColor(Mathf.FloorToInt(1f / ms));
        }

        public Color GetFPSColour(int fps)
        {
            return _GetFPSColor(fps);
        }

        private Color _GetFPSColor(int fps)
        {
            if (fps >= targetFPS)
                return targetFPSColor;
            else if (fps >= targetFPS / 2f)
                return mediumFPSColor;
            else
                return lowFPSColor;
        }
    }
}