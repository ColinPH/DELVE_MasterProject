using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Manager Actions/" + nameof(MASanityControl))]
    public class MASanityControl : LocalManagerAction
    {
        enum SanityControl { StartSanity = 0, StopSanity = 1, ResetSanity = 3, }

        [SerializeField ] private SanityControl _controlType = SanityControl.StartSanity;

        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            if (_controlType == SanityControl.StartSanity)
                Sanity.Start();
            else if (_controlType == SanityControl.StopSanity)
                Sanity.Stop();
            else if (_controlType == SanityControl.ResetSanity)
                Sanity.manager.RestoreSanity(1f);
        }
    }
}