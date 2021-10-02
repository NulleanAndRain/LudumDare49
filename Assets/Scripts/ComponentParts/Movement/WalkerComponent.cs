using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkerComponent : MonoBehaviour {
    public float Speed;
    public float MaxJumpSpeed;
    public float JumpForce;
    public float JumpUpTime;

    private Rigidbody2D rb;
    private Vector2 _v;
    private bool _isJumping;
    private float _jumpStartTime;

    public Vector2 MoveVectorRaw { get; set; }
    public bool CanMove { get; set; } = true;

    [Header("Ground check")]
    public GroundChecker Checker;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {
        if (!CanMove) return;

        var velHorisontal = Mathf.Clamp(MoveVectorRaw.x, -1, 1);
        if (rb.velocity.x < Speed)
        {
            var speedLerp = Mathf.Clamp01(Mathf.InverseLerp(Speed, 0, Mathf.Abs(rb.velocity.x)));
            _v = Vector2.right * velHorisontal * Speed * speedLerp * speedLerp;

            rb.AddForce(_v * rb.mass, ForceMode2D.Force);
        }

        if (_isJumping)
        {
            var speedLerp = Mathf.Clamp01(Mathf.InverseLerp(MaxJumpSpeed, 0, rb.velocity.y));
            if (Time.time >= _jumpStartTime + JumpUpTime)
            {
                _isJumping = false;
            }
            _v = Vector2.up * JumpForce * speedLerp * speedLerp;

            rb.AddForce(_v * rb.mass, ForceMode2D.Force);
        }
    }

    public void Jump()
    {
        if (!_isJumping && Checker.IsOnGround)
        {
            _isJumping = true;
            _jumpStartTime = Time.time;
        }
    }

    public void StopJump()
    {
        _isJumping = false;
    }

    public bool IsOnGround => Checker.IsOnGround;
}
