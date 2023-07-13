using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LocalLevelManager : LocalManager
    {
        [SerializeField] string _managerName = "Default_Level_Manager";
        [SerializeField] bool _restartRunOnPlayerDeath = true;
        [Header("Doors and Corridor Loading")]
        [SerializeField] ActivationTarget _entryDoor;
        [SerializeField] CorridorUnloader _corridorUnloader;
        [SerializeField] List<LocalManagerAction> initializationActionList = new List<LocalManagerAction>();

        LevelLoadingInfo _loadingInfo;
        ActivationTarget _corridorDoor;


        public override string LocalManagerName => _managerName;

        protected override void LocalManagerInitialization(object loadingInfo)
        {
            Debugger.LogInit("Local level manager initialized.");

            //If the scene is already loaded in editor this will not receive loading settings
            if (loadingInfo == null)
                _loadingInfo = new LevelLoadingInfo();
            else
                _loadingInfo = (LevelLoadingInfo)loadingInfo;

            //Invoke all the manager actions in order
            foreach (var item in initializationActionList)
            {
                item.InvokeLocalManagerAction(this);
            }

            Managers.eventManager.SubscribeToGameEvent(GameEvent.PlayerDied, _OnPlayerDeath);

            //Start the level data collection
            Metrics.manager.CloseLevelData();
            Metrics.manager.OpenLevelData(_managerName);
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.PlayerDied, _OnPlayerDeath);
        }


        #region Corridor unloading and door controls
        /// <summary> Provide the door that has been used to load the level. And opens the level entry door that matches the one from the corridor. </summary>
        public void RegisterCorridorExitDoor(ActivationTarget door)
        {
            _corridorDoor = door;
        }
        /// <summary> Also opens the Corridor exit door. </summary>
        public void OpenLevelEntryDoorAndCorridorExitDoor()
        {
            //Open the corridor exit door and level entry door
            _entryDoor.Activate();
            _corridorDoor.Activate();
        }

        /// <summary> Triggered by the Corridor unloader when the player goes inside its trigger. Closes and doors and unloads the corridor. </summary>
        public void CloseEntryDoor()
        {
            _entryDoor.RegisterToDeactivationComplete(_OnEntryDoorClosed);
            _entryDoor.Deactivate();
            _corridorDoor.Deactivate();
        }
        private void _OnEntryDoorClosed()
        {
            _corridorDoor = null;
            _corridorUnloader.UnloadPreviousCorridor();
        }
        #endregion Corridor unloading and door controls

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
                Metrics.manager.CloseLevelData();
                Metrics.manager.CloseRunData();
                if (Managers.runManager.ActiveRunObject != null)
                    Metrics.manager.OpenRunData(Managers.runManager.ActiveRunObject.RunName);
                else
                    Metrics.manager.OpenRunData("Missing active runobject probably tutorial");
                Metrics.manager.OpenLevelData(_managerName);
            }
        }

        public void FailRun()
        {
            Managers.runManager.FailRun();
        }
    }
}