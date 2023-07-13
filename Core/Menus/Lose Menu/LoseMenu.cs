using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class LoseMenu : MonoBehaviour
    {
        public Button _quitButton;
        public Button _mainMenuButton;

        private void Start()
        {
            _quitButton.onClick.AddListener(() => {
                Managers.gameManager.QuitGame();
            });

            _mainMenuButton.onClick.AddListener(() => {

                Destroy(FindObjectOfType<PlayerCharacter>().gameObject);

                _quitButton.gameObject.SetActive(false);
                _mainMenuButton.gameObject.SetActive(false);
                //Managers.sceneLoader.Load(UniqueScene.MainMenu, null, true);
            });

            _quitButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            
            Managers.eventManager.SubscribeToGameEvent(GameEvent.RunFailed, OnPlayerDeath);
        }

        private void OnPlayerDeath()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _quitButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
        }
    }
}
