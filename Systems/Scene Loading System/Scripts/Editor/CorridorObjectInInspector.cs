using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(CorridorObject))]
    [CanEditMultipleObjects]
    public class CorridorObjectInInspector : LoadableObjectInInspector
    {
        CorridorObject _corridorObject;

        private void OnEnable()
        {
            _corridorObject = (CorridorObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
        public override void ConvertSceneObjectsToNameList()
        {
            _corridorObject.ConvertSceneObjectsToNames();
        }
    }
}