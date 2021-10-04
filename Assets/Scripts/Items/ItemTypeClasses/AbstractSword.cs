using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSword : Item
{
    [Header("Sword params")]
    public float Damage;

    protected float _lastUse;
    protected float _anim_length = 0;

    public sealed override bool TryAddItems(Item item) => false;

    private void Start()
    {
        _base.onClickDownMain += _UseMain;
        _base.onClickDownSecondary += UseSecondary;
        _lastUse = -20;
    }

    private void _UseMain()
    {
        if (Time.time < _lastUse + _anim_length) return;
        _lastUse = Time.time;
        UseMain();

    }

    virtual protected void UseMain()
    {
        // trigger animation

        // enable active part
        // wait end of animation
        // disable active part
    }

    virtual protected void UseSecondary() { }

}
