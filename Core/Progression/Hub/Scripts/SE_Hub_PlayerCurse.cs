using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    //SE stands for Special Event
    public class SE_Hub_PlayerCurse : ActivationTarget
    {
        [SerializeField] GameObject timelineDirector = null;
        [SerializeField] VisualEffectClip _curseVFX;
        [SerializeField] VoiceLineBase _curseVoiceLine;
        [SerializeField] SoundClip _hubCurseSoundEffect;
        [SerializeField] SoundClip _curseRumbleSound;
        [SerializeField] float _timeBeforeLightsIgnite = 1.5f;
        [SerializeField] GameObject _treasureObject;
        [SerializeField] Lever _leverToUnlock;
        [Range(0f, 1f)]
        [SerializeField] float _sanityEffectPercentage = 0.2f;
        [SerializeField] string _sanityEffectShaderPropretyName = "_Mask_Size";
        [SerializeField] GameObject _curseLights;
        [SerializeField] List<BrazierWave> _braziersLightUp = new List<BrazierWave>();

        private GameObject VFXObj = null;

        #region WorldObject overrides
        public override string worldName => nameof(SE_Hub_PlayerCurse);
        protected override void MyStart()
        {
            base.MyStart();
            
            //Check if the curse has already been played, if so destroy the treasure object
            if (Saver.progression.curseApplied)
            {
                //Unlock the lever
                _leverToUnlock.UnlockLever();
                
                //Activate the lights
                foreach (var item in _braziersLightUp)
                {
                    item.igniter.Activate();
                }

                TurnOffCurseLights();

                //Destroy the treasure
                Destroy(_treasureObject);
            }
            else
            {
                //Turn off the braziers
                foreach (var item in _braziersLightUp)
                {
                    item.igniter.Deactivate();
                }
            }
        }
        #endregion WorldObject overrides



        protected override void m_Activate()
        {
            base.m_Activate();

            _PlayCurseEvent();
        }

        private void _PlayCurseEvent()
        {
            //StartCoroutine(_Co_CurseEventSequence());
            timelineDirector.SetActive(true);
        }

        IEnumerator _Co_CurseEventSequence()
        {
            //Stop player movement
            StopPlayerMovement();

            //TODO Play pickup animation

            //Remove the object
            RemoveTreasureObject();

            //Play the voice line
            PlayVoiceLine();

            //Turn off the curse lights
            TurnOffCurseLights();

            //Spawn the curse VFX
            SpawnCurseVFX();

            //Show sanity effect
            ShowSanityEffect();

            //yield return new WaitForSecondsRealtime(_timeBeforeLightsIgnite);

            //Remove sanity effect
            HideSanityEffect();

            //Turn on the braziers
            //foreach (var item in _braziersLightUp)
            //{
            //    yield return new WaitForSecondsRealtime(item.activateAfter);
            //    item.igniter.Activate();
            //}

            //Unlock the lever
            UnlockLever();

            //Resume player movement
            ResumePlayerMovement();

            //Destroy the VFX
            DestroyCurseVFX();

            yield return null;
        }

        public void PlayRumbleSound()
        {
            Sound.PlaySound(_curseRumbleSound, gameObject);
        }

        public void RemoveTreasureObject()
        {
            Destroy(_treasureObject);
        }

        public void TurnOffCurseLights()
        {
            _curseLights.SetActive(false);
        }

        public void DestroyCurseVFX()
        {
            Destroy(VFXObj);
        }

        public void SpawnCurseVFX()
        {
            VFXObj = VisualEffects.SpawnVFX(_curseVFX, Player.PlayerObject.transform);
            Sound.PlaySound(_hubCurseSoundEffect, gameObject);
        }

        public void PlayVoiceLine()
        {
            Sound.PlayVoiceLine(_curseVoiceLine, gameObject);
        }

        public void HideSanityEffect()
        {
            HUD.customPassHandler.DisableCustomPass(CustomPassType.InsanityEffect);
        }

        public void ShowSanityEffect()
        {
            HUD.customPassHandler.EnableCustomPass(CustomPassType.InsanityEffect);
            Material _targetMaterial = HUD.customPassHandler.GetPassMaterial(CustomPassType.InsanityEffect, false);
            PropertyControlOptions options = new PropertyControlOptions();
            options.propertyName = _sanityEffectShaderPropretyName;
            options.initialValue = _sanityEffectPercentage;
            ShaderPropertyController _propertyController = new ShaderPropertyController(_targetMaterial, options);
            _propertyController.UpdateProperty();
        }

        public void UnlockLever()
        {
            _leverToUnlock.UnlockLever();
        }

        public void StartSanity()
        {
            
            Sanity.Start();

        }

        public void ResumePlayerMovement()
        {
            Player.PlayerObject.GetComponent<PlayerInputController>().EnableInputs();

            Saver.progression.curseApplied = true;
        }

        public void StopPlayerMovement()
        {
            Player.PlayerObject.GetComponent<PlayerInputController>().DisableInputs();
        }
    }
}
