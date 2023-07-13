using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SanityManager : ManagerBase
    {
        public static SanityManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] bool _afflictSanity = false;
        [SerializeField] float _sanityDepletionRate = 5f;
        [SerializeField] float _maxSanity = 100f;
        [Header("Sanity Thresholds")]
        [SerializeField] float _heartBeatsThreshold = 50f;
        [Header("HUD Components")]
        [SerializeField] GameObject _HUDSanityBarPrefab;
        [SerializeField, Runtime(true)] float _currentSanity = 0f;
        [SerializeField, Runtime] float _totalSanityLost = 0f;
        [SerializeField, Runtime] float _currentMultiplier = 999999999f; //This is just to visialize the current multiplier in the inspector, should not be used
        [SerializeField] List<string> _multipliersIdentifiers = new List<string>();

        bool _sanityHasDepleted = false;
        bool _trackingSanityLoss = false;
        [SerializeField, Runtime] float _lightMultiplier = 1f;
        GameObject _activeHUDSanityBarObj;
        PlayerCharacter _player;

        //Sanity Thresholds
        [SerializeField, Runtime] bool _isBellowHeartBeatsThreshold = false;

        private Dictionary<string, float> _sanityMultipliers = new Dictionary<string, float>();

        #region Public accessors and events
        //Variables
        public float currentSanity => _currentSanity;
        public float maxSanity => _maxSanity;
        public float currentMultiplier => _currentMultiplier;
        public bool sanityIsActive => _afflictSanity;
        public float totalSanityLost => _totalSanityLost;

        //Events
        public delegate void OnSanityChange(float newSanity, float oldSanity, float maxSanity);
        public OnSanityChange onSanityChange { get; set; }

        public delegate void OnSanityDepleated();
        public OnSanityDepleated onSanityDepleted { get; set; }
        public delegate void SanityStopHandler();
        public SanityStopHandler OnSanityStop { get; set; }
        public delegate void SanityStartHandler();
        public SanityStartHandler OnSanityStart { get; set; }

        #endregion Public accessors and events

        //--------------------

        //--------------------

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in Sanity Manager");
            Sanity.manager = this;
            Managers.sanityManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Sanity Manager");
            _currentSanity = _maxSanity;
            Managers.playerManager.onNewPlayerSpawned += _OnNewPlayerSpawned;
        }

        public override void MyStart()
        {
            Debugger.LogInit("MyStart in Sanity Manager");
            Managers.eventManager.SubscribeToGameEvent(GameEvent.SceneFailed, _OnPlayerDeath);
            Managers.eventManager.SubscribeToGameEvent(GameEvent.RunStart, _StartTrackingSanityLoss);
            Managers.eventManager.SubscribeToGameEvent(GameEvent.RunFailed, _StopTrackingSanityLoss);
            Managers.eventManager.SubscribeToGameEvent(GameEvent.RunCompleted, _StopTrackingSanityLoss);

            Managers.playerManager.onPlayerEnteredLight += _OnPlayerEnteredLight;
            Managers.playerManager.onPlayerExitLight += _OnPlayerExitLight;
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<SanityManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion


        public override void MyUpdate()
        {
            if (_afflictSanity)
                RemoveSanity(_sanityDepletionRate * Time.deltaTime);
        }

        private void _OnPlayerEnteredLight()
        {
            _lightMultiplier = 0f;
        }

        private void _OnPlayerExitLight()
        {
            _lightMultiplier = 1f;
        }


        private void _OnNewPlayerSpawned(PlayerCharacter newPlayer)
        {
            _player = newPlayer;
            RestoreSanity(1f);
        }


        private void _OnPlayerDeath()
        {
            _sanityMultipliers.Clear();
        }


        private void _SanityDepleted()
        {
            _sanityHasDepleted = true;
            onSanityDepleted?.Invoke();

        }

        private void _UpdateSanity(float newSanity, float oldSanity)
        {
            try
            {
                onSanityChange?.Invoke(newSanity, oldSanity, _maxSanity);
            }
            catch (Exception ex)
            {
                Debugger.LogError("Catched error while updating sanity: " + ex.Message + ex.StackTrace);
            }
            _currentMultiplier = _ApplyMultipliersTo(1f, _sanityMultipliers);
        }

        /// <summary> Checks if the sanity value has passed any threshold values. Called after sanity has been added or removed. And before the sanity is updated. </summary>
        private void _CheckForSanityThresholds()
        {
            if (_currentSanity <= _heartBeatsThreshold && _isBellowHeartBeatsThreshold == false)
            {
                _isBellowHeartBeatsThreshold = true;
                //Start heartbeat sounds
                _player.StartHeartbeat();
            }
            else if (_currentSanity > _heartBeatsThreshold && _isBellowHeartBeatsThreshold)
            {
                _isBellowHeartBeatsThreshold = false;
                //Stop heartbeat sounds
                _player.StopHeartbeat();
            }

            if (_isBellowHeartBeatsThreshold)
            {
                //Play heartbeat sounds through RTCP
                _player.SetHeartbeatValue(_currentSanity, _heartBeatsThreshold);
            }
        }


        #region Sanity Control
        public void StartAfflictingSanity()
        {
            Debug.Log("SanityStart");
            if (_afflictSanity) return;

            _afflictSanity = true;

            //Add the sanity bar in the HUD
            _activeHUDSanityBarObj = Managers.uiManager.InstantiateNewHUDComponent(_HUDSanityBarPrefab);
            OnSanityStart?.Invoke();
        }

        public void StopAfflictingSanity()
        {
            if (_afflictSanity == false) return;

            _afflictSanity = false;

            _isBellowHeartBeatsThreshold = false;
            //Stop heartbeat sounds
            _player.StopHeartbeat();

            //Remove the sanity bar from the HUD
            Managers.uiManager.RemoveHUDComponent(_activeHUDSanityBarObj);
            _activeHUDSanityBarObj = null;
            OnSanityStop?.Invoke();
        }

        public void AddSanity(float amount, bool applyMultipliers = true)
        {
            if (_sanityHasDepleted)
                return;

            float oldSanity = _currentSanity;
            float sanityToAdd;

            if (applyMultipliers)
                sanityToAdd = _ApplyMultipliersTo(amount, _sanityMultipliers);
            else
                sanityToAdd = amount;

            _currentSanity += sanityToAdd;
            
            if (_currentSanity > _maxSanity)
            {
                _currentSanity = _maxSanity;
            }

            _CheckForSanityThresholds();
            _UpdateSanity(_currentSanity, oldSanity);
        }

        public void RemoveSanity(float amount, bool applyMultipliers = true)
        {
            if (_sanityHasDepleted || _afflictSanity == false) 
                return;

            float oldSanity = _currentSanity;
            float sanityToLose;

            if (applyMultipliers)
                sanityToLose = _ApplyMultipliersTo(amount, _sanityMultipliers);
            else
                sanityToLose = amount;
            
            _currentSanity -= sanityToLose;
            _AddToTotalSanityLost(sanityToLose);

            if (_currentSanity <= 0f)
            {
                _currentSanity = 0f;
                _SanityDepleted();
            }

            _CheckForSanityThresholds();
            _UpdateSanity(_currentSanity, oldSanity);
        }

        public void RestoreSanity(float percentage0To1 = 1f)
        {
            _sanityHasDepleted = false;

            float oldSanity = _currentSanity;

            _currentSanity = _maxSanity * percentage0To1;

            _CheckForSanityThresholds();
            _UpdateSanity(_currentSanity, oldSanity);
        }
        #endregion Sanity Control

        #region Total sanity tracker

        void _StartTrackingSanityLoss()
        {
            _trackingSanityLoss = true;
            _totalSanityLost = 0f;
        }

        void _StopTrackingSanityLoss()
        {
            _trackingSanityLoss = false;
        }

        void _AddToTotalSanityLost(float amount)
        {
            if (_trackingSanityLoss)
                _totalSanityLost += amount;
            Metrics.levelData.totalSanityLost += amount;
        }

        #endregion Total sanity tracker

        //--------------------

        //--------------------

        #region Sanity multipliers
        public void AddMultiplier(float amount, string identifier)
        {
            if (_sanityMultipliers.ContainsKey(identifier) == false)
            {
                _sanityMultipliers.Add(identifier, amount);
                _multipliersIdentifiers.Add(identifier);
            }
            else
            {
                Debug.LogError("Sanity modifier identifier \"" + identifier + "\" already exists. Use another identifier.");
            }
        }

        public void RemoveMultiplier(float amount, string identifier)
        {
            if (_sanityMultipliers.ContainsKey(identifier))
            {
                _sanityMultipliers.Remove(identifier);
                _multipliersIdentifiers.Remove(identifier);
            }
            else
            {
                Debug.LogError("Sanity modifier identifier \"" + identifier + "\" does not exists. Make sure it matches perfectly with the identifier used when adding the multiplier.");
            }
        }

        private float _ApplyMultipliersTo(float currentSanity, Dictionary<string, float> sanityMultipliers)
        {
            float toReturn = currentSanity;
            foreach (var item in sanityMultipliers)
            {
                toReturn = toReturn * item.Value;
            }
            return toReturn * _lightMultiplier;
        }
        #endregion

    }
}