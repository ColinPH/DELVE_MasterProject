using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activators/" + nameof(PressurePlate))]
    public class PressurePlate : Activator
    {

        public string _targetTag = "Player";

        #region Mybehaviour overrides
        public override string worldName => nameof(PressurePlate);
        #endregion Mybehaviour overrides

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                m_InteractionStart(null);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                m_InteractionStop();
            }
        }

        protected override void m_InteractionStart(Action onInteractionCancelled)
        {
            base.m_InteractionStart(onInteractionCancelled);
            foreach (ActivationTarget item in m_activationTargets)
            {
                item.Activate();
            }
        }


        protected override void m_InteractionStop()
        {
            base.m_InteractionStop();
            foreach (ActivationTarget item in m_activationTargets)
            {
                item.Deactivate();
            }
        }
    }
}