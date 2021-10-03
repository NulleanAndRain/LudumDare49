using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class DragableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	[SerializeField] Canvas canvas = null;
	[SerializeField] GameUI ui = null;
	ItemSlot parent;

	private RectTransform rect;
	private CanvasGroup cg;

	int slotTarget = -1;
	int slotNum;
	public void SetSlotTarget (int n) {
		slotTarget = n;
	}
	public int oldSlot => slotNum;

	private void Start () {
		rect = GetComponent<RectTransform>();
		cg = GetComponent<CanvasGroup>();
		parent = transform.parent.GetComponent<ItemSlot>();
		slotNum = parent.CellNum;
	}

	public void init(Canvas c, GameUI g) {
		canvas = c;
		ui = g;
	}

	public void OnPointerDown (PointerEventData eventData) {
		parent.OnPointerDown(eventData);
	}
	public void OnDrag (PointerEventData eventData) {
		rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnBeginDrag (PointerEventData eventData) {
		transform.SetParent(ui.transform, true);
		parent.createItemHolder();
		cg.blocksRaycasts = false;
		cg.alpha = .8f;
	}

	public void OnEndDrag (PointerEventData eventData) {
		if (slotTarget == -1)
			ui.InitItemDrop(slotNum);
		else
			ui.InitItemSwap(slotNum, slotTarget);
		Destroy(gameObject);
	}
}
