using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class RuntimeTotem
    {
        public string totemName = "No_Name";
        public int levelIndex = -1;
        /// <summary> The amount of fragments required to assemble the totem. </summary>
        public int requiredFragments = -1;
        public List<TotemFragmentInfo> collectedFragments = new List<TotemFragmentInfo>();
        public List<TotemFragmentInfo> remainingFragments = new List<TotemFragmentInfo>();
        public int remainingFragmentsAmount => remainingFragments.Count;
        public int collectedFragmentsAmount => collectedFragments.Count;
        public GameObject fragmentPrefab;
        public Color fragmentIconInJournalColor;
        //VoiceLines
        public VoiceLineBase voiceLine_FirstFragmentInCollector;

        public RuntimeTotem(TotemObject referenceTotem)
        {
            totemName = referenceTotem.totemName;
            requiredFragments = referenceTotem.requiredFragmentsAmount;
            collectedFragments = new List<TotemFragmentInfo>();
            remainingFragments = referenceTotem.GetFragmentsInfo();
            fragmentPrefab = referenceTotem.socketFragment;
            fragmentIconInJournalColor = referenceTotem.fragmentIconInJournalColor;
            voiceLine_FirstFragmentInCollector = referenceTotem.voiceLine_FirstFragmentInCollector;
        }

        public void AddNewCollectedFragment(TotemFragmentInfo collectedFragment)
        {
            collectedFragments.Add(collectedFragment);

            remainingFragments.Remove(collectedFragment);
        }
    }
}