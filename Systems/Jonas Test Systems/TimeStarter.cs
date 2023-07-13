using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TimeStarter : MonoBehaviour
    {
        [SerializeField] private SimpleTimer timer;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            timer.StartTimer();
            Destroy(this.gameObject);
        }
    }
}
