using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(DeactivationEffects))]
    public class DeactivationEffectsInInspector : Editor
    {
        DeactivationEffects _deactivationEffects;

        bool _showWarningMessage = false;
        bool _hasMultipleActivationTargets = false;


        private void OnEnable()
        {
            _deactivationEffects = (DeactivationEffects)target;
            _showWarningMessage = _deactivationEffects.SetActivationTarget(_FindActivationTarget());
        }

        private ActivationTarget _FindActivationTarget()
        {
            var targets = _deactivationEffects.GetComponents<ActivationTarget>();
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