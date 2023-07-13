using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachineBehaviour : MonoBehaviour
{
    [SerializeField] BaseState _startingState;
    public Animator bodyAnimator;
    public GameObject characterObject;

    /// <summary>
    /// Called after the transition to a new stage has been done.
    /// </summary>
    /// <param name="newState">The new state that has been entered.</param>
    public delegate void OnNewStageEntered(BaseState newState);
    public OnNewStageEntered onNewStageEntered;

    private StateMachine m_stateMachine;
    StateMachine _stateMachine
    {
        get
        {
            if (m_stateMachine != null) return m_stateMachine;
            Debug.LogWarning("State machine has not been created : " + gameObject.name);
            m_stateMachine = new StateMachine(GetComponentsInChildren<BaseState>().ToList(), this);
            return m_stateMachine;
        }
    }

    public BaseState activeState { get => m_stateMachine.activeState; }

    private void Awake()
    {
        if (characterObject == null) 
            characterObject = this.gameObject;

        //Create a new state machine using the starting state
        m_stateMachine = new StateMachine(GetComponentsInChildren<BaseState>().ToList(), this);

        m_stateMachine.TransitionToNewState<IdleState>();
    }

    private void FixedUpdate()
    {
        if (_stateMachine != null)
            _stateMachine.CallFixedUpdateOnState();
    }
    private void Update()
    {
        if (_stateMachine != null)
            _stateMachine.CallUpdateOnState();
    }

    public bool TransitionToNewState<T>() where T : BaseState
    {
        return TransitionToNewState<T>(out T newState);
    }

    public bool TransitionToNewState<T>(out T newState) where T : BaseState
    {
        bool transitionIsPossible = _stateMachine.TransitionToNewState<T>(out newState);
        
        if (transitionIsPossible)
        {
            onNewStageEntered?.Invoke(newState);
        }
        return transitionIsPossible;
    }

    public T GetStateOfType<T>() where T : BaseState
    {
        return _stateMachine.GetStateOfType<T>();
    }
}
