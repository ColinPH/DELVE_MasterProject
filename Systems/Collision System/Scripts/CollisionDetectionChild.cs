using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class CollisionDetectionChild : DetectionChild
    {
        private void Awake()
        {
            //Check that the object has a collider and that it is not a trigger otherwise log a warning
            Collider coll = GetComponent<Collider>();
            if (coll == null)
                Debug.LogWarning("The " + typeof(CollisionDetectionChild) +
                    " component on " + gameObject.name +
                    " requires a component of type Collider.");
            else
            {
                if (coll.isTrigger)
                    Debug.LogWarning("The " + typeof(CollisionDetectionChild) +
                    " component on " + gameObject.name +
                    " requires the Collider component to not be a trigger.");
            }
        }
    }
}