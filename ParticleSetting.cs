using System;
using UnityEngine;

[Serializable]
public class ParticleSetting : MonoBehaviour
{
	public float LifeTime;

	public ParticleSetting()
	{
		this.LifeTime = (float)1;
	}

	public override void Start()
	{
		if (this.LifeTime >= (float)0)
		{
			UnityEngine.Object.Destroy(this.gameObject, this.LifeTime);
		}
	}

	public override void Main()
	{
	}
}
