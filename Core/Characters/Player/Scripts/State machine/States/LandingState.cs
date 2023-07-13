using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LandingState : BaseState
    {
        [SerializeField] SoundClip _landingSound;
        public override void OnStateEnter(BaseState previousState)
        {
            base.OnStateEnter(previousState);
            Debugger.LogState("Entered LANDING state.");
            Sound.PlaySound(_landingSound, m_characterObject);
        }

        public override void OnStateExit(BaseState nextState)
        {
            base.OnStateExit(nextState);
            Debugger.LogState("Exit LANDING state.");
            m_stateMachineBehaviour.GetStateOfType<JumpState>().ResetJumpAmount();
        }

        public override void OnStateFixedUpdate()
        {

        }

        public override void OnStateUpdate()
        {
            if (m_fpController.HasInputDirection())
            {
                m_stateMachineBehaviour.TransitionToNewState<WalkingState>();
            }
            else
            {
                m_stateMachineBehaviour.TransitionToNewState<IdleState>();
            }
        }

        public override bool IsOfType<T>()
        {
            return typeof(LandingState) == typeof(T);
        }
    }
}