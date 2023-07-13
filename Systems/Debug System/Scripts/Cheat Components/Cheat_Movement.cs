using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class Cheat_Movement : MonoBehaviour
    {
        Key _upKey = Key.LeftShift;
        Key _downKey = Key.LeftCtrl;
        Key _forwardKey = Key.W;
        Key _backwardsKey = Key.S;
        Key _rightKey = Key.D;
        Key _leftKey = Key.A;
        float _speed;

        public void SetMovementCheatControls(float speed)
        {
            _speed = speed;
        }

        private void Update()
        {
            
            if (Keyboard.current[_upKey].isPressed)
            {
                //Go Up
                transform.Translate(Vector3.up * Time.deltaTime * _speed);

            }

            if (Keyboard.current[_downKey].isPressed)
            {
                //Go down
                transform.Translate(-1f * Vector3.up * Time.deltaTime * _speed);
            }

            if (Keyboard.current[_forwardKey].isPressed)
            {
                //Go forward
                transform.Translate(transform.InverseTransformDirection(transform.forward) * Time.deltaTime * _speed);
            }

            if (Keyboard.current[_backwardsKey].isPressed)
            {
                //Go backward
                transform.Translate(transform.InverseTransformDirection(-1f * transform.forward) * Time.deltaTime * _speed);
            }

            if (Keyboard.current[_rightKey].isPressed)
            {
                //Go right
                transform.Translate(transform.InverseTransformDirection(transform.right) * Time.deltaTime * _speed);
            }

            if (Keyboard.current[_leftKey].isPressed)
            {
                //Go left
                transform.Translate(transform.InverseTransformDirection(-1f * transform.right) * Time.deltaTime * _speed);
            }
        }
    }
}
