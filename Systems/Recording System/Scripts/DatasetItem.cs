using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PropellerCap
{
    [System.Serializable]
    public class DatasetItem
    {
        /// <summary> The asset inside the unity project that contains the serialized binary information of the path and actions. </summary>
        public Object datasetAsset { get; set; }
        /// <summary> The path of the file that contains te serialized data. </summary>
        public string filePath { get; set; }
        /// <summary> The name of the file that contains the serialized data, without extension. </summary>
        public string fileName { get; set; }
        /// <summary> The name of the level in which the data has been recorded. </summary>
        public string levelName { get; set; }
        /// <summary> DateTime information retrieved from the file name. </summary>
        public System.DateTime dateTime { get; set; }
        /// <summary> The startColor used by the line renderer's material. </summary>
        public Color startColor { get; set; }
        /// <summary> The endColor used by the line renderer's material. </summary>
        public Color endColor { get; set; }
        /// <summary> Whether the places the player has interacted with other objects should be shown in the path. </summary>
        public bool showInteractions { get; set; }
        /// <summary> Whether the deaths of the player should be shown, if there is one. </summary>
        public bool showDeaths { get; set; }
        /// <summary> Whether the path representing the dataset should be visible or not. </summary>
        public bool isShown { get; set; }
        /// <summary> Whether the dataset is being inspected which gives access to more options. </summary>
        public bool isInspecting { get; set; }
        /// <summary> Whether the dataset currently has a dummy moving along its path. </summary>
        public bool isSimulating { get; set; }
        /// <summary> From 0 to 1, represents the progress of the dummy along the path. </summary>
        public float simulationProgress { get; set; }
        /// <summary> MonoBehaviour in the scene which holds a serialized instance of the datasetItem. </summary>
        public DatasetInstance datasetInstance { get; set; }
        /// <summary> Whether the data has been processed and assigned to a line renderer. </summary>
        public bool hasBeenVisualized { get; set; }
        /// <summary> The line renderer used to visualize the data. </summary>
        public LineRenderer lineRenderer { get; set; }
        /// <summary> The width of the line renderer line.</summary>
        public float lineWidth { get; set; }
        /// <summary> The tolerance used by the line renderer. </summary>
        public float tolerance { get; set; }
        /// <summary> The list of player data samples after being processed. </summary>
        public List<PositionSample> playerDataSamples { get; set; }
        /// <summary> Total amount of points used bythe line renderer. </summary>
        public int totalPoints { get; set; }
        /// <summary> Should be used to modify the speed of the playback. </summary>
        public float playbackMultiplier { get; set; }
        /// <summary> Fixed value based on the amount of points in the curve. Use playbackMultiplier to change the playback speed. </summary>
        public float playbackSpeed { get; private set; }

        #region Sample specific properties
        /// <summary> The positions in the world of the player, over time. </summary>
        public List<Vector3> playerPositions { get; set; }
        /// <summary> The interval in seconds between each data sample. </summary>
        public float sampleFrequencyInteraval { get; private set; }
        #endregion Sample specific properties


        public DatasetItem(Object datasetAsset)
        {
            this.datasetAsset = datasetAsset;
#if UNITY_EDITOR
            filePath = AssetDatabase.GetAssetPath(datasetAsset);
            fileName = Path.GetFileNameWithoutExtension(filePath);
            DatasetName datasetName = new DatasetName(fileName);
            levelName = datasetName.levelName;
            dateTime = datasetName.dateTime;
#endif

            int maxColorValue = 255;
            startColor = new Color(Random.value * maxColorValue / 255f, Random.value * maxColorValue / 255f, Random.value * maxColorValue / 255f);
            endColor = startColor;
            showInteractions = false;
            showDeaths = false;
            isShown = true;
            isInspecting = false;
            isSimulating = false;
            simulationProgress = 0f;
            hasBeenVisualized = false;
            lineWidth = 0.1f;
            tolerance = 1f;
            playerDataSamples = new List<PositionSample>();
            totalPoints = 0;
            playbackMultiplier = 1f;
            playbackSpeed = 0.001f;

            //Sample specific properties
            playerPositions = new List<Vector3>();
            sampleFrequencyInteraval = 0.2f;
        }

        public void ProcessPlayerDataSamples()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Can not process player data from file at path:\n" + filePath);
                return;
            }

            using (FileStream stream = File.OpenRead(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                playerDataSamples = (List<PositionSample>)formatter.Deserialize(stream);
            }

            foreach (PositionSample item in playerDataSamples)
            {
                playerPositions.Add(item.position);
            }
            totalPoints = playerPositions.Count;
            playbackSpeed = (1f / sampleFrequencyInteraval) / (float)totalPoints;
        }
    }
}