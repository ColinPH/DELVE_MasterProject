using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(PropertyControlOptions))]
    [CanEditMultipleObjects]
    public class PropertyControlOptionsInInspector : Editor
    {
        /*PropertyControlOptions controlOptions;

        private void OnEnable()
        {
            controlOptions = (PropertyControlOptions)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }*/
    }
}
