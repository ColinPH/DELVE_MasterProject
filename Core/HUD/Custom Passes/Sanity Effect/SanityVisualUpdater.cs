using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SanityVisualUpdater : WorldObject
    {
        Material _targetMaterial;

        public PropertyControlOptions _sanityCustomPassController;

        ShaderPropertyController _propertyController;

        protected override void MyStart()
        {
            base.MyStart();
            Managers.sanityManager.onSanityChange += _OnSanityChange;
            _targetMaterial = HUD.customPassHandler.GetPassMaterial(CustomPassType.InsanityEffect, false);
            _propertyController = new ShaderPropertyController(_targetMaterial, _sanityCustomPassController);
            Player.onLightEntered += _OnEnteredLight;
            Player.onLightExit += _OnExitLight;
            Managers.sanityManager.OnSanityStop += _OnSanityStop;
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            Player.onLightEntered -= _OnEnteredLight;
            Player.onLightExit -= _OnExitLight;
            Managers.sanityManager.OnSanityStop -= _OnSanityStop;
        }

        private void _OnExitLight()
        {
            if (Sanity.isActive)
                HUD.customPassHandler.EnableCustomPass(CustomPassType.InsanityEffect);
        }

        private void _OnEnteredLight()
        {
            if (Sanity.isActive)
                HUD.customPassHandler.DisableCustomPass(CustomPassType.InsanityEffect);
        }

        private void _OnSanityStop()
        {
            HUD.customPassHandler.DisableCustomPass(CustomPassType.InsanityEffect);
        }

        private void _OnSanityChange(float newSanity, float oldSanity, float maxSanity)
        {
            if (Sanity.isActive == false) return;

            float sanityPercentage = newSanity / maxSanity;

            if (_propertyController == null)
            {
                _targetMaterial = HUD.customPassHandler.GetPassMaterial(CustomPassType.InsanityEffect, false);
                _propertyController = new ShaderPropertyController(_targetMaterial, _sanityCustomPassController);
            }

            _propertyController.SetPercentage(sanityPercentage);
            _propertyController.UpdateProperty();
        }
    }
}
