using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PropellerCap
{
    public class SimpleTimer : WorldObject
    {
        
        public TextMeshProUGUI timerText;

        private float startTime;
        private float elapsedTime;
        private bool isRunning = false;

        protected override void MyAwake()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (isRunning)
            {
                elapsedTime = Time.time - startTime;
                DisplayTime(elapsedTime);
            }
        }

        public void StartTimer()
        {
            if (!isRunning)
            {
                isRunning = true;
                startTime = Time.time - elapsedTime;
            }
        }

        public void PauseTimer()
        {
            if (isRunning)
            {
                isRunning = false;
                elapsedTime = Time.time - startTime;
            }
        }

        public void StopTimer()
        {
            isRunning = false;
            elapsedTime = 0f;
            DisplayTime(elapsedTime);
        }

        public void ResetTimer()
        {
            StopTimer();
            DisplayTime(elapsedTime);
        }

        private void DisplayTime(float time)
        {
            float minutes = Mathf.FloorToInt(time / 60f);
            float seconds = Mathf.FloorToInt(time % 60f);
            float milliseconds = (time % 1f) * 1000f;

            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }
}
