using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyItem : Item
{
    public override bool TryAddItems(Item item) => false;
}
