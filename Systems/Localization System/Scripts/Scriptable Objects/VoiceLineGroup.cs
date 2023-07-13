using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Sound/" + nameof(VoiceLineGroup), order = 1)]
    public class VoiceLineGroup : VoiceLineBase
    {
        enum SelectionMethod { Random = 0, }
        [SerializeField] SelectionMethod _selectionMethod = SelectionMethod.Random;
        [SerializeField] List<VoiceLine> _voiceLines = new List<VoiceLine>();

        private int _lastUsedIndex = 0;

        public VoiceLine GetVoiceLine()
        {
            if (_voiceLines.Count == 0)
            {
                Debugger.LogError($"The voice line group \"{name}\" has no voice line assigned.");
                return null;
            }

            switch (_selectionMethod)
            {
                case SelectionMethod.Random:
                    return _voiceLines[_GetRandomIndex()];
                default:
                    return _voiceLines[0];
            }
        }

        public VoiceLine GetLastVoiceLine()
        {
            return _voiceLines[_lastUsedIndex];
        }

        private int _GetRandomIndex()
        {
            _lastUsedIndex = Mathf.FloorToInt(Random.Range(0f, _voiceLines.Count - 0.001f));
            return _lastUsedIndex;
        }

        #region Implicit conversions
        public static implicit operator VoiceLine(VoiceLineGroup group)
        {
            return group.GetVoiceLine();
        }
        #endregion Implicit conversions
    }
}