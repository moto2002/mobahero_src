using Assets.Scripts.Model;
using System;
using UnityEngine;

public class EffectQualityAdapter : MonoBehaviour
{
	[HideInInspector]
	public bool? IsHighQuality;

	[HideInInspector]
	public Action ReactiveCall;

	private bool isAfterAwake;

	public void Awake()
	{
		if (!this.isAfterAwake)
		{
			this.isAfterAwake = true;
			this.GetFlag();
			MobaMessageManager.RegistMessage((ClientMsg)23078, new MobaMessageFunc(this.OnMsg_Change));
		}
	}

	private void OnEnable()
	{
		if (this.IsEffectShutOff_Internal())
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		this.isAfterAwake = false;
		MobaMessageManager.UnRegistMessage((ClientMsg)23078, new MobaMessageFunc(this.OnMsg_Change));
	}

	public void GetFlag()
	{
		if (ModelManager.Instance != null)
		{
			this.IsHighQuality = new bool?(ModelManager.Instance.Get_SettingData().IsHighQualitySetting);
		}
	}

	private void OnMsg_Change(MobaMessage msg)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		switch ((int)msg.Param)
		{
		case 0:
		case 1:
			this.IsHighQuality = new bool?(false);
			base.gameObject.SetActive(false);
			break;
		case 2:
		case 3:
			this.IsHighQuality = new bool?(true);
			base.gameObject.SetActive(true);
			if (this.ReactiveCall != null)
			{
				this.ReactiveCall();
			}
			break;
		case 4:
			this.IsHighQuality = new bool?(true);
			base.gameObject.SetActive(true);
			if (this.ReactiveCall != null)
			{
				this.ReactiveCall();
			}
			break;
		}
	}

	public void SetActiveForEffect(bool isActive)
	{
		Action action = delegate
		{
			if (this.gameObject != null)
			{
				this.gameObject.SetActive(isActive);
			}
		};
		this.ReactiveCall = action;
		if (!this.IsEffectShutOff_Internal())
		{
			action();
		}
	}

	public bool IsEffectShutOff_Internal()
	{
		this.GetFlag();
		return !this.IsHighQuality.Value;
	}

	public static bool IsEffectShutOff(Transform checkedNode)
	{
		if (checkedNode == null)
		{
			return false;
		}
		EffectQualityAdapter component = checkedNode.GetComponent<EffectQualityAdapter>();
		return !(component == null) && component.IsEffectShutOff_Internal();
	}

	public static void SetReactiveAction(Transform targetNode, Action callback)
	{
		if (targetNode == null || callback == null)
		{
			return;
		}
		EffectQualityAdapter component = targetNode.GetComponent<EffectQualityAdapter>();
		if (component == null)
		{
			return;
		}
		if (!component.isAfterAwake)
		{
			component.Awake();
		}
		component.ReactiveCall = callback;
	}
}
