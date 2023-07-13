using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Commands/Matrix", order = 1)]
    public class Command_Matrix : ConsoleCommandBase
    {
        public override bool Process(DebuggerManager debugManager, string[] arguments)
        {
            if (arguments[0] == "Restart")
            {
               Managers.gameManager.StartCoroutine(Managers.gameManager.Co_RestartGame("Restart command."));
            }

            if (arguments[0] == "Quit")
            {
                Managers.gameManager.QuitGame();
            }


            Debugger.Log($"{commandWord} command has been used.");
            return true;
        }
    }
}