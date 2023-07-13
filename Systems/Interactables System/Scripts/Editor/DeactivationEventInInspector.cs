using PropellerCap;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeactivationEvent))]
[CanEditMultipleObjects]
public class DeactivationEventInInspector : Editor
{
    DeactivationEvent _deactivationEvent;

    bool _showWarningMessage = false;
    bool _hasMultipleActivationTargets = false;

    private void OnEnable()
    {
        _deactivationEvent = (DeactivationEvent)target;
        _showWarningMessage = _deactivationEvent.SetActivationTarget(_FindActivationTarget());
    }

    private ActivationTarget _FindActivationTarget()
    {
        var targets = _deactivationEvent.GetComponents<ActivationTarget>();
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