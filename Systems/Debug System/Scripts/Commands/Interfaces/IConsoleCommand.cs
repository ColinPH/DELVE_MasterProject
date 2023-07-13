using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap.QA
{
    public interface IConsoleCommand
    {
        public string commandWord { get; }
        public string description { get; }
        public abstract bool Process(DebuggerManager debugManager, string[] arguments);
    }
}