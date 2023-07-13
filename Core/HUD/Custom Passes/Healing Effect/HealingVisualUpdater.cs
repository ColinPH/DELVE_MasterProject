using PropellerCap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingVisualUpdater : MonoBehaviour
{
    bool _effectIsActive = false;

    private void Start()
    {
        Managers.sanityManager.onSanityChange += _OnSanityChange;
    }

    private void _OnSanityChange(float newSanity, float oldSanity, float maxSanity)
    {
        if (newSanity > oldSanity)
        {
            if (_effectIsActive == false)
                _StartEffect();
        }
        else if (newSanity < oldSanity)
        {
            if (_effectIsActive)
                _StopEffect();
        }
    }

    private void _StartEffect()
    {
        _effectIsActive = true;
        HUD.customPassHandler.EnableCustomPass(CustomPassType.HealingEffect);
    }

    private void _StopEffect()
    {
        _effectIsActive = false;
        HUD.customPassHandler.DisableCustomPass(CustomPassType.HealingEffect);
    }
}