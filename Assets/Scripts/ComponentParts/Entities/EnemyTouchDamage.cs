using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyTouchDamage : MonoBehaviour {

	public Collider2D trigger;

    public float damage;
	public float attackInterval;
	public float knockback;

	float lastAttackTime;

	private void OnTriggerEnter2D (Collider2D collision) {
		if (Time.time < lastAttackTime + attackInterval) {
			StartCoroutine(refreshTrigger(lastAttackTime + attackInterval - Time.time));
			return;
		}
		if (collision.TryGetComponent(out Health entity)) {
			StartCoroutine(refreshTrigger(attackInterval));
			entity.GetDamage(damage, transform.position, knockback);
			lastAttackTime = Time.time;
		}
	}

	IEnumerator refreshTrigger(float enableDelay) {
		trigger.enabled = false;
		yield return new WaitForSeconds(enableDelay);
		trigger.enabled = true;
	}
}
