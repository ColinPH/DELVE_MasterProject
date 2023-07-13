using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SetNewSpawn : MonoBehaviour
    {

        [SerializeField] private MAPlayerSpawner spawner = null;
        [SerializeField] private PlayerSpawnPoint newSpawnPoint = null;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            spawner._playerSpawn.transform.position = newSpawnPoint.transform.position;
        }


    }
}
