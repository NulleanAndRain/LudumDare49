using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDefault : AbstractSword
{
    private int state = 0;

    protected override void UseMain()
    {
        if (state != 0) return;
        state++;
        var anim = _base.PlayerAnim;
        _projectile.owner = _playerInventory.gameObject;

        void Act()
        {
            if (state == 1)
            {
                EnableActivePart();
                state = 2;
            }
            else
            {
                DisableActivePart();
                _projectile.clearDamagedList();
                anim.Trigger.onAnimTrigger -= Act;
                state = 0;
            }
        }

        anim.Trigger.onAnimTrigger += Act;
        anim.TriggerAnimation(AnimationTrigger.Attack);
    }
}
