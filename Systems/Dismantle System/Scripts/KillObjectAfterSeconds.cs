using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PropellerCap
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class KillObjectAfterSeconds : MonoBehaviour
    {
        [Tooltip("Time in seconds from the moment the object spawns")]
        public float _timeBeforeDeath = 2f;

        private void Start()
        {
            Destroy(gameObject, _timeBeforeDeath + Random.Range(0f, 1f));
        }
    }
}