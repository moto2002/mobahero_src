using Assets.Scripts.Model;
using System;
using UnityEngine;

public class SkinPanel : MobaMono
{
	private SkinDrawer _skinDrawer;

	private string _heroName;

	public event Action<int> onSkinChanged
	{
		add
		{
			this._skinDrawer.onSelectionChanged += value;
		}
		remove
		{
			this._skinDrawer.onSelectionChanged -= value;
		}
	}

	public event Action<int> onBuyBtnEvent
	{
		add
		{
			this._skinDrawer.onBuyBtn += value;
		}
		remove
		{
			this._skinDrawer.onBuyBtn -= value;
		}
	}

	public event Action<Transform, bool> onWearBtnEvent
	{
		add
		{
			this._skinDrawer.onWearBtn += value;
		}
		remove
		{
			this._skinDrawer.onWearBtn -= value;
		}
	}

	public int selection
	{
		get
		{
			return this._skinDrawer.selection;
		}
	}

	private void Awake()
	{
	}

	public void ClearResources()
	{
		if (this._skinDrawer != null)
		{
			this._skinDrawer.ClearResources();
		}
	}

	private void onClickBuy()
	{
		if (this._skinDrawer.animLerping)
		{
			return;
		}
		Debug.LogError("暂未实现功能！");
	}

	public void setHeroName(string heroName, long heroid, int skinid = 0)
	{
		bool isSame = heroName == this._heroName;
		this._heroName = heroName;
		this._skinDrawer = base.getComponent<SkinDrawer>(true);
		this._skinDrawer.setHeroName(heroName, heroid, skinid, isSame);
	}

	public void SetWearBtnState(long heroid, int skinid)
	{
		this._skinDrawer.SetWearBtnState(heroid, skinid);
	}

	public void SetBuyBtnState(int skinid)
	{
		this._skinDrawer.SetBuyBtnState(skinid);
	}

	private void OnDestroy()
	{
		if (this._skinDrawer != null)
		{
			UnityEngine.Object.Destroy(this._skinDrawer.gameObject);
		}
	}

	public static SkinPanel genSkinPanel(Transform parent)
	{
		GameObject gameObject = NGUITools.AddChild(parent.gameObject, CachedRes.getUITemplate("skinPanel"));
		SkinPanel component = gameObject.GetComponent<SkinPanel>();
		gameObject.layer = parent.gameObject.layer;
		return component;
	}

	public static bool IsPossessSkinId(int skinId)
	{
		return ModelManager.Instance.IsPossessSkin(skinId);
	}

	public static bool IsWearSkin(long heroid, int skinid)
	{
		return ModelManager.Instance.IsWearSkin(heroid, skinid);
	}

	public bool IsPossessIndex(int index)
	{
		return this._skinDrawer.IsPossessByIndex(index);
	}

	public int GetSkinByIndex(int index)
	{
		return this._skinDrawer.GetSkinByIndex(index);
	}

	public void RefreshUISkinPanel(string name = null)
	{
		this._skinDrawer.ReRefreshUISkinPanel(name);
	}

	public void ReFreshPrice(long heroid, int skinid)
	{
		this._skinDrawer.RefreshPrice(heroid, skinid);
	}
}
