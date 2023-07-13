using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activators/" + nameof(TriggerDeactivator))]
    public class TriggerDeactivator : Activator
    {
        public string _targetTag = "Player";
        public bool _singleActivation = true;

        private bool _hasBeenActivated = false;

        #region Mybehaviour overrides
        public override string worldName => nameof(TriggerDeactivator);
        protected override void MyAwake()
        {
            base.MyAwake();
            m_EnsureGameObjectHasTrigger();
        }
        #endregion Mybehaviour overrides

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                if (_hasBeenActivated && _singleActivation)
                    return;
                _hasBeenActivated = true;

                m_DeactivateTargets();
            }
        }
    }
}
