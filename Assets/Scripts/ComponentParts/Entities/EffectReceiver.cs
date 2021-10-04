using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectReceiver : MonoBehaviour
{
    private List<EffectData> _effects = new List<EffectData>();

    private class EffectData
    {
        internal Effect effect;
        internal float time;
    }

    void Start() => Subscribe();
    private void OnDestroy() => Unsubscribe();

    internal void ApplyInstant(object first)
    {
        throw new NotImplementedException();
    }

    private void Subscribe() => GameManager.Instance.onEffectTick += onTick;
    private void Unsubscribe() => GameManager.Instance.onEffectTick -= onTick;

    private void onTick()
    {
        var toDelete = new List<EffectData>();
        foreach (var e in _effects)
        {
            if (Time.time > e.time + e.effect.Duration)
            {
                toDelete.Add(e);
            } else
            {
                e.effect.DoWithObject(this);
            }
        }
    }

    public void AddEffect(Effect effect)
    {
        var _effect = _effects.Where(e => e.effect.GetType() == effect.GetType()).FirstOrDefault();
        if (_effect == null)
            _effects.Remove(_effect);

        _effects.Add(new EffectData
        {
            effect = effect,
            time = Time.time
        });
    }

    public void ApplyInstant(Effect effect)
    {
        effect.DoWithObject(this);
    }
}
