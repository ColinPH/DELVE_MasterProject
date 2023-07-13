using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public class MetricsVisualizer : EditorWindow
    {


        [MenuItem("Propeller Cap/Tools/Metrics Visualizer")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(MetricsVisualizer));
            window.titleContent = new GUIContent("Metrics Visualizer");
        }


        private void OnGUI()
        {
            Rect firstRect = GUILayoutUtility.GetRect(position.width, position.height);
            GUI.BeginClip(firstRect);
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(
                Texture2D.whiteTexture,
                2,
                Vector3.zero,
                new Vector3(120, 91, 0),
                new Vector3(220, 91, 0),
                new Vector3(350, 20, 0));
            GUI.EndClip();

        }

    }
}
