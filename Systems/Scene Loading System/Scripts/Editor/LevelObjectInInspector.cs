using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(LevelObject))]
    [CanEditMultipleObjects]
    public class LevelObjectInInspector : LoadableObjectInInspector
    {
        LevelObject _levelObject;

        private void OnEnable()
        {
            _levelObject = (LevelObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
        public override void ConvertSceneObjectsToNameList()
        {
            _levelObject.ConvertSceneObjectsToNames();
        }
    }
}