using Assets.Scripts.Model;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UIListLogin : MonoBehaviour
{
	public UIInput inputLabel;

	private UISprite VICon;

	private UIPanel UIListPanel;

	private float centerY;

	private float sizeY;

	public List<string[]> userData = new List<string[]>();

	private string chooseStr = string.Empty;

	private Transform ListPanel;

	private Transform bg;

	private Transform bg1;

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
		this.inputLabel = base.transform.Find("Input").GetComponent<UIInput>();
		this.VICon = base.transform.Find("V").GetComponent<UISprite>();
		this.ListPanel = base.transform.Find("Panel");
		this.UIListPanel = this.ListPanel.GetComponent<UIPanel>();
		this.bg = base.transform.Find("Bg");
		this.bg1 = base.transform.Find("Bg1");
		this.bg.gameObject.SetActive(true);
		this.bg1.gameObject.SetActive(false);
		this.ListPanelBg = this.UIListPanel.transform.Find("Bg").GetComponent<UISprite>();
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

	private void OnClickVICon()
	{
		if (this.VICon.transform.localEulerAngles.z <= 1f)
		{
			this.InitAndFixItem();
			this.ShowPanel();
		}
		else if (this.VICon.transform.localEulerAngles.z > 179f)
		{
			this.HidePanel();
		}
	}

	private void InitAndFixItem()
	{
		int num = this.ListPanel.childCount - 1;
		for (int i = 0; i < this.userData.Count; i++)
		{
			this.ListPanelBg.height = 10 + 130 * this.userData.Count;
			GameObject gameObject;
			if (i >= num)
			{
				gameObject = NGUITools.AddChild(this.ListPanel.gameObject, Resources.Load("Prefab/UI/Login/ListLoginItem") as GameObject);
				gameObject.transform.Find("X").gameObject.AddComponent<UIEventListener>();
				UIEventListener.Get(gameObject.transform.Find("Label").gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLabel);
				UIEventListener.Get(gameObject.transform.Find("X").gameObject).onClick = new UIEventListener.VoidDelegate(this.DeleteItem);
			}
			else
			{
				gameObject = this.ListPanel.Find(i.ToString()).gameObject;
				gameObject.SetActive(true);
			}
			gameObject.name = i.ToString();
			gameObject.transform.localPosition = new Vector3(0f, -150f - (float)i * 135f, 0f);
			gameObject.transform.Find("Label").GetComponent<UILabel>().text = this.userData[i][0];
			if (i == this.userData.Count - 1)
			{
				gameObject.transform.Find("X").gameObject.SetActive(false);
			}
		}
		for (int j = this.userData.Count; j < num; j++)
		{
			GameObject gameObject2 = this.ListPanel.Find(j.ToString()).gameObject;
			if (gameObject2)
			{
				gameObject2.SetActive(false);
			}
		}
	}

	private void DeleteItem(GameObject item = null)
	{
		ModelManager.Instance.DeleteAccountKey(this.userData[int.Parse(item.transform.parent.name)][0]);
		this.userData.RemoveAt(int.Parse(item.transform.parent.name));
		this.InitAndFixItem();
		if (item.transform.parent.name == "0")
		{
			this.chooseStr = item.transform.parent.Find("Label").GetComponent<UILabel>().text;
			this.inputLabel.value = this.chooseStr;
		}
		if (this.userData.Count <= 1)
		{
			Singleton<LoginView_New>.Instance.SpeedRegister.gameObject.SetActive(true);
			Singleton<LoginView_New>.Instance.ListLogin.gameObject.SetActive(false);
		}
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
		UIListLogin.<OnShowPanel>c__Iterator165 <OnShowPanel>c__Iterator = new UIListLogin.<OnShowPanel>c__Iterator165();
		<OnShowPanel>c__Iterator.<>f__this = this;
		return <OnShowPanel>c__Iterator;
	}

	public void HidePanel()
	{
		this.bg.gameObject.SetActive(true);
		this.bg1.gameObject.SetActive(false);
		TweenRotation.Begin(this.VICon.gameObject, 0.5f, Quaternion.Euler(0f, 0f, 0f));
		base.StartCoroutine(this.OnHidePanel());
	}

	public void QuickHidePanel()
	{
		this.bg.gameObject.SetActive(true);
		this.bg1.gameObject.SetActive(false);
		this.UIListPanel.baseClipRegion = new Vector4(0f, -75.1f, 650f, 0.1f);
		this.UIListPanel.enabled = false;
	}

	[DebuggerHidden]
	private IEnumerator OnHidePanel()
	{
		UIListLogin.<OnHidePanel>c__Iterator166 <OnHidePanel>c__Iterator = new UIListLogin.<OnHidePanel>c__Iterator166();
		<OnHidePanel>c__Iterator.<>f__this = this;
		return <OnHidePanel>c__Iterator;
	}

	private void OnClickLabel(GameObject item = null)
	{
		this.VICon.transform.localEulerAngles = Vector3.zero;
		if (item.GetComponent<UILabel>().text == "其他帐号")
		{
			this.QuickHidePanel();
			Singleton<LoginView_New>.Instance.ShowOtherLogin();
		}
		else
		{
			this.chooseStr = item.GetComponent<UILabel>().text;
			item.transform.parent.GetComponent<UIToggle>().value = true;
			this.HidePanel();
		}
	}
}
