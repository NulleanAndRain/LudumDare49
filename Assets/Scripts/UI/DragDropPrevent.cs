using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropPrevent : MonoBehaviour, IDropHandler {
	public void OnDrop (PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            if (eventData.pointerDrag.TryGetComponent(out DragableItem item)) {
                item.SetSlotTarget(item.oldSlot);
            }
        }
    }
}
