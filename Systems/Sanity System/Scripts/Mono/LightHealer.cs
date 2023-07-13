using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Light and Sanity/" + nameof(LightHealer))]
    [RequireComponent(typeof(LightEmitter))]
    public class LightHealer : WorldObject
    {
        [SerializeField] LightBehaviour _lightBehaviour;
        [SerializeField] bool _healerIsActive = true;
        [SerializeField, Runtime(true)] LightType _lightType = LightType.Fire;
        [SerializeField] List<GameObject> _activeReceivers = new List<GameObject>();


        float _currentHealingAmount = 0f;
        List<ILightReceiver> _lightReceivers = new List<ILightReceiver>();

        IFlammable _flammable;
        LightEmitter _lightEmitter;

        public override string worldName => nameof(LightHealer);

        protected override void MyAwake()
        {
            base.MyAwake();
            m_PropertyIsNull<LightHealer>(_lightBehaviour == null, nameof(_lightBehaviour));

            _flammable = m_FetchForComponent<IFlammable>(gameObject.GetMainObject());
            if (_flammable != null)
            {
                _flammable.RegisterToLightTypeChange(_OnFlammmableLightChange);
                _flammable.RegisterToIgnite(_OnFlammableIgnite);
                _flammable.RegisterToExtinguish(_OnFlammableExtinguish);
            }
            _lightEmitter = m_FetchForComponent<LightEmitter>();
            _lightEmitter.onNewEntityEnteredLight += _OnNewEntityEnteredLight;
            _lightEmitter.onEntityExitLight += _OnEntityExitLight;
        }

        protected override void MyStart()
        {
            base.MyStart();
            if (_flammable != null)
            {
                _lightType = _flammable.GetLightType();
            }
        }

        protected override void MyUpdate()
        {
            base.MyUpdate();

            if (_lightType == LightType.Crystal || _healerIsActive == false)
                return;

            if (_lightReceivers.Count > 0)
            {
                _currentHealingAmount = _lightBehaviour.healingRate * Time.deltaTime;
                foreach (var item in _lightReceivers)
                {
                    item.IApplyLightHealingEffect(this);
                }
            }
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            if (_lightEmitter == null)
                return;

            _lightEmitter.onNewEntityEnteredLight -= _OnNewEntityEnteredLight;
            _lightEmitter.onEntityExitLight -= _OnEntityExitLight;
        }

        /// <summary> Amount of healing applied each frame. Already multiplied by Time.deltaTime. </summary>
        public float HealingAmount => _currentHealingAmount;
        public bool HealerIsActive => _healerIsActive;

        public void EnableHealing()
        {
            _healerIsActive = true;
        }

        public void DisableHealing()
        {
            _healerIsActive = false;
        }


        private void _OnFlammmableLightChange(LightType newLightType)
        {
            _lightType = newLightType;
        }

        private void _OnFlammableIgnite()
        {
            _healerIsActive = true;
        }

        private void _OnFlammableExtinguish()
        {
            _healerIsActive = false;
        }


        private void _OnNewEntityEnteredLight(ILightReceiver lightReceiver)
        {
            if (_lightReceivers.Contains(lightReceiver))
            {
                Debugger.LogError($"{nameof(LightHealer)} already contains the object \"{lightReceiver.IReceivingObject()}\". It should not be assigned twice.");
                return;
            }

            _lightReceivers.Add(lightReceiver);
            _activeReceivers.Add(lightReceiver.IReceivingObject());
        }

        private void _OnEntityExitLight(ILightReceiver lightReceiver)
        {
            if (_lightReceivers.Contains(lightReceiver) == false)
            {
                Debugger.LogError($"Can not remove light emitter \"{lightReceiver.IReceivingObject()}\" because it has not been assingned or has already been removed.");
                return;
            }

            _lightReceivers.Remove(lightReceiver);
            _activeReceivers.Remove(lightReceiver.IReceivingObject());
        }
    }
}