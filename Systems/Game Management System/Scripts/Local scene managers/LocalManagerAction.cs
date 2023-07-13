using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class LocalManagerAction : WorldObject
    {
        protected LocalManager m_localManager { get; set; }
        public virtual void InvokeLocalManagerAction(LocalManager localManager)
        {
            m_localManager = localManager;
        }
    }
}
