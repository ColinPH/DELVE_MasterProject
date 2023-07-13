using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class ATLinearController : ActivationTarget
    {
        [Header("General Settings")]
        [SerializeField] float _speedMultiplier = 1f;
        [SerializeField] bool _isReversable = true;
        [Header("Position")]
        public bool isControllingPosition = true;
        public Vector3 startWorldPosition = new Vector3();
        public Vector3 endWorldPosition = new Vector3();
        [SerializeField] AnimationCurve _positionCurve;
        [Header("Rotation")]
        public bool isControllingRotation = true;
        public Quaternion startRotation = Quaternion.identity;
        public Quaternion endRotation = Quaternion.identity;
        [SerializeField] AnimationCurve _rotationCurve;

        [HideInInspector] public float lerpPos = 0f;
        [HideInInspector] public float lerpRot = 0f;

        [HideInInspector] public Vector3 startHandleOffset = new Vector3();
        [HideInInspector] public Vector3 endHandleOffset = new Vector3();

        [HideInInspector] public Quaternion startHandleOffsetROT = Quaternion.identity;
        [HideInInspector] public Quaternion endHandleOffsetROT = Quaternion.identity;

        float _directionMultiplier = 1f;
        bool _isMoving = false;

        public override void Activate()
        {
            base.Activate();

            _isMoving = true;
            _directionMultiplier = 1f;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_isReversable == false)
                return;

            _isMoving = true;
            _directionMultiplier = -1f;
        }

        private void Update()
        {
            if (_isMoving)
            {
                lerpPos += _GetPositionSpeed(lerpPos) * _directionMultiplier * _speedMultiplier * Time.deltaTime;
                lerpPos = Mathf.Clamp01(lerpPos);

                lerpRot += _GetRotationSpeed(lerpRot) * _directionMultiplier * _speedMultiplier * Time.deltaTime;
                lerpRot = Mathf.Clamp01(lerpRot);

                UpdateObjectPosition();

                if (lerpRot == 1f && lerpPos == 1f || lerpRot == 0f && lerpPos == 0f)
                    _isMoving = false;
            }
        }

        public void UpdateObjectPosition()
        {
            if (isControllingPosition)
            {
                transform.position = Vector3.Lerp(startWorldPosition, endWorldPosition, lerpPos);
            }

            if (isControllingRotation)
            {
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpRot);
            }
        }

        private float _GetPositionSpeed(float lerpPos)
        {
            float point = _positionCurve.Evaluate(lerpPos);
            if (point == 0f)
                point = 0.001f;
            return point;
        }
        private float _GetRotationSpeed(float lerpRot)
        {

            float point = _rotationCurve.Evaluate(lerpPos);
            if (point == 0f)
                point = 0.001f;
            return point;
        }

    }
}