using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomPropertyDrawer(typeof(RuntimeAttribute))]
    public class RuntimeInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Show the property if the editor is in playmode
            if (Application.isPlaying)
            {
                RuntimeAttribute runtime = attribute as RuntimeAttribute;
                if (runtime.hasHeader)
                {
                    string headerText = "/!\\ Runtime Information /!\\";
                    GUIStyle headerStyle = EditorStyles.boldLabel;
                    headerStyle.alignment = TextAnchor.MiddleCenter;
                    headerStyle.fontSize = 14;
                    float headerWidth = headerStyle.CalcSize(new GUIContent(headerText)).x;
                    float centerX = position.x + (position.width / 2) - (headerWidth / 2);
                    GUI.Label(new Rect(centerX, position.y, headerWidth, EditorGUIUtility.singleLineHeight), headerText, headerStyle);
                    position.yMin += EditorGUIUtility.singleLineHeight;
                }
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //Show the property if the editor is in playmode
            if (Application.isPlaying)
            {
                float toReturn = 0f;
                RuntimeAttribute runtime = attribute as RuntimeAttribute;
                if (runtime.hasHeader)
                {
                    toReturn += EditorGUIUtility.singleLineHeight;
                }
                toReturn += EditorGUI.GetPropertyHeight(property, label);
                return toReturn;
            }
            else
                return 0f;
        }
    }
}
