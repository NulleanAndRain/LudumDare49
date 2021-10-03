using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;

    public void TriggerAnimation(AnimationTrigger trigger)
    {
        if (trigger == AnimationTrigger.None) return;
        _animator.SetTrigger(trigger.ToString());
    }
}
public enum AnimationTrigger
{
    None,
    Reset,
    Downed
}
