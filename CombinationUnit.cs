using System;
using UnityEngine;

public class CombinationUnit : MonoBehaviour
{
	private Combination owner;

	public int index;

	public void Init(Combination owner, int index)
	{
		this.owner = owner;
		this.index = index;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DangBan"))
		{
			this.owner.Stop(this.index);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
