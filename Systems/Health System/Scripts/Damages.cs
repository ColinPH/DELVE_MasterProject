using System.Collections;
using UnityEngine;

namespace PropellerCap
{
    public class Damages
    {
        public Damages(GameObject sourceObj, float amount)
        {
            sourcObject = sourceObj;
        }
        public GameObject sourcObject;

        public float amount = 0;
    }
}