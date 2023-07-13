using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class BlockoutLevelFall : MonoBehaviour
    {
        public Animator animator; 

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;


            animator.SetTrigger("Fall");
            
        }
    }
}
