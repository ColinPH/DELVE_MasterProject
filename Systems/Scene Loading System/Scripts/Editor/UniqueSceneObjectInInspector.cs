using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(UniqueSceneObject))]
    [CanEditMultipleObjects]
    public class UniqueSceneObjectInInspector : LoadableObjectInInspector
    {
        UniqueSceneObject _uniqueSceneObject;

        private void OnEnable()
        {
            _uniqueSceneObject = (UniqueSceneObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
        public override void ConvertSceneObjectsToNameList()
        {
            _uniqueSceneObject.ConvertSceneObjectsToNames();
        }
    }
}