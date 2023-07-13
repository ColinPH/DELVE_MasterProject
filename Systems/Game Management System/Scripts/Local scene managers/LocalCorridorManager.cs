using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    public class LocalCorridorManager : LocalManager
    {
        [SerializeField] string _managerName = "Default_Corridor_Manager";
        [SerializeField] bool _restartRunOnPlayerDeath = true;
        [SerializeField] GameObject _corridorObjectToMove;
        [SerializeField] bool _isHubCorridor = false;
        [Header("Doors and Level Loading")]
        [SerializeField] LevelUnloader _levelUnloader;
        [SerializeField] LevelLoader _levelLoader;
        [SerializeField] ActivationTarget _entryDoor;
        [SerializeField] ActivationTarget _exitDoor;
        [Header("Corridor teleportation")]
        [SerializeField] Vector3 _spawnOffset = Vector3.zero;
        [Header("Manager Actions")]
        [SerializeField] List<LocalManagerAction> initializationActionList = new List<LocalManagerAction>();

        CorridorLoadingInfo _loadingInfo;

        ActivationTarget _levelDoor;

        public override string LocalManagerName => _managerName;

        protected override void LocalManagerInitialization(object loadingInfo)
        {
            Debugger.LogInit("Local level manager initialized. " + name);

            //If the scene is already loaded in editor this will not receive loading settings
            if (loadingInfo == null)
                _loadingInfo = new CorridorLoadingInfo();
            else
                _loadingInfo = (CorridorLoadingInfo)loadingInfo;

            //Debug.Log("Corridor loading settings : " + _loadingInfo.corridorPosition);

            //Invoke all the manager actions in order
            foreach (var item in initializationActionList)
            {
                item.InvokeLocalManagerAction(this);
            }

            _TeleportCorridorToDestination();
            Managers.eventManager.SubscribeToGameEvent(GameEvent.PlayerDied, _OnPlayerDeath);

            if (_isHubCorridor)
            {
                Metrics.manager.CloseLevelData();
                Metrics.manager.OpenLevelData(_managerName);
            }
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.PlayerDied, _OnPlayerDeath);
        }


        #region Levels loading and door controls
        /// <summary> Closes the entry door, unloads old level and loads new one, then opens exit door. </summary>
        public void StartLoadingProcess()
        {
            _entryDoor.RegisterToDeactivationComplete(_OnEntryDoorClosed);
            //Close the corridor entry door andlevel exit door
            _entryDoor.Deactivate();
            _levelDoor.Deactivate();
        }

        private void _OnEntryDoorClosed()
        {
            _levelDoor = null;
            StartCoroutine(_Co_LoadingProcess());
        }

        IEnumerator _Co_LoadingProcess()
        {
            yield return StartCoroutine(_levelUnloader.Co_UnloadPreviousLevel());

            if (_isHubCorridor)
                yield break;

            TeleportCorridorBackToOrigin(Player.PlayerObject);
            yield return StartCoroutine(_levelLoader.Co_LoadNextLevel());
        }

        /// <summary> Provide the door that has been used to load the corridor. And opens the corridor entry door that matches the one from the level. </summary>
        public void RegisterLevelDoor(ActivationTarget door)
        {
            _levelDoor = door;

            //Also open the coridor entry door
            _entryDoor.Activate();
        }
        #endregion Levels loading and door controls


        void _OnPlayerDeath()
        {
            if (_restartRunOnPlayerDeath)
            {
                Managers.runManager.FailRun();
            }
            else
            {
                //Teleport the player to the spawn point
                StartCoroutine(Managers.playerManager.Co_RespawnPlayer());
            }
        }

        private void _TeleportCorridorToDestination()
        {
            if (_isHubCorridor)
                return;

            if (_corridorObjectToMove!= null)
            {
                _corridorObjectToMove.transform.position = _loadingInfo.teleportReference + _spawnOffset;
            }
            else
            {
                Debug.LogError("Corridor object to move has not been assigned.");
            }
        }

        public void TeleportCorridorBackToOrigin(GameObject playerObject)
        {
            if (_isHubCorridor)
                return;

            if (_corridorObjectToMove != null)
            {
                playerObject.transform.SetParent(_corridorObjectToMove.transform);
                _corridorObjectToMove.transform.position = Vector3.zero;
                playerObject.transform.SetParent(null);
                DontDestroyOnLoad(playerObject);
            }
            else
            {
                Debug.LogError("Corridor object to move has not been assigned.");
            }
        }
    }
}