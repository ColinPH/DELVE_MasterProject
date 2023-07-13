using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    public class DetectionTriggerEvent
    {
        public DetectionTriggerEvent(string detectionName)
        {
            eventName = detectionName;
        }
        public string eventName = "";
        public delegate void OnTriggerEvent(Collider other);
        public OnTriggerEvent enter { get; set; }
        public OnTriggerEvent stay { get; set; }
        public OnTriggerEvent exit { get; set; }
    }
}