using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class MAPlayerSpawner : LocalManagerAction
    {
        public bool teleportPlayerToSpawn = false;
        public PlayerSpawnPoint _playerSpawn;
        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            base.InvokeLocalManagerAction(localManager);

            if (_playerSpawn != null)
            {
                _playerSpawn.RegisterSpawnPoint();
            }

            SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            Debugger.LogLocalMAction("PlayerSpawner checking for player spawn.");

            if (Player.PlayerObject == null)
            {
                //If there is no player object, spawn one
                Managers.playerManager.SpawnPlayer();
                return;
            }

            //Here there is a player in the scene

            if (teleportPlayerToSpawn)
            {
                Managers.playerManager.SpawnPlayer();
            }
        }
    }
}
