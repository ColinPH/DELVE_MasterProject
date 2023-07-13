using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    [RequireComponent(typeof(WorldEntity))]
    public class Creep : MonoBehaviour, IWatcherDependent
    {
        [Header("------ Runtime ------")]
        [SerializeField] int _lifes = 0;

        List<IWatcher> _watchers = new List<IWatcher>();
        List<Vector3> _watcherPositionsOffset = new List<Vector3>();

        WorldEntity _worldEntity;

        private void Start()
        {
            _worldEntity = GetComponent<WorldEntity>();
            _watchers = GetComponentsInChildren<IWatcher>().ToList();
            _lifes = _watchers.Count;

            foreach (IWatcher item in _watchers)
            {
                _watcherPositionsOffset.Add(item.GetWatcherObject().transform.localPosition);
                item.AddDependentEntity(this);
            }
        }

        #region IWatcherDependent interface

        public void RemoveDependency(IWatcher watcher)
        {
            _watchers.Remove(watcher);
            _lifes -= watcher.GetHealthValue();

            if (_lifes <= 0)
                _worldEntity.InvokeOnEntityDeath();
        }

        public Vector3 GetWatcherRestPosition(IWatcher watcher)
        {
            return transform.position + _watcherPositionsOffset[_watchers.IndexOf(watcher)];
        }

        public GameObject GetWatcherDependentObject()
        {
            return gameObject;
        }

        #endregion


    }
}
