using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    public abstract class ProjectileBase : WorldObject
    {
        [HideInInspector] public GameObject callingObject;

        private bool _initialized = false;

        public void InitializeProjectile(GameObject callingObject)
        {
            this.callingObject = callingObject;
            m_OnProjectileInitialization(callingObject);
            _initialized = true;
        }

        protected override void MyStart()
        {
            base.MyStart();
            if (_initialized == false)
            {
                InitializeProjectile(gameObject);
            }
        }

        /// <summary>
        /// Starts the movement of the projectile along the given trajectory
        /// </summary>
        public void LaunchProjectile(Vector3 direction)
        {
            m_OnProjectileLaunch(direction);
        }

        public void OnProjectileTrajectoryEnded()
        {
            m_OnProjectileArrived();
        }

        protected virtual void m_OnProjectileInitialization(GameObject callingObject)
        {

        }

        protected virtual void m_OnProjectileLaunch(Vector3 direction)
        {

        }

        protected virtual void m_OnProjectileArrived()
        {

        }
    }
}