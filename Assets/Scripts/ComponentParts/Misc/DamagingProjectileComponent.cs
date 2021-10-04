using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingProjectileComponent : MonoBehaviour
{
	private List<GameObject> damagedObjects = new List<GameObject>();

	public GameObject owner;

	public void damageTarget(GameObject target, float damage, Vector3 center, float knockbackForce)
	{
		if (target == owner || damagedObjects.Contains(target)) return;
		damagedObjects.Add(target);

		Health health = target.GetComponent<Health>();
		if (health == null) return;

		health.GetDamage(damage, transform.position + center, knockbackForce);
	}
	//public void damageTarget(GameObject target, float damage)
	//{
	//	damageTarget(target, damage, Vector2.zero, 0);
	//}

	public void clearDamagedList()
	{
		damagedObjects.Clear();
	}


	public bool listContains(GameObject obj) => damagedObjects.Contains(obj);
}

