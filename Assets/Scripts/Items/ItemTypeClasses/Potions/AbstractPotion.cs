using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] protected List<WeightedEffect> _effects;

    public Effect RandomEffect
    {
        get
        {
            var count = _effects.Count;
            if (count == 1) return _effects[0].effect;

            var total = _effects.Sum(e => e.weight);
            var rnd = Random.Range(0, total);

            for (int i = 0; i < count; i++)
            {
                var _e = _effects[i];
                rnd -= _e.weight;
                if (rnd <= 0) return _e.effect;
            }
            return _effects.LastOrDefault()?.effect;
        }
    }

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