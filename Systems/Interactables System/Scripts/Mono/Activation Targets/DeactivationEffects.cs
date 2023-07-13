using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(DeactivationEffects))]
    public class DeactivationEffects : WorldObject
    {
        [SerializeField] ActivationTarget _activationTarget;
        [SerializeField] SoundClip _deactivationSound;
        [SerializeField] VisualEffectClip _deactivationVFX;

        public override string worldName => nameof(DeactivationEffects);
        protected override void MyAwake()
        {
            base.MyAwake();
            if (_activationTarget == null)  
                _activationTarget = m_FetchForComponent<ActivationTarget>();
            _activationTarget.RegisterToDeactivationComplete(_OnDeactivationComplete);
        }

        public bool SetActivationTarget(ActivationTarget target)
        {
            if (_activationTarget == null)
            {
                _activationTarget = target;
                return true;
            }
            return false;
        }

        private void _OnDeactivationComplete()
        {
            Sound.PlaySound(_deactivationSound, gameObject);
            VisualEffects.SpawnVFX(_deactivationVFX, transform);
        }
    }
}