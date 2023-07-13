using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    [Serializable]
    public class AutomatedScreenshotSetings
    {
        [Tooltip("Amount of seconds between each screenshot.")]
        public int secondsBetweenShots = 300;
        [Tooltip("Identifier for the screenshots that will be made. User and date are already in automatically. This could be something like \"Iteration 1\" for example.")]
        public string identifier = "";
        [Tooltip("Whether the process of taking screenshots should be started when unity launches.")]
        public bool startProcessOnStartup = true;
        [Tooltip("Amount of screenshots above which there will be a warning.")]
        public int amountWarningThreshold = 1000;
        //[TextArea, Tooltip("Path to the folder in which to save all the screenshots.")]
        //public string screenshotsFolderPath = "";

        /// <summary>
        /// Called when the scriptable object that contains these settings is Enabled
        /// </summary>
        public void OnContainerEnable()
        {
            
        }
    }
}