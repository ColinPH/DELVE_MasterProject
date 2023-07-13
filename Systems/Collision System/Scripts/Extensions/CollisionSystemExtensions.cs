using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public static class CollisionSystemExtensions
    {
        /// <summary> Returns the gameObject that has the CollisionParent component if there is one. Otherwise returns the given gameObject. </summary>
        public static GameObject GetMainObject(this Collider coll)
        {
            return coll.gameObject.GetMainObject();
        }

        /// <summary> Returns the gameObject that has the CollisionParent component if there is one. Otherwise returns the given gameObject. </summary>
        public static GameObject GetMainObject(this Collision coll)
        {
            return coll.gameObject.GetMainObject();
        }

        /// <summary> Returns the gameObject that has the CollisionParent component if there is one. Otherwise returns the given gameObject. </summary>
        public static GameObject GetMainObject(this GameObject obj)
        {
            //If nothing gets found we return the given gameObject
            GameObject toReturn = obj;

            DetectionChild detectionChild = obj.GetComponent<DetectionChild>();
            if (detectionChild != null)
            {
                if (detectionChild.detectionParentObj != null)
                    toReturn = detectionChild.detectionParentObj;
            }

            return toReturn;
        }
    }
}