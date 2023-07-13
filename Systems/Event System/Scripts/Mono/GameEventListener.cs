using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] GameEvent _event = GameEvent.Unassigned;
        [SerializeField] UnityEvent _onEventActivation;

        private void Start()
        {
            Managers.eventManager.SubscribeToGameEvent(_event, OnEventActivation);
        }

        private void OnEventActivation()
        {
            _onEventActivation?.Invoke();
        }
    }
}
