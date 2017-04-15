using System;
using System.Collections.Generic;
using UnityEngine;

public class TalkingDataDemoScript : MonoBehaviour
{
	private const int left = 90;

	private const int height = 50;

	private const int top = 60;

	private const int step = 60;

	private int index = 1;

	private int level = 1;

	private string gameserver = string.Empty;

	private TDGAAccount account;

	private int width = Screen.width - 180;

	private void OnGUI()
	{
		int num = 0;
		GUI.Box(new Rect(10f, 10f, (float)(Screen.width - 20), (float)(Screen.height - 20)), "Demo Menu");
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Create User"))
		{
			this.account = TDGAAccount.SetAccount("User" + this.index);
			this.index++;
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Account Level +1") && this.account != null)
		{
			this.account.SetLevel(this.level++);
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Chagen Game Server + 'a'") && this.account != null)
		{
			this.gameserver += "a";
			this.account.SetGameServer(this.gameserver);
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Charge Request 10"))
		{
			TDGAVirtualCurrency.OnChargeRequest("order01", "iap", 10.0, "CH", 10.0, "PT");
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Charge Success 10"))
		{
			TDGAVirtualCurrency.OnChargeSuccess("order01");
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Reward 100"))
		{
			TDGAVirtualCurrency.OnReward(100.0, "reason");
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Mission Begin"))
		{
			TDGAMission.OnBegin("miss001");
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Mission Completed"))
		{
			TDGAMission.OnCompleted("miss001");
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Item Purchase 10"))
		{
			TDGAItem.OnPurchase("itemid001", 10, 10.0);
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Item Use 1"))
		{
			TDGAItem.OnUse("itemid001", 1);
		}
		if (GUI.Button(new Rect(90f, (float)(60 + 60 * num++), (float)this.width, 50f), "Custome Event"))
		{
			TalkingDataGA.OnEvent("action_id", new Dictionary<string, object>
			{
				{
					"StartAppStartAppTime",
					"startAppMac#02/01/2013 09:52:24"
				},
				{
					"IntValue",
					1
				}
			});
		}
	}

	private void Start()
	{
		Debug.Log("start...!!!!!!!!!!");
		TalkingDataGA.OnStart("B54977E570492ED5B2CEDD9B3D69C16B", "your_channel_id");
		this.account = TDGAAccount.SetAccount("User01");
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnDestroy()
	{
		TalkingDataGA.OnEnd();
		Debug.Log("onDestroy");
	}

	private void Awake()
	{
		Debug.Log("Awake");
	}

	private void OnEnable()
	{
		Debug.Log("OnEnable");
	}

	private void OnDisable()
	{
		Debug.Log("OnDisable");
	}
}
