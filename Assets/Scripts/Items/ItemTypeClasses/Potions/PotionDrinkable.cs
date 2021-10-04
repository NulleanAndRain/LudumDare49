using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionDrinkable : AbstractPotion
{
    protected override void UseMain()
    {
        var anim = _base.PlayerAnim;
        anim.TriggerAnimation(AnimationTrigger.Attack);
        anim.Trigger.onAnimTrigger += act;
        void act() {
            var er = anim.GetComponent<EffectReceiver>();
            er?.ApplyInstant(_effects.FirstOrDefault().effect);
            CurrentCount--;
            anim.Trigger.onAnimTrigger -= act;
        };
    }


}
