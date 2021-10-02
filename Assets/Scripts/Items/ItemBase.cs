using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
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

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
        //Debug.Log("collision");
        if (collision.TryGetComponent(out Inventory inv))
        {
            //Debug.Log("collision with inventory");
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
