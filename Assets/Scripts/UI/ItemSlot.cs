using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
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

    private bool _mouseIsOver = false;
    private Camera _camera;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        _camera = ui.CameraUI;
    }

    private void Update()
    {
        var info = ui.ItemInfoObj;
        if (!_mouseIsOver || ContainingItem == null)
        {
            info.ToggleOff();
            return;
        }

        info.Header = ContainingItem.ItemName;
        info.Description = ContainingItem.ItemDescription;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, Input.mousePosition, _camera, out var pos);
        //info.transform.position = _camera.ViewportToScreenPoint(Input.mousePosition);
        //info.transform.localPosition = pos;

        //info.transform.localPosition = Input.mousePosition;
        var pos = _camera.ScreenToWorldPoint(_pd.position);
        pos.z = info.transform.position.z;

        info.transform.position = pos;

        info.ToggleOn();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectableSlot) ui.SelectCell(CellNum);
    }

    public void createItemHolder()
    {
        var holder = Instantiate(ItemHolderPrefab, transform);
        holder.SetActive(false);
        holder.GetComponent<DragableItem>().init(canvas, ui);
        Sprite = holder.GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnPointerDown(eventData);
        // todo: finish this method
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.TryGetComponent(out DragableItem item))
            {
                item.SetSlotTarget(CellNum);
            }
        }
    }

    public void SetActiveSprite(bool toggle)
    {
        Sprite.gameObject.SetActive(toggle);
    }

    PointerEventData _pd;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseIsOver = true;
        _pd = eventData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseIsOver = false;
        _pd = null;
    }
}
