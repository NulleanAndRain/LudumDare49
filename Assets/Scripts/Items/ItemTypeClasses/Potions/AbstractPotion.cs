using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPotion : Item
{
    private float _lastUse = -20f;
    private float _useCD = 0.1f;
    // Effect?

    [Serializable]
    public class WeightedEffect
    {
        public Effect effect;
        public float weight;
    } 

    [Header("Potion default params")]
    [SerializeField] protected List<WeightedEffect> Effects;


    private void Start()
    {
        _base.onClickDownMain += _UseMain;
    }

    private void _UseMain()
    {
        if (Time.time < _lastUse + _useCD) return;
        _lastUse = Time.time;
        UseMain();
    }

    virtual protected void UseMain() { }
}