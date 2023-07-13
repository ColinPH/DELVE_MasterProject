using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(ActivationLock))]
    public class ActivationLock : WorldObject
    {
        public enum ActivationType { LinearNoActivatorMatch = 0, LinearActivatorMatch = 1, RandomActivatorMatch = 2, }
        [SerializeField] ActivationType _activationType = ActivationType.LinearNoActivatorMatch;
        [SerializeField] bool _activateTargetOnCompletion = true;
        [SerializeField] UnityEvent _activationProcessComplete;
        [SerializeField] List<ActivationStep> _activationProcess = new List<ActivationStep>();
        [SerializeField, Runtime(true)] int _processProgressionIndex = 0;

        private ActivationTarget _activationTarget;
        private List<Activator> _registeredActivators = new List<Activator>();
        List<Activator> _processActivators = new List<Activator>();
        private bool _checkedForMissingActivators = false;
        [SerializeField, Runtime] bool _lockIsActive = true;

        public override string worldName => nameof(ActivationLock);
        protected override void MyAwake()
        {
            base.MyAwake();
            _activationTarget = m_FetchForComponent<ActivationTarget>();
            _activationTarget.OnActivatorRegistration += _OnNewActivatorRegistered;
            _activationTarget.OnActivationFromActivator += _OnActivatorActivation;
            _activationTarget.RegisterActivationLock(_ActivationLock);

            //Collect the Activators in the process list
            foreach (var item in _activationProcess)
            {
                if (_processActivators.Contains(item.stepActivator) == false)
                    _processActivators.Add(item.stepActivator);
                //Log Error if an activator has been assigned to the process, but that activator doesn't include the activation target in its targets
                if (item.stepActivator.activationTargets.Contains(_activationTarget) == false)
                    Debug.LogError($"The {nameof(Activator)} \"{item.stepActivator.gameObject.name}\" is assigned " +
                        $"in the {nameof(ActivationLock)} process on object \"{gameObject.name}\". But that " +
                        $"{nameof(Activator)} does not include the {nameof(ActivationTarget)} \"{gameObject.name}\" as one of its targets.");
            }
        }

        /// <summary> Should return whether the lock is active. FALSE should enable the activation. </summary>
        private bool _ActivationLock()
        {
            return _lockIsActive;
        }

        private void _OnNewActivatorRegistered(Activator newlyRegisteredActivator)
        {
            if (_registeredActivators.Contains(newlyRegisteredActivator) == false)
                _registeredActivators.Add(newlyRegisteredActivator);

            //Check if the process contains the activator, show warning if not
            if (_processActivators.Contains(newlyRegisteredActivator) == false)
            {
                Debugger.LogWarning($"The {nameof(ActivationTarget)} \"{gameObject.name}\" has been assigned " +
                    $"to the {nameof(Activator)} \"{newlyRegisteredActivator.gameObject.name}\" but that activator is " +
                    $"not part of the {nameof(ActivationLock)} process on object \"{gameObject.name}\".");
            }
        }

        private void _OnActivatorActivation(Activator activatedActivator)
        {
            //Check if any of the required activators is not in the registered activators list
            _CheckForMissingActivatorsInRegisteredList();

            switch (_activationType)
            {
                case ActivationType.LinearNoActivatorMatch:
                    //Unlock the next step in the process regardless of the activator
                    _activationProcess[_processProgressionIndex].ValidateStep();
                    _processProgressionIndex += 1;
                    break;
                case ActivationType.LinearActivatorMatch:
                    //Show warning if the activator is not part of the process list, and return
                    if (_processActivators.Contains(activatedActivator) == false)
                        Debugger.LogWarning($"The {nameof(Activator)} \"{activatedActivator.gameObject.name}\" is activating the {nameof(ActivationTarget)} \"{gameObject.name}\" but it is not part of the {nameof(ActivationLock)} process.");
                    
                    //Unlock next step if next step activator is null or matches the activated activator
                    if (_activationProcess[_processProgressionIndex].stepActivator == activatedActivator)
                    {
                        _activationProcess[_processProgressionIndex].ValidateStep();
                        _processProgressionIndex += 1;
                    }
                    break;
                case ActivationType.RandomActivatorMatch:
                    //Unlock the process step matching the activated Activator
                    foreach (ActivationStep step in _activationProcess)
                    {
                        if (step.stepActivator == activatedActivator)
                            step.ValidateStep();
                    }
                    break;
                default:
                    Debugger.LogError($"Case not implemented \"{_activationType}\" in component \"{nameof(ActivationLock)}\".");
                    break;
            }

            _CheckForActivationProcessComplete();
        }

        private void _CheckForMissingActivatorsInRegisteredList()
        {
            if (_checkedForMissingActivators) return;
            _checkedForMissingActivators = true;

            bool allActivatorsAreRegistered = true;
            string missingActivators = "";
            foreach (ActivationStep step in _activationProcess)
            {
                if (step.stepActivator == null) continue;
                if (_registeredActivators.Contains(step.stepActivator) == false)
                {
                    allActivatorsAreRegistered = false;
                    missingActivators += step.stepActivator.gameObject.name + "\n";
                }
            }

            if (allActivatorsAreRegistered == false)
                Debug.LogError($"The following {nameof(Activator)} objects have been assigned in " +
                    $"the activation process but won't activate the {nameof(ActivationTarget)} \"{gameObject.name}\" " +
                    $"because it has not been assigned as a target: \n{missingActivators}");
        }

        private void _CheckForActivationProcessComplete()
        {
            bool allStepsAreValid = true;
            foreach (ActivationStep step in _activationProcess)
            {
                if (step.isValidated == false)
                {
                    allStepsAreValid = false;
                    break;
                }
            }

            if (allStepsAreValid)
            {
                _lockIsActive = false;
                //Here the activation process has completed
                _activationProcessComplete?.Invoke();

                if (_activateTargetOnCompletion)
                {
                    //Remove the lock
                    _activationTarget.Activate();
                }
            }
        }
    }
}
