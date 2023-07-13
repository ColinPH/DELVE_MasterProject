using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PropellerCap
{
    /// <summary> This component is responsible for changing the text of the targeted Text component based on the localization file. </summary>
    [AddComponentMenu("AAPropellerCap/Localization/" + nameof(LocalizedText))]
    public class LocalizedText : WorldObject
    {
        [SerializeField] TextClip _targetTextClip;

        TMP_Text _targetText;

        public TextClip TargetTextClip => _targetTextClip;

        protected override void MyAwake()
        {
            base.MyAwake();
            _targetText = m_FetchForComponent<TMP_Text>();

            //TODO apply localization to the given TextClip though LocaliationManager and assign it to the Text component
            string localizedText = Managers.localizationManager.GetLocalizedText(_targetTextClip);
            _ApplyTextOnComponent(localizedText);
        }

        void _ApplyTextOnComponent(string textToApply)
        {
            _targetText.text = textToApply;
        }

        #region Inspector methods
        public void Inspector_ApplyTextOnComponent()
        {
            _targetText = m_FetchForComponent<TMP_Text>();
            _ApplyTextOnComponent(_targetTextClip.DefaultText);
        }
        #endregion Inspector methods
    }
}