using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Metrics/" + nameof(MetricZoneStart))]
    public class MetricZoneStart : WorldObject
    {
        [SerializeField] string _zoneIdentifier = "";

        [SerializeField, Runtime(true)] bool _hasBeenActivated = false;

        public override string worldName => nameof(MetricZoneStart);

        protected override void MonoAwake()
        {
            base.MonoAwake();
            m_EnsureGameObjectHasTrigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player" || _hasBeenActivated) return;

            //Register the start of a zone
            Metrics.manager.StartZone(_zoneIdentifier);
            _hasBeenActivated = true;
        }
    }
}