using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class CrystalSprout : WorldObject
    {

        Breakable _breakable;

        #region WorldObject overrides

        public override string worldName => nameof(CrystalSprout);

        protected override void MyAwake()
        {
            base.MyAwake();
            _breakable = m_FetchForComponent<Breakable>();
            _breakable.OnObjectBreak += _OnObjectBreak;
        }

        private void _OnObjectBreak()
        {
            
        }

        #endregion WorldObject overrides
    }
}
