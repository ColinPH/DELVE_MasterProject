using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PropellerCap
{
    public class CollectablesTracker : WorldObject
    {
        [SerializeField] TMP_Text _fragmentsText;
        [SerializeField] TMP_Text _coinsText;
        [SerializeField] Image _fragmentsImage;

        public override string worldName => nameof(CollectablesTracker);
        protected override void MyStart()
        {
            base.MyStart();
            UpdateCollectableValues();
        }

        public void UpdateCollectableValues()
        {
            //Update amount of coins
            int amountCoins = Metrics.player.collectedCoinsAmount;
            _coinsText.text = amountCoins.ToString();

            if (Managers.totemManager.activeRuntimeTotem == null)
            {
                _fragmentsText.text = "";
                _fragmentsImage.color = Color.white;
                return;
            }

            //Update mount of fragments
            int amountFragmetns = Player.PlayerObject.GetComponent<TotemHolder>().collectedFragmentsAmount;
            amountFragmetns += Managers.totemManager.collectedFragments;
            int totalFragmentAmount = Managers.totemManager.requiredFragmentsAmount;
            _fragmentsText.text = amountFragmetns.ToString() + " / " + totalFragmentAmount;

            _fragmentsImage.color = Managers.totemManager.activeRuntimeTotem.fragmentIconInJournalColor;
        }

    }
}
