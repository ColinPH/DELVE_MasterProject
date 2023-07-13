using PropellerCap.QA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public class InitializationManager : MonoBehaviour
    {
        public static InitializationManager instance { get; private set; }

        [Header("Managers pool")]
        [SerializeField] ManagersInitializationPool initializationPool;
        [Header("Runtime managers")]
        public List<ManagerBase> managers = new List<ManagerBase>();

        private bool _gameInitialized = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Managers.initializationManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (_gameInitialized) return;

            Debug.Log("This should only be called if starting from root.");

            StartGameInitialization(initializationPool);

            //Start the game
            Managers.gameManager.StartGame();
        }

        /// <summary> Should be the very first methodof the game. It instantiates the manager prefabs and calls Init, Mywake and MyStart on managers. </summary>
        public void StartGameInitialization(ManagersInitializationPool pool)
        {
            _gameInitialized = true;
            Debug.Log("Game Initialization has started.");

            //If a pool has been given it meant that it is a standalone build and we therefore instantiate the managers
            if (pool != null)
            {
                foreach (var item in pool.essentialManagers)
                {
                    //Skip initialization manager because it should already be instantiated
                    if (item.GetComponent<InitializationManager>() != null)
                        continue;

                    ManagerBase m;
                    if (item.GetComponent<ManagerBase>().AlreadyExistsInScene(out m) == false)
                    {
                        m = Instantiate(item).GetComponent<ManagerBase>();
                    }
                    
                    managers.Add(m);
                }
            }
            else
            {
#if UNITY_EDITOR
                managers = _FindManagersInOrder();
#endif
            }

            foreach (ManagerBase item in managers)
            {
                item.Init();
            }

            //Assign the managers to the game manager
            Managers.gameManager.SetManagers(managers);

            foreach (ManagerBase item in managers)
            {
                item.MyAwake();
            }

            foreach (ManagerBase item in managers)
            {
                item.MyStart();
            }
            Debug.Log("Game Initialization Finished.");
        }

#if UNITY_EDITOR
        private static List<ManagerBase> _FindManagersInOrder()
        {
            List<ManagerBase> toReturn = new List<ManagerBase>();
            ManagersInitializationPool pool = PlayButtonInitialization.FindManagersInitializationPool();
            foreach (var item in pool.essentialManagers)
            {
                ManagerBase m;
                item.GetComponent<ManagerBase>().AlreadyExistsInScene(out m);
                toReturn.Add(m);
            }
            return toReturn;
        }
#endif
    }
}
