using Assets.Scripts.GUILogic.View.BattleEquipment;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEquip_AttriItem : MonoBehaviour
{
	[SerializeField]
	private UISprite sprit_icon;

	[SerializeField]
	private UILabel label_value;

	private AttrType attriType;

	private HeroDetailedAttr detailAttr;

	private float attrValue;

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
			this.RefreshUI_value2();
		}
	}

	public float AttriValue
	{
		get
		{
			return this.attrValue;
		}
		set
		{
			this.attrValue = value;
			this.RefreshUI_value2();
		}
	}

	private void Awake()
	{
	}

	private void RefreshUI_icon()
	{
		if (this.AttriTypeP != AttrType.None)
		{
			Dictionary<AttrType, BattleEquipTools_config.AttriShowInfo> dicAttriShowInfo = BattleEquipTools_config.dicAttriShowInfo;
			if (dicAttriShowInfo.ContainsKey(this.AttriTypeP))
			{
				this.sprit_icon.spriteName = dicAttriShowInfo[this.AttriTypeP]._iconName;
				this.sprit_icon.gameObject.SetActive(true);
			}
		}
		else
		{
			this.sprit_icon.gameObject.SetActive(false);
		}
	}

	private void RefreshUI_value()
	{
		if (this.attriType != AttrType.None && this.detailAttr != null)
		{
			this.label_value.text = BattleEquipTools_config.GetAttriValueStr(this.detailAttr, this.attriType);
		}
		else
		{
			this.label_value.text = string.Empty;
		}
	}

	private void RefreshUI_value2()
	{
		Dictionary<AttrType, BattleEquipTools_config.AttriShowInfo> dicAttriShowInfo = BattleEquipTools_config.dicAttriShowInfo;
		if (this.attriType != AttrType.None && dicAttriShowInfo.ContainsKey(this.AttriTypeP))
		{
			if (dicAttriShowInfo.ContainsKey(this.AttriTypeP))
			{
				this.label_value.text = this.attrValue.ToString(dicAttriShowInfo[this.AttriTypeP]._format);
			}
		}
		else
		{
			this.label_value.text = "0";
		}
	}
}
