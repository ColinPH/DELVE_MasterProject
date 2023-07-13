using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    public abstract class ActivationTarget : WorldObject, IActivatable
    {
        protected List<Activator> m_controllingActivators = new List<Activator>();
        
        IActivatable.ActivationCompleteHandler _OnActivationComplete { get; set; }
        IActivatable.DeactivationCompleteHandler _OnDeactivationComplete { get; set; }
        IActivatable.ActivationStartHandler _OnActivationStart { get; set; }
        IActivatable.DeactivationStartHandler _OnDeactivationStart { get; set; }


        #region Public accessors and events
        public bool activationIsLocked => _ActivationIsLocked();
        public delegate bool ActivationLockHandler();
        private ActivationLockHandler _activationLocks { get; set; }

        public delegate void ActivatorRegistrationHandler(Activator newlyRegisteredActivator);
        public ActivatorRegistrationHandler OnActivatorRegistration { get; set; }
        public delegate void ActivationFromActivatorHandler(Activator initiatingActivator);
        public ActivationFromActivatorHandler OnActivationFromActivator { get; set; }
        public ActivationFromActivatorHandler OnDeactivationFromActivator { get; set; }
        #endregion Public events and accessors

        protected override void MyAwake()
        {
            base.MyAwake();
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            foreach (var item in m_controllingActivators)
            {
                item.RemoveActivationTarget(this);
            }
        }

        public void RegisterActivator(Activator controllingActivator)
        {
            if (m_controllingActivators.Contains(controllingActivator))
            {
                Debugger.LogError($"The object \"{controllingActivator.gameObject.name}\" is already registered as Activator in the {nameof(ActivationTarget)} \"{worldName}\"");
                return;
            }
            m_controllingActivators.Add(controllingActivator);
            OnActivatorRegistration?.Invoke(controllingActivator);
        }

        public void RegisterActivationLock(ActivationLockHandler activationLock)
        {
            _activationLocks += activationLock;
        }
        public void DeregisterActivationLock(ActivationLockHandler activationLock)
        {
            _activationLocks -= activationLock;
        }

        /// <summary> Invokes the event OnActivationFromActivator for potential activation contitions. And then calls Activate().</summary>
        public void ActivateFromActivator(Activator initiatingActivator)
        {
            Activate(); //Must be before the event. The activation lock uses the event and if lock opens and activates the target, we don't want to activate twice. First Activate is blocked by lock and next (called by lock) is not blocked
            OnActivationFromActivator?.Invoke(initiatingActivator);
        }
        /// <summary> Invokes the event OnDeactivationFromActivator for potential activation contitions. And then calls Deactivate().</summary>
        public void DeactivateFromActivator(Activator initiatingActivator)
        {
            Deactivate();
            OnDeactivationFromActivator?.Invoke(initiatingActivator);
        }

        #region IActivatable interface
        public virtual void Activate()
        {
            //Only activate if the lock is no longer active
            if (_ActivationIsLocked() == false)
            {
                _OnActivationStart?.Invoke();
                m_Activate();
            }
        }

        public virtual void Deactivate()
        {
            _OnDeactivationStart?.Invoke();
            m_Deactivate();
        }

        //Activation complete
        public void RegisterToActivationComplete(IActivatable.ActivationCompleteHandler callback)
        {
            _OnActivationComplete += callback;
        }
        public void RegisterToDeactivationComplete(IActivatable.DeactivationCompleteHandler callback)
        {
            _OnDeactivationComplete += callback;
        }

        //Activation start
        public void RegisterToActivationStart(IActivatable.ActivationStartHandler activationStartDelegate)
        {
            _OnActivationStart += activationStartDelegate;
        }

        public void RegisterToDeactivationStart(IActivatable.DeactivationStartHandler deactivationStartDelegate)
        {
            _OnDeactivationStart += deactivationStartDelegate;
        }
        #endregion

        #region IActivatable interface overrides
        protected virtual void m_Activate()
        {

        }

        protected virtual void m_Deactivate()
        {

        }
        #endregion IActivatable interface overrides


        protected void InvokeActivationComplete()
        {
            _OnActivationComplete?.Invoke();
        }

        protected void InvokeDeactivationComplete()
        {
            _OnDeactivationComplete?.Invoke();
        }


        private bool _ActivationIsLocked()
        {
            //If there are no locks, activation is of course possible
            if (_activationLocks == null)
                return false;

            Delegate[] locks = _activationLocks.GetInvocationList();
                        
            bool atLeastOneLockIsActive = false;
            foreach (ActivationLockHandler lockIsActive in locks)
            {
                if (lockIsActive())
                    atLeastOneLockIsActive = true;
            }
            return atLeastOneLockIsActive;
        }
    }
}