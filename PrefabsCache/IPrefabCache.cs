using System;
using UnityEngine;

namespace PrefabsCache
{
	public interface IPrefabCache
	{
		ReusablePrefab Instantiate(ReusablePrefab prefab);

		ReusablePrefab Instantiate(ReusablePrefab prefab, Vector3 position);

		ReusablePrefab Instantiate(ReusablePrefab prefab, Vector3 position, Quaternion rotation);
	}
}
