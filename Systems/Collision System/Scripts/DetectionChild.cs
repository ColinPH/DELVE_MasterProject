using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    public abstract class DetectionChild : MonoBehaviour
    {
        public string detectionName = "Default name";

        //Initialized on Awake by the DetectionParent
        [HideInInspector] public bool hasBeenPopulated = false;
        [HideInInspector] public DetectionParent detectionParent;
        [HideInInspector] public GameObject detectionParentObj;
    }
}