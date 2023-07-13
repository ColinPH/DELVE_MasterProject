using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class VoiceLineTriggerSafeState : BaseSafeState
    {
        public VoiceLineTriggerSafeState() { }
        public VoiceLineTriggerSafeState(bool hasBeenPlayed)
        {
            _hasBeenPlayed = hasBeenPlayed;
        }
        public VoiceLineTriggerSafeState(VoiceLineTriggerSafeState original)
        {
            _hasBeenPlayed = original.hasBeenPlayed;
        }

        [SerializeField] bool _hasBeenPlayed = false;



        public bool hasBeenPlayed { get => _hasBeenPlayed; set => _hasBeenPlayed = value; }
    }
}