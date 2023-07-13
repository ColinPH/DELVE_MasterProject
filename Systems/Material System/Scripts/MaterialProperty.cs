using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Properties/" + nameof(MaterialProperty))]
    public class MaterialProperty : WorldObject
    {
        [SerializeField] MaterialType _materialType = MaterialType.Unassigned;


        public MaterialType materialType => _materialType;

    }
}
