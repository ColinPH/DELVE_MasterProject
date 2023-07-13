using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PropellerCap
{
    public class LevelSectionLoader : MonoBehaviour
    {
        [SerializeField] LoadableObject _parentLoadable;
        [SerializeField] string _levelToLoadSceneName = "";
        [SerializeField] string _playerTag = "Player";

        private bool _hasBeenActivated = false;


        private void Start()
        {
            Collider coll = GetComponent<Collider>();
            if (coll == null)
            {
                Debug.LogWarning("There is no collider on object: " + gameObject.name);
            }

            if (coll.isTrigger == false)
                Debug.LogWarning("The collider is not a trigger on object: " + gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerTag != other.gameObject.tag) return;

            if (_hasBeenActivated)
                return;

            Managers.sceneLoader.LoadLevelSection(_levelToLoadSceneName, _parentLoadable);
        }
    }
}
