using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float MaxHealth;
    private float currHealth;
    public bool isImmortal;

    public bool canRegen;
    public float damageImmunityTime;
    private float lastDamageTime;

    public float regenAfterDamageCD;
    private float _regCD = 0;

    public float regenInterval;
    public float regenPerInterval;

    [Header("Knockback")]
    [Range(0, 1)]
    public float knockbackResist;
    private Rigidbody2D rb;
    public Vector2 center;

    [Header("Damage particles")]
    public ParticleSystem ParticlePrefab;
    public Vector2 ParticlesEmissionPoint;
    Vector3 _particlesPos;

    /// <summary>
    /// invokes when entity gets current hp less or equal to 0
    /// </summary>
    public event Action onDowned = delegate { };
    /// <summary>
    /// invokes when entity gets current hp updated. params:
    /// <para>currHealth - current HP</para>
    /// <para>maxHealth - max HP</para>
    /// </summary>
    public event Action<float, float> onHealthUpdate = delegate { };
    /// <summary>
    /// invokes when entity gets damage. params:
    /// <para> kbForce - calculated knockback force of impact</para>
    /// <para> angle - angle with Vector2.up of knockback force</para>
    /// </summary>
    public event Action<float, float> onGetDamage = delegate { };

    public bool isDowned { get; private set; } = false;

    // Start is called before the first frame update
    void Start() {
        currHealth = MaxHealth;
        _particlesPos = ParticlesEmissionPoint;

        rb = GetComponent<Rigidbody2D>();

        if (canRegen) StartCoroutine(Regen());
    }

    private IEnumerator Regen() {
        while (true) {
            if (isDowned) yield return new WaitForSeconds(GameManager.RespawnTime);
            if (_regCD == 0) {
                if (currHealth != MaxHealth) {
                    if (currHealth < MaxHealth) currHealth += regenPerInterval;
                    if (currHealth > MaxHealth) currHealth = MaxHealth;
                    onHealthUpdate(currHealth, MaxHealth);
                }
                yield return new WaitForSeconds(regenInterval);

            } else {
                yield return new WaitForSeconds(_regCD);
                _regCD = 0;
            }
        }
    }

    private Vector2 _lastDamageDir = new Vector2();

    Vector2 _kbDir = Vector2.zero;
    public void GetDamage(float amount, Vector2 pos, float kbForce = 0) {
        if (Time.time - lastDamageTime < damageImmunityTime) return;
        lastDamageTime = Time.time;
        _regCD = regenAfterDamageCD > _regCD ? regenAfterDamageCD : _regCD;

        _lastDamageDir = (Vector2)transform.position + center - pos;

        float angle = Vector2.SignedAngle(Vector2.up, _lastDamageDir);

        if (Mathf.Abs(rb.velocity.x) >= 1e-4) _kbDir.x += -rb.velocity.x * 2;

        kbForce *= (1 - knockbackResist);
        if (kbForce < 0) kbForce = 0;
        _lastDamageDir *= kbForce;
        rb.AddForce(_lastDamageDir, ForceMode2D.Impulse);

        if (ParticlePrefab != null) {
            Instantiate(
                ParticlePrefab, 
                transform.position + _particlesPos,
                Quaternion.Euler(0, 0, angle)
            );
        }
        onGetDamage(kbForce, angle);

        if (!isImmortal) {
            currHealth -= amount;
            if (currHealth <= 0) {
                onDowned();
                amount += currHealth;
                currHealth = 0;
                isDowned = true;
            }
        }

        onHealthUpdate(currHealth, MaxHealth);
    }

    public void GetDamage(float amount) => GetDamage(amount, transform.position, 0);

    public void Heal(float amount) {
        if (amount <= 0) return;
        currHealth += amount;
        if (currHealth > MaxHealth) {
            amount -= currHealth - MaxHealth;
            currHealth = MaxHealth;
        }

        onHealthUpdate(currHealth, MaxHealth);
    }

    public void Revive(float percent = 1) {
        isDowned = false;
        currHealth = MaxHealth * percent;
        onHealthUpdate(currHealth, MaxHealth);
    }

    public void setDamageImmunityForTime(float time) {
        lastDamageTime = Time.time;
        StartCoroutine(endImmunity(damageImmunityTime, time));
        damageImmunityTime = time;
    }

    private IEnumerator endImmunity(float prevImmVal, float time) {
        yield return new WaitForSeconds(time);
        damageImmunityTime = prevImmVal;
    }
}