using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(ActivationEvent))]
    [CanEditMultipleObjects]
    public class ActivationEventInInspector : Editor
    {
        ActivationEvent _activationEvent;

        bool _showWarningMessage = false;
        bool _hasMultipleActivationTargets = false;

        private void OnEnable()
        {
            _activationEvent = (ActivationEvent)target;
            _showWarningMessage = _activationEvent.SetActivationTarget(_FindActivationTarget());
        }

        private ActivationTarget _FindActivationTarget()
        {
            var targets = _activationEvent.GetComponents<ActivationTarget>();
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