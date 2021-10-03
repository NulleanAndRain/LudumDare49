using System;
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
        get => _currentCount;
        protected set
        {
            if (value > 0)
            {
                var newVal = Mathf.Clamp(value, 0, MaxStackSize);
                if (_currentCount != newVal)
                {
                    _currentCount = newVal;
                    OnItemAmountChange();
                }
            }
            else
            {
                Destroy(gameObject);
                OnItemEnded();
            }
        }
    }

    public event Action OnItemAmountChange = delegate { };
    public event Action OnItemEnded = delegate { };

    protected ItemBase _base;
    protected Inventory _playerInventory => _base.PlayerInventory;

    private void Awake()
    {
        _base = GetComponent<ItemBase>();
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

        var rest = CurrentCount + item.CurrentCount - MaxStackSize;
        CurrentCount += item.CurrentCount;
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
