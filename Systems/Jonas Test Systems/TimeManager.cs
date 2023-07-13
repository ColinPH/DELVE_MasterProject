using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TimeManager : MonoBehaviour
    {
        public SimpleTimer timer;

        private void Start()
        {
            timer.StartTimer();
        }



        public void EndGame()
        {
            timer.StopTimer();
        }
    }
}
