using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activators/" + nameof(ActivationListener))]
    public class ActivationListener : Activator
    {
        [SerializeField] ActivationTarget _listenTo;
        [SerializeField] bool _listenToActivation = true;
        [SerializeField] bool _listenToDeactivation = true;

        public override string worldName => nameof(ActivationListener);
        protected override void MyAwake()
        {
            base.MyAwake();

            if (_listenTo == null)
                _listenTo = m_FetchForComponent<ActivationTarget>();

            if (_listenToActivation)
                _listenTo.RegisterToActivationComplete(_OnActivationComplete);

            if (_listenToDeactivation)
                _listenTo.RegisterToDeactivationComplete(_OnDeactivationComplete);
        }

        private void _OnActivationComplete()
        {
            m_ActivateTargets();
        }

        private void _OnDeactivationComplete()
        {
            m_DeactivateTargets();
        }
    }
}