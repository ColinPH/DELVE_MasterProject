using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public static class InspectorUtils
    {
        public static void DrawInspectorMessages(List<InspectorMessage> messages)
        {
            foreach (var item in messages)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(item.text, item.messageType.Convert());
                if (item.hasFixAction)
                {
                    GUILayoutOption[] buttonOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.wordWrap = true;
                    if (GUILayout.Button(item.errorFixButtonText, buttonStyle, buttonOptions))
                    {
                        item.errorFixAction?.Invoke();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
