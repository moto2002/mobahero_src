using System;
using UnityEngine;

public class HideBySDK : MonoBehaviour
{
	private void Awake()
	{
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			base.gameObject.SetActive(false);
		}
	}
}
