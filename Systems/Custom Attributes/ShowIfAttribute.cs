using UnityEngine;

namespace PropellerCap
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string showProperty;
        public bool isInverted;
        public int targetValue;
        public bool isBoolProperty = false;
        public ShowIfAttribute(string boolPropertyName, bool isInverted = false)
        {
            showProperty = boolPropertyName;
            this.isInverted = isInverted;
            isBoolProperty = true;
        }

        public ShowIfAttribute(string targetIntPropertyName, int targetValue)
        {
            this.showProperty = targetIntPropertyName;
            this.targetValue = targetValue; 
            this.isBoolProperty = false;
        }
    }
}
