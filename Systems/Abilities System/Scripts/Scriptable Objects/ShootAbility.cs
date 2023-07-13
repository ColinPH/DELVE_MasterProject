using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Player Abilities/Shoot Ability", order = 1)]
    public class ShootAbility : PlayerAbilityBase
    {
        [SerializeField] int _maxCharges = 3;
        [SerializeField] int _initialChargesAmount = 3;
        [SerializeField] float _chargeReloadTime = 3f;
        [SerializeField] int _amountChargesPerReload = 1;
        [SerializeField] GameObject _projectilePrefab;
        [Header("Sounds")]
        [SerializeField] SoundClip _chargeCompleteSound;
        [SerializeField] SoundClip _shotSound;
        [SerializeField] SoundClip _misfireSound;

        #region Public accessors and events
        public override AbilityType AbilityType => AbilityType.Shoot;
        public delegate void ProjectileShotHandler(ShootAbility shootAbility, GameObject projectileObj);
        public ProjectileShotHandler onProjectileShot { get; set; }
        public delegate void ChargeAmountChangeHandler(int currentCharges, int maxCharges);
        public ChargeAmountChangeHandler onChargeAmountChanged { get; set; }

        #endregion

        GameObject _activeProjectile;
        Transform _cameraTransform;
        FlareGun _flareGun;

        float _cooldownThreshold = 0f;
        int _availableCharges = 0;
        bool _isCharging = false;

        IEnumerator _reloadRoutine;

        protected override void m_ActionInitialized()
        {
            base.m_ActionInitialized();

            //Fetch the information required for the ability
            _cameraTransform = m_abilityCaster.cameraTransform;
            _flareGun = m_abilityCaster.flareGun;

            _availableCharges = _initialChargesAmount;
        }

        protected override void m_ActionPerformed(ControlledInputContext context)
        {
            base.m_ActionPerformed(context);
            Debugger.LogFlare("Shoot ability has been performed.");

            if (Time.realtimeSinceStartup < _cooldownThreshold)
            {
                Debugger.LogFlare("Flaregun still on cooldown.");
                Metrics.levelData.onCooldownFlareShots += 1;
                return;
            }

            if (_availableCharges > 0)
            {
                //Consume charges
                _ChangeAmountCharges(-1);
                Metrics.levelData.successfulFlareGunUses += 1;

                GameObject projectileObj = _ShootProjectile(_projectilePrefab, _flareGun.GetProjectileInstantiationPosition());
                _flareGun.ProjectileShot(projectileObj);

                //Start reload routine is nor already started
                if (_isCharging == false)
                {
                    _reloadRoutine = _Co_ReloadRoutine();
                    m_abilityCaster.StartCoroutine(_reloadRoutine);
                }
                //Start cooldown time
                _cooldownThreshold = Time.realtimeSinceStartup + m_abilityCooldownTime;
            }
            else
            {
                Debugger.LogFlare("Not enough charges.");
                Metrics.levelData.failedFlareGunUses += 1;
                //Play the shooting sound
                Sound.PlaySound(_misfireSound, m_casterGameObject);
            }
        }

        protected override int m_GetChargesAmount()
        {
            return _availableCharges;
        }

        public void AddCharges(int amount)
        {
            _ChangeAmountCharges(amount);
        }


        private GameObject _ShootProjectile(GameObject projectileObj, Vector3 position)
        {
            //Play the shooting sound
            Sound.PlaySound(_shotSound, m_casterGameObject);

            GameObject projObj = Instantiate(projectileObj);
            projObj.transform.position = position;
            _activeProjectile = projObj;

            Vector3 direction = _cameraTransform.forward;

            _activeProjectile.GetComponent<ProjectileBase>().InitializeProjectile(m_casterGameObject);
            _activeProjectile.GetComponent<ProjectileBase>().LaunchProjectile(direction);

            onProjectileShot?.Invoke(this, projObj);
            return projObj;
        }

        IEnumerator _Co_ReloadRoutine()
        {
            _isCharging = true;
            while (_availableCharges < _maxCharges)
            {
                Debugger.LogFlare("Started reloading.");
                yield return new WaitForSecondsRealtime(_chargeReloadTime);
                Debugger.LogFlare($"Reloaded {_amountChargesPerReload} charges.");
                Sound.PlaySound(_chargeCompleteSound, m_casterGameObject);
                _ChangeAmountCharges(_amountChargesPerReload);
            }
            _isCharging = false;
        }

        private void _ChangeAmountCharges(int amountToAdd)
        {
            _availableCharges += amountToAdd;

            if (_availableCharges > _maxCharges)
                _availableCharges = _maxCharges;
            else if (_availableCharges < 0)
                _availableCharges = 0;

            onChargeAmountChanged?.Invoke(_availableCharges, _maxCharges);
        }
    }
}