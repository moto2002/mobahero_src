using System;
using UnityEngine;

public class LSTest : NewMono
{
	public GameObject heroPrefab;

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(CachedRes.getUnitAtResPath("Zhousi")) as GameObject;
		}
	}
}
