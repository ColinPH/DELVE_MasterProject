using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

namespace PropellerCap
{
    [CustomEditor(typeof(FragmentSpawnMarker))]
    public class FragmentSpawnMarkerInInspector : Editor
    {
        FragmentSpawnMarker _marker;

        private void OnEnable()
        {
            _marker = (FragmentSpawnMarker)target;
            //Generate the GUID if it is not -1
            _marker.GenerateGUID();
        }

        public override void OnInspectorGUI()
        {
            if (_marker.HasGUID() == false)
            {
                string text = $"The GUID has not been assigned. This should not be done in the Prefab otherwise the GUIDs will be the same. Use the component context menu to generate the GUID.";
                EditorGUILayout.HelpBox(text, MessageType.Warning);
                GUILayoutOption[] buttonOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.wordWrap = true;
            }
            base.OnInspectorGUI();
        }
    }
}
