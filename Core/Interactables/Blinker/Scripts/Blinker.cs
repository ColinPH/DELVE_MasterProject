using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class Blinker : MonoBehaviour, IInteractable, IWatcher
    {
        [Tooltip("The amount of health lost by the entity which relies on this Watcher, when this watcher gets destroyed.")]
        public int healthValue = 1;
        public GameObject _deathVFX;
        public GameObject _eyeBallObject;
        public float _interactionResistance = 10f;
        public float _movementSpeed = 5f;

        public float _currentResistance = 0;
        Vector3 _lastDirection = Vector3.zero;
        Vector3 _initialRestPosition;
        Vector3 _startGoingBackPosition;
        Quaternion _initialRotation;
        Transform _targetMoveObject;
        GameObject _targetLookObject;

        List<IWatcherDependent> _dependentEntities = new List<IWatcherDependent>();

        Action _interactionActionCancel;
        private bool _movingToPlayer = false;
        private bool _movingToInitialPosition = false;
        private bool _dislocated = false;

        float _moteToPlayerT = 0f;
        float _moteToWallT = 0f;

        void Start()
        {
            //_targetLookObject = FindObjectOfType<PlayerCharacter>().gameObject;
            _currentResistance = _interactionResistance;

            _initialRestPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        void Update()
        {
            if (_targetLookObject!= null)
            {
                _eyeBallObject.transform.LookAt(_targetLookObject.transform.position);
            }

            if (_movingToPlayer)
            {
                _moteToPlayerT += Time.deltaTime;
                transform.position = Vector3.Lerp(_GetRestPosition(), _targetMoveObject.position, _moteToPlayerT);
                if (Vector3.Distance(transform.position, _targetMoveObject.position) <= 0.2f)
                {
                    _BeCollectedByPlayer();
                }
            }
            else if (_movingToInitialPosition)
            {
                _moteToWallT += Time.deltaTime;
                transform.position = Vector3.Lerp(_startGoingBackPosition, _GetRestPosition(), _moteToWallT * _movementSpeed);
                if (Vector3.Distance(transform.position, _GetRestPosition()) <= 0.01f)
                {
                    _LocateInWall();
                }
            }
        }

        #region IInteractable interface overrides

        public string GetInteractionText()
        {
            throw new System.NotImplementedException();
        }
        public void Highlight()
        {
            throw new System.NotImplementedException();
        }
        public void Interact(GameObject callingObject)
        {
            throw new System.NotImplementedException();
        }
        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }
        public void OnInteractionStop()
        {
            throw new System.NotImplementedException();
        }
        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            _lastDirection = direction;
            _ApplyInteractionForce(intensity);
        }
        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            HookAbility ga = (HookAbility)caller;
            
            _targetMoveObject = pullingObject.GetComponent<AbilityCaster>().hookGun.GetGunTip();
            _lastDirection = direction;
            //Debug.Log("Target object is " + _targetMoveObject);
            _ApplyInteractionForce(intensity);
            _interactionActionCancel = () => { onInteractionCancelled(); };
        }
        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            _GoToInitialPosition();
            _ApplyInteractionForce(intensity);
        }
        public bool IsInteractable(GameObject initiatorObject)
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region IWatcher interface

        public void AddDependentEntity(IWatcherDependent targetObject)
        {
            _dependentEntities.Add(targetObject);
        }

        public int GetHealthValue()
        {
            return healthValue;
        }
        public GameObject GetWatcherObject()
        {
            return gameObject;
        }

        #endregion

        Vector3 _GetRestPosition()
        {
            Vector3 toReturn = new Vector3();
            if (_dependentEntities.Count > 0)
                toReturn = _dependentEntities[0].GetWatcherRestPosition(this);
            else
                toReturn = _initialRestPosition;
            return toReturn;
        }

        void _ApplyInteractionForce(float intensity)
        {
            if (_dislocated) { return; }

            _currentResistance -= intensity;

            if (_currentResistance <= 0f)
            {
                _DislocateFromWall();
            }
        }

        void _GoToInitialPosition()
        {
            _moteToPlayerT = 0f;
            _movingToInitialPosition = true;
            _movingToPlayer = false;
            _startGoingBackPosition = transform.position;
        }

        void _LocateInWall()
        {
            _movingToInitialPosition = false;
            _dislocated = false;
            transform.rotation = _initialRotation;
            _currentResistance = _interactionResistance;

            GameObject vfx = Instantiate(_deathVFX);
            vfx.transform.position = transform.position;
        }

        void _DislocateFromWall()
        {
            Debug.Log("Dislocate");
            GameObject vfx = Instantiate(_deathVFX);
            vfx.transform.position = transform.position;
            _movingToPlayer = true;
            _dislocated = true;
            _moteToWallT = 0f;
        }

        private void _BeCollectedByPlayer()
        {
            _movingToPlayer = false;

            foreach (IWatcherDependent item in _dependentEntities)
            {
                item.RemoveDependency(this);
            }
            _interactionActionCancel();
            Destroy(gameObject);
        }

    }
}
