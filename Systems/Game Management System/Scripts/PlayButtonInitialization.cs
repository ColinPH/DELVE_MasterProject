using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PropellerCap
{
#if UNITY_EDITOR
    public class PlayButtonInitialization
    {
        //This code will execute right after the play button
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnGameStart()
        {
            ManagersInitializationPool pool = FindManagersInitializationPool();

            //Find the initialization manager
            foreach (var item in pool.essentialManagers)
            {
                if (item.GetComponent<InitializationManager>())
                {
                    if (GameObject.FindObjectOfType<InitializationManager>() == null)
                        GameObject.Instantiate(item);
                    break;
                }
            }

            Debug.Log("Game initialized before play button.");

            InitializationManager initializationManager = GameObject.FindObjectOfType<InitializationManager>();
            initializationManager.StartGameInitialization(pool);

            Managers.gameManager.StartGame();
        }

        public static ManagersInitializationPool FindManagersInitializationPool()
        {
            string[] results = AssetDatabase.FindAssets("t:" + typeof(ManagersInitializationPool));
            if (results.Length > 1)
            {
                Debug.LogError("There are multiple assets of type " + typeof(ManagersInitializationPool) + ". Make sure that there is only one.");
            }

            string poolPath = "";
            foreach (string guid in results)
            {
                poolPath = AssetDatabase.GUIDToAssetPath(guid);
                break;
            }

            return (ManagersInitializationPool)AssetDatabase.LoadAssetAtPath(poolPath, typeof(ManagersInitializationPool));
        }
    }
#endif
}
