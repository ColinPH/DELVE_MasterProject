using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class Coin : WorldObject
    {
        [Header("Behaviour")]
        [SerializeField] int _coinAmount = 1;
        [Header("SFX & VFX")]
        [SerializeField] SoundClip _pickupSound;
        [SerializeField] VisualEffectClip _pickupVFX;



        private Collectable _collectable;

        protected override void MyStart()
        {
            base.MyStart();

            _collectable = m_FetchForComponent<Collectable>();
            _collectable.OnObjectCollected += _OnObjectCollected;
        }

        private void _OnObjectCollected(GameObject collectingObject)
        {
            //Add a charge to the player
            Metrics.player.collectedCoinsAmount += _coinAmount;
            Saver.progression.hasCollectedCoin = true;
            Metrics.OnCoinCollected?.Invoke();
            Metrics.levelData.totalCoinsCollected += _coinAmount;

            Managers.playerManager.OnPlayerCollectObject?.Invoke(true, false);

            //Destroy the object
            Sound.PlaySound(_pickupSound, gameObject);
            VisualEffects.SpawnVFX(_pickupVFX, transform);
            DestroyMyObject();
        }
    }
}
