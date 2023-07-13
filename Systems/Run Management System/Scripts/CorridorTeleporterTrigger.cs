using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class CorridorTeleporterTrigger : MonoBehaviour
    {
        [SerializeField] string _targetTag = "Player";
        [SerializeField] LocalCorridorManager _localCorridorManager;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                _localCorridorManager.TeleportCorridorBackToOrigin(other.gameObject);
            }
        }
    }
}
