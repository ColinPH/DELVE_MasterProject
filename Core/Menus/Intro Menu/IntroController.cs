using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class IntroController : MonoBehaviour
    {
        bool _keyHasBeenPressed = false;
        public float timeToWait = 5;
        public Animator animator;
        public GameObject displayText;
        public string animationTrigger;
        [SerializeField] SoundClip _wavesSound;
        [SerializeField] SoundClip _musicSound;

        private void Start()
        {
            HUD.blackFader.FadeFromBlack();

            //Play the waves sound
            Sound.PlaySound(_wavesSound, gameObject);

            //Hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            if ((Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) && _keyHasBeenPressed == false)
            {
                StartCoroutine(Wait());
                _keyHasBeenPressed = true;
            }
        }


        IEnumerator Wait()
        {
            Sound.PlaySound(_musicSound, gameObject);
            animator.SetTrigger(animationTrigger);
            displayText.SetActive(false);
            Metrics.manager.OpenSessionData();

            yield return new WaitForSecondsRealtime(timeToWait);

            Managers.runManager.StartTutorialRunFromIntroScene();
        }

    }
}
