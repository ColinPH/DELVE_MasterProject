using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class JumpState : BaseState
    {
        [Header("Air Control")]
        public float airControlSpeed = 3f;
        public float acceleration = 20f;
        [Header("Jump Settings")]
        public ForceMode forceMode = ForceMode.Force;
        public float jumpThrust = 250f;
        public int maxJumps = 1;
        public float dragValue = 2;

        [Header("/!\\ Runtime Info /!\\")]
        [SerializeField] int _timesJumped = 0;

        private bool _groundedOnStart = false;
        private bool _hasLeftGround = false;

        #region BaseState overrides
        public override bool CanEnterState()
        {
            return (_timesJumped < maxJumps);
        }

        public override void OnStateEnter(BaseState previousState)
        {
            base.OnStateEnter(previousState);
            Debugger.LogState("Entered JUMP state.");

            
            m_fpController.SetDrag(dragValue);

            m_fpController.CancelVerticalVelocity();
            m_fpController.ApplyForce(Vector3.up, jumpThrust, forceMode);
            
            _timesJumped += 1;
            _groundedOnStart = false;
            _hasLeftGround = false;
            _isGoingDown = false;

            _groundedOnStart = m_fpController.grounded;
            if (_groundedOnStart)
            {

            }
            else
            {
                _hasLeftGround = true;
            }
        }

        public override void OnStateExit(BaseState nextState)
        {
            base.OnStateExit(nextState);
            Debugger.LogState("Exit JUMP state.");

            m_fpController.ResetDrag();


            _groundedOnStart = false;
            _hasLeftGround = false;
        }

        bool _isGoingDown = false;

        public override void OnStateFixedUpdate()
        {
            if (_hasLeftGround)
            {
                m_fpController.MoveAlongPlayerInputInAir(airControlSpeed, acceleration);
                _isGoingDown = (m_fpController.Velocity().y <= 0f);
            }

        }

        public override void OnStateUpdate()
        {
            if (_groundedOnStart)
            {
                //Wait to leave the grounded state before checking for ground
                if (m_fpController.grounded == false)
                    _hasLeftGround = true;
            }

            if (m_fpController.grounded == false)
            {
                //Transition to the falling state when we go down again
                if (_isGoingDown)
                {
                    m_stateMachineBehaviour.TransitionToNewState<FallingState>();
                }
            }

            //If grounded go to the idle state
            if (_hasLeftGround && m_fpController.grounded)
            {
                m_stateMachineBehaviour.TransitionToNewState<LandingState>();
            }
        }

        public override bool IsOfType<T>()
        {
            return typeof(JumpState) == typeof(T);
        }

        #endregion
        //------------------------------------------------------------------------------------------------
        
        
        public void ResetJumpAmount()
        {
            _timesJumped = 0;
        }
    }
}
