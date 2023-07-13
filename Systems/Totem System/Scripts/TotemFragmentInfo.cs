using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    public class TotemFragmentInfo
    {
        [SerializeField] string _totemName = "Default_Fragment_Name";
        [SerializeField] string _spawnIdentifier = "-1";
        [SerializeField] GameObject _fragmentPrefab;

        public string totemName => _totemName;
        ///<summary> GUID used to find which FragmentSpawnMarker to use to spawn the totem fragment. A value of -1 means that the fragment has not been spawned yet. </summary>
        public string spawnIdentifier => _spawnIdentifier;
        public GameObject fragmentPrefab => _fragmentPrefab;

        public TotemFragmentInfo(GameObject fragmentPrefab, string _totemName)
        {
            _fragmentPrefab = fragmentPrefab;
            this._totemName = _totemName;
        }

        public void SetSpawnIdentifier(string spawnIdentifier)
        {
            _spawnIdentifier = spawnIdentifier;
        }

        public void ResetSpawnIdentifier()
        {
            _spawnIdentifier = "-1";
        }
    }
}