using PropellerCap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine
{
    private List<BaseState> _existingStates;
    public BaseState activeState { get; private set; }
    public StateMachineBehaviour stateMachineBehaviour { get; private set; }

    public GameObject characterObject { get; private set; }

    public StateMachine(List<BaseState> allStates, StateMachineBehaviour stateMachineBehaviour)
    {
        this.stateMachineBehaviour = stateMachineBehaviour;
        _existingStates = allStates;

        characterObject = stateMachineBehaviour.characterObject;

        //Call initialize on all the states of the state machine
        foreach (BaseState bs in _existingStates)
        {
            //Pass the state machine reference
            bs.InitializeState(stateMachineBehaviour);
            //Disable all the states objects to make sure no one is active
            bs.gameObject.SetActive(false);
        }
    }

    public void CallFixedUpdateOnState()
    {
        if (activeState != null)
            activeState?.OnStateFixedUpdate();
    }

    public void CallUpdateOnState()
    {
        if (activeState != null)
            activeState?.OnStateUpdate();
    }

    public bool TransitionToNewState<T>() where T : BaseState
    {
        return TransitionToNewState<T>(out T newState);
    }

    public bool TransitionToNewState<T>(out T newState) where T : BaseState
    {
        //TODO this check should probably be removed because T must inherit from BaseState
        //Check that the state we transition to inherits from BaseState
        if (typeof(T).IsSubclassOf(typeof(BaseState)) == false)
        {
            Debug.LogError(typeof(T).Name + " is not a state because it doesn't inherit from " + typeof(BaseState));
            newState = default(T);
            return false;
        }

        BaseState tempState = GetStateOfType<T>();

        if (tempState == null)
        {
            newState = default(T);
            return false;
        }

        IEnumerable<T> matchingStates = _existingStates.OfType<T>();
        /*
        if (matchingStates.Count() == 0)
        {
            Debug.LogError("Can not transition to the state " + typeof(T).Name + " because it has't been found in the children");
            newState = default(T);
            return false;
        }
        else if (matchingStates.Count() > 1)
        {
            Debug.LogError("There are more than 1 states of type : " + typeof(T).Name + " in the object : " + characterObject.name);
        }
        BaseState tempState = matchingStates.First();*/

        //Exit previous state if there is one and enter new state

        //Stop the state transition if it is not possible to enter the target state
        if (tempState.CanEnterState() == false)
        {
            newState = default(T);
            return false;
        }

        //If we can enter the target state, exit the current one and enter the target state
        BaseState oldState = null;
        if (activeState != null)
        {
            activeState?.OnStateExit(tempState);
            oldState = activeState;
        }
        activeState = tempState;
        activeState?.OnStateEnter(oldState);

        newState = matchingStates.First();
        return true;
    }

    public T GetStateOfType<T>() where T : BaseState
    {
        IEnumerable<T> matchingStates = _existingStates.OfType<T>();

        if (matchingStates.Count() == 0)
        {
            Debug.LogError("The state " + typeof(T).Name + " has't been found in the children");
            return default(T);
        }
        else if (matchingStates.Count() > 1)
        {
            Debug.LogError("There are more than 1 states of type : " + typeof(T).Name + " in the object : " + characterObject.name);
        }

        return matchingStates.First();
    }
}
