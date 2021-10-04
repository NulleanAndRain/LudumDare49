using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffectVariant", menuName = "ScriptableObjects/Effects/Heal", order = 0)]
public class HealEffect : Effect
{
    [SerializeField] private float _healAmount;

    public override void DoWithObject(EffectReceiver receiver)
    {
        var health = receiver.GetComponent<Health>();
        if (health == null) return;
        health.Heal(_healAmount);
    }
}
