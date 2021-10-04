using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    public AnimTrigger Trigger = null;
    private int _layer = 0;

    public void TriggerAnimation(AnimationTrigger trigger)
    {
        if (trigger == AnimationTrigger.None) return;
        _animator.SetTrigger(trigger.ToString());
    }

    public float CurrentAnimLength => _animator.GetCurrentAnimatorStateInfo(_layer).length;

}
public enum AnimationTrigger
{
    None,
    Reset,
    Downed
}
