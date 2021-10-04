using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item
{
    public GameObject ParticlePrefab;

    [SerializeField] private float CD = 0.1f;
    private float lastUse;

    [SerializeField] public float HealAmnt = 10;
    private void Start()
    {
        _base.onClickDownMain += UseMain;
        _base.onClickDownSecondary += UseSecondary;
        lastUse = -2 * CD;
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

        StartCoroutine(_animate());
    }

    private IEnumerator _animate()
    {
        var anim = _base.PlayerAnim;
        anim.TriggerAnimation(AnimationTrigger.Downed);
        yield return new WaitForSeconds(anim.CurrentAnimLength(AnimationLayer.Base) - 0.5f);
        anim.TriggerAnimation(AnimationTrigger.Reset);
    }
}
