using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Player Abilities/Jump Ability", order = 1)]
    public class JumpAbility : PlayerAbilityBase
    {
        FirstPersonCharacterController _fpController;
        PlayerCharacter _player;
        public override AbilityType AbilityType => AbilityType.Jump;

        protected override void m_ActionInitialized()
        {
            base.m_ActionInitialized();

            _fpController = m_casterGameObject.GetComponent<FirstPersonCharacterController>();
            _player = m_casterGameObject.GetComponent<PlayerCharacter>();
        }

        protected override void m_ActionPerformed(ControlledInputContext context)
        {
            base.m_ActionPerformed(context);
            //Debug.Log("Jump");
            if (_player.stateMachineBehaviour.TransitionToNewState<JumpState>() == false)
            {
                Debug.Log("Reached max amounts of jumps.");
            }
        }
    }
}