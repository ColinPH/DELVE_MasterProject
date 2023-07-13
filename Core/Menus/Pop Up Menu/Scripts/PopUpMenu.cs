using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class PopUpMenu : WorldObject
    {
        [SerializeField] Animator _coinAnimator;
        [SerializeField] string _showTrigger = "Show";
        [SerializeField] string _hideTrigger = "Hide";

        bool _isShown = false;

        public override string worldName => nameof(PopUpMenu);

        protected override void MyStart()
        {
            base.MyStart();
            //Register to the coin pick up event
            Managers.playerManager.OnPlayerCollectObject += _OnObjectCollected;
        }

        protected override void MyUpdate()
        {
            base.MyUpdate();

            if (Keyboard.current[Key.Tab].wasPressedThisFrame)
            {
                if (_isShown)
                    _HideCoinIcon();
            }
        }

        private void _OnObjectCollected(bool isCoin, bool isFragment)
        {
            _ShowCoinIcon();
            Managers.playerManager.OnPlayerCollectObject -= _OnObjectCollected;
        }

        void _ShowCoinIcon()
        {
            _coinAnimator.SetTrigger("Show"); 
            _isShown = true;
        }

        void _HideCoinIcon()
        {
            _coinAnimator.SetTrigger("Hide");
            _isShown = false;
        }
    }
}
