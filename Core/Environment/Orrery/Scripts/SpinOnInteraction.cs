using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SpinOnInteraction : WorldObject, IInteractable
    {
        [SerializeField] float _continuousForceMultiplier = 1f;
        [SerializeField] float _startForceMultiplier = 0.1f;
        [SerializeField] float _interactionIntensity = 5f;

        Rigidbody _rigidbody;
        Hookable _hookable;

        public override string worldName => nameof(SpinOnInteraction);

        protected override void MyAwake()
        {
            base.MyAwake();
            _rigidbody = m_FetchForComponent<Rigidbody>();
            _hookable = m_FetchForComponent<Hookable>();
        }


        private void _ApplyForceOnObject(Vector3 direction, float intensity, float multiplier, Vector3 forcePosition)
        {
            Vector3 forceToApply = direction.normalized * intensity * multiplier;
            _rigidbody.AddForceAtPosition(forceToApply, forcePosition);
        }



        #region IInteractable interface
        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public void Interact(GameObject callingEntity)
        {
            _ApplyForceOnObject(callingEntity.transform.forward, _interactionIntensity, 1f, callingEntity.transform.position);
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            _ApplyForceOnObject(direction, intensity, _continuousForceMultiplier, _hookable.GetHookAttachmentPoint());
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            return true;
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionStop()
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            _ApplyForceOnObject(direction, intensity, _startForceMultiplier, _hookable.GetHookAttachmentPoint());
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            
        }
        #endregion IInteractable interface

        #region Inspector methods
        public override List<InspectorMessage> GetInspectorHelpMessages()
        {
            List<InspectorMessage> toReturn = new List<InspectorMessage>();

            if (GetComponent<Hookable>() == null)
            {
                var newMessage = new InspectorMessage($"This component requires a '{nameof(Hookable)}' component.", InspectorMessageType.Warning);
                newMessage.errorFixAction = () => { gameObject.AddComponent<Hookable>(); };
                toReturn.Add(newMessage);
            }
            if (GetComponent<Rigidbody>() == null)
            {
                var newMessage = new InspectorMessage($"This component requires a '{nameof(Rigidbody)}' component.", InspectorMessageType.Warning);
                newMessage.errorFixAction = () => { gameObject.AddComponent<Rigidbody>(); };
                toReturn.Add(newMessage);
            }
            if (GetComponent<DetectionParent>() == null)
            {
                var newMessage = new InspectorMessage($"This component requires a '{nameof(DetectionParent)}' component.", InspectorMessageType.Warning);
                newMessage.errorFixAction = () => { gameObject.AddComponent<DetectionParent>(); };
                toReturn.Add(newMessage);
            }

            if (TryGetComponent(out Rigidbody rb))
            {
                if (rb.useGravity)
                    toReturn.Add(new InspectorMessage($"The '{nameof(Rigidbody)}' field \"{nameof(rb.useGravity)}\" needs to be set to FALSE.", InspectorMessageType.Warning));
                if (rb.automaticCenterOfMass)
                    toReturn.Add(new InspectorMessage($"The '{nameof(Rigidbody)}' field \"{nameof(rb.automaticCenterOfMass)}\" needs to be set to FALSE.", InspectorMessageType.Warning));
                if (rb.automaticInertiaTensor)
                    toReturn.Add(new InspectorMessage($"The '{nameof(Rigidbody)}' field \"{nameof(rb.automaticInertiaTensor)}\" needs to be set to FALSE.", InspectorMessageType.Warning));
            }

            if (TryGetComponent(out Hookable hookable))
            {
                if (hookable.IsMovingObject() == false)
                    toReturn.Add(new InspectorMessage($"The '{nameof(Hookable)}' field \"{nameof(hookable.IsMovingObject)}\" needs to be set to TRUE.", InspectorMessageType.Warning));
            }


            return toReturn;
        }
        #endregion Inspector methods
    }
}
