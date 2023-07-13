using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Sound/" + nameof(VoiceLine), order = 1)]
    public class VoiceLine : VoiceLineBase
    {
        [SerializeField] int _localizationID = -1;
        [SerializeField, TextArea] string _defaultText = "";
        [SerializeField, TextArea] string _context = "";
        public AK.Wwise.Event soundEvent;
        [SerializeField] float _screenTime = 2f;
        [SerializeField, TextArea] string _notes = "";
        [Header("Implementation Details")]
        [SerializeField, TextArea] string _implementationNotes = "";
        [SerializeField] bool _isInGroup = false;
        [SerializeField] List<Object> _implementedInObjects;

        #region Public accessors
        /// <summary> The ID used to find the translations in the localization datafile. </summary>
        public int LocalizationID { get => _localizationID; set => _localizationID = value; }
        /// <summary> The text to use by default. </summary>
        public string DefaultText { get => _defaultText; set => _defaultText = value; }
        /// <summary> What is the context of the voice line,hat is the situation in which it is expressed. </summary>
        public string Context { get => _context; set => _context = value; }
        /// <summary> Any comments for the developpers about this text. </summary>
        public string Notes { get => _notes; set => _notes = value; }
        /// <summary> The amount of time the text should stay on the screen. </summary>
        public float ScreenTime { get => _screenTime; set => _screenTime = value; }
        /// <summary> The name of the voice line scriptable object. </summary>
        public string VoiceLineName => name;
        #endregion Public accessors


        #region Implicit conversions
        public static implicit operator TextClip(VoiceLine voiceLine)
        {
            return new TextClip(voiceLine.LocalizationID, voiceLine.DefaultText, voiceLine.Context, voiceLine.Notes, voiceLine.ScreenTime);
        }
        #endregion Implicit conversions
    }
}