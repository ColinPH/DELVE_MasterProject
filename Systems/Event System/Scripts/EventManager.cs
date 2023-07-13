using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class EventManager : ManagerBase
    {
        public static EventManager Instance { get; private set; }

        public delegate void SimpleEvent();
        public delegate void SimpleEventWithCaller(object eventCaller);
        public delegate void MainGameEvent(object eventCaller, SceneGroup sceneGroup, object sceneIdentifier);

        //GameEvent banks
        Dictionary<GameEvent, SimpleEvent> _simpleGameEvents = new Dictionary<GameEvent, SimpleEvent>();
        Dictionary<GameEvent, SimpleEventWithCaller> _simpleGameEventsWithCaller = new Dictionary<GameEvent, SimpleEventWithCaller>();
        Dictionary<GameEvent, MainGameEvent> _mainGameEvents = new Dictionary<GameEvent, MainGameEvent>();
        //RoomEvent banks
        Dictionary<RoomEvent, SimpleEvent> _simpleRoomEvents = new Dictionary<RoomEvent, SimpleEvent>();

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in Event Manager.");
            Managers.eventManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Event Manager.");
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<EventManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return true;
        }
        #endregion

        //----------

        #region GameEvent Subscriptions
        public void SubscribeToGameEvent(GameEvent eventType, SimpleEvent eventDelegate)
        {
            if (_simpleGameEvents.ContainsKey(eventType))
            {
                _simpleGameEvents[eventType] += eventDelegate;
            }
            else
            {
                _simpleGameEvents[eventType] = eventDelegate;
            }
        }
        public void SubscribeToGameEvent(GameEvent eventType, SimpleEventWithCaller eventDelegate)
        {
            if (_simpleGameEventsWithCaller.ContainsKey(eventType))
            {
                _simpleGameEventsWithCaller[eventType] += eventDelegate;
            }
            else
            {
                _simpleGameEventsWithCaller[eventType] = eventDelegate;
            }
        }
        public void SubscribeToMainGameEvent(GameEvent eventType, MainGameEvent eventDelegate)
        {
            if (_mainGameEvents.ContainsKey(eventType))
            {
                _mainGameEvents[eventType] += eventDelegate;
            }
            else
            {
                _mainGameEvents[eventType] = eventDelegate;
            }
        }
        #endregion GameEvent Subscriptions

        #region RoomEvent Subscriptions
        public void SubscribeToRoomEvent(RoomEvent eventType, SimpleEvent eventDelegate)
        {
            if (_simpleRoomEvents.ContainsKey(eventType))
            {
                _simpleRoomEvents[eventType] += eventDelegate;
            }
            else
            {
                _simpleRoomEvents[eventType] = eventDelegate;
            }
        }
        #endregion

        //----------

        #region GameEvents Unsubscription
        public void UnsubscribeFromGameEvent(GameEvent eventType, SimpleEvent eventDelegate)
        {
            if (_simpleGameEvents.ContainsKey(eventType))
            {
                _simpleGameEvents[eventType] -= eventDelegate;
            }
        }
        public void UnsubscribeFromGameEvent(GameEvent eventType, SimpleEventWithCaller eventDelegate)
        {
            if (_simpleGameEventsWithCaller.ContainsKey(eventType))
            {
                _simpleGameEventsWithCaller[eventType] -= eventDelegate;
            }
        }
        public void UnsubscribeFromGameEvent(GameEvent eventType, MainGameEvent eventDelegate)
        {
            if (_mainGameEvents.ContainsKey(eventType))
            {
                _mainGameEvents[eventType] -= eventDelegate;
            }
        }
        #endregion


        #region RoomEvent Unsubscriptions
        public void UnsubscribeFromRoomEvent(RoomEvent eventType, SimpleEvent eventDelegate)
        {
            if (_simpleRoomEvents.ContainsKey(eventType))
            {
                _simpleRoomEvents[eventType] -= eventDelegate;
            }
        }
        #endregion

        public void FireRoomEvent(RoomEvent eventType, object eventCaller)
        {
            Debugger.LogEvent($"{eventType} has been fired from {eventCaller.GetType()}.");

            if (_simpleRoomEvents.ContainsKey(eventType))
                _simpleRoomEvents[eventType]?.Invoke();
        }

        public void FireMainGameEvent(GameEvent eventType, object eventCaller, SceneGroup sceneGroup, object sceneIdentifier)
        {
            Debugger.LogEvent($"{eventType} has been fired from {eventCaller.GetType()}.");

            if (_mainGameEvents.ContainsKey(eventType))
                _mainGameEvents[eventType]?.Invoke(eventCaller, sceneGroup, sceneIdentifier);

            if (_simpleGameEventsWithCaller.ContainsKey(eventType))
                _simpleGameEventsWithCaller[eventType]?.Invoke(eventCaller);

            if (_simpleGameEvents.ContainsKey(eventType))
                _simpleGameEvents[eventType]?.Invoke();
        }
        public void FireGameEvent(GameEvent eventType, object eventCaller)
        {
            Debugger.LogEvent($"{eventType} has been fired from {eventCaller.GetType()}.");

            if (_simpleGameEventsWithCaller.ContainsKey(eventType))
                _simpleGameEventsWithCaller[eventType]?.Invoke(eventCaller);

            if (_simpleGameEvents.ContainsKey(eventType))
                _simpleGameEvents[eventType]?.Invoke();
        }
    }
}
