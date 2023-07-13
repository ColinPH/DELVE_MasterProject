using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Behaviours/Light Behaviour", order = 1)]
    public class LightBehaviour : ScriptableObject
    {
        public float healingRate = 5f;
    }
}