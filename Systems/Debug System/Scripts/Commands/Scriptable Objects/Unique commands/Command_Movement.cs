using PropellerCap.QA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Commands/Movement", order = 1)]
    public class Command_Movement : ConsoleCommandBase
    {
        [SerializeField] float _speed = 10f;

        public override bool Process(DebuggerManager debugManager, string[] arguments)
        {
            if (arguments[0] == "fly")
            {
                PlayerCharacter character = FindObjectOfType<PlayerCharacter>();
                if (character == null)
                    return true;

                GameObject playerObject = character.gameObject;
                if (arguments[1] == "true")
                {
                    Sanity.Stop();
                    playerObject.GetComponent<FirstPersonCharacterController>().SetGravityTo(false);
                    playerObject.GetComponent<Rigidbody>().isKinematic = true;
                    playerObject.GetComponentInChildren<FirstPersonCharacterController>().enabled = false;
                    playerObject.GetComponentInChildren<StateMachineBehaviour>().enabled = false;
                    if (playerObject.GetComponent<Cheat_Movement>() == false)
                    {
                        var movement = playerObject.AddComponent<Cheat_Movement>();
                        movement.SetMovementCheatControls(_speed);
                    }
                }
                else if (arguments[1] == "false")
                {
                    Sanity.Start();
                    playerObject.GetComponent<Rigidbody>().isKinematic = false;
                    playerObject.GetComponentInChildren<FirstPersonCharacterController>().enabled = true;
                    playerObject.GetComponentInChildren<StateMachineBehaviour>().enabled = true;
                    playerObject.GetComponent<FirstPersonCharacterController>().SetGravityTo(true);
                    if (playerObject.GetComponent<Cheat_Movement>())
                    {
                        Destroy(playerObject.GetComponent<Cheat_Movement>());
                    }
                }
            }
            Debugger.Log($"{commandWord} command has been used.");
            return true;
        }
    }
}
