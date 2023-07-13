using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Loadable/Run Object", order = 1)]
    public class RunObject : ScriptableObject
    {
        [SerializeField] string _runName = "";
        [Header("Levels")]
        [SerializeField] LoadableGroup _levelsFamily;
        [Header("Corridors")]
        [SerializeField] LoadableGroup _corridorsFamily;
        [Header("Totems")]
        [SerializeField] List<TotemObject> _totems = new List<TotemObject>();

        public string RunName => _runName;
        public LoadableGroup levelsFamily => _levelsFamily;
        public LoadableGroup corridorsFamily => _corridorsFamily;
        public List<TotemObject> totems => _totems;

        int _currentLevelStepIndex = -1;
        int _currentCorridorStepIndex = -1;

        public void Init()
        {
            _currentLevelStepIndex = -1;
            _currentCorridorStepIndex = -1;
        }

        #region Access Levels
        public LoadableObject GetNextLevel()
        {
            _currentLevelStepIndex += 1;
            return _levelsFamily.GetLoadableObjects()[_currentLevelStepIndex];
        }
        public LoadableObject GetPreviousLevel()
        {
            _currentLevelStepIndex -= 1;
            return _levelsFamily.GetLoadableObjects()[_currentLevelStepIndex];
        }
        #endregion Access Levels



        #region Access corridors
        public LoadableObject GetNextCorridor()
        {
            _currentCorridorStepIndex += 1;
            if (_currentCorridorStepIndex >= _corridorsFamily.GetLoadableObjects().Count)
                _currentCorridorStepIndex = 0;
            return _corridorsFamily.GetLoadableObjects()[_currentCorridorStepIndex];
        }
        public LoadableObject GetPreviousCorridor()
        {
            _currentCorridorStepIndex -= 1;
            return _corridorsFamily.GetLoadableObjects()[_currentCorridorStepIndex];
        }
        #endregion Access corridors
    }
}