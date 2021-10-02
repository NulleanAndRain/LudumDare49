using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDropHandler {
    [SerializeField] Canvas canvas = null;
    [SerializeField] GameUI ui = null;
    public int CellNum;
    public bool selectableSlot;

    public GameObject itemHolderPrefab;

	private void Start () {
    }

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
}
