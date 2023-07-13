using System;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(ATLinearController))]
    [CanEditMultipleObjects]
    public class ATLinearControllerInInspector : Editor
    {
        ATLinearController targetObj;

        string editObjectPositionButtonText = "Edit Object Position";
        bool isEditingObjectPosition = false;

        private void OnEnable()
        {
            targetObj = (ATLinearController)target;
        }

        public override void OnInspectorGUI()
        {
            //Show the ActivationComplete and DeactivationComplete UnityEvents
            /*serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onActivationComplete"), includeChildren: true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onDeactivationComplete"), includeChildren: true);
            serializedObject.ApplyModifiedProperties();*/

            base.OnInspectorGUI();

            if (isEditingObjectPosition == false)
            {
                GUIContent content = new GUIContent("Position Slider", "Position of the object between the start and end position.");
                float newValue = EditorGUILayout.Slider(content, targetObj.lerpPos, 0f, 1f);
                if (newValue != targetObj.lerpPos)
                {
                    targetObj.lerpPos = newValue;
                    targetObj.lerpRot = newValue;
                    targetObj.UpdateObjectPosition();
                }
            }

            if (GUILayout.Button(editObjectPositionButtonText))
            {
                isEditingObjectPosition = !isEditingObjectPosition;

                if (isEditingObjectPosition)
                {
                    //Here we are editing
                    editObjectPositionButtonText = "Stop Editing Object Position";

                    targetObj.startHandleOffset = targetObj.startWorldPosition - targetObj.transform.position;
                    targetObj.endHandleOffset = targetObj.endWorldPosition - targetObj.transform.position;

                    targetObj.startHandleOffsetROT = targetObj.startRotation * Quaternion.Inverse(targetObj.transform.rotation);
                    targetObj.endHandleOffsetROT = targetObj.endRotation * Quaternion.Inverse(targetObj.transform.rotation);
                }
                else
                {
                    //Here we are not editing
                    editObjectPositionButtonText = "Edit Object Position";
                }
            }
        }

        private void OnSceneGUI()
        {
            if (isEditingObjectPosition)
            {
                MakeHandlesFollowObject();
                DrawStartPositionHandle(false);
                DrawEndPositionHandle(false);
            }
            else
            {
                DrawStartPositionHandle(true);
                DrawEndPositionHandle(true);
                targetObj.UpdateObjectPosition();
            }
        }

        private void MakeHandlesFollowObject()
        {
            targetObj.startWorldPosition = targetObj.transform.position + targetObj.startHandleOffset;
            targetObj.endWorldPosition = targetObj.transform.position + targetObj.endHandleOffset;

            targetObj.startRotation = targetObj.transform.rotation * targetObj.startHandleOffsetROT;
            targetObj.endRotation = targetObj.transform.rotation * targetObj.endHandleOffsetROT;
        }

        private void DrawStartPositionHandle(bool hasControls)
        {
            if (hasControls)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(targetObj.startWorldPosition, Quaternion.identity);
                Quaternion newRot = Handles.RotationHandle(targetObj.startRotation, newPos);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetObj, "Changed Start Position");
                    targetObj.startWorldPosition = newPos;
                    targetObj.startRotation = newRot;
                }
            }
            else
            {
                Handles.color = Color.blue;
                Handles.SphereHandleCap(
                    0,
                    targetObj.startWorldPosition,
                    Quaternion.identity,
                    0.2f,
                    EventType.Repaint
                );
            }
            Handles.Label(targetObj.startWorldPosition, "Start Position");
        }

        private void DrawEndPositionHandle(bool hasControls)
        {
            if (hasControls)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(targetObj.endWorldPosition, Quaternion.identity);
                Quaternion newRot = Handles.RotationHandle(targetObj.endRotation, newPos);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetObj, "Changed End Position");
                    targetObj.endWorldPosition = newPos;
                    targetObj.endRotation = newRot;
                }
            }
            else
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(
                    0,
                    targetObj.endWorldPosition,
                    Quaternion.identity,
                    0.2f,
                    EventType.Repaint
                );
            }
            Handles.Label(targetObj.endWorldPosition, "End Position");
        }
    }
}