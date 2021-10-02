using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameUI ui;
    public Transform center;

    [Header("Inventory Objects Folders")]
    public Transform InventoryFolder;
    public Transform RightHand;
    public Transform LeftHand;

    [Header("Inventory Slots")]

    [SerializeField] ItemBase[] _inventory = null;
    private int invSlots;
    private int _cell = 0;
    private int currCell
    {
        get => _cell; set
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
    private PlayerControl pc;
    public float dropDistance;
    public float DropVertOffset;

    public Item GetItem(int cell) => _inventory[cell].Item;

    private void Start()
    {
        invSlots = _inventory.Length;
        pc = GetComponent<PlayerControl>();

        void cellOnClick(int n)
        {
            if (isCellClickable)
                currCell = n;
        }
        ui.onCellClick += cellOnClick;

        RightHand.DetachChildren();
        LeftHand.DetachChildren();

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
        }
        ui.onItemSwap += itemSwap;

        updateInv();
    }

    private void Update()
    {
        InputCellControll();
        inputItemControl();
    }

    private void initSerializedInv()
    {
        throw new NotImplementedException("tbd later, dont use");
        // todo: finish initialization
        //      for (int i = 0; i < invSlots; i++) {

        //}
    }

    public void InputCellControll()
    {
        // Mouse Wheel
        if (activeMouseWheel)
        {
            float mw = Input.GetAxisRaw("Mouse ScrollWheel");
            if (mw != 0)
            {
                if (mw < -0.1)
                {
                    currCell++;
                }
                if (mw > 0.1)
                {
                    currCell--;
                }
            }
        }
        // Numbers 
        if (activeNumbersCell)
        {
            if (Input.GetKey(KeyCode.Alpha1))
                currCell = 0;
            if (Input.GetKey(KeyCode.Alpha2))
                currCell = 1;
            if (Input.GetKey(KeyCode.Alpha3))
                currCell = 2;
            if (Input.GetKey(KeyCode.Alpha4))
                currCell = 3;
        }
    }

    void updateInv()
    {
        for (int i = 0; i < invSlots; i++)
        {
            var item = _inventory[i];
            if (item != null)
            {
                ui.updateInventorySprite(i, item.GetSprite, item.Item.CurrentCount);
            }
            else
            {
                ui.updateInventorySprite(i, null, 0);
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
        //currCell = _cell; // ???
    }

    void inputItemControl()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        { // left click down
            if (_inventory[currCell] != null)
            {
                _inventory[currCell].ClickDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        { // right click down
            if (_inventory[currCell] != null)
            {
                _inventory[currCell].ClickDown();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        { // left click up
            if (_inventory[currCell] != null)
            {
                _inventory[currCell].ClickDown();
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        { // right click up
            if (_inventory[currCell] != null)
            {
                _inventory[currCell].ClickDown();
            }
        }
    }

    /// <summary>
    /// Пробует добавить предмет в инвентарь
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true - предмет добавлен в инвентарь, false - предмет не добавлен</returns>
    public bool AddItem(ItemBase item)
    {
        int i = 0;
        while (_inventory[i] != null &&
            i < invSlots)
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
