using Codice.Client.BaseCommands;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(Dismantle))]
    [CanEditMultipleObjects]
    public class DismantleInInspector : Editor
    {
        Dismantle _dismantle;

        private void OnEnable()
        {
            _dismantle = (Dismantle)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }    
}