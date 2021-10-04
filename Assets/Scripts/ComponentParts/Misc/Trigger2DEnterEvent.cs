using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger2DEnterEvent : MonoBehaviour
{
    public event Action<Collider2D> onTriggerEnter = delegate { };
    private void OnTriggerEnter2D(Collider2D collision) => onTriggerEnter(collision);
}
