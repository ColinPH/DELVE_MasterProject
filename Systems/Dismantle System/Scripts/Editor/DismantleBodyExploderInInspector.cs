using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace PropellerCap
{
    [CustomEditor(typeof(BodyExploder))]
    [CanEditMultipleObjects]
    public class DismantleBodyExploderInInspector : Editor
    {
        BodyExploder _bodyEploder;

        private void OnEnable()
        {
            _bodyEploder = (BodyExploder)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}