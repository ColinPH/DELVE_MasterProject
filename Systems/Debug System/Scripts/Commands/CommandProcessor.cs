using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap.QA
{
    public class CommandProcessor
    {
        CommandsBundle _commandsBundle;

        public CommandProcessor()
        {
            _commandsBundle = ScriptableObject.CreateInstance<CommandsBundle>();
            _commandsBundle.Initialize();
        }

        public CommandProcessor(CommandsBundle commandsBundle)
        {
            _commandsBundle = ScriptableObject.CreateInstance<CommandsBundle>();
            _commandsBundle.Initialize(commandsBundle);
        }

        public void AddCommand(IConsoleCommand commandToAdd)
        {
            //Check that there isn't already a name with that command
            _commandsBundle.AddCommand(commandToAdd);
        }

        public bool ProcessCommand(string inputText, DebuggerManager debugManager)
        {
            string commandWord;
            string[] arguments;

            bool toReturn = _ConvertCommand(inputText, out commandWord, out arguments);

            foreach (IConsoleCommand item in _commandsBundle.commands)
            {
                if (string.Equals(item.commandWord, commandWord) == false)
                    continue;

                if (item.Process(debugManager, arguments) == false)
                    toReturn = false;
            }
            return toReturn;
        }

        private bool _ConvertCommand(string inputText, out string commandWord, out string[] arguments)
        {
            bool toReturn = true;

            if (inputText.StartsWith(_commandsBundle.prefix) == false)
            {
                commandWord = "Wrong Prefix";
                arguments = null;
                return false;
            }

            inputText = inputText.Remove(0, _commandsBundle.prefix.Length);
            string[] splitText = inputText.Split(' ');
            commandWord = splitText[0];
            arguments = splitText.Skip(1).ToArray();
            return toReturn;
        }

    }
}