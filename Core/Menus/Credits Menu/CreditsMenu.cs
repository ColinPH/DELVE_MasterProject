using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class CreditsMenu : MonoBehaviour
    {
        [SerializeField] Button _quitButton;
        [SerializeField] Button _backToHubButton;

        private void Start()
        {
            _backToHubButton.onClick.AddListener(() => {
                Managers.gameManager.StartGameFromSave(1);
            });
            _quitButton.onClick.AddListener(() => {
                Managers.gameManager.QuitGame();
            });
        }
    }
}
