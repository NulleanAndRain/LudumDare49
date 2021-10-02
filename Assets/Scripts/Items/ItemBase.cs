using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Item))]
public class ItemBase : MonoBehaviour
{
    public Action onClickDown = delegate { };
    public Action onClickUp = delegate { };

    public Action onSelect = delegate { };
    public Action onUnselect = delegate { };

    private SpriteRenderer _renderer;
    private Rigidbody2D rb;
    public Collider2D pickUpTrigger;
    public GameObject WorldspacePart;
    public GameObject ActiveItemPart;

    public Item Item;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Item = GetComponent<Item>();
    }

    public Sprite GetSprite => _renderer.sprite;

    public void ClickDown()
    {
        onClickDown();
    }
    public void ClickUp()
    {
        onClickUp();
    }

    public void Select()
    {
        ActiveItemPart?.SetActive(true);
        onSelect();
    }
    public void Unselect()
    {
        ActiveItemPart?.SetActive(false);
        onUnselect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Inventory inv))
        {
            if (inv.AddItem(this))
            {
                DisableWorldspaceItem();
            }
        }
    }

    public void HandleDrop()
    {
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
