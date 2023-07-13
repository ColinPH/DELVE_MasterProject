using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Breakables/" + nameof(BreakableEffects))]
    public class BreakableEffects : WorldObject
    {
        [SerializeField] SoundClip _breakSound;
        [SerializeField] VisualEffectClip _breakVFX;

        private Breakable _breakable;

        protected override void MyStart()
        {
            base.MyStart();

            _breakable = m_FetchForComponent<Breakable>();
            _breakable.OnObjectBreak += _OnObjectBreak;
        }

        private void _OnObjectBreak()
        {
            //Play break sound and spawn VFX
            Sound.PlaySound(_breakSound, gameObject);
            VisualEffects.SpawnVFX(_breakVFX, transform);
        }
    }
}
