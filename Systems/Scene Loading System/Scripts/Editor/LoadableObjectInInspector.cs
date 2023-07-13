using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(LoadableObject))]
    [CanEditMultipleObjects]
    public class LoadableObjectInInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Convert scene objects to names"))
            {
                ConvertSceneObjectsToNameList();
                EditorUtility.SetDirty(target);
            }

            base.OnInspectorGUI();
        }

        public virtual void ConvertSceneObjectsToNameList()
        {

        }
    }
}