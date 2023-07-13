using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(Igniter))]
    public class Igniter : ActivationTarget
    {
        [SerializeField] bool _canBeActivated = true;
        [SerializeField] bool _canBeDeactivated = true;
        [SerializeField] LightType _targetLightType = LightType.Fire;
        [SerializeField] List<GameObject> _flammableTargets = new List<GameObject>();

        #region ActivationTarget overrides

        public override void Activate()
        {
            base.Activate();

            if (_canBeActivated == false) return;
            
            foreach (var item in _flammableTargets)
            {
                if (item.TryGetComponent(out IFlammable flammable))
                {
                    flammable.Ignite(_targetLightType);
                }
                else
                {
                    item.SetActive(true);
                }
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_canBeDeactivated == false) return;

            foreach (var item in _flammableTargets)
            {
                if (item.TryGetComponent(out IFlammable flammable))
                {
                    flammable.Extinguish();
                }
                else
                {
                    item.SetActive(false);
                }
            }
        }

        #endregion
    }
}
