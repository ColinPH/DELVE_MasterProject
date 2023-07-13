using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PropellerCap
{
    [CustomEditor(typeof(Brazier))]
    [CanEditMultipleObjects]
    public class BrazierInInspector : WorldObjectInInspector
    {
        Brazier _brazier;

        protected override void m_MyOnEnable()
        {
            base.m_MyOnEnable();
            _brazier = (Brazier)target;
        }

        protected override void m_MyOnInspectorGUI()
        {
            base.m_MyOnInspectorGUI();
            if (Application.isPlaying == false)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set Fire Light"))
                {
                    _brazier.Inspector_SetLightType(LightType.Fire);
                }
                if (GUILayout.Button("Set Crystal Light"))
                {
                    _brazier.Inspector_SetLightType(LightType.Crystal);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
