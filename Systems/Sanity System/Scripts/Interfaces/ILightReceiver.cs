using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface ILightReceiver
    {
        public void IEntityEnteredLight(GameObject emitter);
        public void IEntityExitLight(GameObject emitter);
        public void IApplyLightHealingEffect(LightHealer healer);
        public GameObject IReceivingObject();
    }
}