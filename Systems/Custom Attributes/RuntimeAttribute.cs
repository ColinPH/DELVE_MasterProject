using UnityEngine;

namespace PropellerCap
{
    public class RuntimeAttribute : PropertyAttribute
    {
        public bool hasHeader = false;
        public RuntimeAttribute(bool isHeader = false)
        {
            hasHeader = isHeader;
        }
    }
}
