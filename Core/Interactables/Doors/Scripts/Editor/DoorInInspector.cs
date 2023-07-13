using PropellerCap;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(Door))]
    [CanEditMultipleObjects]
    public class DoorInInspector : Editor
    {
        /*Door _door;

        private void OnEnable()
        {
            _door = (Door)target;
        }

        public override void OnInspectorGUI()
        {
            //Show the ActivationComplete and DeactivationComplete UnityEvents
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onActivationComplete"), includeChildren: true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onDeactivationComplete"), includeChildren: true);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }*/
    }
}