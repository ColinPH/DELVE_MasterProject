using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class RunStarter : MonoBehaviour
    {
        public string _targetTag = "Player";
        public ActivationTarget _exitDoor;
        public bool _unloadDisposableScenes = false;
        public RunObject _runObject;
        private bool _hasBeenActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                StartRun();
            }
        }

        public void StartRun()
        {
            if (_unloadDisposableScenes)
            {
                //If we dispose of the current scenes, the scene this runstarter is in could be unloaded which will stop the coroutine
                //Therefore we start the coroutine on the manager object instead using the void method
                Managers.runManager.StartRun(_runObject, _unloadDisposableScenes, true);
            }
            else
            {
                StartCoroutine(Co_LoadNextLevel());
            }
        }

        public IEnumerator Co_LoadNextLevel()
        {
            if (_hasBeenActivated) yield break;
            _hasBeenActivated = true;

            yield return StartCoroutine(Managers.runManager.Co_StartRun(_runObject, _unloadDisposableScenes));

            //Register the door to the newly loaded level manager
            LocalLevelManager levelManager = FindObjectOfType<LocalLevelManager>();
            levelManager.RegisterCorridorExitDoor(_exitDoor);

            //Start the process of opening the doors
            levelManager.OpenLevelEntryDoorAndCorridorExitDoor();
        }
    }
}
