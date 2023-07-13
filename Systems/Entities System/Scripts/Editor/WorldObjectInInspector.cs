using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(WorldObject), true)]
    public class WorldObjectInInspector : Editor
    {
        WorldObject _worldObject;

        private void OnEnable()
        {
            _worldObject = (WorldObject)target;

            _CheckForWorldObjectGUID();

            m_MyOnEnable();
        }

        public override void OnInspectorGUI()
        {
            //Draw the GUID
            if (PrefabUtility.GetPrefabInstanceStatus(_worldObject.gameObject) == PrefabInstanceStatus.Connected)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("GUID", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.TextField(_worldObject.worldID);
                EditorGUILayout.EndHorizontal();
            }
            
            _DrawHelpMessages();
            _DrawWarningMessages();
            m_MyOnInspectorGUI();
            base.OnInspectorGUI();
        }


        protected virtual void m_MyOnEnable()
        {

        }
        protected virtual void m_MyOnInspectorGUI()
        {

        }

        private void _CheckForWorldObjectGUID()
        {
            //Only generate the GUID id the object is not a prefab
            if (PrefabUtility.GetPrefabInstanceStatus(_worldObject.gameObject) == PrefabInstanceStatus.Connected)
            {
                if (_worldObject.HasGUID == false)
                {
                    _worldObject.GenerateGUID();
                    EditorUtility.SetDirty(_worldObject);
                    Debug.Log("Generating GUID");
                }
            }
            else if (_worldObject.HasGUID)
            {
                //Here we are in an original prefab
                Debug.Log("Erased GUID on original prefab object");
                _worldObject.EraseGUID();
            }
        }

        private void _DrawHelpMessages()
        {
            InspectorUtils.DrawInspectorMessages(_worldObject.GetInspectorHelpMessages());
        }

        private void _DrawWarningMessages()
        {
            InspectorUtils.DrawInspectorMessages(_worldObject.GetInspectorWarnings());
        }
    }
}
