using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(Lever))]
    [CanEditMultipleObjects]
    public class LeverInInspector : Editor
    {
        Lever _lever;

        private void OnEnable()
        {
            _lever = (Lever)target;
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Activate"))
                {
                    _lever.ActivateTargets();
                }
                if (GUILayout.Button("Deactivate"))
                {
                    _lever.DeactivateTargets();
                }
                EditorGUILayout.EndHorizontal();
            }
            base.OnInspectorGUI();
        }
    }
}