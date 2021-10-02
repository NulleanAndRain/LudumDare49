using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private int _maxStackSize = 1;
    public int MaxStackSize => _maxStackSize;

    [SerializeField] private int _currentCount = 1;
    public int CurrentCount
    {
        get => _currentCount; protected set
        {
            if (value > 0) _currentCount = value;
            else Destroy(gameObject);
        }
    }

    /// <summary>
    /// Add stack of items to this one
    /// </summary>
    /// <param name="count">Adding stack</param>
    /// <returns>True if items are added and none are left in the stack
    /// False if no items added or more than 0 left in stack
    /// </returns>
    public virtual bool TryAddItems(Item item)
    {
        if (item.GetType() != this.GetType()) return false;

        CurrentCount += item.CurrentCount;
        var rest = CurrentCount - MaxStackSize;
        item.CurrentCount = rest;
        if (rest > 0)
        {
            CurrentCount = MaxStackSize;
            return false;
        } 

        return true;
    }

    [SerializeField] private string _itemName = string.Empty;
    public string ItemName => _itemName;

    [SerializeField] private string _itemDescription = string.Empty;
    public string ItemDescription => _itemDescription;
}
