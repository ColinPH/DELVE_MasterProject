using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class PlayerLevelData
    {
        public PlayerLevelData() { }
        public PlayerLevelData(PlayerLevelData original)
        {
            _name = original.name;
            _startTime = original.startTime;
            _endTime = original.endTime;
            _totalSanityLost = original.totalSanityLost;
            _totalSanityLostByFallDamage = original.totalSanityLostByFallDamage;
            _totalCoinsCollected = original.totalCoinsCollected;
            _successfulFlareGunUses = original.successfulFlareGunUses;
            _failedFlareGunUses = original.failedFlareGunUses;
            _onCooldownFlareShots = original.onCooldownFlareShots;
            _successfulFastHookTravels = original.successfulFastHookTravels;
            _successfulSwingHookTravels = original.successfulSwingHookTravels;
            _successfulFastHookUses = original.successfulFastHookUses;
            _successfulSwingHookUses = original.successfulSwingHookUses;
            _fastHookStopByCollision = original.fastHookStopByCollision;
            _successfulHookLatches = original.successfulHookLatches;
            _failedHookLatches = original.failedHookLatches;
            _onCooldownHookShots = original.onCooldownHookShots;
            _swingHookDurationTimes = new List<float>(original.swingHookDurationTimes);
            _fastHookDurationTimes = new List<float>(original.fastHookDurationTimes);
            _collectedFragmentAmount = original.collectedFragmentAmount;
            _zones = new List<MetricZone>(original.zones);
            _interaction_BrazierExtinguish = original.interaction_BrazierExtinguish;
            _interaction_BrazierIgnite = original.interaction_BrazierIgnite;
            _interaction_FlammableIgniteWithFlare = original.interaction_FlammableIgniteWithFlare;
            _interaction_PotsDestroyed = original.interaction_PotsDestroyed;
        }

        [SerializeField] string _name = "";
        [SerializeField] float _startTime = 0f;
        [SerializeField] float _endTime = 0f;
        [SerializeField] float _totalSanityLost = 0f;
        [SerializeField] float _totalSanityLostByFallDamage = 0f;
        [SerializeField] int _totalCoinsCollected = 0;
        [SerializeField] int _successfulFlareGunUses = 0;
        [SerializeField] int _failedFlareGunUses = 0;
        [SerializeField] int _onCooldownFlareShots = 0;
        [SerializeField] int _successfulFastHookTravels = 0;
        [SerializeField] int _successfulSwingHookTravels = 0;
        [SerializeField] int _successfulFastHookUses = 0;
        [SerializeField] int _successfulSwingHookUses = 0;
        [SerializeField] int _fastHookStopByCollision = 0;
        [SerializeField] int _successfulHookLatches = 0;
        [SerializeField] int _failedHookLatches = 0;
        [SerializeField] int _onCooldownHookShots = 0;
        [SerializeField] List<float> _swingHookDurationTimes = new List<float>();
        [SerializeField] List<float> _fastHookDurationTimes = new List<float>();
        [SerializeField] int _collectedFragmentAmount = 0;
        [SerializeField] List<MetricZone> _zones = new List<MetricZone>();
        [SerializeField] int _interaction_BrazierExtinguish = 0;
        [SerializeField] int _interaction_BrazierIgnite = 0;
        [SerializeField] int _interaction_FlammableIgniteWithFlare = 0;
        [SerializeField] int _interaction_PotsDestroyed = 0;

        /// <summary> Name of the level. </summary>
        public string name { get => _name; set => _name = value; }
        /// <summary> When the level has been loaded. </summary>
        public float startTime { get => _startTime; set => _startTime = value; }
        /// <summary> When the collector in the corridor has been activated. </summary>
        public float endTime { get => _endTime; set => _endTime = value; }
        /// <summary> Sanity lost in the level. </summary>
        public float totalSanityLost { get => _totalSanityLost; set => _totalSanityLost = value; }
        /// <summary> The amount of sanity the player lost due to fall damages. </summary>
        public float totalSanityLostByFallDamage { get => _totalSanityLostByFallDamage; set => _totalSanityLostByFallDamage = value; }
        /// <summary> Coins collected in the level. </summary>
        public int totalCoinsCollected { get => _totalCoinsCollected; set => _totalCoinsCollected = value; }
        /// <summary> When the flare gun ability has been used with charges. </summary>
        public int successfulFlareGunUses { get => _successfulFlareGunUses; set => _successfulFlareGunUses = value; }
        /// <summary> When the flare gun ability has been used without charges. </summary>
        public int failedFlareGunUses { get => _failedFlareGunUses; set => _failedFlareGunUses = value; }
        /// <summary> When the flare gun ability has been used while on cooldown. </summary>
        public int onCooldownFlareShots { get => _onCooldownFlareShots; set => _onCooldownFlareShots = value; }
        /// <summary> When the fast hook is activated for more than the threshold time. </summary>
        public int successfulFastHookTravels { get => _successfulFastHookTravels; set => _successfulFastHookTravels = value; }
        /// <summary> When the player has swang for more than the thresholdtime. </summary>
        public int successfulSwingHookTravels { get => _successfulSwingHookTravels; set => _successfulSwingHookTravels = value; }
        /// <summary> When the fast hook is activated. </summary>
        public int successfulFastHookUses { get => _successfulFastHookUses; set => _successfulFastHookUses = value; }
        /// <summary> When the swing hook is activated. </summary>
        public int successfulSwingHookUses { get => _successfulSwingHookUses; set => _successfulSwingHookUses = value; }
        /// <summary> When the fast hook has ended due to collision. </summary>
        public int fastHookStopByCollision { get => _fastHookStopByCollision; set => _fastHookStopByCollision = value; }
        /// <summary> When the hook latches successfuly. </summary>
        public int successfulHookLatches { get => _successfulHookLatches; set => _successfulHookLatches = value; }
        /// <summary> When the hook does not latch. </summary>
        public int failedHookLatches { get => _failedHookLatches; set => _failedHookLatches = value; }
        /// <summary> When the hook ability has been used while on cooldown. </summary>
        public int onCooldownHookShots { get => _onCooldownHookShots; set => _onCooldownHookShots = value; }
        /// <summary> List of swing hook durations after successful swing hook usess. </summary>
        public List<float> swingHookDurationTimes { get => _swingHookDurationTimes; set => _swingHookDurationTimes = value; }
        /// <summary> List of fast hook durations after successful fast hook usess. </summary>
        public List<float> fastHookDurationTimes { get => _fastHookDurationTimes; set => _fastHookDurationTimes = value; }
        /// <summary> Amount of fragments collected in the level. </summary>
        public int collectedFragmentAmount { get => _collectedFragmentAmount; set => _collectedFragmentAmount = value; }
        /// <summary> The zones of the level and the time it took to go through them. </summary>
        public List<MetricZone> zones { get => _zones; set => _zones = value; }
        /// <summary> The amount of times the player has turned extinguished a brazier. </summary>
        public int interaction_BrazierExtinguish { get => _interaction_BrazierExtinguish; set => _interaction_BrazierExtinguish = value; }
        /// <summary> The amount of times the player has turned ignited a brazier. </summary>
        public int interaction_BrazierIgnite { get => _interaction_BrazierIgnite; set => _interaction_BrazierIgnite = value; }
        /// <summary> When the player ignites a flammable object (brazier etc..) through shooting a flare. </summary>
        public int interaction_FlammableIgniteWithFlare { get => _interaction_FlammableIgniteWithFlare; set => _interaction_FlammableIgniteWithFlare = value; }
        /// <summary> The amount of times the player has destroyed a pot. </summary>
        public int interaction_PotsDestroyed { get => _interaction_PotsDestroyed; set => _interaction_PotsDestroyed = value; }
    }
}