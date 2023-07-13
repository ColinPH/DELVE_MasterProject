using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

namespace PropellerCap.EditorUsability
{
#if UNITY_EDITOR
    public class FindManagers
    {
        [MenuItem("Propeller Cap/Managers/Initialization Manager")]
        public static void FindInitializationManager()
        {
            //Put the asset in the selection
            Selection.objects = new Object[] { FindManagerOfType<InitializationManager>() };
            //Warning if the inspector window is locked
            _CheckInspectorLock();

        }

        [MenuItem("Propeller Cap/Managers/Debugger Manager")]
        public static void FindDebuggerManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<DebuggerManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Game Manager")]
        public static void FindGameManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<GameManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Event Manager")]
        public static void FindEventManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<EventManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Scene Loader")]
        public static void FindSceneLoader()
        {
            Selection.objects = new Object[] { FindManagerOfType<SceneLoader>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Sound Manager")]
        public static void FindSoundManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<SoundManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Sanity Manager")]
        public static void FindSanityManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<SanityManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Totem Manager")]
        public static void FindTotemManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<TotemManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Run Manager")]
        public static void FindRunManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<RunManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Player Manager")]
        public static void FindPlayerManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<PlayerManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/UI Manager")]
        public static void FindUIManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<UIManager>() };
            _CheckInspectorLock();
        }

        [MenuItem("Propeller Cap/Managers/Localization Manager")]
        public static void FindLocalizationManager()
        {
            Selection.objects = new Object[] { FindManagerOfType<LocalizationManager>() };
            _CheckInspectorLock();
        }

        private static Object FindManagerOfType<T>()
        {
            List<Object> settingsAssets = new List<Object>();

            string[] guids = AssetDatabase.FindAssets("l:Manager");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                T component = go.GetComponent<T>();
                if (component != null)
                {
                    settingsAssets.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
                }
            }

            if (settingsAssets.Count > 1)
            {
                Debug.LogError("There is more than one " + typeof(T) + " manager prefab in the assets. Search for " + settingsAssets[1].name);
            }
            else if (settingsAssets.Count == 0)
            {
                Debug.LogWarning("No " + typeof(T) + " manager prefab could be found in the assets. Make sure it has been labeled \"Manager\".");
            }

            return settingsAssets[0];
        }

        private static void _CheckInspectorLock()
        {
            //This has been found here:
            //https://answers.unity.com/questions/348791/check-to-see-if-inspector-is-locked.html
            System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow window = EditorWindow.GetWindow(type);
            PropertyInfo info = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
            
            if ((bool)info.GetValue(window, null))
            {
                Debug.LogWarning("The inspector window is locked, make sure to have one which is not locked if you want to access the settings.");
            }
        }
    }
#endif
}
