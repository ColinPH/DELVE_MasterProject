using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PropellerCap
{
    public class BlackFader : HUDComponent
    {
        [SerializeField] Image _fadingImage;

        IEnumerator _fadeRoutine;

        [SerializeField] float _fadeTime = 1f;
        float _currentAlphaValue = 0f;
        float _directionMultiplier = 1f;
        float _alphaMultiplier = 255f;

        #region HUD Component
        protected override void MonoStart()
        {
            HUD.blackFader = this;
            Managers.uiManager.blackFader = this;
            Managers.uiManager.RegisterHUDComponent(this);
        }

        public override void HideComponent()
        {
            gameObject.SetActive(false);
        }

        public override void ShowComponent()
        {
            gameObject.SetActive(true);
        }
        #endregion  HUD Component

        public void FadeToBlack(float fadeTime = 1f)
        {         
            _fadeRoutine = Co_FadeToBlack(fadeTime);
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            StartCoroutine(_fadeRoutine);
        }

        public IEnumerator Co_FadeToBlack(float fadeTime = 1f)
        {
            //_fadeTime = fadeTime;
            _currentAlphaValue = 1f;
            _directionMultiplier = -1f;

            yield return StartCoroutine(_Co_FadeRoutine(1f));
        }

        public void FadeFromBlack(float fadeTime = 1f)
        {
            _fadeRoutine = Co_FadeFromBlack(fadeTime);
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            StartCoroutine(_fadeRoutine);
        }

        public IEnumerator Co_FadeFromBlack(float fadeTime = 1f)
        {
            //_fadeTime = fadeTime;
            _currentAlphaValue = 0f;
            _directionMultiplier = 1f;

            yield return StartCoroutine(_Co_FadeRoutine(0f));
        }

        private IEnumerator _Co_FadeRoutine(float alpha)
        {
            _fadingImage.CrossFadeAlpha(alpha, _fadeTime, true);
            yield return new WaitForSeconds(_fadeTime);
            /*while (_currentAlphaValue >= 0f && _currentAlphaValue <= 1f)
            {
                _currentAlphaValue += (Time.deltaTime / _fadeTime) * _directionMultiplier;

                Color colour = _fadingImage.color;
                colour.a = _currentAlphaValue * _alphaMultiplier;
                Debug.Log("Alpha " + colour.a);
                _fadingImage.color = colour;
                yield return null;
            }*/
        }
    }
}
