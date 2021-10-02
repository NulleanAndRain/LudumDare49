using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustyKnife : Item
{
    public override bool TryAddItems(Item item)
    {
        return false;
    }
}
