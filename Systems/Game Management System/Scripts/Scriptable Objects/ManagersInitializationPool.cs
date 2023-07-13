using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Settings/Managers Initialization Pool", order = 1)]
    public class ManagersInitializationPool : ScriptableObject
    {
        [Tooltip("Managers that are mandatory for the game to run.")]
        public List<GameObject> essentialManagers = new List<GameObject>();
    }
}
