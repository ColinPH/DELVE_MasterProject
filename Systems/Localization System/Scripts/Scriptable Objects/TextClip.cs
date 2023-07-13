using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Localization/" + nameof(TextClip), order = 1)]
    public class TextClip : ScriptableObject
    {
        [SerializeField] int _localizationID = -1;
        [SerializeField, TextArea] string _defaultText = "";
        [SerializeField, TextArea] string _context = "";
        [SerializeField, TextArea] string _notes = "";
        [SerializeField] float _screenTime = 2f;


        #region Public accessors
        /// <summary> The ID used to find the translations in the localization datafile. </summary>
        public int LocalizationID => _localizationID;
        /// <summary> The text to use by default. </summary>
        public string DefaultText => _defaultText;
        /// <summary> What is the context of the text. Is it a menu item or a voice line in a particular situation. </summary>
        public string Context => _context;
        /// <summary> Any comments for the developpers about this text. </summary>
        public string Notes => _notes;/// <summary> The amount of time the text should stay on the screen. </summary>
        public float ScreenTime => _screenTime;
        #endregion Public accessors

        public TextClip(int localizationID, string defaultText, string context, string notes, float screenTime)
        {
            _localizationID = localizationID;
            _defaultText = defaultText;
            _context = context;
            _notes = notes;
            _screenTime = screenTime;
        }
    }
}