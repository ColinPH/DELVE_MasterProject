using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class MetricsManager : ManagerBase
    {
        public static MetricsManager Instance { get; private set; }

        string _playerDataIdentification = "";
        public PlayerSessionData sessionData { get; set; }
        public PlayerRunData runData { get; set; }
        public PlayerLevelData levelData { get; set; }


        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in metrics manager");
            Managers.metricsManager = this;
            Metrics.manager = this;
            Metrics.player = new PlayerMetrics();
            Metrics.game = new GameMetrics();
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in metrics manager");
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in metrics manager");
            _playerDataIdentification = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<MetricsManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion

        public override void MyUpdate()
        {
            base.MyUpdate();
            if (Keyboard.current[Key.U].wasPressedThisFrame)
                _SaveSessionData(sessionData);
        }

        private void OnApplicationQuit()
        {
            CloseSessionData();
        }

        #region Player data sessions, runs and levels
        public void OpenSessionData()
        {
            sessionData = new PlayerSessionData();
            sessionData.startTime = Time.realtimeSinceStartup;
        }

        public void CloseSessionData()
        {
            if (sessionData == null) return;

            if (runData != null)
            {
                Debug.LogError($"The run data has not been closed while trying to close the session data, closing run data now.");
                CloseRunData();
            }
            sessionData.endTime = Time.realtimeSinceStartup;
            //Save the data to a Json file
            _SaveSessionData(sessionData);
            sessionData = null;
        }

        public void OpenRunData(string runName)
        {
            if (runData != null)
            {
                Debug.LogError($"The previous run data has not been closed before opening a new one, closing it now.");
                CloseRunData();
            }
            if (sessionData == null)
            {
                Debug.LogError($"No session data open while trying to open a run data, opening session data now.");
                OpenSessionData();
            }

            runData = new PlayerRunData();
            runData.startTime = Time.realtimeSinceStartup;
            runData.name = runName;
            Debug.Log("Created a new run in the metrics: " + runName);
        }

        public void CloseRunData()
        {
            if (runData == null || sessionData == null) return;

            if (levelData != null)
            {
                Debug.LogError($"The level data has not been closed while trying to close the run data, closing level data now.");
                CloseLevelData();
            }
            runData.endTime = Time.realtimeSinceStartup;
            //Save the current run data in the current session
            sessionData.runs.Add(new PlayerRunData(runData));
            runData = null;
        }

        public void OpenLevelData(string levelName)
        {
            if (levelData != null)
            {
                Debug.LogError($"The previous level data has not been closed before opening a new one, closing it now.");
                CloseLevelData();
            }
            if (runData == null)
            {
                Debug.LogError($"No run data open while trying to open a level data, opening run data now.");
                OpenRunData("Run not found");
            }

            levelData = new PlayerLevelData();
            levelData.startTime = Time.realtimeSinceStartup;
            levelData.name = levelName;
            Debug.Log("Created a new level in the metrics: " + levelName);
        }

        public void CloseLevelData()
        {
            if (levelData == null || runData == null) return;
            levelData.endTime = Time.realtimeSinceStartup;
            //Close all the zone timers
            foreach (var item in levelData.zones)
            {
                if ((item.exitTime > 0f) == false)
                    item.exitTime = Time.time;
            }

            //Save the current level data in the current run
            runData.levels.Add(new PlayerLevelData(levelData));
            levelData = null;
        }
        #endregion Player data sessions, runs and levels


        #region Metric zones

        public void StartZone(string identifier)
        {
            //Check if there is already a zone with the given identifier
            foreach (var item in levelData.zones)
            {
                if (item.ID == identifier)
                {
                    Debugger.Log($"The {nameof(MetricZone)} identifier \"{identifier}\" has already been used to start a zone.");
                    return;
                }
            }

            //The identifier has not been used yet
            MetricZone newZone = new MetricZone(identifier, Time.time);
            levelData.zones.Add(newZone);
        }

        public void StopZone(string identifier)
        {
            //Make sure the given identifier has been used to start a zone
            foreach (var item in levelData.zones)
            {
                if (item.ID == identifier)
                {
                    //Make sure it has not already been stopped
                    if (item.exitTime > 0f)
                    {
                        Debugger.Log($"The {nameof(MetricZone)} identifier \"{identifier}\" has already been used to stop a zone.");
                        return;
                    }

                    //Add the end time to the zone
                    item.exitTime = Time.time;
                    return;
                }
            }

            Debugger.Log($"The {nameof(MetricZone)} identifier \"{identifier}\" could not stop a zone because no zone was previously started with that identifier.");
        }

        #endregion Metric zones

        private void _SaveSessionData(PlayerSessionData sessionData)
        {
            Debugger.Log($"Saving player data.");
            // Create a directory if it does not exist
            string directoryPath = Application.persistentDataPath + "/PlayerData";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create a file name with the current date and time
            string fileName = "DELVE_PlayerSessionData_" + _playerDataIdentification + ".json";

            // Create a file path
            string filePath = Path.Combine(directoryPath, fileName);

            // Serialize the session data to JSON
            string json = JsonUtility.ToJson(sessionData, true);

            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
        }
    }
}