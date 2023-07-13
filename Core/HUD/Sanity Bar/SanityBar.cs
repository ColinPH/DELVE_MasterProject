using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

namespace PropellerCap
{
    public class SanityBar : HUDComponent
    {
        [SerializeField] Image _sanitySlider;

        protected override void MonoStart()
        {
            HUD.sanityBar = this;
            Managers.uiManager.sanityBar = this;
            Managers.uiManager.RegisterHUDComponent(this);
            Sanity.manager.onSanityChange += _OnSanityChange;
        }

        private void OnDestroy()
        {
            Sanity.manager.onSanityChange -= _OnSanityChange;
        }

        public override void HideComponent()
        {
            gameObject.SetActive(false);
        }

        public override void ShowComponent()
        {
            gameObject.SetActive(true);
        }


        private void _OnSanityChange(float newSanity, float oldSanity, float maxSanity)
        {
            float progressPercent = newSanity / maxSanity;
            _sanitySlider.fillAmount = progressPercent;
        }
    }
}
