using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [SerializeField] protected float _duration;
    public virtual float Duration { get => _duration; }

    public abstract void DoWithObject(EffectReceiver receiver);
}
