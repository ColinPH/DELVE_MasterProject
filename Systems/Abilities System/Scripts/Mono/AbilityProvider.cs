using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class AbilityProvider : Activator
    {
        [SerializeField] PlayerAbilityBase _abilityToAdd;
        [SerializeField] GameObject _propToRemove;
        [SerializeField] bool _showTextOnPickUp = false;
        [SerializeField] string _textOnPickUp = "";
        [SerializeField] float _displayTime = 4f;
        public bool _requireInteraction = false;
        public string playerTag = "Player";

        private bool _hasBeenActivated = false;

        protected override void MyStart()
        {
            if (_requireInteraction == false)
                m_EnsureGameObjectHasTrigger();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (_requireInteraction) return;

            if (playerTag != other.gameObject.tag) return;

            if (_hasBeenActivated)
                return;

            ProvideAbility(other.gameObject);
        }

        public void ProvideAbility(GameObject other)
        {
            var abilityCaster = other.GetComponent<AbilityCaster>();
            abilityCaster.AddAbility(_abilityToAdd);

            //Remove the prop
            if (_propToRemove != null)
                Destroy(_propToRemove);

            if (_showTextOnPickUp)
                HUD.subtitlesControler.DisplayText(_textOnPickUp, _displayTime);

            _hasBeenActivated = true;
        }

        protected override void m_Interact(GameObject callingObject)
        {
            base.m_Interact(callingObject);

            if (_requireInteraction == false) return;

            if (_hasBeenActivated)
                return;

            ProvideAbility(callingObject);
        }

        protected override bool m_IsInteractable(GameObject initiatorObject)
        {
            return true;
        }
    }
}
