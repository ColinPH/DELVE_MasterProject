using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class AbilityCastListener : MonoBehaviour
    {
        [SerializeField] string _targetTag = "Player";
        [SerializeField] int _triggerCounter = 1;
        private int currentCount = 0;
        [SerializeField] AbilityType _abilityType = AbilityType.Unassigned;
        [SerializeField] List<GameObject> _objectsToShow = new List<GameObject>();
        public GameObject lookAtObject;
        [Header("/!\\ Runtime information /!\\")]
        [SerializeField] bool _hasBeenValidated = false;
        private Transform playerTransform;

        
        
        PlayerAbilityBase targetAbility;

        private void Start()
        {
            _HideObjects();
        }

        private void LateUpdate()
        {
            if (_hasBeenValidated)
                return;
            lookAtObject.transform.LookAt(playerTransform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                if (_hasBeenValidated)
                    return;

                if(_abilityType == AbilityType.Unassigned)
                {
                    _ShowObjects();
                    return;
                }
                playerTransform = other.gameObject.transform;
                //Hook to the ability event
                targetAbility = other.gameObject.GetComponent<AbilityCaster>().GetAbility(_abilityType);
                targetAbility.onAbilityUsed += _OnAbilityExecuted;
                _ShowObjects();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                if (_hasBeenValidated)
                    return;

                //Unhook from the ability event
                targetAbility.onAbilityUsed -= _OnAbilityExecuted;

                _HideObjects();
            }
        }

        private void _OnAbilityExecuted()
        {
            currentCount++;
            if (currentCount < _triggerCounter)
                return;
            _hasBeenValidated = true;

            //Unhook from the ability event
            targetAbility.onAbilityUsed -= _OnAbilityExecuted;

            _HideObjects();
        }

        private void _ShowObjects()
        {
            foreach (var item in _objectsToShow)
            {
                item.SetActive(true);
            }
        }

        void _HideObjects()
        {
            foreach (var item in _objectsToShow)
            {
                item.SetActive(false);
            }
        }
    }
}
