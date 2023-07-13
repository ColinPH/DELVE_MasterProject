using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activators/" + nameof(Lever))]
    public class Lever : Activator
    {
        public Transform _handleTransform;
        public Quaternion _activatedRot = new Quaternion();
        public Quaternion _notActivatedRot = new Quaternion();
        public bool _startActivated = false;
        public float _rotationSpeed = 30f;
        [SerializeField] float _timeBeforeActivation = 0f;
        [SerializeField] SoundClip _leverPullSound;
        [SerializeField] bool _isLocked = false;

        [SerializeField, Runtime(true)] bool _activated = false;

        bool _isRotating = false;
        IEnumerator _rotationRoutine;

        public override string worldName => nameof(Lever);

        protected override void MyStart()
        {
            base.MyStart();
            
            if (_isLocked) return;

            _activated = _startActivated;

            if (_startActivated)
            {
                //Rotate the handle to the activated position
                _ActivateLever();
            }
        }


        public void LockLever() => _isLocked = true;
        public void UnlockLever() => _isLocked = false;

        void _ActivateLever()
        {
            if (_isLocked) return;

            if (_isRotating)
                return;

            Action afterRotation = () =>
            {
                m_ActivateTargets();
            };

            _StartRotationMovement(_rotationSpeed, _notActivatedRot, _activatedRot, afterRotation);

        }

        void _DeactivateLever()
        {
            if (_isLocked) return;

            if (_isRotating)
                return;

            Action afterRotation = () =>
            {
                m_DeactivateTargets();
            };

            _StartRotationMovement(_rotationSpeed, _activatedRot, _notActivatedRot, afterRotation);
        }

        void _StartRotationMovement(float speed, Quaternion fromRot, Quaternion toRot, Action afterRotation)
        {
            //Play pull sound
            Sound.PlaySound(_leverPullSound, gameObject);

            if (_rotationRoutine != null)
                StopCoroutine(_rotationRoutine);
            _rotationRoutine = _Co_HandleRotation(speed, fromRot, toRot, afterRotation);
            StartCoroutine(_rotationRoutine);
        }

        IEnumerator _Co_HandleRotation(float speed, Quaternion fromRot, Quaternion toRot, Action afterRotation)
        {
            _isRotating = true;
            float t = 0f;

            while (t < 1f)
            {
                _handleTransform.localRotation = Quaternion.Lerp(fromRot, toRot, t);
                t += speed * Time.deltaTime;
                yield return null;
            }

            //Make surewe reach the exact end position
            _handleTransform.localRotation = toRot;

            yield return new WaitForSecondsRealtime(_timeBeforeActivation);

            _isRotating = false;
            afterRotation();
        }

        #region Activator overrides

        protected override void m_Interact(GameObject callingObject)
        {
            if (_isLocked) return;

            _activated = !_activated;

            if (_activated)
                _ActivateLever();
            else
                _DeactivateLever();
        }

        protected override void m_InteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            if (_isLocked) return;

            _activated = !_activated;

            if (_activated)
                _ActivateLever();
            else
                _DeactivateLever();
        }

        protected override bool m_IsInteractable(GameObject initiatorObject)
        {
            if (_isLocked) return false;

            return !_isRotating;
        }

        #endregion

    }
}