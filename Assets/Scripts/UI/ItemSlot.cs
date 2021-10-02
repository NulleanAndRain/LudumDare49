using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDropHandler {
    public int CellNum;
    public bool selectableSlot;
    public Image Sprite;

    public int Amount
    {
        set
        {
            if (value <= 1)
                AmountText.text = string.Empty;
            else
                AmountText.text = value.ToString();
        }
    }

    public RectTransform Rect { get; private set; }
    public Item ContainingItem => ui.PlayerInventory.GetItem(CellNum);

    public GameObject ItemHolderPrefab => ui.ItemHolderPrefab;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private GameUI ui = null;
    [SerializeField] private TMPro.TMP_Text AmountText = null;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void OnPointerDown (PointerEventData eventData) {
        if (selectableSlot) ui.SelectCell(CellNum);
    }

    public void createItemHolder () {
        var holder = Instantiate(ItemHolderPrefab, transform);
        holder.SetActive(false);
        holder.GetComponent<DragableItem>().init(canvas, ui);
        Sprite = holder.GetComponent<Image>();
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

    public void SetActiveSprite(bool toggle)
    {
        Sprite.gameObject.SetActive(toggle);
    }

    private void OnMouseOver()
    {
        if (ContainingItem == null) return;

        Debug.Log("hover");

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
