using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Flammable/" + nameof(Brazier))]
    public class Brazier : WorldObject, IFlammable, IInteractable
    {
        [SerializeField] bool _isFlammable = true;
        [SerializeField] bool _lightUpOnStart = false;
        [SerializeField] bool _crystalOverridesFire = false;
        [SerializeField] GameObject _fireLightParent;
        [SerializeField] GameObject _crystalLightParent;
        [SerializeField] GameObject _healingTriggerObject;
        [SerializeField] LightType _lightType = LightType.Fire;
        [Header("Triggers and Colliders")]
        [SerializeField] string _flareTriggerName = "FlareTrigger";
        [Header("Sounds")]
        [SerializeField] SoundClip _crystalIgniteSound;
        [SerializeField] SoundClip _crystalExtinguishSound;
        [SerializeField] SoundClip _fireIgniteSound;
        [SerializeField] SoundClip _fireExtinguishSound;
        [Header("Effects")]
        [SerializeField] VisualEffectClip _crystalIgniteVFX;
        [SerializeField] VisualEffectClip _crystalExtinguishVFX;
        [SerializeField] VisualEffectClip _fireIgniteVFX;
        [SerializeField] VisualEffectClip _fireExtinguishVFX;
        [SerializeField] Vector3 _VFXOffset = Vector3.zero;
        [SerializeField, Runtime(true)] bool _isIgnited = false;

        IFlammable.LightTypeChangeHandler _onLightTypeChange { get; set; }
        IFlammable.IgniteHandler _onIgnite { get; set; }
        IFlammable.ExtinguishHandler _onExtinguish { get; set; }
        private DetectionParent _detectionParent;

        #region WorldObject overrides
        public override string worldName => nameof(Brazier);
        protected override void MyAwake()
        {
            base.MyAwake();
            _detectionParent = m_FetchForComponent<DetectionParent>();
            _detectionParent.GetTriggerEvent(_flareTriggerName).enter += _OnFlareTriggerEnter;

            //Save the state and read it from memory if it already exists
            BrazierSafeState safeState = new BrazierSafeState(_lightUpOnStart, _lightType);
            Managers.saveManager.RegisterObjectForActivation(worldID, new BrazierSafeState(safeState));
            if (Managers.saveManager.GetObjectSafeState(worldID, out BaseSafeState baseState))
            {
                BrazierSafeState brazierSafeState = (BrazierSafeState)baseState;
                _lightUpOnStart = brazierSafeState.isIgnited;
                _lightType = brazierSafeState.lightType;
            }

            _SetStartState();
        }

        protected override void MyStart()
        {
            base.MyStart();
        }
        #endregion WorldObject overrides

        private void _OnFlareTriggerEnter(Collider other)
        {
            if (_isIgnited == false)
                return;

            Flare flare = other.gameObject.GetMainObject().GetComponent<Flare>();
            if (flare != null)
            {
                flare.SetLightType(_lightType);
            }
        }



        #region IFlammable interface
        public void Ignite(LightType lightType)
        {
            if (_isFlammable == false)
                return;

            if (_isIgnited && lightType == LightType.Crystal && _crystalOverridesFire == false)
                return;

            _healingTriggerObject.SetActive(true);

            switch (lightType)
            {
                case LightType.Crystal:
                    _crystalLightParent.SetActive(true);
                    _fireLightParent.SetActive(false);
                    //Play light up sound and spawn VFX
                    Sound.PlaySound(_crystalIgniteSound, _crystalLightParent);
                    VisualEffects.SpawnVFX(_crystalIgniteVFX, _crystalLightParent.transform.position + _VFXOffset, transform.up);
                    break;
                case LightType.Fire:
                    _fireLightParent.SetActive(true);
                    _crystalLightParent.SetActive(false);
                    //Play light up sound and spawn VFX
                    Sound.PlaySound(_fireIgniteSound, _fireLightParent);
                    VisualEffects.SpawnVFX(_fireIgniteVFX, _fireLightParent.transform.position + _VFXOffset, transform.up);
                    break;
                default:
                    Debugger.Log($"Case not implemented \"{_lightType}\" in component \"{nameof(Brazier)}\".");
                    break;
            }

            if (lightType != _lightType)
                _onLightTypeChange?.Invoke(lightType);
            
            if (_isIgnited == false)
                _onIgnite?.Invoke();

            _isIgnited = true;
            _lightType = lightType;
            Managers.saveManager.GetObjectSafeState<BrazierSafeState>(worldID).isIgnited = _isIgnited;
            Managers.saveManager.GetObjectSafeState<BrazierSafeState>(worldID).lightType = _lightType;
        }

        public void Extinguish()
        {
            if (_isIgnited == false) return;
            _isIgnited = false;
            Managers.saveManager.GetObjectSafeState<BrazierSafeState>(worldID).isIgnited = _isIgnited;

            _crystalLightParent.SetActive(false);
            _fireLightParent.SetActive(false);

            switch (_lightType)
            {
                case LightType.Crystal:
                    //Play extinguish sound and spawn VFX
                    Sound.PlaySound(_crystalExtinguishSound, _crystalLightParent);
                    VisualEffects.SpawnVFX(_crystalExtinguishVFX, _crystalLightParent.transform.position + _VFXOffset, transform.up);
                    break;
                case LightType.Fire:
                    //Play extinguish sound and spawn VFX
                    Sound.PlaySound(_fireExtinguishSound, _fireLightParent);
                    VisualEffects.SpawnVFX(_fireExtinguishVFX, _fireLightParent.transform.position + _VFXOffset, transform.up);
                    break;
                default:
                    Debugger.Log($"Case not implemented \"{_lightType}\" in component \"{nameof(Brazier)}\".");
                    break;
            }

            _onExtinguish?.Invoke();
        }

        public LightType GetLightType()
        {
            return _lightType;
        }

        public bool IsIgnited()
        {
            return _isIgnited;
        }

        public void RegisterToLightTypeChange(IFlammable.LightTypeChangeHandler lightTypeChangeDelegate)
        {
            _onLightTypeChange += lightTypeChangeDelegate;
        }

        public void RegisterToIgnite(IFlammable.IgniteHandler igniteDelegate)
        {
            _onIgnite += igniteDelegate;
        }

        public void DeregisterFromIgnite(IFlammable.IgniteHandler igniteDelegate)
        {
            _onIgnite -= igniteDelegate;
        }

        public void RegisterToExtinguish(IFlammable.ExtinguishHandler extinguishDelegate)
        {
            _onExtinguish += extinguishDelegate;
        }

        public void DeregisterFromExtinguish(IFlammable.ExtinguishHandler extinguishDelegate)
        {
            _onExtinguish -= extinguishDelegate;
        }

        public void DeregisterFromLightTypeChange(IFlammable.LightTypeChangeHandler lightTypeChangeDelegate)
        {
            _onLightTypeChange -= lightTypeChangeDelegate;
        }
        #endregion IFlammable interface

        private void _SetStartState()
        {
            _crystalLightParent.SetActive(false);
            _fireLightParent.SetActive(false);
            _healingTriggerObject.SetActive(false);
            
            if (_lightUpOnStart == false)
            {
                _isIgnited = false;
                return;
            }

            if (_lightType == LightType.Fire)
                _fireLightParent.SetActive(true);
            else if (_lightType == LightType.Crystal)
                _crystalLightParent.SetActive(true);

            _healingTriggerObject.SetActive(true);

            //Post the brazier start sound
            if (_lightType == LightType.Crystal)
                Sound.PlaySound(_crystalExtinguishSound, _crystalLightParent);
            else if (_lightType == LightType.Fire)
                Sound.PlaySound(_fireIgniteSound, _fireLightParent);

            _isIgnited = true;
        }

        #region IInteractable interface
        public void Interact(GameObject callingObject)
        {
            if (_isIgnited)
            {
                Extinguish();
                Metrics.levelData.interaction_BrazierExtinguish += 1;
            }
            else
            {
                FlareGun gun = callingObject.GetComponent<AbilityCaster>().flareGun;
                if (gun == null)
                    return;

                Ignite(gun.lightType);
                Metrics.levelData.interaction_BrazierIgnite += 1;
            }                
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionStop()
        {
            throw new NotImplementedException();
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            if (_isIgnited) return true;

            if (initiatorObject.TryGetComponent(out AbilityCaster caster))
            {
                if (caster.flareGun != null)
                    return true;
            }
            
            return false;
        }

        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }
        #endregion IInteractable interface



        #region Custom Inspector methods

        public void Inspector_SetLightType(LightType lightType)
        {
            _crystalLightParent.SetActive(false);
            _fireLightParent.SetActive(false);

            if (lightType == LightType.Fire)
                _fireLightParent.SetActive(true);
            else if (lightType == LightType.Crystal)
                _crystalLightParent.SetActive(true);

             _lightType = lightType;
        }
        #endregion
    }
}
