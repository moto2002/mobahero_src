using System;
using UnityEngine;

public sealed class MessengerHelper : MonoBehaviour
{
	private void Awake()
	{
	}

	public void OnDestroy()
	{
		NotificationManager.Cleanup();
	}
}
