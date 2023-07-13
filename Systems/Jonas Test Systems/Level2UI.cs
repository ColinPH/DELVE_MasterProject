using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace PropellerCap
{
    public class Level2UI : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI _sanityText;
        [SerializeField]private TextMeshProUGUI _totemText;

        private int fragments;

        private void Start()
        {
            
            Managers.eventManager.SubscribeToRoomEvent(RoomEvent.FragmentCollected, onFragmentCollected);
            Managers.eventManager.SubscribeToGameEvent(GameEvent.SceneStart, onLevelStart);
        }

        private void OnDestroy()
        {
            Managers.eventManager.UnsubscribeFromRoomEvent(RoomEvent.FragmentCollected, onFragmentCollected);
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.SceneStart, onLevelStart);
        }

        private void onLevelStart()
        {


            Debug.Log("onLevelStart");
            fragments = Managers.totemManager.remainingFragments;
            if (_totemText != null )
                _totemText.text = "Totems Left: " + fragments.ToString();

        }

        private void onFragmentCollected()
        {
            _totemText.text = "Totems Left: " + Managers.totemManager.remainingFragments.ToString();
        }

        public void displaySanity()
        {
            _sanityText.text = "Sanity Left " + Sanity.current.ToString("0.0") + "/" + Sanity.max.ToString("0");
        }

    }
}
