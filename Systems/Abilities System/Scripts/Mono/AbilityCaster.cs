using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PropellerCap
{
    public class AbilityCaster : WorldObject
    {
        [Header("Required by the abilities")]
        public FlareGun flareGun;
        public HookGun hookGun;
        public Transform cameraTransform;
        public Transform _rightHandGunPoint;
        public Transform _leftHandGunPoint;
        public Animator bodyAnimator;

        [Header("Ability Objects Prefabs")]
        public GameObject _flareGunPrefab;
        public GameObject _hookGunPrefab;

        [Header("Abilities to start with")]
        public List<PlayerAbilityBase> _startingAbilities = new List<PlayerAbilityBase>();

        [Header("Runtime information")]
        public List<PlayerAbilityBase> _activeAbilities = new List<PlayerAbilityBase>();
        


        PlayerInputController _inputController;

        #region Public accessors and Events
        public delegate void OnNewAbilityGained(PlayerAbilityBase newAbility);
        public OnNewAbilityGained onNewAbilityGained { get; set; }
        #endregion


        #region MyBehaviour and MonoBehaviour
        public override string worldName => nameof(AbilityCaster);

        protected override void MyAwake()
        {
            _inputController = m_FetchForComponent<PlayerInputController>();
            m_PropertyIsNull<AbilityCaster>((bodyAnimator == null), nameof(bodyAnimator));

            if (_inputController == null) 
                Debug.LogError("Player is missing a " + typeof(PlayerInputController) + " component.");
        }

        protected override void MyStart()
        {
            if (_startingAbilities.Count == 0)
                Debug.LogWarning("No abilities assigned to the " + nameof(AbilityCaster) + " on object : " + gameObject.name);

            //Make sure no abilities were in the active list at start so that they don't link to asset's data
            _activeAbilities.Clear();

            //Copy starting abilities to not modify the scriptable objects' asset data
            foreach (PlayerAbilityBase action in _startingAbilities)
            {
                _activeAbilities.Add(Instantiate(action));
            }

            //Initialize all abilities
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.InitializeAbility(gameObject, this, _inputController);
            }
        }

        protected override void MyDestroy()
        {
            base.MonoDestroy();
            
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.OnAbilityDestruction();
            }
        }

        private void Update()
        {
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.UpdateLoop();
            }
        }
        private void FixedUpdate()
        {
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.FixedUpdateLoop();
            }
        }
        private void LateUpdate()
        {
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.LateUpdateLoop();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.OnCollisionEnter(collision);
            }
        }
        #endregion MyBehaviour and MonoBehaviour

        #region Abilities controls

        public void AddAbility(PlayerAbilityBase abilityToAdd)
        {
            if (ContainsAbility(abilityToAdd.AbilityType))
            {
                Debugger.LogError($"The player already has the ability \"{abilityToAdd.actionName}\".");
                return;
            }

            if (abilityToAdd.AbilityType == AbilityType.Shoot)
            {
                GameObject flareObj = Instantiate(_flareGunPrefab);
                flareObj.transform.position = _rightHandGunPoint.transform.position;
                flareObj.transform.rotation = _rightHandGunPoint.transform.rotation;
                flareObj.transform.SetParent(_rightHandGunPoint);
                flareGun = flareObj.GetComponent<FlareGun>();
            }
            else if (abilityToAdd.AbilityType == AbilityType.Hook)
            {
                GameObject hookObj = Instantiate(_hookGunPrefab);
                hookObj.transform.position = _leftHandGunPoint.transform.position;
                hookObj.transform.rotation = _leftHandGunPoint.transform.rotation;
                hookObj.transform.SetParent(_leftHandGunPoint);
                hookGun = hookObj.GetComponent<HookGun>();
            }

            PlayerAbilityBase instantiatedAbility = Instantiate(abilityToAdd);
            instantiatedAbility.InitializeAbility(gameObject, this, _inputController);
            _activeAbilities.Add(instantiatedAbility);
            onNewAbilityGained?.Invoke(instantiatedAbility);
        }

        public void RemoveAbility(AbilityType abilityType)
        {
            PlayerAbilityBase ability = GetAbility(abilityType);
            ability.OnAbilityDestruction();
            _activeAbilities.Remove(ability);
            Destroy(ability);
        }

        private bool ContainsAbility(AbilityType abilityType)
        {
            bool toReturn = false;
            foreach (var item in _activeAbilities)
            {
                if (item.AbilityType == abilityType)
                    return true;
            }
            return toReturn;
        }

        #endregion Abilities controls

        public PlayerAbilityBase GetAbility(AbilityType abilityType) 
        {
            foreach (var item in _activeAbilities)
            {
                if (item.AbilityType == abilityType)
                    return item;
            }

            return null;
        }

        public List<T> GetAbilitiesOfType<T>() where T : PlayerAbilityBase
        {
            return _activeAbilities.OfType<T>().ToList();
        }

        public T GetAbilityOfType<T>() where T : PlayerAbilityBase
        {
            List<T> toReturn = GetAbilitiesOfType<T>();

            if (toReturn.Count > 1)
                Debug.LogWarning("There is more than 1 ability of type " + typeof(T) + " in the list of abilities : " + gameObject.name);

            if (toReturn.Count == 0)
                return null;

            return toReturn.First();
        }

        /// <summary> Called by the SimpleAnimationEventCaller based on an animation event. </summary>
        public void BroadcastAnimationEventToAbilities(string eventIdentifier)
        {
            foreach (PlayerAbilityBase action in _activeAbilities)
            {
                action.OnAnimationEvent(eventIdentifier);
            }
        }
    }
}
