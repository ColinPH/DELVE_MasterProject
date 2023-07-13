using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class Lantern : ActivationTarget, IFlammable, IInteractable
    {
        [SerializeField] bool _isFlammable = true;
        [SerializeField] bool _lightUpOnStart = false;
        [SerializeField] GameObject _crystalLightParent;
        [SerializeField] GameObject _healingTriggerObject;
        [Header("Lights")]
        [SerializeField] MeshRenderer _emissiveMesh;
        [SerializeField] Color _emissiveColor;
        [SerializeField] int _emissiveIndex = 1;
        [SerializeField] float _offIntensity = 0f;
        [SerializeField] float _onIntensity = 1f;
        [Header("Sounds")]
        [SerializeField] SoundClip _crystalIgniteSound;
        [SerializeField] SoundClip _crystalExtinguishSound;
        [Header("Effects")]
        [SerializeField] VisualEffectClip _crystalIgniteVFX;
        [SerializeField] VisualEffectClip _crystalExtinguishVFX;
        [SerializeField] Vector3 _VFXOffset = Vector3.zero;

        [SerializeField, Runtime(true)] bool _isIgnited = false;

        private DetectionParent _detectionParent;
        private Material _emissiveMaterial;

        IFlammable.LightTypeChangeHandler _onLightTypeChange { get; set; }
        IFlammable.IgniteHandler _onIgnite { get; set; }
        IFlammable.ExtinguishHandler _onExtinguish { get; set; }

        #region WorldObject overrides
        public override string worldName => nameof(Lantern);
        protected override void MyAwake()
        {
            base.MyAwake();
            _detectionParent = m_FetchForComponent<DetectionParent>();
            _emissiveMaterial = _emissiveMesh.materials[_emissiveIndex];

            LanternSafeState safeState = new LanternSafeState(_isIgnited);
            Managers.saveManager.RegisterObjectForActivation(worldID, new LanternSafeState(safeState));
            if (Managers.saveManager.GetObjectSafeState(worldID,out BaseSafeState baseState))
            {
                LanternSafeState lanternSafeState = (LanternSafeState)baseState;
                _isIgnited = lanternSafeState.isIgnited;
            }
        }

        protected override void MyStart()
        {
            base.MyStart();
            _SetStartState();
        }
        #endregion WorldObject overrides


        private void _SetStartState()
        {
            _crystalLightParent.SetActive(false);
            _healingTriggerObject.SetActive(false);
            DisableEmission();

            if (_lightUpOnStart == false)
            {
                _isIgnited = false;
                return;
            }

            _crystalLightParent.SetActive(true);
            EnableEmission();
            _healingTriggerObject.SetActive(true);

            //Post the brazier start sound
            Sound.PlaySound(_crystalExtinguishSound, _crystalLightParent);

            _isIgnited = true;
        }

        public void EnableEmission()
        {
            _emissiveMaterial.SetColor("_EmissiveColor", _emissiveColor * _onIntensity);
            _emissiveMaterial.EnableKeyword("_EMISSIVE_COLOR_MAP");
        }

        public void DisableEmission()
        {
            _emissiveMaterial.DisableKeyword("_EMISSIVE_COLOR_MAP");
            _emissiveMaterial.SetColor("_EmissiveColor", _emissiveColor * _offIntensity);
        }


        #region IFlammable interface
        public void Ignite(LightType lightType)
        {
            if (_isFlammable == false || _isIgnited)
                return;

            _healingTriggerObject.SetActive(true);
            EnableEmission();
            _crystalLightParent.SetActive(true);
            //Play light up sound and spawn VFX
            Sound.PlaySound(_crystalIgniteSound, _crystalLightParent);
            VisualEffects.SpawnVFX(_crystalIgniteVFX, _crystalLightParent.transform.position + _VFXOffset, transform.up);


            if (_isIgnited == false)
                _onIgnite?.Invoke();

            _isIgnited = true;
            Managers.saveManager.GetObjectSafeState<LanternSafeState>(worldID).isIgnited = _isIgnited;
        }

        public void Extinguish()
        {
            _crystalLightParent.SetActive(false);
            DisableEmission();
            //Play extinguish sound and spawn VFX
            Sound.PlaySound(_crystalExtinguishSound, _crystalLightParent);
            VisualEffects.SpawnVFX(_crystalExtinguishVFX, _crystalLightParent.transform.position + _VFXOffset, transform.up);
            
            _onExtinguish?.Invoke();

            _isIgnited = false;
            Managers.saveManager.GetObjectSafeState<LanternSafeState>(worldID).isIgnited = _isIgnited;
        }

        public LightType GetLightType()
        {
            return LightType.Crystal;
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



        #region IInteractable interface
        public void Interact(GameObject callingEntity)
        {
            if (_isIgnited)
                Extinguish();
            else
                Ignite(LightType.Crystal);
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
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            return true;
        }

        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }
        #endregion IInteractable interface
    }
}