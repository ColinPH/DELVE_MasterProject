using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public virtual bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            Debug.LogWarning("Manager base should be overwriten " + gameObject.name);
            existingManager = null;
            return false;
        }

        #region Monobehaviour methods
        private void Awake()
        {
            DontDestroyOnLoad(this);
            MonoAwake();
        }
        private void Update()
        {
            MyUpdate();
        }
        #endregion



        #region Initialization
        /// <summary> Called when the Awake has been called on the gameObject from Unity. Same as Awake. </summary>
        protected virtual void MonoAwake()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        /// <summary> Assigning to static classes and local initializations. First method to be called. </summary>
        public virtual void Init()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        /// <summary> Called after Init and before MyStart. </summary>
        public virtual void MyAwake()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        /// <summary> Called after MyAwake and before Awake on the scene's MonoBehaviours. </summary>
        public virtual void MyStart()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        /// <summary> Called every frame just like the update method. </summary>
        public virtual void MyUpdate()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        /// <summary> Called after the game has been prepared and before the game loop starts. Mandatory scenes have been loaded and MyAwake, MyStart have been called on the loaded objects. This is before the first game scene is loaded. </summary>
        public virtual void OnGamePreparationComplete()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }
        #endregion

        /// <summary> Called when a new level has finished loading. </summary>
        public virtual void OnNewLevelLoaded()
        {
            //These are not necessarily calling base, therefore there should be nothing here
        }

    }
}
