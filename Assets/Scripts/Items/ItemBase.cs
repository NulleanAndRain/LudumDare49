using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemBase : MonoBehaviour
{
    public event Action onClickDownMain = delegate { };
    public event Action onClickUpMain = delegate { };

    public event Action onClickDownSecondary = delegate { };
    public event Action onClickUpSecondary = delegate { };

    public event Action onSelect = delegate { };
    public event Action onUnselect = delegate { };

    public bool IsInInventory => PlayerInventory != null;
    public bool IsSelected { get; private set; }

    public Item Item { get; private set; }
    [Header("Components")]
    public Collider2D pickUpTrigger;
    [SerializeField] private SpriteRenderer _renderer;
    private Rigidbody2D rb;

    [Header("Objects")]
    public GameObject WorldspacePart;
    public GameObject ActiveItemPart;
    public Transform DisplayData;

    public Inventory PlayerInventory { get; private set; }
    public AnimationControl PlayerAnim { get; private set; }


    //[Header("InitState")]
    //[SerializeField] private bool _isInWorldOnInit = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Item = GetComponent<Item>();
        //ActiveItemPart.SetActive(!_isInWorldOnInit);
        //WorldspacePart.SetActive(_isInWorldOnInit);
        ActiveItemPart.SetActive(false);
        WorldspacePart.SetActive(true);
    }

    public Sprite Sprite => _renderer.sprite;
    public Material SpriteMaterial => _renderer.material;

    private Action _addAction;

    public void ClickDownMain()
    {
        onClickDownMain();
    }
    public void ClickUpMain()
    {
        onClickUpMain();
    }

    public void ClickDownSecondary()
    {
        onClickDownSecondary();
    }
    public void ClickUpSecondary()
    {
        onClickUpSecondary();
    }

    public void Select()
    {
        if (IsSelected) return;
        //ActiveItemPart?.SetActive(true);
        IsSelected = true;
        onSelect();
    }
    public void Unselect()
    {
        if (!IsSelected) return;
        //ActiveItemPart?.SetActive(false);
        IsSelected = false;
        onUnselect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInInventory) return;
        if (collision.TryGetComponent(out Inventory inv))
        {
            bool tryAddToInv()
            {
                if (inv.AddItem(this))
                {
                    PlayerInventory = inv;
                    PlayerAnim = inv.GetComponent<AnimationControl>();
                    DisableWorldspaceItem();
                    inv.OnInventoryChange -= _addAction;
                    _addAction = null;
                    return true;
                }
                return false;
            }

            _addAction = () => tryAddToInv();

            if (!tryAddToInv())
                inv.OnInventoryChange += _addAction;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_addAction != null && collision.TryGetComponent(out Inventory inv))
        {
            inv.OnInventoryChange -= _addAction;
        }
    }

    public void HandleDrop()
    {
        PlayerInventory = null;
        EnableWorldspaceItem();
    }

    private void DisableWorldspaceItem()
    {
        rb.isKinematic = true;
        WorldspacePart.SetActive(false);
        pickUpTrigger.enabled = false;
    }

    private void EnableWorldspaceItem()
    {
        transform.localScale = Vector3.one;
        rb.isKinematic = false;
        WorldspacePart.SetActive(true);
        pickUpTrigger.enabled = true;
    }
}
