using System;
using UnityEngine;

public class OnlyPlayOnceTowerDeadEffect : MonoBehaviour
{
	private int disableCount;

	private void OnDisable()
	{
		this.disableCount++;
		if (this.disableCount == 2)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
