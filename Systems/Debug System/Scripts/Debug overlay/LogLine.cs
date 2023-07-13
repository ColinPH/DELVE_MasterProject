using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace PropellerCap.QA
{
    public class LogLine
    {
        public string message;

        public LogLine(string message)
        {
            this.message = message;
        }
        
        public string GetLogText()
        {
            return message;
        }

        public int GetLogLength()
        {
            return message.Length;
        }
    }
}