using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class FlareCharge : WorldObject
    {
        [Header("Behaviour")]
        [SerializeField] int _amountCharges = 1;
        [Header("SFX & VFX")]
        [SerializeField] SoundClip _pickupSound;
        [SerializeField] VisualEffectClip _pickupVFX;



        private Collectable _collectable;

        protected override void MyStart()
        {
            base.MyStart();

            _collectable = m_FetchForComponent<Collectable>();
            _collectable.OnObjectCollected += _OnObjectCollected;
        }

        private void _OnObjectCollected(GameObject collectingObject)
        {
            //Add a charge to the player
            collectingObject.GetComponent<AbilityCaster>().GetAbilityOfType<ShootAbility>().AddCharges(_amountCharges);

            //Destroy the object
            Sound.PlaySound(_pickupSound, gameObject);
            VisualEffects.SpawnVFX(_pickupVFX, transform);
            DestroyMyObject();
        }
    }
}
