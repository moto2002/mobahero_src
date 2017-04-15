using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using System;
using UnityEngine;

public class BattleEquip_MenuItem : MonoBehaviour
{
	[SerializeField]
	private new UISprite name;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite sprite;

	private BattleEquipType itemType = BattleEquipType.recommend;

	private bool chooseState;

	private Action<BattleEquip_MenuItem> onClick;

	public BattleEquipType MenuItemType
	{
		get
		{
			return this.itemType;
		}
		set
		{
			this.itemType = value;
			this.RefreshUI();
		}
	}

	public bool ChooseState
	{
		get
		{
			return this.chooseState;
		}
		set
		{
			this.chooseState = value;
			this.RefreshUI_state();
		}
	}

	public Action<BattleEquip_MenuItem> OnClick
	{
		get
		{
			return this.onClick;
		}
		set
		{
			this.onClick = value;
		}
	}

	private void Awake()
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickFunc);
	}

	private void OnClickFunc(GameObject go)
	{
		AudioMgr.PlayUI("Play_Shop_Select", null, false, false);
		if (this.onClick != null)
		{
			this.onClick(this);
		}
	}

	private void RefreshUI()
	{
		this.RefreshUI_name();
		this.RefreshUI_state();
		this.RefreshUI_icon();
	}

	private void RefreshUI_name()
	{
		this.name.spriteName = BattleEquipTools_config.GetNameByEquipType(this.MenuItemType);
	}

	private void RefreshUI_state()
	{
		this.sprite.alpha = ((!this.ChooseState) ? 0f : 1f);
	}

	private void RefreshUI_icon()
	{
		this.icon.spriteName = BattleEquipTools_config.GetIconByEquipType(this.MenuItemType);
		this.icon.MakePixelPerfect();
	}
}
