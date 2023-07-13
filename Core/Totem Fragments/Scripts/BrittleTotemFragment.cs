using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class BrittleTotemFragment : TotemFragment
    {
        /*public string playerTag = "Player";

        public override void m_InitializeFragment()
        {

        }

        private void Start()
        {
            Collider coll = GetComponent<Collider>();
            if (coll == null)
            {
                Debug.LogWarning("There is no collider on object: " + gameObject.name);
            }

            if (coll.isTrigger == false)
                Debug.LogWarning("The collider is not a trigger on object: " + gameObject.name);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (playerTag != collider.gameObject.tag) return;
            if (m_totemManager == null)
            {
                m_totemManager = Managers.totemManager;
            }
            m_totemManager.NewTotemCollected(this);
            Destroy(gameObject);
        }*/
    }
}