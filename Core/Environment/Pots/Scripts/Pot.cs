using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Props/" + nameof(Pot))]
    public class Pot : WorldObject
    {
        [Header("Behaviour")]
        [SerializeField] bool _dropItemsWhenBroken = true;
        [SerializeField] List<GameObject> _objectsToDrop = new List<GameObject>();
        [Header("SFX & VFX")]
        [SerializeField] SoundClip _breakSound;
        [SerializeField] VisualEffectClip _breakVFX;


        private Breakable _breakable;
        private ItemDropper _itemDropper;

        protected override void MyStart()
        {
            base.MyStart();

            _breakable = m_FetchForComponent<Breakable>();
            _itemDropper = m_FetchForComponent<ItemDropper>();
            _breakable.OnObjectBreak += _OnObjectBreak;
        }

        private void _OnObjectBreak()
        {            
            //Play break sound and spawn VFX
            Sound.PlaySound(_breakSound, gameObject);
            VisualEffects.SpawnVFX(_breakVFX, transform);
            _itemDropper.DropItems(_objectsToDrop);
            Metrics.levelData.interaction_PotsDestroyed += 1;
        }


    }
}
