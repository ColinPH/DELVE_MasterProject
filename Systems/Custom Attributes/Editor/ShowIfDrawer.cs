using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = attribute as ShowIfAttribute;
            SerializedProperty boolProperty = property.serializedObject.FindProperty(showIf.showProperty);
            if (boolProperty == null)
                return EditorGUIUtility.singleLineHeight;

            //Here we draw the property based on a simple boolean
            if (showIf.isBoolProperty)
            {
                bool displayProperty = boolProperty.boolValue;

                if (showIf.isInverted)
                    displayProperty = !displayProperty;

                if (displayProperty)
                    return EditorGUI.GetPropertyHeight(property, label);
                else
                    return 0;
            }

            //Here we have to compare the value of the property against the target value to draw or not
            if (boolProperty.intValue == showIf.targetValue)
                return EditorGUI.GetPropertyHeight(property, label);
            else
                return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = attribute as ShowIfAttribute;
            SerializedProperty boolProperty = property.serializedObject.FindProperty(showIf.showProperty);

            if (boolProperty == null)
                return;

            if (showIf.isBoolProperty)
            {
                bool displayProperty = boolProperty.boolValue;

                if (showIf.isInverted)
                    displayProperty = !displayProperty;

                if (displayProperty)
                    EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            if (boolProperty.intValue == showIf.targetValue)
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
