using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IActivatable
    {
        //Activation
        void Activate();
        delegate void ActivationStartHandler();
        void RegisterToActivationStart(ActivationStartHandler activationStartDelegate);
        delegate void ActivationCompleteHandler();
        void RegisterToActivationComplete(ActivationCompleteHandler activationCompleteDelegate);

        //Deactivation
        void Deactivate();
        delegate void DeactivationStartHandler();
        void RegisterToDeactivationStart(DeactivationStartHandler deactivationStartDelegate);
        delegate void DeactivationCompleteHandler();
        void RegisterToDeactivationComplete(DeactivationCompleteHandler deactivationCompleteDelegate);
    }
}