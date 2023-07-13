using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class RunCompleter : MonoBehaviour
    {
        [SerializeField] VoiceLine _completeWithAllTotemsVoiceLine;
        [SerializeField] VoiceLine _completeWithoutAllTotemsVoiceLine;
        public string _targetTag = "Player";
        bool _hasBeenActivated = false;
        private void OnTriggerEnter(Collider other)
        {
            if (_hasBeenActivated) return;

            if (other.gameObject.tag == _targetTag)
            {
                _hasBeenActivated = true;
                StartCoroutine(_Co_WaitBeforeEndingRun());
            }
        }

        IEnumerator _Co_WaitBeforeEndingRun()
        {
            if (Managers.totemManager.AllFragmentsDeposited())
            {
                //If the player has all the totems, complete the run
                Sound.PlayVoiceLine(_completeWithAllTotemsVoiceLine, gameObject);
                yield return new WaitForSecondsRealtime(_completeWithAllTotemsVoiceLine.ScreenTime);
                Managers.runManager.CompleteRun();
            }
            else
            {
                //If the player does not have all the totems, complete the run
                Sound.PlayVoiceLine(_completeWithoutAllTotemsVoiceLine, gameObject);
                yield return new WaitForSecondsRealtime(_completeWithoutAllTotemsVoiceLine.ScreenTime);
                Managers.runManager.FailRun();
            }
        }
    }
}
