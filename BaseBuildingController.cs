using System;
using System.Collections;
using UnityEngine;

public abstract class BaseBuildingController : MonoBehaviour
{
	public abstract void OnCreate(Tower owner);

	public abstract void OnDamage(Units attacker, float damage);

	public abstract void OnDead();

	public abstract IEnumerator OnPrewarm();

	public static void SetActive(GameObject obj, bool active)
	{
		if (obj)
		{
			obj.SetActive(active);
		}
	}
}
