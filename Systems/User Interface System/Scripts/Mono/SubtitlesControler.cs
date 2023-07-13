using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PropellerCap
{
    public class SubtitlesControler : WorldObject
    {
        enum SubtitlesDisplayMethod { WordByWord = 0, LetterByLetter = 1, }

        [SerializeField] TMP_Text _targetTextComponent;
        [SerializeField] SubtitlesDisplayMethod _displayMethod = SubtitlesDisplayMethod.WordByWord;
        [ShowIf(nameof(_displayMethod), 0)]
        [SerializeField] float _wordsPerSec = 5f;
        [ShowIf(nameof(_displayMethod), 1)]
        [SerializeField] float _letterPerSec = 20f;


        private IEnumerator _writingRoutine;
        private bool _isWriting = false;

        #region Public accessors and events
        /// <summary> Whether the SubtitlesController is currently busy writing to the screen. </summary>
        public bool IsWriting => _isWriting;

        #endregion Public accessors and events



        #region WorldObject overrides
        public override string worldName => nameof(SubtitlesControler);
        protected override void MyAwake()
        {
            base.MyAwake();
            HUD.subtitlesControler = this;
        }

        protected override void MyStart()
        {
            base.MyStart();
            _targetTextComponent.text = "";
        }
        #endregion WorldObject overrides

        public void SetSubtitlesText(string text)
        {
            if (_isWriting) return;

            _targetTextComponent.text = text;
        }

        public void DisplayText(VoiceLine voiceLine)
        {
            if (voiceLine == null)
            {
                Debugger.LogError($"{nameof(VoiceLineBase)} is null, can not create subtitles.");
                return;
            }

            string textToDisplay = Managers.localizationManager.GetLocalizedText(voiceLine);
            DisplayText(textToDisplay, voiceLine.ScreenTime);
        }

        public void DisplayText(string textToDisplay, float displayTime)
        {
            if (_writingRoutine != null)
                StopCoroutine(_writingRoutine);
            _writingRoutine = _Co_DisplayTextCoroutine(textToDisplay, _wordsPerSec, _letterPerSec, displayTime);
            StartCoroutine(_writingRoutine);
        }

        private IEnumerator _Co_DisplayTextCoroutine(string textToDisplay, float wordsPerSec, float lettersPerSec, float removeAfterSeconds)
        {
            _isWriting = true;
            _targetTextComponent.text = "";
            string[] words = textToDisplay.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (_displayMethod == SubtitlesDisplayMethod.LetterByLetter)
                {
                    //Display the letter one by one
                    string word = words[i];
                    for (int j = 0; j < word.Length; j++)
                    {
                        _targetTextComponent.text += word[j];
                        yield return new WaitForSeconds(1f / lettersPerSec);
                    }
                    _targetTextComponent.text += " ";
                }
                else
                {
                    _targetTextComponent.text += words[i] + " ";
                    yield return new WaitForSeconds(1f / wordsPerSec);
                }
            }

            yield return new WaitForSecondsRealtime(removeAfterSeconds);
            _targetTextComponent.text = "";

            _isWriting = false;
        }
    }
}