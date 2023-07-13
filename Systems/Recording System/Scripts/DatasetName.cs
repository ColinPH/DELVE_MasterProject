using UnityEngine;
using System;

namespace PropellerCap
{
    public class DatasetName
    {
        public string prefix;
        public string levelName;
        public DateTime dateTime;
        public DatasetName(string rawName)
        {
            string[] sections = rawName.Split('_');

            prefix = sections[0];
            levelName = sections[1];
            string time = sections[2];
            string[] tsec = time.Split(' ');
            string dTime = $"{tsec[0]} {tsec[1].Replace('-', ':')}";

            if (DateTime.TryParse(dTime, out dateTime) == false)
                Debug.Log($"Failed to parse \"{dTime}\"");
        }
    }
}