using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HideBattleUI : MonoBehaviour
{
	public bool isHidden;

	public GameObject CtrlBtn;

	public static bool useThisScript;

	public static bool openBattleEquipmentSystem = true;

	private Transform battle;

	private GameObject FPS;

	private GameObject waterBar;

	private GameObject skillBg;

	private List<GameObject> GosToHidden = new List<GameObject>();

	private UIPanel skillPanel;

	private void Awake()
	{
		if (HideBattleUI.useThisScript)
		{
			UIEventListener expr_15 = UIEventListener.Get(this.CtrlBtn);
			expr_15.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_15.onClick, new UIEventListener.VoidDelegate(this.Click));
			this.battle = GameObject.Find("Battle").transform;
			this.skillPanel = base.transform.parent.GetComponent<UIPanel>();
		}
	}

	private void Start()
	{
		if (HideBattleUI.useThisScript)
		{
			for (int i = 0; i < this.battle.childCount; i++)
			{
				Transform child = this.battle.GetChild(i);
				if (!child.name.Contains("SkillView") && !child.name.Contains("CharacterView") && !child.name.Contains("MessageView"))
				{
					this.GosToHidden.Add(child.gameObject);
				}
			}
			this.FPS = GameObject.Find("FPSView(Clone)");
			this.waterBar = GameObject.Find("LanTiao");
			this.GosToHidden.Add(this.FPS);
			this.CtrlBtn.SetActive(true);
		}
		else
		{
			this.CtrlBtn.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (this.CtrlBtn != null)
		{
			UIEventListener expr_1C = UIEventListener.Get(this.CtrlBtn);
			expr_1C.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(expr_1C.onClick, new UIEventListener.VoidDelegate(this.Click));
		}
	}

	private void Click(GameObject gObj)
	{
		this.isHidden = !this.isHidden;
		if (this.isHidden)
		{
			foreach (GameObject current in this.GosToHidden)
			{
				if (current != null)
				{
					current.SetActive(true);
				}
			}
			this.skillPanel.GetComponent<UISprite>().enabled = true;
			this.skillPanel.alpha = 1f;
			if (this.waterBar != null)
			{
				this.waterBar.gameObject.SetActive(true);
			}
			MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshRenderer meshRenderer = array[i];
				meshRenderer.gameObject.SetActive(true);
			}
			TrailRenderer[] componentsInChildren2 = base.transform.GetComponentsInChildren<TrailRenderer>();
			TrailRenderer[] array2 = componentsInChildren2;
			for (int j = 0; j < array2.Length; j++)
			{
				TrailRenderer trailRenderer = array2[j];
				trailRenderer.gameObject.SetActive(true);
			}
		}
		else
		{
			this.skillPanel.GetComponent<UISprite>().enabled = false;
			this.skillPanel.alpha = 0.01f;
			foreach (GameObject current2 in this.GosToHidden)
			{
				if (current2 != null)
				{
					current2.SetActive(false);
				}
			}
			MeshRenderer[] componentsInChildren3 = base.transform.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array3 = componentsInChildren3;
			for (int k = 0; k < array3.Length; k++)
			{
				MeshRenderer meshRenderer2 = array3[k];
				meshRenderer2.gameObject.SetActive(false);
			}
			TrailRenderer[] componentsInChildren4 = base.transform.GetComponentsInChildren<TrailRenderer>();
			TrailRenderer[] array4 = componentsInChildren4;
			for (int l = 0; l < array4.Length; l++)
			{
				TrailRenderer trailRenderer2 = array4[l];
				trailRenderer2.gameObject.SetActive(false);
			}
			if (this.waterBar != null)
			{
				this.waterBar.gameObject.SetActive(false);
			}
		}
	}

	public static void HideUI(bool ret)
	{
		if (Singleton<SkillView>.Instance.gameObject != null)
		{
			Singleton<SkillView>.Instance.gameObject.SetActive(ret);
		}
		if (Singleton<GoldView>.Instance.gameObject != null)
		{
			Singleton<GoldView>.Instance.gameObject.SetActive(ret);
		}
		if (Singleton<AttachedPropertyView>.Instance.gameObject != null)
		{
			Singleton<AttachedPropertyView>.Instance.gameObject.SetActive(ret);
		}
		if (Singleton<BuffView>.Instance.gameObject != null)
		{
			Singleton<BuffView>.Instance.gameObject.SetActive(ret);
		}
		if (Singleton<HUDModuleManager>.Instance.gameObject != null)
		{
			Singleton<HUDModuleManager>.Instance.gameObject.SetActive(ret);
		}
	}
}
