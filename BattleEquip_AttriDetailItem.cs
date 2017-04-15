using Assets.Scripts.GUILogic.View.BattleEquipment;
using MobaHeros;
using System;
using UnityEngine;

public class BattleEquip_AttriDetailItem : MonoBehaviour
{
	[SerializeField]
	private UISprite spri_icon;

	[SerializeField]
	private UILabel label_value;

	private AttrType attriType;

	private HeroDetailedAttr detailAttr;

	public AttrType AttriTypeP
	{
		get
		{
			return this.attriType;
		}
		set
		{
			this.attriType = value;
			this.RefreshUI_icon();
			this.RefreshUI_value();
		}
	}

	public HeroDetailedAttr AttriData
	{
		get
		{
			return this.detailAttr;
		}
		set
		{
			this.detailAttr = value;
			this.RefreshUI_value();
		}
	}

	private void RefreshUI_icon()
	{
	}

	private void RefreshUI_value()
	{
		if (this.attriType != AttrType.None && this.detailAttr != null)
		{
			this.label_value.text = BattleEquipTools_config.GetAttriDetailValueStr(this.detailAttr, this.attriType);
		}
		else
		{
			this.label_value.text = string.Empty;
		}
	}
}
