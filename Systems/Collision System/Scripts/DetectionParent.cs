using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class DetectionParent : WorldObject
    {
        [HideInInspector] public bool hasBeenPopulated = false;

        public override string worldName => nameof(DetectionParent);
        protected override void MonoAwake()
        {
            if (hasBeenPopulated == false)
                PopulateDetectionChildren();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        
        #region Do not change

        Dictionary<string, DetectionTriggerEvent> _triggerEvents = new Dictionary<string, DetectionTriggerEvent>();
        Dictionary<string, DetectionCollisionEvent> _collisionEvents = new Dictionary<string, DetectionCollisionEvent>();

        public void PopulateDetectionChildren()
        {
            hasBeenPopulated = true;
            //First we look if there are any parent which are in the children objects
            DetectionParent[] parents = GetComponentsInChildren<DetectionParent>();
            foreach (DetectionParent parent in parents)
            {
                if (parent.hasBeenPopulated == false)
                    parent.PopulateDetectionChildren();
            }

            //Then we populate the detection children
            DetectionChild[] children = GetComponentsInChildren<DetectionChild>();
            foreach (DetectionChild child in children)
            {
                //If there already is a parent assigned it must have been one of the parents in the children objects, so we skip the child
                if (child.hasBeenPopulated)
                    continue;

                child.detectionParent = this;
                child.detectionParentObj = gameObject;
                child.hasBeenPopulated = true;
            }
        }

        public DetectionTriggerEvent GetTriggerEvent(string detectionName)
        {
            if (_triggerEvents.ContainsKey(detectionName) == false)
                _triggerEvents.Add(detectionName, new DetectionTriggerEvent(detectionName));

            return _triggerEvents[detectionName];
        }
        public DetectionCollisionEvent GetCollisionEvent(string detectionName)
        {
            if (_collisionEvents.ContainsKey(detectionName) == false)
                _collisionEvents.Add(detectionName, new DetectionCollisionEvent(detectionName));

            return _collisionEvents[detectionName];
        }

        #endregion
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        
        #region Triggers

        /// <summary> Called by the trigger detection on a nested child object. </summary>
        public void _OnChildTriggerDetection_Enter(Collider other, string detectionName)
        {
            foreach (KeyValuePair<string, DetectionTriggerEvent> item in _triggerEvents)
            {
                if (item.Key == detectionName)
                {
                    item.Value.enter?.Invoke(other);
                }
            }
        }

        /// <summary> Called by the trigger detection on a nested child object. </summary>
        public void _OnChildTriggerDetection_Stay(Collider other, string detectionName)
        {
            foreach (KeyValuePair<string, DetectionTriggerEvent> item in _triggerEvents)
            {
                if (item.Key == detectionName)
                {
                    item.Value.stay?.Invoke(other);
                }
            }
        }

        /// <summary> Called by the trigger detection on a nested child object. </summary>
        public void _OnChildTriggerDetection_Exit(Collider other, string detectionName)
        {
            foreach (KeyValuePair<string, DetectionTriggerEvent> item in _triggerEvents)
            {
                if (item.Key == detectionName)
                {
                    item.Value.exit?.Invoke(other);
                }
            }
        }

        #endregion
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------
       
        #region Collisions

        private void OnCollisionEnter(Collision collision)
        {
            string detectionName = "This should not be seen";
            DetectionChild child = collision.contacts[0].thisCollider.gameObject.GetComponent<DetectionChild>();

            if (child == null)
                Debug.LogWarning("The object " + gameObject.name +
                    " was hit by " + collision.gameObject.name +
                    ", but the child object with the hit collider component" +
                    " does not have a component of type " + typeof(CollisionDetectionChild) +
                    ". Appropriate events might not be called.");
            else
                detectionName = child.detectionName;

            _OnChildCollisionDetection_Enter(collision, detectionName);
        }

        private void OnCollisionStay(Collision collision)
        {
            string detectionName = "This should not be seen";
            DetectionChild child = collision.contacts[0].thisCollider.gameObject.GetComponent<DetectionChild>();

            if (child == null)
                Debug.LogWarning("The object " + gameObject.name +
                    " is being hit by " + collision.gameObject.name +
                    ", but the child object with the hit collider component" +
                    " does not have a component of type " + typeof(CollisionDetectionChild) +
                    ". Appropriate events might not be called.");
            else
                detectionName = child.detectionName;

            _OnChildCollisionDetection_Stay(collision, detectionName);
        }

        private void OnCollisionExit(Collision collision)
        {
            _OnChildCollisionDetection_Exit(collision);
        }

        private void _OnChildCollisionDetection_Enter(Collision collision, string detectionName)
        {
            foreach (KeyValuePair<string, DetectionCollisionEvent> coll in _collisionEvents)
            {
                if (coll.Key == detectionName)
                {
                    coll.Value.enter?.Invoke(collision);
                }
            }
        }

        private void _OnChildCollisionDetection_Stay(Collision collision, string detectionName)
        {
            foreach (KeyValuePair<string, DetectionCollisionEvent> coll in _collisionEvents)
            {
                if (coll.Key == detectionName)
                {
                    coll.Value.stay?.Invoke(collision);
                }
            }
        }

        private void _OnChildCollisionDetection_Exit(Collision collision)
        {
            foreach (KeyValuePair<string, DetectionCollisionEvent> coll in _collisionEvents)
            {
                coll.Value.globalCollisionExit?.Invoke(collision);
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        #region Custom Inspector
        public override List<InspectorMessage> GetInspectorWarnings()
        {
            List<InspectorMessage> toReturn = new List<InspectorMessage>();

            //Fetch all the colliders without a DetectionChild component
            string childlessColliders = "";
            int childlessCollidersAmount = 0;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider coll in colliders)
            {
                if (coll.gameObject.GetComponent<DetectionChild>() == null)
                {
                    childlessColliders += coll.gameObject.name + "\n";
                    childlessCollidersAmount += 1;
                }
            }

            if (childlessCollidersAmount == 0)
                return toReturn;

            InspectorMessage message = new InspectorMessage(InspectorMessageType.Warning);
            message.text = $"Some objects have colliders but no DetectionChild component. " +
                $"Behaviours might not work or be reliable. " +
                $"If some objects intentionaly have no colliders then you can ignore this warning." +
                $"\nObjects with no DetectionChild are : \n{childlessColliders}";
            toReturn.Add(message);

            return toReturn;
        }
        #endregion Custom Inspector
    }
}