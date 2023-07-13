using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    [CustomEditor(typeof(LoadableObjectGroup))]
    [CanEditMultipleObjects]
    public class LevelFamilyInInspector : Editor
    {
        LoadableObjectGroup _levelFamily;

        private void OnEnable()
        {
            _levelFamily = (LoadableObjectGroup)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("A LevelFamily asset groups multiple levels together to follow a progression." +
                " Each level contains 1 or more scenes to be loaded when loading that level." +
                "\n \n A LevelFamily is different from a SceneFamily because of the progression." +
                " In a SceneFamily asset all scenes are loaded in sequence once.", MessageType.Info);

            string wrongAssetName;
            if (_levelFamily.GroupIsValid(out wrongAssetName) == false)
            {
                EditorGUILayout.HelpBox("Asset must contain only Level assets." +
                    " The following asset is not a Level asset : " + wrongAssetName, MessageType.Error);
            }
            base.OnInspectorGUI();
        }
    }
}