using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LevelLoader : MonoBehaviour
    {
        public ActivationTarget _exitDoor;
        public LevelLoadingInfo _levelSettings;

        private bool _hasBeenActivated = false;

        private void Start()
        {
            if (_exitDoor == null)
                Debug.LogWarning("There is no door assigned to Level Loader " + gameObject.name);
        }

        /*private void OnDestroy()
        {
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.SceneStart, _LevelDoneLoading);
        }

        public void LoadNextLevel()
        {
            if (_hasBeenActivated) return;
            _hasBeenActivated = true;

            Managers.eventManager.SubscribeToGameEvent(GameEvent.SceneStart, _LevelDoneLoading);
            Managers.runManager.LoadNextLevel(_levelSettings);
        }*/

        public IEnumerator Co_LoadNextLevel()
        {
            if (_hasBeenActivated) yield break;
            _hasBeenActivated = true;

            yield return StartCoroutine(Managers.runManager.Co_LoadNextLevel(_levelSettings));

            //Register the door to the newly loaded level manager
            LocalLevelManager levelManager = FindObjectOfType<LocalLevelManager>();
            levelManager.RegisterCorridorExitDoor(_exitDoor);

            //Start the process of opening the doors
            levelManager.OpenLevelEntryDoorAndCorridorExitDoor();
        }
    }
}
