using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    public AnimTrigger Trigger = null;

    public void TriggerAnimation(AnimationTrigger trigger)
    {
        if (trigger == AnimationTrigger.None) return;

        _animator.SetTrigger(trigger.ToString());
    }

    public float CurrentAnimLength(AnimationLayer layer) => _animator.GetCurrentAnimatorStateInfo((int)layer).length;

}

public enum AnimationTrigger
{
    None,
    Reset,
    Downed,
    Attack
}

public enum AnimationLayer
{
    Base = 0,
    Hands = 1
}