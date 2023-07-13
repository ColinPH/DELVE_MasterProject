using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SpeedVisualUpdater : MonoBehaviour
    {
        Material _targetMaterial;
        public float _speedThreshold = 5f;

        public PropertyControlOptions _speedCustomPassController;

        private float _lastSpeedValue = 0f;

        ShaderPropertyController _propertyController;

        FirstPersonCharacterController _fpController;

        private void Start()
        {
            _targetMaterial = HUD.customPassHandler.GetPassMaterial(CustomPassType.SpeedEffect, false);
            _propertyController = new ShaderPropertyController(_targetMaterial, _speedCustomPassController);
            Managers.playerManager.onNewPlayerSpawned += _NewPlayerSpawned;

            if (Player.ActivePlayer != null)
                _NewPlayerSpawned(Player.ActivePlayer);
        }

        private void OnDestroy()
        {
            Managers.playerManager.onNewPlayerSpawned -= _NewPlayerSpawned;
        }

        private void _NewPlayerSpawned(PlayerCharacter newPlayer)
        {
            _fpController = newPlayer.gameObject.GetComponent<FirstPersonCharacterController>();
        }


        private void Update()
        {
            float speedValue = _fpController.Velocity().magnitude;

            if (_propertyController == null)
            {
                _targetMaterial = HUD.customPassHandler.GetPassMaterial(CustomPassType.SpeedEffect, false);
                _propertyController = new ShaderPropertyController(_targetMaterial, _speedCustomPassController);
            }

            if (speedValue >= _speedThreshold)
            {
                
            }
        }
    }
}
