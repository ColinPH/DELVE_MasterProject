using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IWatcherDependent
    {
        /// <summary>
        /// Remove a watcher on which the entity's life relies on
        /// </summary>
        public void RemoveDependency(IWatcher watcher);
        public GameObject GetWatcherDependentObject();
        public Vector3 GetWatcherRestPosition(IWatcher watcher);
    }
}