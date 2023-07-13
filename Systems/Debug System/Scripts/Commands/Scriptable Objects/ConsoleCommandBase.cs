using System.Collections;
using UnityEngine;

namespace PropellerCap.QA
{
    public abstract class ConsoleCommandBase : ScriptableObject, IConsoleCommand
    {
        [SerializeField] string _commandWord = "";
        [SerializeField, TextArea] string _description = "";
        public string commandWord => _commandWord;
        public string description => _description;
        public abstract bool Process(DebuggerManager debugManagerstring, string[] arguments);
    }
}