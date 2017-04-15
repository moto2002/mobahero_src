using System;
using UnityEngine;

public class TestAdapter : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.A))
		{
			ParticleAdapter.AdaptDown();
		}
	}
}
