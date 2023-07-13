using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class PauseMenuController : MonoBehaviour
    {
        public static PauseMenuController Instance { get; private set; }

        [SerializeField] Key _pauseKey = Key.Escape;
        [SerializeField] GameObject _pauseMenuObj;

        bool _menuIsShown = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _pauseMenuObj.SetActive(false);
        }

        private void Update()
        {
            if (Keyboard.current[_pauseKey].wasPressedThisFrame)
            {
                if (_menuIsShown)
                    HidePauseMenu();
                else
                    ShowPauseMenu();
            }
        }

        public void HidePauseMenu()
        {
            _pauseMenuObj.SetActive(false);
            _menuIsShown = false;
            _ControlCursor(true);
        }

        public void ShowPauseMenu()
        {
            _pauseMenuObj.SetActive(true);
            _menuIsShown = true;
            _ControlCursor(false);
            _pauseMenuObj.GetComponent<PauseMenu>().ShowPauseMenu();
        }

        private void _ControlCursor(bool lockCursor)
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
