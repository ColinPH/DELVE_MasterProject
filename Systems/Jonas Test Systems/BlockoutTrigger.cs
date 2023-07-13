using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class BlockoutTrigger : MonoBehaviour
    {
        [SerializeField] private Animator Anim;
        [SerializeField] private string triggerName = "Trigger";

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Anim.SetTrigger(triggerName);
        }
    }
}
