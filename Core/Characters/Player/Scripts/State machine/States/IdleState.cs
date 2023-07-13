using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{

    public override void OnStateEnter(BaseState previousState)
    {
        base.OnStateEnter(previousState);
        Debugger.LogState("Entered IDLE state.");
    }

    public override void OnStateExit(BaseState nextState)
    {
        base.OnStateExit(nextState);
        Debugger.LogState("Exit IDLE state.");
    }

    public override void OnStateFixedUpdate()
    {
        
    }

    public override void OnStateUpdate()
    {
        if (m_fpController.grounded == false)
        {
            m_stateMachineBehaviour.TransitionToNewState<FallingState>();
            return;
        }

        if (m_fpController.HasInputDirection())
        {
            m_stateMachineBehaviour.TransitionToNewState<WalkingState>();
            return;
        }
    }

    public override bool IsOfType<T>()
    {
        return typeof(IdleState) == typeof(T);
    }
}
