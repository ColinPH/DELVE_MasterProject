using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Light and Sanity/" + nameof(SanityAfflictor))]
    public class SanityAfflictor : WorldObject
    {
        public float sanityMultiplier = 1f;
        /// <summary> Identifier for user and help debugging. </summary>
        public string identifier = "";

        string _uniqueIdentifier = "";

        /// <summary> Unique identifier using GUID. </summary>
        public string UniqueIdentifier => _uniqueIdentifier;

        protected override void MonoAwake()
        {
            base.MonoAwake();
            _uniqueIdentifier = Guid.NewGuid().ToString("N");
        }

        protected override void MyAwake()
        {
            base.MyAwake();
            m_EnsureGameObjectHasTrigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            ISanityReceiver receiver = other.gameObject.GetComponent<ISanityReceiver>();
            if (receiver != null)
            {
                receiver.IApplySanityEffect(gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ISanityReceiver receiver = other.gameObject.GetComponent<ISanityReceiver>();
            if (receiver != null)
            {
                receiver.IRemoveSanityEffect(gameObject);
            }
        }
    }
}