﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkerComponent : MonoBehaviour {
    public float speed;

    private Rigidbody2D rb;

    public Vector2 moveVectorRaw { get; set; }
    public bool canMove { get; set; } = true;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    Vector2 _v;
    private void FixedUpdate () {
        if (!canMove) {
            return;
        }

        moveVectorRaw = Vector2.ClampMagnitude(moveVectorRaw, 1);
        if (rb.velocity.x < speed)
        {
            _v = moveVectorRaw * speed;

            rb.AddForce(_v * rb.mass, ForceMode2D.Force);
        }
	}
}
