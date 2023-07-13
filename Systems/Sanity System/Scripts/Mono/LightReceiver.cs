using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Light and Sanity/" + nameof(LightReceiver))]
    public class LightReceiver : WorldObject, ILightReceiver
    {
        [Header("/!\\ Runtime Information /!\\")]
        [SerializeField] bool _isInLight = false;
        [SerializeField] int _amountLights = 0;
        [SerializeField] List<GameObject> _activeLights = new List<GameObject>();

        #region Public accessors and events
        public delegate void LightEnteredHandler();
        public LightEnteredHandler OnLightEntered { get; set; }

        public delegate void LightExitHandler();
        public LightExitHandler OnLightExit { get; set; }

        public delegate void LightHealingEffectHandler(LightHealer healer);
        /// <summary> Called each frame while the entity is in the light. </summary>
        public LightHealingEffectHandler onLightHealingEffect { get; set; }

        #endregion Public accessors and events

        protected override void MyStart()
        {
            base.MyStart();

            Managers.eventManager.SubscribeToGameEvent(GameEvent.SceneUnloaded, _OnSceneUnloaded);
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            Managers.eventManager.UnsubscribeFromGameEvent(GameEvent.SceneUnloaded, _OnSceneUnloaded);
        }

        private void _OnSceneUnloaded()
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var item in _activeLights)
            {
                if (item == null)
                {
                    Debugger.Log($"Found a null item in the active lights \"{gameObject.GetMainObject().name}\"");
                    toRemove.Add(item);
                }
            }
            foreach (var item in toRemove)
            {
                _amountLights -= 1;
                _activeLights.Remove(item);
            }
            if (_amountLights == 0)
            {
                _ExitLight();
            }
        }


        #region ILightReceiver interface
        public void IEntityEnteredLight(GameObject emitter)
        {
            _amountLights += 1;
            _activeLights.Add(emitter);

            if (_isInLight)
                return;

            _EnteredLight();
        }

        public void IEntityExitLight(GameObject emitter)
        {
            if (_activeLights.Contains(emitter) == false)
            {
                Debugger.LogError($"The entity \"{gameObject.GetMainObject().name}\" has exited the light \"{emitter.GetMainObject().name}\" but that light was not registered by the entity. It should have been registered.");
                return;
            }
            _amountLights -= 1;
            _activeLights.Remove(emitter);

            if (_isInLight == false)
            {
                Debugger.LogError($"{gameObject.name} is not in light but the following light has been removed : {emitter.name}");
                return;
            }

            if (_amountLights == 0)
                _ExitLight();
        }

        public void IApplyLightHealingEffect(LightHealer healer)
        {
            onLightHealingEffect?.Invoke(healer);
        }

        public GameObject IReceivingObject() => gameObject;

        #endregion ILightReceiver interface

        private void _EnteredLight()
        {
            _isInLight = true;
            OnLightEntered?.Invoke();
        }

        private void _ExitLight()
        {
            _isInLight = false;
            OnLightExit?.Invoke();
        }
    }
}