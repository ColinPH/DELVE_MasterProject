using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Breakables/" + nameof(Breakable))]
    public class Breakable : Activator
    {
        [SerializeField] bool _reactToInteraction = false;
        [ShowIf(nameof(_reactToInteraction))]
        [SerializeField] float _forceAppliedOnInteract = 5f;
        [SerializeField] float _breakForceThreshold = 10f;
        [SerializeField, Runtime(true)] float _forceApplied = 0f;

        Action _interactionCancelledAction;
        bool _cancelActionInvoked = false;
        Dismantle _dismantle;
        bool _elementIsBroken = false;

        public delegate void ObjectBreakHandler();
        public ObjectBreakHandler OnObjectBreak { get; set; }

        protected override void MyAwake()
        {
            base.MyAwake();

            _dismantle = m_FetchForComponent<Dismantle>();
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            _CancelInteractionAction();
        }

        public void BreakObject()
        {
            StartCoroutine(_Co_BreakElement());
            _elementIsBroken = true;
        }

        private IEnumerator _Co_BreakElement()
        {
            yield return null; // Wait 1 frame before destroying the object so that we don't destroy the object in the same stack that applied the pull force

            m_ActivateTargets();

            _CancelInteractionAction();

            OnObjectBreak?.Invoke();

            _dismantle.Activate(); //This might destroy the object
        }

        private void _CancelInteractionAction()
        {
            if (_cancelActionInvoked == false)
            {
                _interactionCancelledAction?.Invoke();
                _cancelActionInvoked = true;
            }
        }

        private void _ApplyForce(float intensity)
        {
            //Apply the pulling force
            _forceApplied += intensity;
            if (_forceApplied >= _breakForceThreshold)
            {
                StartCoroutine(_Co_BreakElement());
                _elementIsBroken = true;
            }
        }

        #region Overrides from Activator's IInteractable interface
        protected override void m_Interact(GameObject callingObject)
        {
            base.m_Interact(callingObject);
            if (_reactToInteraction)
            {
                _ApplyForce(_forceAppliedOnInteract);
            }
        }
        protected override void m_InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            _ApplyForce(intensity);
        }

        protected override bool m_IsInteractable(GameObject initiatorObject)
        {
            return true;
        }
        protected override void m_InteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            _interactionCancelledAction = onInteractionCancelled;
            _ApplyForce(intensity);
        }

        protected override void m_InteractionWithForceStop(Vector3 direction, float intensity)
        {
            
        }
        #endregion Overrides from Activator's IInteractable interface
    }
}
