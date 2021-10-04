using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingProjectileComponent : MonoBehaviour {

	public float critChance;
	public float critMultipler;
	public float knockbackInflictedMultipler;

	private List<GameObject> damagedObjects = new List<GameObject>();

	public GameObject owner;

	public void damageTarget(GameObject target, float damage, Vector3 center) {
		if (target == owner || damagedObjects.Contains(target)) return;
		damagedObjects.Add(target);

		Health health = target.GetComponent<Health>();
		if (health == null) return;

		if (Random.value < critChance) {
			damage *= (1 + critMultipler);
		}

		health.GetDamage(damage, transform.position + center, knockbackInflictedMultipler);
	}
	public void damageTarget(GameObject target, float damage) {
		damageTarget(target, damage, Vector2.zero);
	}

	public void clearDamagedList() {
		damagedObjects.Clear();
	}


	public bool listContains(GameObject obj) => damagedObjects.Contains(obj);
}
