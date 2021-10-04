using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Item))]
public class ItemBase : MonoBehaviour
{
    public event Action onClickDownMain = delegate { };
    public event Action onClickUpMain = delegate { };

    public event Action onClickDownSecondary = delegate { };
    public event Action onClickUpSecondary = delegate { };

    public event Action onSelect = delegate { };
    public event Action onUnselect = delegate { };

    public bool IsSelected { get; private set; }

    public Item Item { get; private set; }
    public Collider2D pickUpTrigger;
    public GameObject WorldspacePart;
    public GameObject ActiveItemPart;

    private SpriteRenderer _renderer;
    private Rigidbody2D rb;

    public Inventory PlayerInventory { get; private set; }
    public AnimationControl PlayerAnim { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Item = GetComponent<Item>();
    }

    public Sprite GetSprite => _renderer.sprite;

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
        ActiveItemPart?.SetActive(true);
        IsSelected = true;
        onSelect();
    }
    public void Unselect()
    {
        if (!IsSelected) return;
        ActiveItemPart?.SetActive(false);
        IsSelected = false;
        onUnselect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Inventory inv))
        {
            if (inv.AddItem(this))
            {
                PlayerInventory = inv;
                PlayerAnim = inv.GetComponent<AnimationControl>();
                DisableWorldspaceItem();
            }
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
