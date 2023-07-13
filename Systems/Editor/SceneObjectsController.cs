using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public class SceneObjectsController : EditorWindow
    {


        [MenuItem("Propeller Cap/Tools/Objects Controller")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(SceneObjectsController));
            window.titleContent = new GUIContent("Objects Controller");
        }


        private void OnGUI()
        {
            if (GUILayout.Button("Generate all missing GUIDs"))
            {
                int amountGUIDsGenerated = 0;
                foreach (var item in FindObjectsOfType<WorldObject>())
                {
                    if (item.HasGUID == false)
                    {
                        amountGUIDsGenerated += 1;
                        item.GenerateGUID();
                        EditorUtility.SetDirty(item);
                    }
                }
                Debug.Log($"Generated ({amountGUIDsGenerated}) GUIDs.");
            }
        }

    }
}
