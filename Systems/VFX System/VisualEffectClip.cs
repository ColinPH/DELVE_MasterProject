using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/VFX/Visual Effect Clip", order = 1)]
    public class VisualEffectClip : ScriptableObject
    {
        public string vfxName = "Default VFX name";
        [TextArea]
        public string description = "";
        public GameObject vfxPrefab;
        public Vector3 spawnOffset = new Vector3(0, 0, 0);
        public Quaternion rotationOffset = Quaternion.identity;
    }
}