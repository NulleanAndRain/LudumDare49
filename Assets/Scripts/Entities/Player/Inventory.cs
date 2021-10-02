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

    [SerializeField] ItemBase _leftHand = null;
    [SerializeField] ItemBase[] _inventory = null;
    int invSlots => _inventory.Length;
    int currCell = 0;

    [Header("Controls")]

    public bool activeMouseWheel = true;
    public bool activeNumbersCell = true;
    public bool isCellClickable = true;

    [Header("Other")]
    private PlayerControl pc;
    public float dropDistance;

    void Start()
    {
        pc = GetComponent<PlayerControl>();

        void cellOnClick(int n)
        {
            if (isCellClickable)
                setActiveRightCell(n);
        }
        ui.onCellClick += cellOnClick;

        RightHand.DetachChildren();
        LeftHand.DetachChildren();

        void itemDrop(int n)
        {
            ItemBase item;
            if (n == 5)
            {
                item = _leftHand;
                _leftHand = null;
            }
            else
            {
                item = _inventory[n];
                _inventory[n] = null;
            }
            item.transform.parent = null;

            Vector2 newPos = Quaternion.Euler(0, 0, pc.animNum * 45) * Vector2.down;

            item.transform.position = (Vector2)center.position + newPos;
            item.handleDrop();

            updateInv();
        }
        ui.onItemDrop += itemDrop;

        void itemSwap(int i1, int i2)
        { // 5 - слот левой руки
            if (i1 == 5 || i2 == 5)
            {
                if (i1 == 5)
                {
                    i1 = i2;
                    i2 = 5;
                }
                var temp = _leftHand;
                _leftHand = _inventory[i1];
                _inventory[i1] = temp;

                moveItemToNewParent(_inventory[i1], RightHand);
                moveItemToNewParent(_leftHand, LeftHand);
            }
            else
            {
                var temp = _inventory[i2];
                _inventory[i2] = _inventory[i1];
                _inventory[i1] = temp;
            }

            updateInv();
        }
        ui.onItemSwap += itemSwap;

        updateInv();
    }

    void Update()
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
                var cell = currCell; // если менять напрямую currCell, выбор предметов ломается
                if (mw < -0.1)
                {
                    cell++;
                    if (cell > invSlots - 1)
                        cell = 0;
                }
                if (mw > 0.1)
                {
                    cell--;
                    if (cell < 0)
                        cell = invSlots - 1;
                }
                setActiveRightCell(cell);
            }
        }
        // Numbers 
        if (activeNumbersCell)
        {
            if (Input.GetKey(KeyCode.Alpha1))
                setActiveRightCell(0);
            if (Input.GetKey(KeyCode.Alpha2))
                setActiveRightCell(1);
            if (Input.GetKey(KeyCode.Alpha3))
                setActiveRightCell(2);
            if (Input.GetKey(KeyCode.Alpha4))
                setActiveRightCell(3);
        }
    }

    void setActiveRightCell(int n)
    {
        var oldItem = _inventory[currCell];
        if (oldItem != null)
        {
            oldItem.Unselect();
            moveItemToNewParent(oldItem, InventoryFolder);
        }
        currCell = n;
        var newItem = _inventory[currCell];
        if (newItem != null)
        {
            newItem.Select();
            moveItemToNewParent(newItem, RightHand);
        }
        ui.setActiveRightCell(currCell);
    }

    void updateInv()
    {
        updateRightHandStots();
        updateLeftHandStot();
        setActiveRightCell(currCell);
    }
    void updateRightHandStots()
    {
        for (int i = 0; i < invSlots; i++)
        {
            ui.updateInventorySprite(i, _inventory[i] != null ? _inventory[i].GetSprite : null);
        }
    }
    void updateLeftHandStot()
    {
        //ui.updateLeftHandSprite(_leftHand != null ? _leftHand.GetSprite : null);
    }

    void inputItemControl()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        { // left click down
            if (_inventory[currCell] != null)
            {
                _inventory[currCell].ClickDown();
            }
            else if (_leftHand != null)
            {
                _leftHand.ClickDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        { // right click down
            if (_leftHand != null)
            {
                _leftHand.ClickDown();
            }
            else if (_inventory[currCell] != null)
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
            else if (_leftHand != null)
            {
                _leftHand.ClickDown();
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        { // right click up
            if (_leftHand != null)
            {
                _leftHand.ClickDown();
            }
            else if (_inventory[currCell] != null)
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
        while (_inventory[i] != null && i < invSlots)
        {
            i++;
        }
        if (i < invSlots)
        {
            _inventory[i] = item;
            moveItemToNewParent(item, InventoryFolder);
            updateInv();
            return true;
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
