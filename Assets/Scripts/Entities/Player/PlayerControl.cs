using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WalkerComponent), typeof(Health))]
public class PlayerControl : MonoBehaviour
{

    public GameUI gameUI = null; //added

    private Vector2 _vel;

    private Animator animator;
    private Health health;
    private WalkerComponent walker;

    public Transform viewPos;
    public int animNum { get; private set; }

    private Vector3 _localScale = Vector3.one;

    public bool isFacingToCursor;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        walker = GetComponent<WalkerComponent>();

        void updateHpBar(float currHP, float maxHP)
        {
            gameUI.updateHpBar(currHP / maxHP);
        }

        health.onHealthUpdate += updateHpBar;

        void revive()
        {
            _vel.x = 0;
            _vel.y = 0;
            walker.MoveVectorRaw = _vel;
            animator.SetBool("isWalking", false);
            animator.SetBool("isDowned", true);
            if (animNum < 4)
            {
                _localScale.x *= -1;
                transform.localScale = _localScale;
            }
            StartCoroutine(waitForRevive());
        }

        IEnumerator waitForRevive()
        {
            yield return new WaitForSeconds(GameManager.RespawnTime);
            animator.SetBool("isDowned", false);
            health.Revive();
            health.Heal(health.MaxHealth);
            _localScale = Vector3.one;
            transform.localScale = _localScale;
        }

        health.onDowned += revive;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseControl.TogglePause();
        }

        if (health.isDowned || PauseControl.isPaused) return;
        _vel.x = Input.GetAxis("Horizontal");
        _vel.y = Input.GetAxis("Vertical");

        float _magn = _vel.magnitude;

        animator.SetBool("isWalking", _magn > 1e-3);

        //animNum = Mathf.FloorToInt((viewAngle + 22.5f) / 45) % 8;
        if (_vel.x > 1e-3)
            animNum = 2;
        else if (_vel.x < -1e-3)
            animNum = 6;
        //else
        //    animNum = 0;

        animator.SetInteger("currDir", animNum);

        walker.MoveVectorRaw = _vel;

        if (Input.GetButtonDown("Jump"))
        {
            // todo: add animation
            walker.Jump();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            walker.StopJump();
        }
    }

}
