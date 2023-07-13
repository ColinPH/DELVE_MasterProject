using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IWatcher
    {
        /// <summary>
        /// Add an entity which's life depends on the watcher
        /// </summary>
        public void AddDependentEntity(IWatcherDependent targetObject);
        public int GetHealthValue();
        public GameObject GetWatcherObject();
    }
}