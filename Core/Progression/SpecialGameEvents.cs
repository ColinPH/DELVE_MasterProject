using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SpecialGameEvents : MonoBehaviour
    {
        [SerializeField] VoiceLineBase _100CoinsCollectedVoiceLine;
        [SerializeField] VoiceLineBase _200CoinsCollectedVoiceLine;
        [SerializeField] VoiceLineBase _300CoinsCollectedVoiceLine;
        [SerializeField] VoiceLineBase _400CoinsCollectedVoiceLine;
        [SerializeField] VoiceLineBase _500CoinsCollectedVoiceLine;
        [SerializeField] VoiceLineBase _1000CoinsCollectedVoiceLine;

        private void Start()
        {
            DontDestroyOnLoad(this);
            Metrics.OnCoinCollected += _OnCoinCollected;
        }

        private void _OnCoinCollected()
        {
            if (Metrics.player.collectedCoinsAmount >= 1000 && Saver.progression.reachedCoinThreshold_1000 == false)
            {
                Sound.PlayVoiceLine(_1000CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_1000 = true;
                Metrics.OnCoinCollected -= _OnCoinCollected;
            }
            else if (Metrics.player.collectedCoinsAmount >= 500 && Saver.progression.reachedCoinThreshold_500 == false)
            {
                Sound.PlayVoiceLine(_500CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_500 = true;
            }
            else if (Metrics.player.collectedCoinsAmount >= 400 && Saver.progression.reachedCoinThreshold_400 == false)
            {
                Sound.PlayVoiceLine(_400CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_400 = true;
            }
            else if (Metrics.player.collectedCoinsAmount >= 300 && Saver.progression.reachedCoinThreshold_300 == false)
            {
                Sound.PlayVoiceLine(_300CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_300 = true;
            }
            else if (Metrics.player.collectedCoinsAmount >= 200 && Saver.progression.reachedCoinThreshold_200 == false)
            {
                Sound.PlayVoiceLine(_200CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_200 = true;
            }
            else if (Metrics.player.collectedCoinsAmount >= 100 && Saver.progression.reachedCoinThreshold_100 == false)
            {
                Sound.PlayVoiceLine(_100CoinsCollectedVoiceLine, gameObject);
                Saver.progression.reachedCoinThreshold_100 = true;
            }
        }
    }
}
