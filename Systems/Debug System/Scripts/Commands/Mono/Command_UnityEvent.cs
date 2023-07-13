using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap.QA
{
    public class Command_UnityEvent : MonoBehaviour, IConsoleCommand
    {
        [SerializeField] string _commandWord = "DefaultCommand";
        [SerializeField, TextArea] string _descrition = "DefaultDescription";
        [SerializeField] UnityEvent _upponCommandValidation;

        private void Start()
        {
            //Register this component to the Cheat console
            Debugger.AddCommand(this);
        }

        #region IConsole Interface implementation

        public string commandWord { get => _commandWord; }

        public string description { get => _descrition; }

        public bool Process(DebuggerManager debuggerManager, string[] arguments)
        {
            if (_commandWord == "DefaultCommand")
                Debug.Log(nameof(Command_UnityEvent) + " component has an unassigned command word on object : " + gameObject.name);

            _upponCommandValidation?.Invoke();
            return true;
        }

        #endregion
    }
}