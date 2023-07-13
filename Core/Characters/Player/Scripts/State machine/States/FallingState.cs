using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PropellerCap
{
    public class FallingState : BaseState
    {
        public float airControlSpeed = 3f;
        public float acceleration = 20f;
        [Header("Falling Gravity Settings")]
        public float _additionalGravityMultiplier = 1f;
        public AnimationCurve _additionalGravityCurve;
        public float _additionalGravityTimeSec = 0.3f;
        [Header("Falling Sanity Damages")]
        public float _damageHeightMultiplier = 1f;
        public float _maxDamages = 80f;
        public float _minFallDistance = 4f;

        bool _applyAdditionalGravity = false;
        float _stateEnterTime = 0f;
        float _stateEnterAltitude = 0f;

        public override void OnStateEnter(BaseState previousState)
        {
            base.OnStateEnter(previousState);
            Debugger.LogState("Entered FALLING state.");
            _applyAdditionalGravity = true;
            _stateEnterTime = Time.realtimeSinceStartup;
            _stateEnterAltitude = m_characterObject.transform.position.y;
        }

        public override void OnStateExit(BaseState nextState)
        {
            base.OnStateExit(nextState);
            Debugger.LogState("Exit FALLING state.");
            _applyAdditionalGravity = false;

            //If we are landing, deal fall damages
            if (nextState.IsOfType<LandingState>())
            {
                float positiveFallDistance = _GetFallingDistance() * -1f;
                float safeDistanceMultiplier = (positiveFallDistance <= _minFallDistance) ? 0 : 1f;
                float fallDamages = positiveFallDistance * safeDistanceMultiplier * _damageHeightMultiplier;
                if (fallDamages > _maxDamages)
                    fallDamages = _maxDamages;
                Sanity.Remove(fallDamages, false);
                Metrics.levelData.totalSanityLostByFallDamage += fallDamages;
            }
        }

        public override void OnStateFixedUpdate()
        {
            base.OnStateFixedUpdate();

            m_fpController.MoveAlongPlayerInputInAir(airControlSpeed, acceleration);



            if (_applyAdditionalGravity)
            {
                //Apply additional gravity over time based on the curve
                float remainingTime = _stateEnterTime + _additionalGravityTimeSec - Time.realtimeSinceStartup;
                if (remainingTime >= 0f)
                {
                    float progress = 1f - (remainingTime / _additionalGravityTimeSec);
                    float sampledValue = _additionalGravityCurve.Evaluate(progress);
                    float force = sampledValue * _additionalGravityMultiplier * Time.fixedDeltaTime;
                    m_fpController.ApplyForce(Vector3.down, force, ForceMode.VelocityChange);
                }
                else
                {
                    _applyAdditionalGravity = false;
                }
            }
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            if (m_fpController.grounded)
            {
                m_stateMachineBehaviour.TransitionToNewState<LandingState>();
            }
        }

        public override bool IsOfType<T>()
        {
            return typeof(FallingState) == typeof(T);
        }

        private float _GetFallingDistance()
        {
            return m_characterObject.transform.position.y - _stateEnterAltitude;
        }
    }
}
