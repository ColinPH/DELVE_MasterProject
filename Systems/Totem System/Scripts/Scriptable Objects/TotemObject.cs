using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Totems/Totem Object", order = 1)]
    public class TotemObject : ScriptableObject
    {
        [SerializeField] string _totemName = "Default_Totem_Name";
        [Tooltip("Amount of fragments required to assemble the totem.")]
        [SerializeField] int _requiredFragments = 3;
        [SerializeField] List<GameObject> _fragmentPrefabs = new List<GameObject>();
        [SerializeField] GameObject _socketFragment;
        [Header("Voice Lines")]
        [SerializeField] VoiceLineBase _firstFragmentInCollectorVoiceLine;
        public Color fragmentIconInJournalColor;

        #region Public accessors and events

        public int requiredFragmentsAmount => _requiredFragments;
        public string totemName => _totemName;
        public List<GameObject> fragmentPrefabs => _fragmentPrefabs;
        public GameObject socketFragment => _socketFragment;
        public VoiceLineBase voiceLine_FirstFragmentInCollector => _firstFragmentInCollectorVoiceLine;
        #endregion Public accessors and events

        public List<TotemFragmentInfo> GetFragmentsInfo()
        {
            List<TotemFragmentInfo> toReturn = new List<TotemFragmentInfo>();

            if (_fragmentPrefabs.Count < _requiredFragments)
            {
                Debugger.LogError($"There are not enough fragment prefabs ({_fragmentPrefabs.Count}) while ({_requiredFragments}) are needed, in object named \"{name}\". You can duplicate the elements of the list.");
                return null;
            }

            for (int i = 0; i < _requiredFragments; i++)
            {
                toReturn.Add(new TotemFragmentInfo(_fragmentPrefabs[i], _totemName));
            }
            return toReturn;
        }
    }
}
