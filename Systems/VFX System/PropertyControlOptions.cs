using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public enum PropertyControlType
    {
        Set = 0,
        Increment = 1,
        PercentageMapping = 2,
    }

    [Serializable]
    public class PropertyControlOptions
    {
        public string propertyName = "";
        public PropertyControlType controlType = PropertyControlType.Set;
        public bool hasInitialValue = true;
        public float initialValue = 0f;
        public bool isClamped = true;
        public float minValue = 0f;
        public float maxValue = 1f;
        [Header("Increment")]
        public float incrementValue = 0f;
        public bool useDeltaTime = true;
        [Header("Set")]
        public float newValue = 0f; 
        [Header("Percentage Mapping")]
        public float percentageFrom = 0f;
        public float percentageTo = 1f;
        public bool inversePercentage = false;
        public float percentageDiff => percentageTo - percentageFrom;
    }
}
