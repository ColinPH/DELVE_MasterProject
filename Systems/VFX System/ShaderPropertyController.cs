using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class ShaderPropertyController
    {
        public Material targetMaterial { get; set; }
        public PropertyControlOptions options { get; set; }

        private float _percentageValue = 0f;

        public ShaderPropertyController(Material targetMaterial, PropertyControlOptions controlOptions)
        {
            this.targetMaterial = targetMaterial;
            this.options = controlOptions;

            if (options.hasInitialValue)
                _SetValue(options.initialValue);
        }

        public void SetPercentage(float percentage)
        {
            if (percentage < 0f || percentage > 1f)
                Debugger.LogError($"Perentage is not between 0 and 1. Percentage is {percentage}");
            _percentageValue = percentage;
        }

        public void SetTargetValue(float targetValue)
        {
            options.newValue = targetValue;
        }

        public void SetIncrementValue(float incrementValue)
        {
            options.incrementValue = incrementValue;
        }

        public void UpdateProperty()
        {
            switch (options.controlType)
            {
                case PropertyControlType.Set:
                    _SetValue(options.newValue);
                    break;
                case PropertyControlType.Increment:                   
                    _SetValue(GetPropertyValue() + options.incrementValue * (options.useDeltaTime ? Time.deltaTime : 1f));
                    break;
                case PropertyControlType.PercentageMapping:
                    float tempPercentage = _percentageValue;
                    if (options.inversePercentage)
                        tempPercentage = 1f - tempPercentage;
                    _SetValue(options.percentageFrom + options.percentageDiff * tempPercentage);
                    break;
            }
        }

        public float GetPropertyValue()
        {
            return targetMaterial.GetFloat(options.propertyName);
        }

        private void _SetValue(float newValue)
        {
            if (options.isClamped)
                newValue = Mathf.Clamp(newValue, options.minValue, options.maxValue);
            targetMaterial.SetFloat(options.propertyName, newValue);
        }
    }
}
