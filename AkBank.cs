using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Wwise/AkBank")]
public class AkBank : AkUnityEventHandler
{
	public string bankName = string.Empty;

	public bool loadAsynchronous;

	public List<int> unloadTriggerList = new List<int>
	{
		-358577003
	};

	protected override void Awake()
	{
		base.Awake();
		base.RegisterTriggers(this.unloadTriggerList, new AkTriggerBase.Trigger(this.UnloadBank));
		if (this.unloadTriggerList.Contains(1151176110))
		{
			this.UnloadBank(null);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (this.unloadTriggerList.Contains(1281810935))
		{
			this.UnloadBank(null);
		}
	}

	public override void HandleEvent(GameObject in_gameObject)
	{
		if (!this.loadAsynchronous)
		{
			AkBankManager.LoadBank(this.bankName, 0);
		}
		else
		{
			AkBankManager.LoadBankAsync(this.bankName, null);
		}
	}

	public void UnloadBank(GameObject in_gameObject)
	{
		AkBankManager.UnloadBank(this.bankName, false, 0);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.UnregisterTriggers(this.unloadTriggerList, new AkTriggerBase.Trigger(this.UnloadBank));
		if (this.unloadTriggerList.Contains(-358577003))
		{
			this.UnloadBank(null);
		}
	}
}
