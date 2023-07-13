using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    [Serializable]
    public class ActivationStep
    {
        public Activator stepActivator;
        public bool isValidated = false;
        public UnityEvent stepActivation;

        public void ValidateStep()
        {
            if (isValidated) return;

            stepActivation?.Invoke();
            isValidated = true;
        }
    }
}
