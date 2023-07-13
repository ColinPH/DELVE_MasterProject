using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class PlayerCharacter : WorldObject
    {
        [SerializeField] VoiceLineBase _firstDeathVoiceLine;
        [SerializeField] VoiceLineBase _secondDeathVoiceLine;
        [SerializeField] VoiceLineBase _playerDeathVoiceLine;
        [SerializeField] SoundClip _playerDeathSound;
        [SerializeField] SoundClip _heartbeatSound;
        [HideInInspector] public StateMachineBehaviour stateMachineBehaviour { get; set; }

        LightReceiver _lightReceiver;
        SanityReceiver _sanityReceiver;

        protected override void MonoAwake()
        {
            if (Managers.playerManager.RegisterPlayer(this) == false)
                return; //If couldn't regiter, it means there should already be a player, we do nothing because the object will be destroyed

            _lightReceiver = m_FetchForComponent<LightReceiver>();
            _lightReceiver.OnLightEntered += _OnLightEntered;
            _lightReceiver.OnLightExit += _OnLightExit;
            _lightReceiver.onLightHealingEffect += _OnLightHealingEffect;

            _sanityReceiver = m_FetchForComponent<SanityReceiver>();
            _sanityReceiver.onSanityAfflictorEntered += _OnSanityAfflictorEntered;
            _sanityReceiver.onSanityAfflictorExit += _OnSanityAfflictorExit;
            _sanityReceiver.onSanityAfflictorAcquired += _OnSanityAfflictorAcquired;
            _sanityReceiver.onSanityAfflictorLost += _OnSanityAfflictorLost;
        }

        protected override void MyAwake()
        {
            stateMachineBehaviour = m_FetchForComponentInChildren<StateMachineBehaviour>();
        }


        public void OnPlayerDeathSound()
        {
            Sound.PlaySound(_playerDeathSound, gameObject);

            //If the player has not been cursed yet we do not play the voicelines
            if (Saver.progression.curseApplied == false) return;

            if (Metrics.player.deathsAmount == 1)
                Sound.PlayVoiceLine(_firstDeathVoiceLine, gameObject);
            else if (Metrics.player.deathsAmount == 2)
                Sound.PlayVoiceLine(_secondDeathVoiceLine, gameObject);
            else
                Sound.PlayVoiceLine(_playerDeathVoiceLine, gameObject);
        }


        #region Light and Sanity

        float _lastSanityValue = 0f;
        public void StartHeartbeat()
        {
            Sound.PlaySound(_heartbeatSound, gameObject);
            //Debug.Log("Start heart beat");
        }
        public void SetHeartbeatValue(float currentSanity, float _heartBeatsThreshold)
        {
            if (Mathf.Abs(currentSanity - _lastSanityValue) >= 1f)
            {
                _lastSanityValue = currentSanity;
                //Map the sanity to match 0 to 100 where 0 is rest and 100 is extreme heartbeats
                float mappedValue = (_heartBeatsThreshold - currentSanity) / _heartBeatsThreshold;

                Sound.SetRTCP(_heartbeatSound, gameObject, mappedValue * 100f);
            }
        }
        public void StopHeartbeat()
        {
            Sound.StopSound(_heartbeatSound, gameObject);
            //Debug.Log("Stop heart beat");
        }

        private void _OnLightExit()
        {
            Managers.playerManager.PlayerHasExitLight();
        }

        private void _OnLightEntered()
        {
            Managers.playerManager.PlayerHasEnteredLight();
        }

        private void _OnLightHealingEffect(LightHealer healer)
        {
            Sanity.Add(healer.HealingAmount, false);
        }

        private void _OnSanityAfflictorEntered()
        {
            Managers.playerManager.PlayerHasEnteredSanityAfflictor();
        }

        private void _OnSanityAfflictorExit()
        {
            Managers.playerManager.PlayerHasExitSanityAfflictor();
        }

        private void _OnSanityAfflictorAcquired(SanityAfflictor sanityAfflictor)
        {
            Sanity.AddMultiplier(sanityAfflictor.sanityMultiplier, sanityAfflictor.UniqueIdentifier);
        }

        private void _OnSanityAfflictorLost(SanityAfflictor sanityAfflictor)
        {
            Sanity.RemoveMultiplier(sanityAfflictor.sanityMultiplier, sanityAfflictor.UniqueIdentifier);
        }
        #endregion Light and Sanity


    }
}
