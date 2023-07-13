using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class PlayerSessionData
    {
        public PlayerSessionData()
        {
            _ID = Guid.NewGuid().ToString();
            _dateTime = DateTime.Now;
        }

        [SerializeField] string _ID;
        [SerializeField] DateTime _dateTime;
        [SerializeField] float _startTime;
        [SerializeField] float _endTime;
        [SerializeField] List<PlayerRunData> _runs = new List<PlayerRunData>();

        /// <summary> Unique Guid identifier for the game session. </summary>
        public string ID { get => _ID; set => _ID = value; }
        /// <summary> The DateTime information of when the session has been created. </summary>
        public DateTime dateTime { get => _dateTime; set => _dateTime = value; }
        /// <summary> When the session has started. </summary>
        public float startTime { get => _startTime; set => _startTime = value; }
        /// <summary> When the session has ended. </summary>
        public float endTime { get => _endTime; set => _endTime = value; }
        /// <summary> List of data about the runs that were played during the session. </summary>
        public List<PlayerRunData> runs { get => _runs; set => _runs = value; }
    }
}