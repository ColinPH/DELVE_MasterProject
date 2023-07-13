using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Metrics/" + nameof(MetricZoneStop))]
    public class MetricZoneStop : WorldObject
    {
        [SerializeField] string _zoneIdentifier = "";

        [SerializeField, Runtime(true)] bool _hasBeenActivated = false;

        public override string worldName => nameof(MetricZoneStop);

        protected override void MonoAwake()
        {
            base.MonoAwake();
            m_EnsureGameObjectHasTrigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player" || _hasBeenActivated) return;

            //Register the end of a zone
            Metrics.manager.StopZone(_zoneIdentifier);
            _hasBeenActivated = true;
        }
    }
}