using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : WorldObject
{
    SoundClip _collisionSound;
    int _maxAmountCollisions = 5;
    AnimationCurve _soundFiringChance;
    float _curveTime = 2f;
    LayerMask _contactMask;

    private int _amountCollisions = 0;
    float _startTime = 0f;

    public override string worldName => nameof(SoundOnCollision);

    protected override void MonoAwake()
    {
        base.MonoAwake();
        _startTime = Time.time;
    }

    public void InitializeCollisionSound(
        SoundClip collisionSound, 
        int maxAmountCollisions, 
        AnimationCurve soundFiringChance,
        float curveTime,
        LayerMask contactMask)
    {
        _collisionSound = collisionSound;
        _maxAmountCollisions = maxAmountCollisions;
        _soundFiringChance = soundFiringChance;
        _curveTime = curveTime;
        _contactMask = contactMask;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Return if the layer of the object we collide with is not in the contact layer mask
        int layer = collision.collider.gameObject.layer;
        if (((_contactMask.value & (1 << layer)) != 0) == false) return;

        if (_amountCollisions >= _maxAmountCollisions) return;

        float elapsedTime = Time.time - _startTime;
        float sampleIndex = elapsedTime / _curveTime;
        if (elapsedTime >= _curveTime)
            sampleIndex = 1f;

        float spawnChance = _soundFiringChance.Evaluate(sampleIndex);

        if (Random.Range(0f, 1f) <= spawnChance)
        {
            Debug.Log("Collision");
            Sound.PlaySound(_collisionSound, gameObject);
            _amountCollisions += 1;
        }
    }
}
