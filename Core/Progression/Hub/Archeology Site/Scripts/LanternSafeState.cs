using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class LanternSafeState : BaseSafeState
    {
        public LanternSafeState() { }
        public LanternSafeState(bool isIgnited)
        {
            _isIgnited = isIgnited;
        }
        public LanternSafeState(LanternSafeState original)
        {
            _isIgnited = original.isIgnited;
        }

        [SerializeField] bool _isIgnited = false;



        public bool isIgnited { get => _isIgnited; set => _isIgnited = value; }
    }
}