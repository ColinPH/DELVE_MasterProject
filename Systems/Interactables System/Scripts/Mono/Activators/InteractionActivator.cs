using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activators/" + nameof(InteractionActivator))]
    public class InteractionActivator : Activator
    {
        [SerializeField] bool _singleCycle = true;
        
        [SerializeField, Runtime(true)] bool _singleCycleEnded = false;
        [SerializeField, Runtime] bool _isActivated = false;

        #region WorldObject overrides
        public override string worldName => nameof(InteractionActivator);
        #endregion WorldObject overrides

        protected override void m_Interact(GameObject callingObject)
        {
            base.m_Interact(callingObject);

            if (_singleCycleEnded && _singleCycle)
                return;
            _singleCycleEnded = true;

            if (_isActivated)
            {
                m_DeactivateTargets(); //If we are already active we deactivate the targets
                _isActivated = false;
            }
            else
            {
                m_ActivateTargets(); //If we are not active we activate them
                _isActivated = true;
            }
        }
    }
}