using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class HeroItem : ChildView
{
	private Transform Panel;

	private UIWidget UIPanel;

	private Transform Level;

	private UILabel UILevel;

	private UITexture UIAvatar;

	private UISprite UIQuality;

	private Transform Star;

	private UIGrid StarGrid;

	protected Transform Frame;

	protected UISprite UIFrame;

	private UIProgressBar HpSlider;

	private UIProgressBar MpSlider;

	private Transform FlagItem;

	private Transform SignFlag;

	private bool recordIsInit;

	protected bool m_Selected;

	protected bool m_MasterHero;

	public BattleType battletype;

	public Callback<HeroItem, bool> OnChangeHeroCallback;

	public HeroItem(Transform root) : base(root)
	{
		if (this.recordIsInit)
		{
			return;
		}
		this.Panel = base.transform.Find("Panel");
		this.UIPanel = this.Panel.GetComponent<UIWidget>();
		Transform transform = this.Panel.Find("Avatar");
		this.UIAvatar = transform.GetComponent<UITexture>();
		Transform transform2 = this.Panel.Find("Quality");
		this.UIQuality = transform2.GetComponent<UISprite>();
		this.Star = this.Panel.Find("Star");
		this.StarGrid = this.Star.GetComponent<UIGrid>();
		this.HpSlider = this.Panel.Find("Progress Bar_HP").GetComponent<UIProgressBar>();
		this.MpSlider = this.Panel.Find("Progress Bar_MP").GetComponent<UIProgressBar>();
		this.Level = base.transform.Find("Level");
		this.UILevel = this.Level.GetComponent<UILabel>();
		this.Frame = base.transform.Find("Frame");
		this.UIFrame = this.Frame.GetComponent<UISprite>();
		this.FlagItem = base.transform.Find("FlagItem");
		this.SignFlag = this.FlagItem.Find("Sign");
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSelectHero);
		this.recordIsInit = true;
	}

	public void ShowHero(string hero_id, bool showframe)
	{
		if (hero_id != string.Empty)
		{
			this.Panel.gameObject.SetActive(true);
			this.Level.gameObject.SetActive(true);
			HeroInfoData heroData = CharacterDataMgr.instance.GetHeroData(hero_id);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
			this.ShowStar((heroData == null) ? 1 : heroData.Level);
			this.UIAvatar.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
			this.UILevel.text = CharacterDataMgr.instance.GetLevel((heroData == null) ? 0L : heroData.Exp).ToString();
			this.UIQuality.spriteName = CharacterDataMgr.instance.GetFrame_HeroIcon((heroData == null) ? 1 : heroData.Grade);
			if (this.battletype == BattleType.YZ)
			{
				if (!Singleton<SelectView>.Instance.TBCCheckLive(hero_id, this))
				{
					this.UIAvatar.color = new Color(0.5f, 0.5f, 0.5f);
				}
				else
				{
					this.UIAvatar.color = new Color(1f, 1f, 1f);
				}
			}
			else
			{
				this.ShowHpValue(false, 0f);
				this.ShowMpValue(false, 0f);
			}
		}
		else
		{
			this.Panel.gameObject.SetActive(false);
			this.Level.gameObject.SetActive(false);
		}
		this.ShowFrame(showframe);
	}

	public void ShowFrame(bool enabled)
	{
		this.Frame.gameObject.SetActive(enabled);
	}

	private void ShowStar(int number)
	{
		int childCount = this.Star.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = this.Star.GetChild(i);
			if (i <= number - 1)
			{
				child.gameObject.SetActive(true);
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}
		this.StarGrid.enabled = true;
		this.StarGrid.Reposition();
	}

	public void ShowHpValue(bool isShow, float value = 0f)
	{
		this.HpSlider.gameObject.SetActive(isShow);
		this.HpSlider.value = value;
	}

	public void ShowMpValue(bool isShow, float value = 0f)
	{
		this.MpSlider.gameObject.SetActive(isShow);
		this.MpSlider.value = value;
	}

	public virtual void UpdateSelect(bool isSelected, bool isMaster)
	{
		if (isSelected)
		{
			this.UIPanel.alpha = 0.5f;
			this.FlagItem.gameObject.SetActive(true);
			this.m_Selected = true;
			this.UpdateMaster(isMaster);
		}
		else
		{
			this.UIPanel.alpha = 1f;
			this.UpdateMaster(false);
			this.FlagItem.gameObject.SetActive(false);
			this.m_Selected = false;
		}
	}

	protected virtual void UpdateMaster(bool isMaster)
	{
		if (isMaster)
		{
			this.SignFlag.gameObject.SetActive(true);
			this.m_MasterHero = true;
		}
		else
		{
			this.SignFlag.gameObject.SetActive(false);
			this.m_MasterHero = false;
		}
	}

	private void OnSelectHero(GameObject objct_1 = null)
	{
		if (this.OnChangeHeroCallback != null)
		{
			this.OnChangeHeroCallback(this, !this.m_Selected);
		}
	}
}
