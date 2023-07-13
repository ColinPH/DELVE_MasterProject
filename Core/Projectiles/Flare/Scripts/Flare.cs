using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    public class Flare : ProjectileBase
    {
        [Header("Light Dimming")]
        [SerializeField] float _dimIntensity = 0.1f;
        [SerializeField] float _dimRange = 14f;
        [SerializeField] AnimationCurve _dimmingCurve;
        [SerializeField] float _dimAtDistance = 14;
        [Header("Behaviour")]
        [SerializeField] LightType _lightType = LightType.Crystal;
        [SerializeField] bool _stickImpact = false;
        [ShowIf(nameof(_stickImpact), true)]
        [SerializeField] float _dragOnImpact = 0.2f;
        [SerializeField] float _lifeTime = 12f;
        [SerializeField] GameObject _healingTriggerObject;
        [Header("Sprout Spawning")]
        [SerializeField] GameObject _sproutPrefab;
        [SerializeField] float _sproutGrowthTime = 3f;
        [SerializeField] AnimationCurve _sproutGrowthCurve;
        [Header("Lights")]
        [SerializeField] GameObject _crystalLightParent;
        [SerializeField] GameObject _fireLightParent;
        [Header("Effects")]
        [SerializeField] VisualEffectClip _crystalImpactVFX;
        [SerializeField] VisualEffectClip _fireImpactVFX;
        [SerializeField] GameObject _crystalLitEffect;
        [SerializeField] GameObject _fireLitEffect;
        [Header("Sounds")]
        [SerializeField] SoundClip _smolderSound;
        [SerializeField] SoundClip _flightSound;
        [SerializeField] SoundClip _SproutGrowthSound;

        Rigidbody _rigidbody;
        GameObject _fireLightObject;
        GameObject _crystalLightObject;
        SphereCollider _healingSphere;
        LightHealer _lightHealer;

        HDAdditionalLightData _fireLightData;
        HDAdditionalLightData _crystalLightData;

        bool _hasCollided = false;
        bool _isDimming = false;
        float _dimStartTime = 0f;
        ContactPoint _sproutSpawnContact;

        #region WorldObject overrides
        public override string worldName => nameof(Flare);
        protected override void MonoAwake()
        {
            base.MonoAwake();
            _SetStartState();
        }
        protected override void MyUpdate()
        {
            base.MyUpdate();

            _DimLightOverTime();
        }
        #endregion WorldObject overrides

        #region MonoBehaviour methods
        private void OnCollisionEnter(Collision collision)
        {
            if (_hasCollided) return;
            _hasCollided = true;

            bool applyImpactEffects = true;
            bool applyImpactVFX = true;
            IFlammable flammable = collision.gameObject.GetMainObject().GetComponent<IFlammable>();
            if (flammable != null)
            {
                if (flammable.IsIgnited())
                {
                    applyImpactVFX = true;
                    applyImpactEffects = false;
                }
                else
                {
                    applyImpactVFX = false;
                    applyImpactEffects = false;
                    flammable.Ignite(_lightType);
                    Metrics.levelData.interaction_FlammableIgniteWithFlare += 1;
                }
                Destroy(gameObject);
            }


            //Spawn the VFX based on the light type
            if (applyImpactVFX)
            {
                Debug.Log($"amoutn of collision points are : {collision.contacts.Length}");
                if (_lightType == LightType.Fire)
                    VisualEffects.SpawnVFX(_fireImpactVFX, collision.contacts[0].point, collision.contacts[0].normal);
                else if (_lightType == LightType.Crystal)
                {
                    VisualEffects.SpawnVFX(_crystalImpactVFX, collision.contacts[0].point, collision.contacts[0].normal);
                    //Spawn the crystal sprout
                    _sproutSpawnContact = collision.contacts[0];
                }
            }

            if (applyImpactEffects == false) return;

            //Become kinematic to stop all movement or apply drag
            if (_stickImpact)
            {
                _rigidbody.isKinematic = true;
                GetComponent<Collider>().enabled = false;
            }
            else
                _rigidbody.drag = _dragOnImpact;

            //Start dimming the light
            _EnterDimmingState();
        }

        
        #endregion MonoBehaviour methods



        #region ProjectileBase overrides
        protected override void m_OnProjectileInitialization(GameObject callingObject)
        {
            base.m_OnProjectileInitialization(callingObject);
            _rigidbody = m_FetchForComponent<Rigidbody>();
            _healingSphere = m_FetchForComponent<SphereCollider>(_healingTriggerObject);
            _lightHealer = m_FetchForComponent<LightHealer>(_healingTriggerObject);

            //Get the gameObjets that have the light Component on them
            _fireLightObject = m_FetchForComponentInChildren<Light>(_fireLightParent).gameObject;
            _crystalLightObject = m_FetchForComponentInChildren<Light>(_crystalLightParent).gameObject;
            
            //Get the lightData to control the values of the light
            _fireLightData = m_FetchForComponent<HDAdditionalLightData>(_fireLightObject);
            _crystalLightData = m_FetchForComponent<HDAdditionalLightData>(_crystalLightObject);

            if (_rigidbody == null)
                Debug.Log("Flare object needs a rigidbody component.");
        }
        protected override void m_OnProjectileLaunch(Vector3 direction)
        {
            base.m_OnProjectileLaunch(direction);

            GetComponent<ProjectileMoverBase>().StartMoving(direction);

            Sound.PlaySound(_flightSound, gameObject);
        }

        protected override void m_OnProjectileArrived()
        {

        }
        #endregion ProjectileBase overrides



        private void _SpawnCrystalSprout(ContactPoint point)
        {
            Vector3 direction = point.normal;
            GameObject gameObj = Instantiate(_sproutPrefab);
            gameObj.transform.position = point.point;

            Quaternion rot = Quaternion.FromToRotation(gameObj.transform.up, direction);
            gameObj.transform.rotation *= rot;

            gameObj.GetComponent<MonoBehaviour>().StartCoroutine(_Co_GrowSprout(gameObj));
        }

        IEnumerator _Co_GrowSprout(GameObject sproutObj)
        {
            Sound.PlaySound(_SproutGrowthSound, gameObject);
            sproutObj.transform.localScale = Vector3.zero;

            float growthPercentage = 0f;
            float totalGrowthTime = 0f;
            float newScale;
            while (growthPercentage < 1f)
            {
                growthPercentage = totalGrowthTime / _sproutGrowthTime;
                newScale = _sproutGrowthCurve.Evaluate(growthPercentage); 
                sproutObj.transform.localScale = Vector3.one * newScale;
                yield return null;
                totalGrowthTime += Time.deltaTime;
            }

            newScale = _sproutGrowthCurve.Evaluate(1f);
            sproutObj.transform.localScale = Vector3.one * newScale;
        }

        private void _DimLightOverTime()
        {
            if (_isDimming == false) return;

            float elapsedDimTime = Time.time - _dimStartTime;
            float sampleIndex = (elapsedDimTime / _lifeTime);
            float sampledValue = _dimmingCurve.Evaluate(sampleIndex);
            float range = _dimRange * sampledValue;

            //Set the intensity of the light
            _GetActiveLightData().intensity = _dimIntensity * sampledValue;
            _GetActiveLightData().range = range;
            _GetActiveLightData().luxAtDistance = _dimAtDistance * sampledValue;
            //Change the size of the healing trigger sphere
            _healingSphere.radius = range;

            //Stop dimming if we are past the lifetime
            if (elapsedDimTime >= _lifeTime)
            {
                _isDimming = false;
                _ExitDimmingState();
            }
        }

        HDAdditionalLightData _GetActiveLightData()
        {
            if (_lightType == LightType.Fire)
                return _fireLightData;
            else
                return _crystalLightData;
        }

        public void SetLightType(LightType lightType)
        {
            _lightType = lightType;

            _crystalLightParent.SetActive(false);
            _fireLightParent.SetActive(false);

            if (_lightType == LightType.Fire)
            {
                _fireLightParent.SetActive(true);
                _lightHealer.EnableHealing();
            }
            else if (_lightType == LightType.Crystal)
            {
                _crystalLightParent.SetActive(true);
                _lightHealer.DisableHealing();
            }
        }

        private void _EnterDimmingState()
        {
            //Play Smolder sound
            Sound.PlaySound(_smolderSound, gameObject);

            _dimStartTime = Time.time;
            _isDimming = true;

            if (_lightType == LightType.Fire)
            {
                _fireLitEffect.transform.LookAt(_fireLitEffect.transform.position + _sproutSpawnContact.normal);
                _fireLitEffect.SetActive(true);
            }
            else if (_lightType == LightType.Crystal)
            {
                _crystalLitEffect.transform.LookAt(_crystalLitEffect.transform.position + _sproutSpawnContact.normal);
                _crystalLitEffect.SetActive(true);
            }
        }

        private void _ExitDimmingState()
        {
            if (_lightType == LightType.Crystal)
            {
                //Spawn the crystal sprout
                _SpawnCrystalSprout(_sproutSpawnContact);
            }
            Destroy(gameObject);
        }

        private void _SetStartState()
        {
            _crystalLightParent.SetActive(false);
            _fireLightParent.SetActive(false);
            _healingTriggerObject.SetActive(false);

            if (_lightType == LightType.Fire)
                _fireLightParent.SetActive(true);
            else if (_lightType == LightType.Crystal)
                _crystalLightParent.SetActive(true);

            _healingTriggerObject.SetActive(true);
        }
    }
}