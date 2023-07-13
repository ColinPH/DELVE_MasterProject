using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class LocalManager : MonoBehaviour
    {
        public virtual string LocalManagerName => "Default LocalManager name";
        bool _applyTryCatch = true;

        bool _hasBeenInitialized = false;

        #region MonoBehaviour
        private void Awake()
        {
            Managers.gameManager.RegisterNewLocalManager(this);
            _applyTryCatch = Managers.debuggerManager._applyTryCatchOnMyBehaviour;
            MonoAwake();
        }

        protected virtual void MonoAwake()
        {

        }

        private void OnDestroy()
        {
            //TODO find a way to not have this check (GameManager is null when exiting playmode)
            if (Managers.gameManager != null)
                Managers.gameManager.DeregisterLocalManager(this);

            MonoDestroy();
        }
        protected virtual void MonoDestroy()
        {

        }
        #endregion

        #region GameManager controls
        public void CallLocalManagerInitialization(object loadingSettings)
        {
            if (_hasBeenInitialized)
            {
                Debugger.LogError("Should not initialize a local manager twice.");
                return;
            }

            if (_applyTryCatch)
            {
                try
                {
                    LocalManagerInitialization(loadingSettings);
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling LocalManagerInitialization on LocalManager \"{LocalManagerName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                LocalManagerInitialization(loadingSettings);
            }

            _hasBeenInitialized = true;
        }
        #endregion GameManager controls

        /// <summary> Called by the scene loader after all sceces of a level have been loaded. </summary>
        protected virtual void LocalManagerInitialization(object loadingSettings)
        {

        }
    }
}