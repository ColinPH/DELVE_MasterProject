using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Manager Actions/" + nameof(MAControlCursor))]
    public class MAControlCursor : LocalManagerAction
    {
        public bool lockCursor = true;


        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Inputs.mouseIsActive = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Inputs.mouseIsActive = true;
            }
        }
    }
}
