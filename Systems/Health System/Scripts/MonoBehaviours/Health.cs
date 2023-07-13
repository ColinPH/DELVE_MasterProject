using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float _health = 0;
        [SerializeField] float _maxHealth = 0;
        [SerializeField] bool _isInvincible = false;


        private WorldEntity _worldEntity;


        private void Awake()
        {
            _worldEntity = GetComponent<WorldEntity>();
            if (_worldEntity == null)
                Debug.LogError("There is no " + typeof(WorldEntity) + " component on the object " + gameObject.name);

            _worldEntity.onDamagesTaken += ReceiveDamage;
            _worldEntity.onHealthInfoRequest += GetHealthInfo;
        }


        public float health
        {
            get { return _health; }
        }
        public void ReceiveDamage(Damages damages)
        {
            if (_health <= 0)
                return;

            if (_isInvincible == false)
                _health -= damages.amount;

            _worldEntity.onDamagesTaken?.Invoke(damages);

            if (_health <= 0 && _isInvincible == false)
            {
                _health = 0f;
                _worldEntity.InvokeOnEntityDeath();
            }
        }

        public HealthInfo GetHealthInfo()
        {
            HealthInfo h = new HealthInfo();
            h.health = _health;
            h.maxHealth = _maxHealth;

            return h;
        }

        public void AddHealth(int amount)
        {
            _health += amount;
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
        }

        public void ResetHealth()
        {
            _health = _maxHealth;
        }
    }
}