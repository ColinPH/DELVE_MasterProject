using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class MAControlUIElements : LocalManagerAction
    {
        [Header("Set crosshair and sanitybar to")]
        public bool newState = true;
        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            //HUD.crosshair.gameObject.SetActive(newState);
            //HUD.sanityBar.gameObject.SetActive(newState);
        }
    }
}
