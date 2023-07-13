using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class BrazierSafeState : BaseSafeState
    {
        public BrazierSafeState() { }
        public BrazierSafeState(bool isIgnited, LightType lightType)
        {
            _isIgnited = isIgnited;
            _lightType = lightType;
        }
        public BrazierSafeState(BrazierSafeState original)
        {
            _isIgnited = original.isIgnited;
            _lightType = original.lightType;
        }

        [SerializeField] bool _isIgnited = false;
        [SerializeField] LightType _lightType = LightType.Fire;

        

        public bool isIgnited { get => _isIgnited; set => _isIgnited = value; }
        public LightType lightType { get => _lightType; set => _lightType = value; }
    }
}