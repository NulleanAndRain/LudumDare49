using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDefault : AbstractSword
{
    protected override void UseMain()
    {
        var anim = _base.PlayerAnim;
        _projectile.owner = _playerInventory.gameObject;

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
                _projectile.clearDamagedList();
                anim.Trigger.onAnimTrigger -= Act;
            }
        }

        anim.Trigger.onAnimTrigger += Act;
        anim.TriggerAnimation(AnimationTrigger.Attack);
    }
}
