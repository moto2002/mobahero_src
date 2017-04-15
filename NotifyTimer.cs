using Com.Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NotifyTimer : MonoBehaviour
{
	private static NotifyTimer _instance;

	public static NotifyTimer instance
	{
		get
		{
			if (NotifyTimer._instance == null)
			{
				NotifyTimer._instance = GlobalObject.Instance.gameObject.AddComponent<NotifyTimer>();
			}
			return NotifyTimer._instance;
		}
	}

	private void Awake()
	{
		if (!LocalNoti.CheckOpen())
		{
			return;
		}
	}

	private void IOSCheck(List<SysNotificationVo> notis)
	{
	}

	public void Check()
	{
	}
}
