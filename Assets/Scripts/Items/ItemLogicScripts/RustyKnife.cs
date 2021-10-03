using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustyKnife : AbstractSword
{
    protected override void UseMain()
    {
        IEnumerator _anim()
        {
            var animator = _base.PlayerInventory.GetComponent<AnimationControl>();
            animator.TriggerAnimation(AnimationTrigger.Downed);
            yield return new WaitForEndOfFrame();
            _anim_length = animator.CurrentAnimLength;
            yield return new WaitForSeconds(_anim_length);
            animator.TriggerAnimation(AnimationTrigger.Reset);
        }

        StartCoroutine(_anim());
    }
}
