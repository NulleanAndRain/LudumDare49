﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item
{
    public GameObject ParticlePrefab;
    [SerializeField] private ItemBase Base = null;

    [SerializeField] private float CD = 0.1f;
    private float lastUse;

    [SerializeField] public float HealAmnt = 10;
    private void Start()
    {
        Base.onClickDownMain += UseMain;
        Base.onClickDownSecondary += UseSecondary;
        lastUse = 0;
    }

    private void UseMain()
    {
        if (Time.time < lastUse + CD) return;
        lastUse = Time.time;
        CurrentCount--;
        Instantiate(ParticlePrefab, transform.position, Quaternion.Euler(0, 0, 90));

        var health = _playerInventory.GetComponent<Health>();
        health.GetDamage(HealAmnt);
    }

    private void UseSecondary()
    {
        if (Time.time < lastUse + CD) return;
        lastUse = Time.time;
        CurrentCount++;

        var health = _playerInventory.GetComponent<Health>();
        health.Heal(HealAmnt);
    }
}
