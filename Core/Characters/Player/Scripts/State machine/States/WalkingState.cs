using UnityEngine;
using System.Collections;
using PropellerCap;

public class WalkingState : BaseState
{
    public float walkSpeed = 3f;
    public float acceleration = 30f;
    [Header("Sounds")]
    public SoundClip _footstepsSound;
    public SoundClip _playerClothesSound;
    public float footStepInterval = 0.2f;


    float _walkStartTime = 0f;

    public override void OnStateEnter(BaseState previousState)
    {
        base.OnStateEnter(previousState);
        Debugger.LogState("Entered WALKING state.");
        MaterialType mType = m_fpController.groundObject.GetMaterialType();
        Sound.PlaySound(_footstepsSound, m_characterObject, mType);
        Sound.PlaySound(_playerClothesSound, m_characterObject);
        _walkStartTime = Time.realtimeSinceStartup;
    }

    public override void OnStateExit(BaseState nextState)
    {
        base.OnStateExit(nextState);
        Debugger.LogState("Exit WALKING state.");

        Sound.StopSound(_playerClothesSound, m_characterObject);

        //Do not remove the following commented line
        if (m_fpController.grounded && nextState.IsOfType<JumpState>() == false)
            m_fpController.MoveAlongPlayerInputOnGround(0f, 0f);
    }

    public override void OnStateFixedUpdate()
    {
        base.OnStateFixedUpdate();

        m_fpController.MoveAlongPlayerInputOnGround(walkSpeed, acceleration);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (Time.realtimeSinceStartup > _walkStartTime + footStepInterval)
        {
            _walkStartTime = Time.realtimeSinceStartup;
            MaterialType mType = m_fpController.groundObject.GetMaterialType();
            Sound.PlaySound(_footstepsSound, m_characterObject, mType);
        }

        if (m_fpController.grounded == false)
        {
            m_stateMachineBehaviour.TransitionToNewState<FallingState>();
            return;
        }

        if (m_fpController.HasInputDirection() == false)
        {
            m_stateMachineBehaviour.TransitionToNewState<IdleState>();
            return;
        }
    }

    public override bool IsOfType<T>()
    {
        return typeof(WalkingState) == typeof(T);
    }
}