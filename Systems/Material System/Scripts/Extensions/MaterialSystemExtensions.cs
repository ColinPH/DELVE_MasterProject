using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public static class MaterialSystemExtensions
    {
        public static MaterialType GetMaterialType(this GameObject obj)
        {
            //Try to find the material property on the given object
            if (obj.TryGetComponent(out MaterialProperty matProperty))
                return matProperty.materialType;

            //Then try on the main object
            GameObject mainObj = obj.GetMainObject();
            if (mainObj.TryGetComponent(out MaterialProperty materialProperty))
                return materialProperty.materialType;

            //If both don't work then there is no MaterialProperty component assigned
            /*Debugger.LogError($"Failed to retrieve the {nameof(MaterialProperty)} from object \"{obj.name}\". " +
                $"There is no {nameof(MaterialProperty)} component on the " +
                $"object \"{obj.name}\" nor on its main object \"{mainObj.name}\". " +
                $"Is it missing the DetectionParent/Child components ?");*/
            return MaterialType.Unassigned;
        }
    }
}
