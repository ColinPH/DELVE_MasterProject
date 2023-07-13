using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SE_Tutorial_FinalBridgeLightUp : WorldObject
    {
        [SerializeField] Door _doorToOpen;
        [SerializeField] float _timeBeforeDoorOpening = 0.4f;
        [SerializeField] List<BrazierWave> _brazierWaves = new List<BrazierWave>();

        private bool _hasBeenTriggered = false;

        public override string worldName => nameof(SE_Tutorial_FinalBridgeLightUp);

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;

            if (_hasBeenTriggered) return;
            _hasBeenTriggered = true;

            StartCoroutine(_Co_LightUpBraziers());
        }


        IEnumerator _Co_LightUpBraziers()
        {
            //Turn on the braziers
            foreach (var item in _brazierWaves)
            {
                yield return new WaitForSecondsRealtime(item.activateAfter);
                item.igniter.Activate();
            }
            yield return new WaitForSecondsRealtime(_timeBeforeDoorOpening);
            _doorToOpen.Activate();
        }
    }
}
