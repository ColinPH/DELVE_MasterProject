using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    public class DetectionCollisionEvent
    {
        public DetectionCollisionEvent(string detectionName)
        {
            eventName = detectionName;
        }
        public string eventName = "";
        public delegate void OnCollisionEvent(Collision collision);
        public OnCollisionEvent enter;
        public OnCollisionEvent stay;
        public OnCollisionEvent globalCollisionExit;
    }
}