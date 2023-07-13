using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    /// <summary> This should replace the InputAction.CallbackContext. To give an additional layer of control. </summary>
    public class ControlledInputContext
    {
        InputAction.CallbackContext _context;
        bool _isSimulated;
        Vector2 _axisInput;

        public ControlledInputContext(InputAction.CallbackContext context)
        {
            this._context = context;
            this._isSimulated = false;
        }

        public ControlledInputContext(Vector2 axisInput)
        {
            this._isSimulated = true;
            _axisInput = axisInput;
        }

        public ControlledInputContext()
        {
            this._isSimulated = true;
        }

        public Vector2 GetVector2()
        {
            if (_isSimulated)
                return _axisInput;
            else
                return _context.ReadValue<Vector2>();
        }
    }
}