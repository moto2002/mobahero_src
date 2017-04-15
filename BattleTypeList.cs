using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BattleTypeList : MonoBehaviour
{
	public UIInput inputLabel;

	private UISprite VICon;

	private UIPanel UIListPanel;

	private float centerY;

	private float sizeY;

	public string[] battleData = new string[]
	{
		"Twenty",
		"800055",
		"80003",
		"80006",
		"Other"
	};

	public string chooseStr = string.Empty;

	public bool isInMainPanel = true;

	private GameObject twentyGo;

	private GameObject allGo;

	private Transform ListPanel;

	private UISprite ListPanelBg;

	public string Account
	{
		get
		{
			return this.chooseStr;
		}
		set
		{
			this.chooseStr = value;
			if (null != this.inputLabel)
			{
				this.inputLabel.value = this.chooseStr;
			}
		}
	}

	private void Awake()
	{
		this.inputLabel = this.inputLabel.transform.GetComponent<UIInput>();
		this.VICon = base.transform.Find("V").GetComponent<UISprite>();
		this.ListPanel = base.transform.Find("Panel");
		this.UIListPanel = this.ListPanel.GetComponent<UIPanel>();
		this.ListPanelBg = this.UIListPanel.transform.Find("Bg").GetComponent<UISprite>();
		UIEventListener expr_88 = UIEventListener.Get(base.transform.gameObject);
		expr_88.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_88.onClick, new UIEventListener.VoidDelegate(this.OnClickVICon));
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.inputLabel.value != this.chooseStr)
		{
			this.inputLabel.value = this.chooseStr;
		}
	}

	private void OnClickVICon(GameObject obj)
	{
		if (this.VICon.transform.localEulerAngles.z <= 1f)
		{
			base.StartCoroutine(this.InitAndFixItem());
			this.ShowPanel();
		}
		else if (this.VICon.transform.localEulerAngles.z > 179f)
		{
			this.HidePanel();
		}
	}

	[DebuggerHidden]
	private IEnumerator InitAndFixItem()
	{
		BattleTypeList.<InitAndFixItem>c__IteratorAC <InitAndFixItem>c__IteratorAC = new BattleTypeList.<InitAndFixItem>c__IteratorAC();
		<InitAndFixItem>c__IteratorAC.<>f__this = this;
		return <InitAndFixItem>c__IteratorAC;
	}

	private void ShowPanel()
	{
		TweenRotation.Begin(this.VICon.gameObject, 0.5f, Quaternion.Euler(0f, 0f, 180f));
		this.UIListPanel.enabled = true;
		base.StartCoroutine(this.OnShowPanel());
	}

	[DebuggerHidden]
	private IEnumerator OnShowPanel()
	{
		BattleTypeList.<OnShowPanel>c__IteratorAD <OnShowPanel>c__IteratorAD = new BattleTypeList.<OnShowPanel>c__IteratorAD();
		<OnShowPanel>c__IteratorAD.<>f__this = this;
		return <OnShowPanel>c__IteratorAD;
	}

	public void HidePanel()
	{
		TweenRotation.Begin(this.VICon.gameObject, 0.5f, Quaternion.Euler(0f, 0f, 0f));
		base.StartCoroutine(this.OnHidePanel());
	}

	public void QuickHidePanel()
	{
		this.UIListPanel.baseClipRegion = new Vector4(138f, 99.9f, 600f, 0.1f);
		this.UIListPanel.enabled = false;
	}

	[DebuggerHidden]
	private IEnumerator OnHidePanel()
	{
		BattleTypeList.<OnHidePanel>c__IteratorAE <OnHidePanel>c__IteratorAE = new BattleTypeList.<OnHidePanel>c__IteratorAE();
		<OnHidePanel>c__IteratorAE.<>f__this = this;
		return <OnHidePanel>c__IteratorAE;
	}

	public void OnClickLabel(GameObject item = null)
	{
		this.VICon.transform.localEulerAngles = Vector3.zero;
		this.chooseStr = item.transform.Find("Label").GetComponent<UILabel>().text;
		item.transform.GetComponent<UIToggle>().value = true;
		Singleton<AchievementView>.Instance.SelectBattle(this.isInMainPanel, item.name);
		Singleton<AchievementView>.Instance.vBtn = 1;
		Singleton<AchievementView>.Instance.CreatBattleRecord(this.isInMainPanel, 0, item.name, null);
		this.HidePanel();
	}

	public void RefreshLabelToggle()
	{
		if (null != this.twentyGo)
		{
			this.twentyGo.transform.GetComponent<UIToggle>().value = true;
		}
		if (null != this.allGo)
		{
			this.allGo.transform.GetComponent<UIToggle>().value = true;
		}
	}
}
