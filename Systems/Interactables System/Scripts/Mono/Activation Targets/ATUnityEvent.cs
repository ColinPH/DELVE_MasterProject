using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/AT Unity Event")]
    public class ATUnityEvent : ActivationTarget
    {
        [SerializeField] UnityEvent _onActivation;
        [SerializeField] UnityEvent _onDeactivation;

        #region ActivationTarget overrides

        public override void Activate()
        {
            _onActivation?.Invoke();
        }

        public override void Deactivate()
        {
            _onDeactivation?.Invoke();
        }

        #endregion
    }
}