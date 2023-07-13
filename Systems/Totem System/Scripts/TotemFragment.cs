using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class TotemFragment : WorldObject, IInteractable
    {
        public string playerTag = "Player";
        public VisualEffectClip _collectVFXClip;
        public SoundClip _collectSound;
        [SerializeField] VoiceLineBase _firstFragmentCollectedEver;


        protected TotemManager m_totemManager { get; set; }

        bool _hasBeenInitialized = false;
        TotemFragmentInfo _fragmentInfo;

        public void InitializeFragment(TotemFragmentInfo item)
        {
            if (_hasBeenInitialized)
                return;

            _fragmentInfo = item;
            m_totemManager = Managers.totemManager;

            m_InitializeFragment();

            _hasBeenInitialized = true;
        }

        public virtual void m_InitializeFragment()
        {
            
        }

        protected override void MyStart()
        {
            base.MyStart();

            //m_EnsureGameObjectHasTrigger();
        }

        /*void OnTriggerEnter(Collider collider)
        {
            if (playerTag != collider.gameObject.tag) return;
            *//*if (m_totemManager == null)
            {
                m_totemManager = Managers.totemManager;
            }*//*
            //m_totemManager.NewTotemCollected(this);

            _AddFragmentToFragmentHolder(collider.gameObject.GetComponent<TotemHolder>());

            Destroy(gameObject);
        }*/



        private void _AddFragmentToFragmentHolder(TotemHolder holder)
        {
            //Spawn the collect VFX
            VisualEffects.SpawnVFX(_collectVFXClip, transform);
            Sound.PlaySound(_collectSound, gameObject);

            if (Saver.progression.hasCollectedFragment == false)
                Sound.PlayVoiceLine(_firstFragmentCollectedEver, gameObject);

            holder.CollectNewTotemFragment(_fragmentInfo);

            Metrics.levelData.collectedFragmentAmount += 1;

            Destroy(gameObject);
        }



        #region IInteractable interface
        public void Interact(GameObject callingObject)
        {
            _AddFragmentToFragmentHolder(callingObject.GetComponent<TotemHolder>());
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionStop()
        {
            throw new NotImplementedException();
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            return true;
        }

        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }
        #endregion IInteractable interface
    }
}
