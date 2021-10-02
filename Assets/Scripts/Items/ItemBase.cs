using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemBase : MonoBehaviour {
    public Action onClickDown = delegate { };
    public Action onClickUp = delegate { };

    public Action onSelect = delegate { };
    public Action onUnselect = delegate { };

    private SpriteRenderer _renderer;
    public Collider2D pickUpTrigger;

    // Start is called before the first frame update
    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public Sprite GetSprite => _renderer.sprite;

    public void ClickDown () {
        onClickDown();
    }
    public void ClickUp () {
        onClickUp();
    }

    public void Select () {
        onSelect();
    }
    public void Unselect () {
        onUnselect();
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        //Debug.Log("collision");
        if (collision.TryGetComponent(out Inventory inv)) {
            //Debug.Log("collision with inventory");
            if (inv.AddItem(this)) {
                pickUpTrigger.enabled = false;
            }
        }
    }

    public void handleDrop () {
        pickUpTrigger.enabled = true;
    }
}
