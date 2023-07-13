using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TotemPillarSection : ActivationTarget
    {
        enum PillarSection { Unassigned, Section1, Section2, Section3, Section4, Section5, Section6 }

        [SerializeField] PillarSection _section = PillarSection.Unassigned;
        [SerializeField] List<GameObject> _objectsToActivate = new List<GameObject>();
        [SerializeField] Transform _socket;


        public override void Activate()
        {
            base.Activate();

            foreach (var item in _objectsToActivate)
            {
                item.SetActive(true);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            foreach (var item in _objectsToActivate)
            {
                item.SetActive(false);
            }
        }

        public void SpawnFragmentInSocket(GameObject fragmentPrefab)
        {
            Instantiate(fragmentPrefab, _socket);
        }
    }
}
