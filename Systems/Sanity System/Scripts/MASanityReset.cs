using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Manager Actions/" + nameof(MASanityReset))]
    public class MASanityReset : LocalManagerAction
    {
        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            base.InvokeLocalManagerAction(localManager);
            Managers.sanityManager.RestoreSanity(1f);
        }
    }
}
