using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class HookCrosshair : HUDComponent
    {
        public Image hookRangeIndicator;
        public string fillAmountShaderPropertyName = "_FillAmount";
        public string activatedPropertyName = "_On_Off";

        private void Awake()
        {
            HideGrappleIndicator();
        }

        protected override void MonoStart()
        {
            HUD.crosshair = this;
            Managers.uiManager.hookCrossair = this;
            Managers.uiManager.RegisterHUDComponent(this);

            //Register to the hook events
            Managers.eventManager.SubscribeToMainGameEvent(GameEvent.SceneStart, _OnSceneStart);
        }

        public override void OnComponentInstantiation()
        {
            base.OnComponentInstantiation();
            _OnSceneStart(null, SceneGroup.Unassigned, null);
        }

        public override void HideComponent()
        {
            gameObject.SetActive(false);
        }

        public override void ShowComponent()
        {
            gameObject.SetActive(true);
        }

        private void _OnSceneStart(object eventCaller, SceneGroup sceneGroup, object sceneIdentifier)
        {
            AbilityCaster caster = Player.PlayerObject.GetComponent<AbilityCaster>();
            
            //Hook to the event when the player receives a new ability
            caster.onNewAbilityGained += _OnNewAbilityGained;

            _RegisterHook(caster);
        }

        private void _OnNewAbilityGained(PlayerAbilityBase newAbility)
        {
            AbilityCaster caster = Player.PlayerObject.gameObject.GetComponent<AbilityCaster>();

            _RegisterHook(caster);
        }

        private void _RegisterHook(AbilityCaster caster)
        {
            HookAbility hook = caster.GetAbilityOfType<HookAbility>();

            if (hook == null)
            {
                hookRangeIndicator.material.SetFloat(fillAmountShaderPropertyName, 0f);
                return;
            }
            hookRangeIndicator.material.SetFloat(fillAmountShaderPropertyName, 1f);
            hook.onHookCoollingdown += _OnHookCoollingDown;
            hook.onHookCooldownEnded += _OnHookCooldownEnded;
        }

        private void _OnHookCooldownEnded()
        {
            hookRangeIndicator.material.SetFloat(fillAmountShaderPropertyName, 1f);
        }

        private void _OnHookCoollingDown(float remainingTime, float maxTime)
        {
            float fillAmount = 1f - (remainingTime / maxTime);
            hookRangeIndicator.material.SetFloat(fillAmountShaderPropertyName, fillAmount);
        }

        public void ShowGrappleIndicator()
        {
            hookRangeIndicator.material.SetInteger(activatedPropertyName, 1);

            //hookRangeIndicator.enabled = true;
        }
        public void HideGrappleIndicator()
        {
            hookRangeIndicator.material.SetInteger(activatedPropertyName, 0);

            //hookRangeIndicator.enabled = false;
        }
    }
}
