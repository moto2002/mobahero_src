using System;
using UnityEngine;

public class UIPvpEntrance_RankRewardBtn : MonoBehaviour
{
	public GameObject mGained;

	public GameObject mGetRewardBtn;

	public GameObject mLinkToBtn;

	public UIEventListener.VoidDelegate OnClick;

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	public void SetButtonState(int _stateCode)
	{
		if (_stateCode == 2)
		{
			this.mGained.SetActive(true);
			this.mGetRewardBtn.gameObject.SetActive(false);
			this.mLinkToBtn.gameObject.SetActive(false);
		}
		else if (_stateCode == 1)
		{
			this.mGained.SetActive(false);
			this.mGetRewardBtn.gameObject.SetActive(true);
			this.mLinkToBtn.gameObject.SetActive(false);
		}
		else
		{
			this.mGained.SetActive(false);
			this.mGetRewardBtn.gameObject.SetActive(false);
			this.mLinkToBtn.gameObject.SetActive(true);
		}
	}

	public void SetClickEvent()
	{
		UIEventListener.Get(this.mGetRewardBtn).onClick = this.OnClick;
		UIEventListener.Get(this.mLinkToBtn).onClick = this.OnClick;
	}

	[ContextMenu("0")]
	public void Set0()
	{
		this.SetButtonState(0);
	}

	[ContextMenu("1")]
	public void Set1()
	{
		this.SetButtonState(1);
	}

	[ContextMenu("2")]
	public void Set2()
	{
		this.SetButtonState(2);
	}
}
