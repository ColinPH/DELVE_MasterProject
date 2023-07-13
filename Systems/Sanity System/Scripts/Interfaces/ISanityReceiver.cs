using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface ISanityReceiver
    {
        public void IApplySanityEffect(GameObject afflictor);
        public void IRemoveSanityEffect(GameObject afflictor);
    }
}