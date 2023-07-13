using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SpawnPoint : WorldObject
    {
        //[SerializeField] bool _raycastForGround = false;


        protected bool m_pointIsActive = true;

        public virtual bool PointIsValid()
        {
            return m_pointIsActive;
        }

        public virtual Vector3 GetSpawnPosition()
        {
            return transform.position;
        }

        public virtual Quaternion GetSpawnRotation()
        {
            return transform.rotation;
        }
    }
}