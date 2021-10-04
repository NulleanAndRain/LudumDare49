using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Global objects")]
    public GameUI ui;
    public Transform center;

    [Header("Inventory Objects Folders")]
    public Transform InventoryFolder;
    public Transform RightHand;
    //public Transform LeftHand;

    [Header("Inventory Slots")]
    [SerializeField] List<ItemBase> _inventory = null;
    private int invSlots;
    private int _cell = 0;
    public int currCell
    {
        get => _cell;
        set
        {
            _cell = value;
            if (_cell > invSlots - 1)
                _cell = 0;
            if (_cell < 0)
                _cell = invSlots - 1;

            updateInv();
        }
    }

    [Header("Controls")]
    public bool activeMouseWheel = true;
    public bool activeNumbersCell = true;
    public bool isCellClickable = true;

    [Header("Other")]
    public float dropDistance;
    public float DropVertOffset;

    public Item GetItem(int cell) => _inventory[cell]?.Item;
    public event Action OnInventoryChange = delegate { };

    private void Start()
    {
        invSlots = _inventory.Count;

        void cellOnClick(int n)
        {
            if (isCellClickable)
                currCell = n;
        }
        ui.onCellClick += cellOnClick;

        RightHand.DetachChildren();

        void itemDrop(int n)
        {
            ItemBase item = _inventory[n];
            _inventory[n] = null;

            item.transform.parent = null;

            // todo: item droping
            var dir = Vector2.right * transform.localPosition.x * dropDistance;

            // todo: ignore player at raycast
            var hit = Physics2D.Raycast(transform.position, dir, dropDistance);
            if (hit.collider != null)
            {
                var p = hit.point + Vector2.up * DropVertOffset;
                item.transform.position = p;
            }
            else
            {
                item.transform.position = (Vector2)center.position + dir + Vector2.up * DropVertOffset;
            }
            OnInventoryChange();
            item.HandleDrop();

            updateInv();
        }
        ui.onItemDrop += itemDrop;

        void itemSwap(int i1, int i2)
        {
            var temp = _inventory[i2];
            _inventory[i2] = _inventory[i1];
            _inventory[i1] = temp;

            updateInv();
            OnInventoryChange();
        }
        ui.onItemSwap += itemSwap;

        updateInv();
    }

    void updateInv()
    {
        for (int i = 0; i < invSlots; i++)
        {
            var item = _inventory[i];
            if (item != null)
            {
                ui.UpdateInventorySprite(i, item.Sprite, item.Item.CurrentCount, item.DisplayData, item.SpriteMaterial);
            }
            else
            {
                ui.UpdateInventorySprite(i, null);
            }

            if (item != null)
            {
                if (i != currCell)
                {
                    item.Unselect();
                    moveItemToNewParent(item, InventoryFolder);
                } else
                {
                    item.Select();
                    moveItemToNewParent(item, RightHand);
                }
            }
        }
    
        ui.setActiveCell(currCell);
    }

    public void MainClickDown() => _inventory[currCell]?.ClickDownMain();
    public void MainClickUp() => _inventory[currCell]?.ClickUpMain();
    public void SecondaryClickDown() => _inventory[currCell]?.ClickDownSecondary();
    public void SecondaryClickUp() => _inventory[currCell]?.ClickDownSecondary();

    /// <summary>
    /// Пробует добавить предмет в инвентарь
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true - предмет добавлен в инвентарь, false - предмет не добавлен</returns>
    public bool AddItem(ItemBase item)
    {
        //if (_inventory.Contains(item)) return true;
        int i = 0;
        while (i < invSlots &&
            _inventory[i] != null)
        {
            if (_inventory[i].Item.TryAddItems(item.Item))
            {
                updateInv();
                return true;
            }
            i++;
        }

        if (i < invSlots)
        {
            if (_inventory[i] == null)
            {
                _inventory[i] = item;
                moveItemToNewParent(item, InventoryFolder);
                updateInv();
                item.Item.OnItemAmountChange += updateInv;
                
                void remove()
                {
                    item.Item.OnItemAmountChange += updateInv;
                    _inventory[i] = null;
                    updateInv();
                    OnInventoryChange();
                }
                item.Item.OnItemEnded += remove;
                OnInventoryChange();
                return true;
            }
        }
        return false;
    }

    void moveItemToNewParent(ItemBase item, Transform newParent)
    {
        if (item == null)
            return;
        item.transform.SetParent(newParent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }
}
