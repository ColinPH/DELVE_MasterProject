using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(DetectionParent))]
    [CanEditMultipleObjects]
    public class DetectionParentInInspector : WorldObjectInInspector
    {
        DetectionParent _detectionParent;

        protected override void m_MyOnEnable()
        {
            base.m_MyOnEnable();
            _detectionParent = (DetectionParent)target;

        }
        protected override void m_MyOnInspectorGUI()
        {
            base.m_MyOnInspectorGUI();
        }
    }
}