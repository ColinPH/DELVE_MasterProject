using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Commands/Console Commands Bundle", order = 1)]
    public class CommandsBundle : ScriptableObject
    {
        [SerializeField] List<ConsoleCommandBase> _baseCommands = new List<ConsoleCommandBase>();
        public string prefix = "/";
        public List<IConsoleCommand> commands = new List<IConsoleCommand>();


        public void Initialize()
        {
            foreach (IConsoleCommand item in _baseCommands)
            {
                commands.Add(item);
            }
        }

        public void Initialize(CommandsBundle commandsBundle)
        {
            //Add the commands contained in this bundle
            foreach (IConsoleCommand item in _baseCommands)
            {
                commands.Add(item);
            }

            //Add the commands contained in the bundle parameter
            foreach (ConsoleCommandBase item in commandsBundle._baseCommands)
            {
                if (ContainsCommandWithWord(item.commandWord) == false)
                    commands.Add(item);
            }
        }

        public bool ContainsCommandWithWord(string commandWord)
        {
            bool toReturn = false;
            foreach (IConsoleCommand item in commands)
            {
                if (string.Equals(item.commandWord, commandWord))
                    toReturn = true;
            }
            return toReturn;
        }

        public bool AddCommand(IConsoleCommand commandToAdd)
        {
            commands.Add(commandToAdd);
            return true;
        }
    }
}