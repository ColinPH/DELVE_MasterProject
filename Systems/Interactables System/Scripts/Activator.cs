using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class Activator : WorldObject, IInteractable
    {
        [SerializeField] protected List<ActivationTarget> m_activationTargets = new List<ActivationTarget>();

        public List<ActivationTarget> activationTargets => m_activationTargets;

        protected override void MyStart()
        {
            base.MyStart();

            if (m_activationTargets.Count == 0) return;

            foreach (var item in m_activationTargets)
            {
                item.RegisterActivator(this);
            }
        }

        public void RemoveActivationTarget(ActivationTarget targetToRemove)
        {
            if (m_activationTargets.Contains(targetToRemove) == false)
            {
                Debugger.LogError($"The Activator \"{gameObject.name}\" does not contain the {nameof(ActivationTarget)} \"{targetToRemove.worldName}\" on object \"{targetToRemove.gameObject.name}\".");
                return;
            }
            m_activationTargets.Remove(targetToRemove);
        }

        protected void m_ActivateTargets()
        {
            foreach (var item in m_activationTargets)
            {
                item.ActivateFromActivator(this);
            }
        }

        protected void m_DeactivateTargets()
        {
            foreach (var item in m_activationTargets)
            {
                item.DeactivateFromActivator(this);
            }
        }

        /// <summary> To be used by other classes of by custom inspector to activate the ActivationTargets. Inheriting classes should call m_ActivateTargets(). </summary>
        public void ActivateTargets()
        {
            m_ActivateTargets();
        }

        /// <summary> To be used by other classes of by custom inspector to deactivate the ActivationTargets. Inheriting classes should call m_DeactivateTargets(). </summary>
        public void DeactivateTargets()
        {
            m_DeactivateTargets();
        }

        //--------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------
        #region IInteractable interface

        public string GetInteractionText()
        {
            return m_GetInteractionText();
        }

        public void Highlight()
        {
            m_Highlight();
        }

        public void Interact(GameObject callingObject)
        {
            m_Interact(callingObject);
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            m_InteractWithForceContinuous(forceOrigin, direction, intensity);
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            m_InteractionWithForceStart(forceOrigin, direction, intensity, pullingObject, caller, onInteractionCancelled);
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            m_InteractionWithForceStop(direction, intensity);
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            return m_IsInteractable(initiatorObject);
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            m_InteractionStart(onInteractionCancelled);
        }

        public void OnInteractionStop()
        {
            m_InteractionStop();
        }

        #endregion
        //--------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------
        #region Virtual functions for IInteractable interface

        protected virtual string m_GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }

        protected virtual void m_Highlight()
        {

        }

        protected virtual void m_Interact(GameObject callingObject)
        {

        }

        protected virtual void m_InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {

        }

        protected virtual void m_InteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {

        }

        protected virtual void m_InteractionWithForceStop(Vector3 direction, float intensity)
        {

        }

        protected virtual bool m_IsInteractable(GameObject initiatorObject)
        {
            return true;
        }

        protected virtual void m_InteractionStart(Action onInteractionCancelled)
        {

        }

        protected virtual void m_InteractionStop()
        {

        }

        #endregion
    }
}