using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using System;
using UnityEngine;

internal class BattleEquip_recommendItem : MonoBehaviour
{
	[SerializeField]
	private UISprite sprite_icon;

	[SerializeField]
	private UILabel label_name;

	[SerializeField]
	private UILabel label_price;

	[SerializeField]
	private UILabel label_attri;

	[SerializeField]
	private UILabel label_des;

	[SerializeField]
	private GameObject go_Effect;

	private GameObject rootObj;

	private UIWidget widget;

	private float alpha;

	private bool showDetail;

	private RItemData recommendItem;

	private Callback<BattleEquip_recommendItem> callback_ClickItem;

	private TweenAlpha tweenA;

	public RItemData RecommendItem
	{
		get
		{
			return this.recommendItem;
		}
		set
		{
			this.recommendItem = value;
			this.RefreshUI_item();
		}
	}

	public float Alpha
	{
		get
		{
			return this.alpha;
		}
		set
		{
			if (this.alpha != value)
			{
				this.alpha = value;
				this.RefreshUI_alpha();
			}
		}
	}

	public bool ShowDetail
	{
		private get
		{
			return this.showDetail;
		}
		set
		{
			this.showDetail = value;
			if (value)
			{
				this.RefreshUI_hide();
			}
		}
	}

	public Callback<BattleEquip_recommendItem> Callback_ClickItem
	{
		get
		{
			return this.callback_ClickItem;
		}
		set
		{
			this.callback_ClickItem = value;
		}
	}

	public GameObject RootGoObj
	{
		get
		{
			return this.rootObj;
		}
		set
		{
			this.rootObj = value;
			this.SetEffectParam();
		}
	}

	private void Awake()
	{
		this.alpha = 1f;
		this.widget = base.gameObject.GetComponent<UIWidget>();
		Transform parent = this.label_name.gameObject.transform.parent;
		this.tweenA = parent.GetComponent<TweenAlpha>();
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickItem);
	}

	private void RefreshUI_item()
	{
		RItemData rItemData = this.RecommendItem;
		if (rItemData != null)
		{
			this.sprite_icon.spriteName = rItemData.Config.icon;
			this.label_name.text = LanguageManager.Instance.GetStringById(rItemData.Config.name);
			this.label_attri.text = BattleEquipTools_config.GetAttriDes(rItemData.Config, " ", 2);
			this.label_des.text = LanguageManager.Instance.GetStringById(rItemData.Config.recommend_describe);
			this.label_price.text = rItemData.RealPrice.ToString();
			base.name = rItemData.ID;
		}
	}

	private void RefreshUI_alpha()
	{
		this.widget.alpha = this.alpha;
		base.transform.GetComponent<BoxCollider>().enabled = (this.alpha >= 1f);
		this.go_Effect.SetActive(this.alpha >= 1f);
	}

	private void RefreshUI_hide()
	{
		this.tweenA.Begin();
	}

	private void OnClickItem(GameObject go)
	{
		if (this.callback_ClickItem != null)
		{
			this.callback_ClickItem(this);
		}
	}

	public void NewbieClickItem()
	{
		if (this.callback_ClickItem != null)
		{
			this.callback_ClickItem(this);
		}
	}

	private void SetEffectParam()
	{
		if (null != this.RootGoObj)
		{
			UIEffectSort component = this.go_Effect.GetComponent<UIEffectSort>();
			component.panel = this.RootGoObj.GetComponent<UIPanel>();
			component.UpdateSortUI();
		}
	}
}
