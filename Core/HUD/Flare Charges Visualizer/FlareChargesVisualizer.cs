using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class FlareChargesVisualizer : HUDComponent
    {
        public Image flareAmountImage;
        public List<Sprite> flareChargesAmount = new List<Sprite>();

        protected override void MonoStart()
        {
            HUD.flareChargesVisualizer = this;
            Managers.uiManager.flareChargesVisualizer = this;
            Managers.uiManager.RegisterHUDComponent(this);

            //Register to the hook events
            Managers.eventManager.SubscribeToMainGameEvent(GameEvent.SceneStart, _OnSceneStart);
        }

        private void OnDestroy()
        {
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.SceneStart, _OnSceneStart);
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

            _RegisterFlare(caster);
        }

        private void _OnNewAbilityGained(PlayerAbilityBase newAbility)
        {
            AbilityCaster caster = Player.PlayerObject.GetComponent<AbilityCaster>();

            _RegisterFlare(caster);
        }

        private void _RegisterFlare(AbilityCaster caster)
        {
            ShootAbility shootAbility = caster.GetAbilityOfType<ShootAbility>();

            if (shootAbility == null)
                return;

            shootAbility.onProjectileShot += _OnFlareShot;
            shootAbility.onChargeAmountChanged += _OnNewFlareCharge;
            int amountFlares = shootAbility.GetChargesAmount();
            _DisplayFlares(amountFlares);
        }

        private void _OnNewFlareCharge(int currentCharges, int maxCharges)
        {
            _DisplayFlares(currentCharges);
        }

        private void _OnFlareShot(ShootAbility shootAbility, GameObject projectileObj)
        {
            int amountFlares = shootAbility.GetChargesAmount();
            _DisplayFlares(amountFlares);
        }

        private void _DisplayFlares(int amount)
        {
            if (amount >= flareChargesAmount.Count)
                Debug.LogError("Not enough sprites for the amount of flares, check crosshair.");

            if (amount <= 0)
            {
                flareAmountImage.color = Color.red;
                amount = 1;
            }
            else
            {
                flareAmountImage.color = Color.white;
            }

            flareAmountImage.sprite = flareChargesAmount[amount - 1];
        }

    }
}
