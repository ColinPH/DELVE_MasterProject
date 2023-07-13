using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(ActivationEffects))]
    public class ActivationEffects : WorldObject
    {
        public enum Alignment
        {
            LocalUp = 0,
            LocalForward = 1,
            LocalRight = 2,
            WorldUp = 3,
        }
        [SerializeField] ActivationTarget _activationTarget;
        [SerializeField] SoundClip _ActivationSound;
        [SerializeField] VisualEffectClip _ActivationVFX;
        [SerializeField] bool _hasOffset = false;
        [ShowIf(nameof(_hasOffset))]
        [SerializeField] Vector3 _spawnOffset = Vector3.zero;
        [SerializeField] Alignment _alignment = Alignment.LocalUp;
        public override string worldName => nameof(ActivationEffects);
        protected override void MyAwake()
        {
            base.MyAwake();
            if (_activationTarget == null)
                _activationTarget = m_FetchForComponent<ActivationTarget>();
            _activationTarget.RegisterToActivationComplete(_OnActivationComplete);
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

        private void _OnActivationComplete()
        {
            Vector3 spawnPos = transform.position;

            if (_hasOffset)
                spawnPos += _spawnOffset;

            Sound.PlaySound(_ActivationSound, gameObject);
            VisualEffects.SpawnVFX(_ActivationVFX, spawnPos, _GetAlignmentVector());
        }

        private Vector3 _GetAlignmentVector()
        {
            switch (_alignment)
            {
                case Alignment.LocalUp:
                    return transform.up;
                case Alignment.LocalForward:
                    return transform.forward;
                case Alignment.LocalRight:
                    return transform.right;
                case Alignment.WorldUp:
                    return Vector3.up;
                default:
                    Debugger.LogError($"Missing switch statement case '{_alignment}' in '{nameof(ActivationEffects)}'.");
                    return transform.up; 
            }
        }
    }
}
