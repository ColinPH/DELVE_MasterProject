using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SE_FirstSanityLoss : WorldObject
    {
        [SerializeField] VoiceLineBase _firstSanityLossVoiceLine;
        [SerializeField] bool _hasBeenActivated = false;

        public override string worldName => nameof(SE_FirstSanityLoss);

        protected override void MyStart()
        {
            base.MyStart();
            Managers.sanityManager.onSanityChange += _OnSanityChange;
        }

        private void _OnSanityChange(float newSanity, float oldSanity, float maxSanity)
        {
            if (_hasBeenActivated)
            {
                Managers.sanityManager.onSanityChange -= _OnSanityChange;
                return;
            }

            if (newSanity < oldSanity)
            {
                _hasBeenActivated = true;
                Sound.PlayVoiceLine(_firstSanityLossVoiceLine, gameObject);
                Managers.sanityManager.onSanityChange -= _OnSanityChange;
            }
        }
    }
}
