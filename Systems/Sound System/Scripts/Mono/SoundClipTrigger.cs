using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClipTrigger : WorldObject
{
    [SerializeField] SoundClip _soundClipToPlay;
    bool _hasBeenTriggered = false;

    #region WorldObject overrides
    public override string worldName => nameof(SoundClipTrigger);
    protected override void MyAwake()
    {
        base.MyAwake();
        m_EnsureGameObjectHasTrigger();

        //Read from the safe state if there is one
    }
    #endregion WorldObject overrides

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        if (_hasBeenTriggered) return;

        Sound.PlaySound(_soundClipToPlay, gameObject);
        _hasBeenTriggered = true;
    }

}
