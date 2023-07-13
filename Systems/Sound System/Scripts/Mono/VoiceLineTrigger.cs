using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class VoiceLineTrigger : WorldObject
    {
        [SerializeField] VoiceLineBase _voiceLineToPlay;
        bool _hasBeenTriggered = false;

        #region WorldObject overrides
        public override string worldName => nameof(VoiceLineTrigger);
        protected override void MyAwake()
        {
            base.MyAwake();
            m_EnsureGameObjectHasTrigger();

            //Read from the safe state if there is one
            Managers.saveManager.RegisterObjectForActivation(worldID, new VoiceLineTriggerSafeState());
            if (Managers.saveManager.GetObjectSafeState(worldID, out BaseSafeState baseState))
            {
                VoiceLineTriggerSafeState triggerSafeState = (VoiceLineTriggerSafeState)baseState;
                _hasBeenTriggered = triggerSafeState.hasBeenPlayed;
            }
        }
        #endregion WorldObject overrides

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;

            if (_hasBeenTriggered) return;

            Sound.PlayVoiceLine(_voiceLineToPlay, gameObject);
            _hasBeenTriggered = true;
            Managers.saveManager.GetObjectSafeState<VoiceLineTriggerSafeState>(worldID).hasBeenPlayed = true;
        }

    }
}
