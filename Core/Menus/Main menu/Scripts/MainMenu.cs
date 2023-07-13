using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button _startSave;
        [SerializeField] Button _quitButton;
        [SerializeField] Button _resetIntroButton;
        [SerializeField] GameObject _tutorialExplanationText;

        private void Start()
        {
            _startSave.onClick.AddListener(() => {
                Managers.gameManager.StartGameFromSave(1);
            });
            _quitButton.onClick.AddListener(() => {
                Managers.gameManager.QuitGame();
            });
            _resetIntroButton.onClick.AddListener(() => {
                PlayerPrefs.SetInt("StartWithMenu", 0);
                _tutorialExplanationText.SetActive(true);
            });
        }
    }
}
