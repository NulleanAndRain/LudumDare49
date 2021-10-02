using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour
{
    public bool IsOnGround { get; private set; }
    public event Action OnGrounded = delegate { };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Untagged")
        {
            IsOnGround = true;
            OnGrounded();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Untagged")
        {
            IsOnGround = false;
        }
    }
}
