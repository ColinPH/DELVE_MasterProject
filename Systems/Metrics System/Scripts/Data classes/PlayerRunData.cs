using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class PlayerRunData
    {
        public PlayerRunData() { }
        public PlayerRunData(PlayerRunData original)
        {
            _name = original.name;
            _startTime = original.startTime;
            _endTime = original.endTime;
            _totemNames = original.totalFragmentsDeposited.Keys.ToList();
            _totemAmounts = original.totalFragmentsDeposited.Values.ToList();
            _levels = new List<PlayerLevelData>(original.levels);
        }

        [SerializeField] string _name;
        [SerializeField] float _startTime;
        [SerializeField] float _endTime;
        [SerializeField] List<string> _totemNames = new List<string>();
        [SerializeField] List<int> _totemAmounts = new List<int>();
        [SerializeField] List<PlayerLevelData> _levels = new List<PlayerLevelData>();

        /// <summary> Name of the level. </summary>
        public string name { get => _name; set => _name = value; }
        /// <summary> When the level has been loaded. </summary>
        public float startTime { get => _startTime; set => _startTime = value; }
        /// <summary> When the collector in the corridor has been activated. </summary>
        public float endTime { get => _endTime; set => _endTime = value; }
        /// <summary> The totem fragments that have been deposite in the collector throughout the run. </summary>
        public Dictionary<string, int> totalFragmentsDeposited
        {
            get 
            {
                Dictionary<string, int> toReturn = new Dictionary<string, int>();
                for (int i = 0; i < _totemNames.Count; i++)
                {
                    toReturn.Add(_totemNames[i], _totemAmounts[i]);
                }
                return toReturn;            
            }
        }
        public void AddDepositedFragment(string fragmentName, int amount)
        {
            if (_totemNames.Contains(fragmentName))
                _totemAmounts[_totemNames.IndexOf(fragmentName)] += amount;
            else
            {
                _totemNames.Add(fragmentName);
                _totemAmounts.Add(amount);
            }
        }
        /// <summary> List of data about the levels that were played during the run. </summary>
        public List<PlayerLevelData> levels { get => _levels; set => _levels = value; }
    }
}