using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class GrapplingState : BaseState
    {
        public float speed = 5f;
        public float acceleration = 5f;
        public float _dragValue = 0f;

        [Header("Speed Effect")]
        public float speedThreshold = 5f;

        private bool _groundedOnStart = false;
        private bool _hasLeftGround = false;

        bool _speedEffectActive = false;

        public override void OnStateEnter(BaseState previousState)
        {
            base.OnStateEnter(previousState);
            Debugger.LogState("Entered GRAPPLING state.");
            //HUD.customPassHandler.EnableCustomPass(CustomPassType.SpeedEffect);

            _groundedOnStart = m_fpController.grounded;
            if (_groundedOnStart)
            {
                
            }
            else
            {
                m_fpController.SetDrag(_dragValue);
                _hasLeftGround = true;
            }
            m_stateMachineBehaviour.GetStateOfType<JumpState>().ResetJumpAmount();
        }

        public override void OnStateExit(BaseState nextState)
        {
            base.OnStateExit(nextState);
            Debugger.LogState("Exit GRAPPLING state.");
            HUD.customPassHandler.DisableCustomPass(CustomPassType.SpeedEffect);

            m_fpController.ResetDrag();
            _groundedOnStart = false;
            _hasLeftGround = false;
        }

        public override void OnStateFixedUpdate()
        {
            base.OnStateFixedUpdate();

            /*_groundedOnStart = false;
            _isCheckingForGrounded = false;*/

            m_fpController.MoveAlongPlayerInputInAir(speed, acceleration);

        }
         
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            //Control the speed effect
            if (_speedEffectActive == false && m_fpController.Velocity().magnitude >= speedThreshold)
            {
                HUD.customPassHandler.EnableCustomPass(CustomPassType.SpeedEffect);
                _speedEffectActive = true;
            }
            else if (_speedEffectActive && m_fpController.Velocity().magnitude < speedThreshold)
            {
                HUD.customPassHandler.DisableCustomPass(CustomPassType.SpeedEffect);
                _speedEffectActive = false;
            }

            //Wait to leave the grounded state before checking for ground
            if (_groundedOnStart)
            {
                if (m_fpController.grounded == false)
                    _hasLeftGround = true;
            }

            //If grounded go to the idle state
            if (_hasLeftGround && m_fpController.grounded)
            {
                m_stateMachineBehaviour.TransitionToNewState<IdleState>();
            }
        }

        public override bool IsOfType<T>()
        {
            return typeof(GrapplingState) == typeof(T);
        }

    }
}
