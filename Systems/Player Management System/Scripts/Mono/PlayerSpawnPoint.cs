using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class PlayerSpawnPoint : SpawnPoint
    {
        public bool isMainSpawnPoint = true;

        public override string worldName => nameof(PlayerSpawnPoint);
        protected override void PreLocalManagerInitialization()
        {
            base.PreLocalManagerInitialization();
            Managers.playerManager.RegisterPlayerSpawnPoint(this);
        }

        /// <summary> This should not exist, it's a quick fix. </summary>
        public void RegisterSpawnPoint()
        {
            Managers.playerManager.RegisterPlayerSpawnPoint(this);
        }

        protected override void MonoDestroy()
        {
            Managers.playerManager.DeregisterPlayerSpawnPoint(this);
        }

        public override bool PointIsValid()
        {
            return base.PointIsValid();
        }

        public Vector3 GetPlayerSpawnPosition()
        {
            return GetSpawnPosition();
        }

        public Quaternion GetPlayerSpawnRotation()
        {
            return GetSpawnRotation();
        }
    }
}
