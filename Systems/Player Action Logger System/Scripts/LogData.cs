using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    public class LogData
    {
        public string logText = "";
        public PlayerLogsType logType = PlayerLogsType.Unassigned;

        public LogData(string logText, PlayerLogsType logType)
        {
            this.logText = logText;
            this.logType = logType;
        }
        public LogData(string logText)
        {
            this.logText = logText;
            logType = PlayerLogsType.Unassigned;
        }
    }
}
