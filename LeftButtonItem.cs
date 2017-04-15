using Assets.Scripts.Model;
using System;
using UnityEngine;

public class LeftButtonItem : MonoBehaviour
{
	[SerializeField]
	private new UILabel name;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite sprite;

	private BattleEquipType type = BattleEquipType.recommend;

	public BattleEquipType Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	public void SetName(string str)
	{
		this.name.text = str;
	}

	public void ChoseState(bool type)
	{
		this.sprite.alpha = ((!type) ? 0f : 1f);
	}

	public void SetIcon(BattleEquipType Type)
	{
		if (Type == BattleEquipType.attack)
		{
			this.icon.spriteName = "HUD_shop_icons_attack";
		}
		else if (Type == BattleEquipType.assist)
		{
			this.icon.spriteName = "HUD_shop_icons_help";
		}
		else if (Type == BattleEquipType.defense)
		{
			this.icon.spriteName = "HUD_shop_icons_protect";
		}
		else if (Type == BattleEquipType.magic)
		{
			this.icon.spriteName = "HUD_shop_icons_magic";
		}
		else if (Type == BattleEquipType.recommend)
		{
			this.icon.spriteName = "HUD_shop_icons_recommend";
		}
		this.icon.MakePixelPerfect();
	}
}
