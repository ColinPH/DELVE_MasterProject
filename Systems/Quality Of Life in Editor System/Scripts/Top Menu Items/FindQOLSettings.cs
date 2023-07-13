using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class FindQOLSettings
    {
        [MenuItem("Propeller Cap/Settings/QOL Settings/Edit QOL Settings")]
        public static void PlayFromRoot()
        {
            //Find the asset
            Object settingsAsset = Utils.GetAssetOfType<QOLSettings>();

            if (settingsAsset == null)
            {
                Debug.Log("There is no " + nameof(QOLSettings) + " asset in the project. Create one by going Right Click/Create/Scriptable Objects/Settings.");
                return;
            }

            //Put the asset in the selection
            Selection.objects = new Object[] { settingsAsset };

            //Warning if the inspector window is locked
            if (_InspectorIsLock())
                Debug.LogWarning("The inspector window is locked, make sure to have one which is not locked if you want to access the settings.");
        }

        private static bool _InspectorIsLock()
        {
            //This has been found here:
            //https://answers.unity.com/questions/348791/check-to-see-if-inspector-is-locked.html
            System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow window = EditorWindow.GetWindow(type);
            PropertyInfo info = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
            return (bool)info.GetValue(window, null);
        }
    }
}