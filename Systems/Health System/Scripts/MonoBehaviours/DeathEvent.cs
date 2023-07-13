using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class DeathEvent : MonoBehaviour
    {
        [SerializeField] GameObject _deahtVFX;
        [SerializeField] SoundClip _deathSound;

        WorldEntity _worldEntity;

        private void Awake()
        {
            //Fetch components
            _worldEntity = GetComponent<WorldEntity>();

            //Register for events
            _worldEntity.onEntityDeath += _OnEntityDeath;
        }

        private void _OnEntityDeath()
        {
            if (_deathSound != null)
                Sound.PlaySound(_deathSound, gameObject);

            StartCoroutine(_Co_WaitAndDestroy());
        }

        private void _InstantiateFX(GameObject prefab, bool inheritRotation = true)
        {
            if (prefab != null)
            {
                GameObject o = Instantiate(prefab);
                o.transform.position = transform.position;

                if (inheritRotation)
                    o.transform.rotation = transform.rotation;
            }
        }

        IEnumerator _Co_WaitAndDestroy()
        {
            yield return new WaitForSecondsRealtime(1f);

            _InstantiateFX(_deahtVFX);

            Destroy(gameObject);
        }
    }
}