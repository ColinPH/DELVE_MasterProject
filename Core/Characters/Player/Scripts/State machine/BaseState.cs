using UnityEngine;
using System.Collections;
using PropellerCap;

public class BaseState : MonoBehaviour
{
    public string m_stateAnimationTrigger = "";
    protected StateMachineBehaviour m_stateMachineBehaviour;
    protected GameObject m_characterObject;
    protected FirstPersonCharacterController m_fpController;
    protected PlayerInputController m_inputController;
    protected FirstPersonViewController m_viewController;
    protected Animator m_bodyAnimator;

    public virtual void InitializeState(StateMachineBehaviour tempStateMachine)
    {
        m_stateMachineBehaviour = tempStateMachine;
        m_characterObject = m_stateMachineBehaviour.characterObject;
        m_fpController = m_characterObject.GetComponent<FirstPersonCharacterController>();
        m_inputController = m_characterObject.GetComponent<PlayerInputController>();
        m_viewController = m_characterObject.GetComponent<FirstPersonViewController>();
        m_bodyAnimator = m_stateMachineBehaviour.bodyAnimator;
    }

    public virtual bool CanEnterState()
    {
        return true;
    }

    public virtual void OnStateEnter(BaseState previousState)
    {
        gameObject.SetActive(true);
    }

    public virtual void OnStateExit(BaseState nextState)
    {
        gameObject.SetActive(false);
    }

    public virtual void OnStateFixedUpdate()
    {

    }

    public virtual void OnStateUpdate()
    {

    }

    public virtual bool IsOfType<T>() where T : BaseState
    {
        Debug.Log("StateIs function ahs not been overwritten. It should be overwritten and not call base. Object name : " + m_characterObject.name);
        return false;
    }
}