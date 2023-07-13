using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class Collectable : WorldObject, IInteractable
    {
        [Header("Behaviour")]
        [SerializeField] bool _collectOnTriggerEnter = true;
        [SerializeField] bool _collectOnInteraction = true;
        [Header("Detection Events")]
        [SerializeField] List<string> _validTriggerTags = new List<string>() { "Player" };
        [SerializeField] string _playerTriggerName = "PlayerTrigger";

        public delegate void ObjectCollectedHandler(GameObject collectingObject);
        public ObjectCollectedHandler OnObjectCollected { get; set; }

        DetectionParent _detectionParent;

        protected override void MyAwake()
        {
            base.MyAwake();
            _detectionParent = m_FetchForComponent<DetectionParent>();
            _detectionParent.GetTriggerEvent(_playerTriggerName).enter += _OnPlayerEnterTrigger;
        }

        private void _OnPlayerEnterTrigger(Collider other)
        {
            if (_collectOnTriggerEnter == false) return;

            if (_validTriggerTags.Contains(other.tag))
            {
                _CollectObject(other.gameObject);
            }
        }

        private void _CollectObject(GameObject collectingObject)
        {
            OnObjectCollected?.Invoke(collectingObject);
        }

        #region IInteractable interface
        public void Interact(GameObject callingEntity)
        {
            if (_collectOnInteraction)
            {
                _CollectObject(callingEntity);
            }
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionStop()
        {
            throw new NotImplementedException();
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            return true;
        }

        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }
        #endregion IInteractable interface
    }
}
