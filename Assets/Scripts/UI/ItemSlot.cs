using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDropHandler {
    [SerializeField] Canvas canvas = null;
    [SerializeField] GameUI ui = null;
    public int CellNum;
    public bool selectableSlot;
    public Item ContainingItem => ui.PlayerInventory.GetItem(CellNum);

    public GameObject itemHolderPrefab;

    public void OnPointerDown (PointerEventData eventData) {
        if (selectableSlot) ui.SelectCell(CellNum);
    }

    public void createItemHolder () {
        var holder = Instantiate(itemHolderPrefab, transform);
        holder.SetActive(false);
        holder.GetComponent<DragableItem>().init(canvas, ui);
    }

    public void OnDrop (PointerEventData eventData) {
        OnPointerDown(eventData);
        // todo: finish this method
        if (eventData.pointerDrag != null) {
            if (eventData.pointerDrag.TryGetComponent(out DragableItem item)) {
                item.SetSlotTarget(CellNum);
            }
        }
    }

    private void OnMouseOver()
    {
        if (ContainingItem == null) return;

        var obj = ui.ItemInfoObj;
        obj.Header = ContainingItem.ItemName;
        obj.Description = ContainingItem.ItemDescription;

        obj.transform.localPosition = Input.mousePosition;

        obj.ToggleOn();

    }

    private void OnMouseExit()
    {
        ui.ItemInfoObj.ToggleOff();
    }
}
