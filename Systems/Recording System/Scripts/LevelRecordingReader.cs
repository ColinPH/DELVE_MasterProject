using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PropellerCap
{
    [RequireComponent(typeof(LineRenderer))]
    public class LevelRecordingReader : MonoBehaviour
    {
        [SerializeField] Object levelData;
        public Color lineColor = Color.green;

        [ContextMenu("ClearLineRenderer")]
        private void ClearLineRenderer()
        {
            GetComponent<LineRenderer>().positionCount = 0;
        }

#if UNITY_EDITOR
        [ContextMenu("READ")]
        private void ReadFile()
        {
            string path = AssetDatabase.GetAssetPath(levelData);
            if (!File.Exists(path))
            {
                Debug.LogWarning("File not found: " + path);
                return;
            }

            List<PositionSample> positions = new List<PositionSample>();
            using (FileStream stream = File.OpenRead(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                positions = (List<PositionSample>)formatter.Deserialize(stream);                    
                Debug.Log("Done reading"); 
            }
            List<Vector3> poss = new List<Vector3>();
            foreach (var item in positions)
            {
                poss.Add(item.position);
            }

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = poss.Count;
            lineRenderer.SetPositions(poss.ToArray());
            lineRenderer.material.color = lineColor;
        }
#endif
    }
}