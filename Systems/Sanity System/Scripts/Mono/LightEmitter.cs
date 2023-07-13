using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Light and Sanity/" + nameof(LightEmitter))]
    public class LightEmitter : WorldObject
    {

        List<ILightReceiver> _receivers = new List<ILightReceiver>();
        List<ILightReceiver> _activeReceivers = new List<ILightReceiver>();

        [SerializeField, Runtime(true)] bool _flammableIsActive = true;

        IFlammable _flammable;

        #region Public accessors and events
        public delegate void NewEntityEnteredLightHandlder(ILightReceiver lightReceiver);
        public NewEntityEnteredLightHandlder onNewEntityEnteredLight { get; set; }

        public delegate void EntityExitLightHandlder(ILightReceiver lightReceiver);
        /// <summary> Event for when an entity exits the light emitter. </summary>
        public EntityExitLightHandlder onEntityExitLight { get; set; }
        #endregion


        protected override void MyAwake()
        {
            base.MyAwake();

            m_EnsureGameObjectHasTrigger();

            _flammable = m_FetchForComponent<IFlammable>(gameObject.GetMainObject());
            if (_flammable != null)
            {
                _flammable.RegisterToIgnite(_OnFlammableIgnite);
                _flammable.RegisterToExtinguish(_OnFlammableExtinguish);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ILightReceiver receiver = other.gameObject.GetComponent<ILightReceiver>();
            if (receiver != null)
            {
                _receivers.Add(receiver);

                if (_activeReceivers.Contains(receiver) == false && _flammableIsActive)
                {
                    receiver.IEntityEnteredLight(gameObject);
                    onNewEntityEnteredLight?.Invoke(receiver);
                    _activeReceivers.Add(receiver);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ILightReceiver receiver = other.gameObject.GetComponent<ILightReceiver>();
            if (receiver != null)
            {
                _receivers.Remove(receiver);

                if (_activeReceivers.Contains(receiver) && _flammableIsActive)
                {
                    receiver.IEntityExitLight(gameObject);
                    onEntityExitLight?.Invoke(receiver);
                    _activeReceivers.Remove(receiver);
                }
            }
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            foreach (ILightReceiver receiver in _activeReceivers)
            {
                receiver.IEntityExitLight(gameObject);
                onEntityExitLight?.Invoke(receiver);
            }
        }



        private void _OnFlammableIgnite()
        {
            foreach (var item in _receivers)
            {
                if (_activeReceivers.Contains(item) == false)
                {
                    item.IEntityEnteredLight(gameObject);
                    _activeReceivers.Add(item);
                    onNewEntityEnteredLight?.Invoke(item);
                }
            }
            _flammableIsActive = true;
        }

        private void _OnFlammableExtinguish()
        {
            foreach (var item in _activeReceivers)
            {
                item.IEntityExitLight(gameObject);
                onEntityExitLight?.Invoke(item);
            }
            _activeReceivers.Clear(); 
            _flammableIsActive = false;
        }
    }
}