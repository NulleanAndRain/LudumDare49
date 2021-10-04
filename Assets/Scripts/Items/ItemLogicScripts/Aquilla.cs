using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquilla : AbstractSword
{
    protected override void UseMain()
    {
        var anim = _base.PlayerAnim;

        int state = 0;
        void Act()
        {
            if (state == 0)
            {
                EnableActivePart();
                state++;
            }
            else
            {
                DisableActivePart();
                anim.Trigger.onAnimTrigger -= Act;
            }
        }

        anim.Trigger.onAnimTrigger += Act;
        anim.TriggerAnimation(AnimationTrigger.Attack);
    }
}
