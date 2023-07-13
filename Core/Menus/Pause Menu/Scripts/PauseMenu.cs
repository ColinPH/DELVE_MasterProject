using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] Button _resumeButton;
        [SerializeField] Button _quitButton;
        [SerializeField] Button _skipIntroButton;
        [SerializeField] Button _questionnaireButton;
        [SerializeField] string _questionnaireURL = "";
        [SerializeField] CollectablesTracker _collectablesTracker;

        private void Start()
        {
            _resumeButton.onClick.AddListener(() => {
                PauseMenuController.Instance.HidePauseMenu();
            });
            _quitButton.onClick.AddListener(() => {
                Managers.gameManager.QuitGame();
            });
            _skipIntroButton.onClick.AddListener(() => {
                PlayerPrefs.SetInt("StartWithMenu", 1);
            });
            _questionnaireButton.onClick.AddListener(() => {
                string url = _questionnaireURL;
                Application.OpenURL(url);
            });
        }

        public void ShowPauseMenu()
        {
            _collectablesTracker.UpdateCollectableValues();
        }
    }
}
