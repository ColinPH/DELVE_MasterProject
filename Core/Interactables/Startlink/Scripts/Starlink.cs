using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class Starlink : ActivationTarget
    {
        [Header("Behaviour Settings")]
        [SerializeField] GameObject _lightObject;
        [SerializeField] bool _lightUpOnStart = false;
        [SerializeField] bool _singleActivation = true;
        [SerializeField] float _minActivationDelay = 1f;
        [SerializeField] float _maxActivationDelay = 3f;
        [SerializeField] float _minDeactivationDelay = 0.2f;
        [SerializeField] float _maxDeactivationDelay = 0.5f;
        [SerializeField] float _offIntensity = 0.2f;
        [SerializeField] CustomRenderTexture _offTexture;
        [SerializeField] CustomRenderTexture _onTexture;
        [SerializeField] float _onIntensity = 1f;
        [SerializeField] MeshRenderer[] emissiveMeshes;
        [SerializeField] int _emissionMaterialIndex = 0;
        [SerializeField] Color _emissioncolor;
        [Header("Connected Sources")]
        [SerializeField] List<Starlink> _sources = new List<Starlink>();
        List<Starlink> _targetLinks = new List<Starlink>();

        bool _hasBeenActivated = false;

        #region MyBehaviour overrides
        public override string worldName => nameof(Starlink);

        protected override void MyAwake()
        {
            base.MyAwake();
            foreach (var item in _sources)
            {
                item.RegisterNewTargetLink(this);
            }
        }
        protected override void MyStart()
        {
            base.MyStart();
            _lightObject.SetActive(_lightUpOnStart);
            foreach (MeshRenderer i in emissiveMeshes)
            {
                Material m = i.materials[_emissionMaterialIndex];
                m.EnableKeyword("_EMISSIVE_COLOR_MAP");
                m.SetTexture("_EmissiveColorMap", _offTexture);
                m.SetColor("_EmissiveColor", _emissioncolor * _offIntensity);
            }
        }
        #endregion MyBehaviour overrides

        public void RegisterNewTargetLink(Starlink newLink)
        {
            if (_targetLinks.Contains(newLink))
            {
                Debugger.LogError($"Already contains {nameof(Starlink)} object as target link : {newLink.gameObject.name}");
                return;
            }
            _targetLinks.Add(newLink);
        }

        private IEnumerator _Co_LightUp()
        {
            _lightObject.SetActive(true);
            _hasBeenActivated = true;
            foreach (MeshRenderer i in emissiveMeshes)
            {
                Material m = i.materials[_emissionMaterialIndex];
                m.EnableKeyword("_EMISSIVE_COLOR_MAP");
                m.SetTexture("_EmissiveColorMap", _onTexture);
                m.SetColor("_EmissiveColor", _emissioncolor * _onIntensity);
            }
            
            

            yield return new WaitForSecondsRealtime(_GetActivationTime());

            foreach (var item in _targetLinks)
            {
                item.Activate();
            }
        }

        private IEnumerator _Co_LightDown()
        {
            _lightObject.SetActive(false);
            _hasBeenActivated = false;

            foreach (MeshRenderer i in emissiveMeshes)
            {
                Material m = i.materials[_emissionMaterialIndex];
                m.EnableKeyword("_EMISSIVE_COLOR_MAP");
                m.SetTexture("_EmissiveColorMap", _offTexture);
                m.SetColor("_EmissiveColor", _emissioncolor * _offIntensity);
            }

            yield return new WaitForSecondsRealtime(_GetDeactivationTime());

            foreach (var item in _targetLinks)
            {
                item.Deactivate();
            }
        }

        private float _GetActivationTime()
        {
            return Random.Range(_minActivationDelay, _maxActivationDelay);
        }

        private float _GetDeactivationTime()
        {
            return Random.Range(_minDeactivationDelay, _maxDeactivationDelay);
        }

        #region Activation target
        public override void Activate()
        {
            base.Activate();
            if (_hasBeenActivated && _singleActivation)
                return;


            StartCoroutine(_Co_LightUp());
            InvokeActivationComplete();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (_hasBeenActivated && _singleActivation)
                return;

            StartCoroutine(_Co_LightDown());
            InvokeDeactivationComplete();
        }
        #endregion Activation target
    }
}
