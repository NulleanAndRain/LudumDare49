using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item
{
    int MaxStackSize { get; }
    int CurrentCount { get; set; }
}
