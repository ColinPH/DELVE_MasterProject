using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsProjectileMover : ProjectileMoverBase
    {
        [SerializeField] float startVelocity = 20f;

        Rigidbody _rigidbody;

        protected override void m_InitializeProjectileMover()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        protected override void m_StartMoving(Vector3 direction)
        {
            _rigidbody.AddForce(direction * startVelocity, ForceMode.VelocityChange);
        }
        protected override void m_StopMoving()
        {

        }
    }
}