using System;

namespace PropellerCap
{
    /// <summary> A zone in a level in which the player can enter then exit after a certain period of time. Used to measure progression time. </summary>
    [Serializable]
    public class MetricZone
    {
        public string ID = "";
        public float entryTime = 0f;
        public float exitTime = 0f;

        public MetricZone(string iD, float entryTime)
        {
            ID = iD;
            this.entryTime = entryTime;
        }
    }
}