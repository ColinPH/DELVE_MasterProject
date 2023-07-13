using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap
{
    public class FragmentSpawnMarker : MonoBehaviour
    {
        [SerializeField] string _spawnIdentifier = "-1";
        /// <summary> GUID used when spawning totem fragments by matching the fragments identifiers with the corresponding spawn marker identifier. 
        /// If the marker identifier is -1, a new GUID will be generated.</summary>
        public string spawnIdentifier
        {
            get
            {
                if (_spawnIdentifier == "-1")
                {
                    _spawnIdentifier = Guid.NewGuid().ToString();
                    Debug.Log($"GUID for spawn identifier has not been generated yet. Generating it now but this should not happen at runtime.");
                }
                return _spawnIdentifier;
            }
        }

        public bool HasGUID()
        {
            return _spawnIdentifier != "-1";
        }

        [ContextMenu("Generate GUID")]
        /// <summary> Used by the custom in spector class to create the GUID when the object is created.</summary>
        public void GenerateGUID()
        {
            if (_spawnIdentifier == "-1")
                _spawnIdentifier = Guid.NewGuid().ToString();
        }
    }
}
