using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace PropellerCap
{
    [Serializable]
    public class AnimEvent
    {
        public string eventName = "";
        public bool isSingleTimeEvent = false;
        public UnityEvent animatedEvent;
        [HideInInspector] public bool eventHasBeenPlayed = false;
    }

    public class SimpleAnimationEventCaller : MonoBehaviour
    {
        [SerializeField] List<AnimEvent> _animEvents;

        public void CallAnimationEvent(string eventName)
        {
            bool foundEvent = false;
            foreach (AnimEvent e in _animEvents)
            {
                if (e.eventName == eventName)
                {
                    if (e.isSingleTimeEvent && e.eventHasBeenPlayed) continue;

                    e.animatedEvent.Invoke();
                    foundEvent = true;
                    e.eventHasBeenPlayed = true;
                }
            }

            if (foundEvent == false)
            {
                Debug.LogError(
                    "The " + typeof(SimpleAnimationEventCaller)
                    + " does not contain an animation event for the event named \""
                    + eventName + "\". Make sure the names in the list and in the animation match, "
                    + "otherwise add a new event to the list. Check object called : " + gameObject.name
                    );
            }
        }
    }
}
