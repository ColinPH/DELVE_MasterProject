using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class CorridorLoader : MonoBehaviour
    {
        public string playerTag = "Player";
        public ActivationTarget targetDoor;
        public CorridorLoadingInfo loadingInfo = new CorridorLoadingInfo();
        public bool _waitForDoorActivation = false;
        private bool _hasBeenActivated = false;

        private void Awake()
        {
            loadingInfo.teleportReference = targetDoor.transform.position;
        }

        private void Start()
        {
            Collider coll = GetComponent<Collider>();
            if (coll == null)
            {
                Debug.LogWarning("There is no collider on object: " + gameObject.name);
            }

            if (coll.isTrigger == false)
                Debug.LogWarning("The collider is not a trigger on object: " + gameObject.name);

            if (targetDoor == null)
                Debug.LogWarning("There is no door assigned to Corridor loader " + gameObject.name);

            var interactable = targetDoor.GetComponent<IActivatable>();
            interactable.RegisterToActivationStart(_DoorActivationStart);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerTag != other.gameObject.tag) return;

            if (_hasBeenActivated || _waitForDoorActivation)
                return;
            _hasBeenActivated = true;

            if (_waitForDoorActivation == false)
                _LoadNextCorridor();
        }

        private void OnDestroy()
        {
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.SceneStart, _CorridorDoneLoading);
        }

        private void _DoorActivationStart()
        {
            if (_hasBeenActivated)
                return;
            _hasBeenActivated = true;

            _LoadNextCorridor();
        }

        private void _LoadNextCorridor()
        {
            Managers.eventManager.SubscribeToGameEvent(GameEvent.SceneStart, _CorridorDoneLoading);

            Managers.runManager.LoadNextCorridor(loadingInfo);
        }

        void _CorridorDoneLoading()
        {
            //Register the door to the corridor mnager so that it also closes
            LocalCorridorManager corridorManager = FindObjectOfType<LocalCorridorManager>();
            corridorManager.RegisterLevelDoor(targetDoor);

            //Open the door
            targetDoor.Activate();
        }
    }
}
