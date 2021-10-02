using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkerComponent : MonoBehaviour {
    public float speed;
    public float jumpForce;

    private Rigidbody2D rb;
    private Vector2 _v;

    public Vector2 moveVectorRaw { get; set; }
    public bool canMove { get; set; } = true;

    [Header("Ground check")]
    public GroundChecker Checker;


    // onground check overlap areas
    private Vector2 _overlap1 = Vector2.zero;
    private Vector2 _overlap2 = Vector2.zero;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {
        if (!canMove) return;

        moveVectorRaw = Vector2.ClampMagnitude(moveVectorRaw, 1);
        if (rb.velocity.x < speed)
        {
            var speedLerp = Mathf.Clamp01(Mathf.InverseLerp(speed, 0, Mathf.Abs(rb.velocity.x)));
            _v = moveVectorRaw * speed * speedLerp * speedLerp;

            rb.AddForce(_v * rb.mass, ForceMode2D.Force);
        }
	}

    public void Jump()
    {
        if (IsOnGround)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public bool IsOnGround => Checker.IsOnGround;
}
