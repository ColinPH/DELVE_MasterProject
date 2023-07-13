using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(LogMessage))]
    [CanEditMultipleObjects]
    public class ATLogMessageInInspector : Editor
    {
        LogMessage _logMessage;

        bool _showWarningMessage = false;
        bool _hasMultipleActivationTargets = false;


        private void OnEnable()
        {
            _logMessage = (LogMessage)target;
            _showWarningMessage = _logMessage.SetActivationTarget(_FindActivationTarget());
        }

        private ActivationTarget _FindActivationTarget()
        {
            var targets = _logMessage.GetComponents<ActivationTarget>();
            if (targets.Length > 1)
            {
                _hasMultipleActivationTargets = true;
            }
            return targets[0];
        }

        public override void OnInspectorGUI()
        {
            if (_hasMultipleActivationTargets && _showWarningMessage)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox($"There are multiple {nameof(ActivationTarget)} components on this object. Check which one has been assigned.", MessageType.Warning);
                if (GUILayout.Button("Done", GUILayout.ExpandHeight(true)))
                {
                    _showWarningMessage = false;
                }
                EditorGUILayout.EndHorizontal();
            }
            base.OnInspectorGUI();
        }
    }
}