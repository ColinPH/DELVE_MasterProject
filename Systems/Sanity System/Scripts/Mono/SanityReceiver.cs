using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Light and Sanity/" + nameof(SanityReceiver))]
    public class SanityReceiver : MonoBehaviour, ISanityReceiver
    {
        [Header("/!\\ Runtime Information /!\\")]
        [SerializeField] bool _isAfflicted = false;
        [SerializeField] int _amountAfflictors = 0;
        [SerializeField] List<GameObject> _activeAfflictors = new List<GameObject>();

        #region Public accessors and events
        public delegate void SanityAfflictorEnteredHandler();
        public SanityAfflictorEnteredHandler onSanityAfflictorEntered { get; set; }

        public delegate void SanityAfflictorExitHandler();
        public SanityAfflictorExitHandler onSanityAfflictorExit { get; set; }
        public delegate void SanityAfflictorAcquiredHandler(SanityAfflictor sanityAfflictor);
        public SanityAfflictorAcquiredHandler onSanityAfflictorAcquired { get; set; }
        public delegate void SanityAfflictorLostHandler(SanityAfflictor sanityAfflictor);
        public SanityAfflictorLostHandler onSanityAfflictorLost { get; set; }
        #endregion Public accessors and events

        public void IApplySanityEffect(GameObject afflictor)
        {
            _amountAfflictors += 1;
            _activeAfflictors.Add(afflictor);

            _AcquireNewSanityAfflictor(afflictor.GetComponent<SanityAfflictor>());

            if (_isAfflicted)
                return;
            
            _EnteredSanity();
        }

        public void IRemoveSanityEffect(GameObject afflictor)
        {
            _amountAfflictors -= 1;
            _activeAfflictors.Remove(afflictor);

            if (_isAfflicted == false)
            {
                Debugger.LogError($"{gameObject.name} is not afflicted by sanity but the following affilctor has been removed : {afflictor.name}");
                return;
            }

            _LoseSanityAfflictor(afflictor.GetComponent<SanityAfflictor>());

            if (_amountAfflictors == 0)
                _ExitSanity();
        }

        private void _EnteredSanity()
        {
            _isAfflicted = true;
            onSanityAfflictorEntered?.Invoke();
        }

        private void _ExitSanity()
        {
            _isAfflicted = false;
            onSanityAfflictorExit?.Invoke();
        }

        private void _AcquireNewSanityAfflictor(SanityAfflictor sanityAfflictor)
        {
            onSanityAfflictorAcquired?.Invoke(sanityAfflictor);
        }

        private void _LoseSanityAfflictor(SanityAfflictor sanityAfflictor)
        {
            onSanityAfflictorLost?.Invoke(sanityAfflictor);
        }

    }
}