using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public static class Sanity
    {
        static SanityManager _sanityManager;

        /// <summary> Sanity Manager. </summary>
        public static SanityManager manager {
            get
            {
                if (_sanityManager == null)
                    Debug.Log("Sanity manager has not been set in the static Sanity class yet. Check initialization order.");
                return _sanityManager;
            }
            set
            {
                _sanityManager = value;
            }
        }

        /// <summary> Returns the current sanity of the player. </summary>
        public static float current => _sanityManager.currentSanity;
        /// <summary> Returns the maximum sanity value. </summary>
        public static float max => _sanityManager.maxSanity;
        /// <summary> Returns the current sanity multiplier. </summary>
        public static float multiplier => _sanityManager.currentMultiplier;
        public static bool isActive => _sanityManager.sanityIsActive;




        public static void Start() => _sanityManager.StartAfflictingSanity();
        public static void Stop() => _sanityManager.StopAfflictingSanity();

        /// <summary> Adds sanity through the sanity manager. </summary>
        public static void Add(float amount, bool applyMultipliers = true)
        {
            _sanityManager.AddSanity(amount, applyMultipliers);
        }

        /// <summary> Removes sanity through the sanity manager. </summary>
        public static void Remove(float amount, bool applyMultipliers = true) => _sanityManager.RemoveSanity(amount, applyMultipliers);

        /// <summary> Multiplies the current insanity multiplier by the amount. </summary>
        public static void AddMultiplier(float amount, string identifier)
        {
            _sanityManager.AddMultiplier(amount, identifier);
        }

        /// <summary> Divides the current insanity multiplier by the amount. </summary>
        public static void RemoveMultiplier(float amount, string identifier)
        {
            _sanityManager.RemoveMultiplier(amount, identifier);
        }
    }
}