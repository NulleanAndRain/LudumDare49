using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSword : Item
{
    [Header("Sword params")]
    public float Damage;
    public float CritRate;
    public float CritDmgMultiplyer;
    public float KnockbackForce;

    protected float _lastUse;
    protected float _anim_length = 0;

    [Header("Components")]
    [SerializeField] private Trigger2DEnterEvent _trigger = null;
    [SerializeField] protected DamagingProjectileComponent _projectile;

    public float DmgCalculated => Damage + Random.value < CritRate ? Damage * CritDmgMultiplyer : 0;

    public sealed override bool TryAddItems(Item item) => false;

    private void Start()
    {
        _base.onClickDownMain += _UseMain;
        _base.onClickDownSecondary += UseSecondary;
        _lastUse = -20;

        _trigger.onTriggerEnter += c => 
            _projectile.damageTarget(
                c.gameObject,
                DmgCalculated,
                Vector2.zero,
                KnockbackForce
            );
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
