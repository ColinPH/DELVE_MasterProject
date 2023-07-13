using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class ProjectileMoverBase : MonoBehaviour
    {
        protected ProjectileBase m_projectileBase;

        private void Awake()
        {
            InitializeProjectileMover();
        }
        private void FixedUpdate()
        {
            m_FixedUpdate();
        }

        public void InitializeProjectileMover()
        {
            m_projectileBase = GetComponent<ProjectileBase>();
            m_InitializeProjectileMover();
        }

        public void StartMoving(Vector3 direction)
        {
            m_StartMoving(direction);
        }

        public void StopMoving()
        {
            m_StopMoving();
        }


        protected virtual void m_InitializeProjectileMover()
        {

        }
        protected virtual void m_FixedUpdate()
        {

        }
        protected virtual void m_StartMoving(Vector3 direction)
        {

        }
        protected virtual void m_StopMoving()
        {

        }
    }
}