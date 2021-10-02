using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkerComponent : MonoBehaviour {
    public float Speed;
    public float MaxJumpSpeed;
    public float JumpForce;
    public float JumpHorizontalSpeed;
    public float JumpSpeedFactor;
    public float JumpUpTime;

    

    private Rigidbody2D rb;
    //private Vector2 _v;
    private bool _isJumping;
    private float _jumpStartTime;

    public Vector2 MoveVectorRaw { get; private set; }
    public bool CanMove { get; set; } = true;

    [Header("Ground check")]
    public GroundChecker Checker;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {
        if (!CanMove) return;

        var velHorisontal = MoveVectorRaw.x;
        var affectedSpeed = !Checker.IsOnGround ? JumpHorizontalSpeed : Speed;
        if (Mathf.Abs(rb.velocity.x) < affectedSpeed)
        {
            if (!Checker.IsOnGround) velHorisontal *= JumpSpeedFactor;
            
            var speedLerp = Mathf.Clamp01(Mathf.InverseLerp(affectedSpeed, 0, Mathf.Abs(rb.velocity.x)));
            
            var _v = Vector2.right * velHorisontal * affectedSpeed * speedLerp * speedLerp * 0.95f;

            rb.AddForce(_v * rb.mass/40f, ForceMode2D.Impulse);

        }

        if (_isJumping)
        {
            var speedLerp = Mathf.Clamp01(Mathf.InverseLerp(MaxJumpSpeed, 0, rb.velocity.y));
            if (Time.time >= _jumpStartTime + JumpUpTime)
            {
                _isJumping = false;
                return;
            }
            var _v = Vector2.up * JumpForce  * speedLerp*0.95f;

            rb.AddForce(_v * rb.mass/50f, ForceMode2D.Impulse);

            
        }
        if (!Checker.IsOnGround && !_isJumping)
        {
            
            
        }

        if (Checker.IsOnGround)
        {
            _jumpStartTime = Time.time;
        }
    }

    public void UpdateMoveVector(Vector2 direction)
    {
        MoveVectorRaw = direction.normalized;
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
