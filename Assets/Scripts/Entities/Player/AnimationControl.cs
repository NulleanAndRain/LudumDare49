﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    private int _layer = 0;

    public void TriggerAnimation(AnimationTrigger trigger)
    {
        if (trigger == AnimationTrigger.None) return;
        _animator.SetTrigger(trigger.ToString());
    }

    public float GetCurrentAnimLength() => _animator.GetCurrentAnimatorStateInfo(_layer).length;
}
public enum AnimationTrigger
{
    None,
    Reset,
    Downed
}
