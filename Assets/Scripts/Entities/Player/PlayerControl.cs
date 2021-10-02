﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WalkerComponent), typeof(Health))]
public class PlayerControl : MonoBehaviour {

    [System.NonSerialized] GameUI gameUI = null; //added

    private Vector2 _vel;

    private Animator animator;
    private Health health;
    private WalkerComponent walker;

    public Transform viewPos;

    Vector3 _ls = Vector3.one;

    public bool isFacingToCursor;
    void Start() {
        gameUI = GameObject.Find("UI_System").GetComponent<GameUI>(); //added
        //лучше сделать через синглтон GameUI, но поебать

        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        walker = GetComponent<WalkerComponent>();

        void updateHpBar (float currHP, float maxHP) {
            gameUI.updateHpBar(currHP/maxHP);
        }

        health.onHealthUpdate += updateHpBar;

        void revive () {
            _vel.x = 0;
            _vel.y = 0;
            walker.moveVectorRaw = _vel;
            animator.SetBool("isWalking", false);
            animator.SetBool("isDowned", true);
            if (animNum < 4) {
                _ls.x *= -1;
                transform.localScale = _ls;
            }
            StartCoroutine(waitForRevive());
        }

        IEnumerator waitForRevive() {
            yield return new WaitForSeconds(GameManager.RespawnTime);
            animator.SetBool("isDowned", false);
            health.Revive();
            health.Heal(health.MaxHealth);
            _ls = Vector3.one;
            transform.localScale = _ls;
        }

        health.onDowned += revive;
    }

    private Vector2 visionVec;
    public float viewAngle { get; private set; }
    public int animNum { get; private set; }
    void Update() {
        if (health.isDowned || PauseControl.isPaused) {
            return;
        }
        _vel.x = Input.GetAxis("Horizontal");
        _vel.y = Input.GetAxis("Vertical");

        visionVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - viewPos.position;

        float _magn = _vel.magnitude;

        if (isFacingToCursor) {
            viewAngle = Vector2.SignedAngle(Vector2.up, visionVec) + 180f;
        } else if (_magn > 1e-3) {
            viewAngle = Vector2.SignedAngle(Vector2.up, _vel) + 180f;
        }
        if (_magn > 1e-3) {
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
        }

        animNum = Mathf.FloorToInt((viewAngle + 22.5f) / 45) % 8;

        animator.SetInteger("currDir", animNum);

        walker.moveVectorRaw = _vel;

        if (Input.GetKeyDown(KeyCode.Escape))
            gameUI.SetPause();
    }

}
