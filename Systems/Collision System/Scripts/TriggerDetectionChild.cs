using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TriggerDetectionChild : DetectionChild
    {
        [SerializeField] bool _ignoreTriggers = true;

        private void Awake()
        {
            //Check that the object has a trigger and that it is a trigger otherwise log a warning
            Collider coll = GetComponent<Collider>();
            if (coll == null)
                Debug.LogWarning("The " + typeof(TriggerDetectionChild) +
                    " component on " + gameObject.name +
                    " requires a component of type Collider.");
            else
            {
                if (coll.isTrigger == false)
                    Debug.LogWarning("The " + typeof(TriggerDetectionChild) +
                    " component on " + gameObject.name +
                    " requires the Collider component to be a trigger.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger == false)
            {
                detectionParent._OnChildTriggerDetection_Enter(other, detectionName);
            }
            else if (_ignoreTriggers == false && other.isTrigger)
            {
                detectionParent._OnChildTriggerDetection_Enter(other, detectionName);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.isTrigger == false)
            {
                detectionParent._OnChildTriggerDetection_Stay(other, detectionName);
            }
            else if (_ignoreTriggers == false && other.isTrigger)
            {
                detectionParent._OnChildTriggerDetection_Stay(other, detectionName);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger == false)
            {
                detectionParent._OnChildTriggerDetection_Exit(other, detectionName);
            }
            else if (_ignoreTriggers == false && other.isTrigger)
            {
                detectionParent._OnChildTriggerDetection_Exit(other, detectionName);
            }
        }
    }
}