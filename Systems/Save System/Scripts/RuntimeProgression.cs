using System;

namespace PropellerCap
{
    /// <summary> Holds information about the total progress of the game. </summary>
    [Serializable]
    public class RuntimeProgression
    {
        /// <summary> Whether the player has completed the tutorial and has entered the hub. </summary>
        public bool tutorialCompleted { get; set; } = false;
        /// <summary> Whether the curse in the hub has been taken or not. </summary>
        public bool curseApplied { get; set; } = false;
        /// <summary> Whether the player has collected a coin at least once. </summary>
        public bool hasCollectedCoin { get; set; } = false;
        /// <summary> Whether the player has collected a totem fragment at least once. </summary>
        public bool hasCollectedFragment { get; set; } = false;
        /// <summary> Whether the player has down to the bottom of the abys in the bridge level. </summary>
        public bool hasLookedInBridgeAbyss { get; set; } = false;

        //Coins
        public bool reachedCoinThreshold_100 { get; set; } = false;
        public bool reachedCoinThreshold_200 { get; set; } = false;
        public bool reachedCoinThreshold_300 { get; set; } = false;
        public bool reachedCoinThreshold_400 { get; set; } = false;
        public bool reachedCoinThreshold_500 { get; set; } = false;
        public bool reachedCoinThreshold_1000 { get; set; } = false;
    }
}