using System;
using UnityEngine;

public class CheckWhenEnable : MonoBehaviour
{
	private void OnEnable()
	{
		Debug.Log("CheckWhenEnable!!!OnEnable");
	}

	private void OnDisable()
	{
		Debug.Log("CheckWhenEnable!!!OnDisable");
	}
}
