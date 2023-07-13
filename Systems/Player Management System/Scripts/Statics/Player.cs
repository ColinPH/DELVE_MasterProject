using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PropellerCap.PlayerManager;

namespace PropellerCap
{
    public static class Player
    {
        static PlayerManager _playerManager;
        public static PlayerManager playerManager
        {
            get
            {
                if (_playerManager == null)
                    Debugger.LogError("PlayerManager has not been assigned in the static Player class. Check Initialization.");
                return _playerManager;
            }
            set
            {
                _playerManager = value;
            }
        }

        public static GameObject PlayerObject => _playerManager.ActivePlayerObject;
        public static PlayerCharacter ActivePlayer => _playerManager.ActivePlayer;

        #region Events

        public static PlayerEnteredLightHandler onLightEntered { get; set; }
        public static PlayerExitLightHandler onLightExit { get; set; }
        public static PlayerEnteredSanityAfflictor onSanityAfflictorEntered { get; set; }
        public static PlayerExitSanityAfflictor onSanityAfflictorExit { get; set; }

        #endregion
    }
}