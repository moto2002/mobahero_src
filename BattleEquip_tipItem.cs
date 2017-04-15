using Assets.Scripts.Model;
using System;
using UnityEngine;

public class BattleEquip_tipItem : MonoBehaviour
{
	[SerializeField]
	private UISprite sp_icon;

	[SerializeField]
	private UILabel lb_name;

	[SerializeField]
	private UILabel lb_price;

	private IBEItem item;

	public IBEItem Item
	{
		get
		{
			return this.item;
		}
		set
		{
			if (this.item != value)
			{
				this.item = value;
				this.RefreshUI_item();
			}
		}
	}

	private void Awake()
	{
		this.item = null;
	}

	private void RefreshUI_item()
	{
		if (this.item != null)
		{
			this.lb_name.text = LanguageManager.Instance.GetStringById(this.item.Config.name);
			this.sp_icon.spriteName = this.item.Config.icon;
			this.lb_name.color = new Color32(255, 255, 255, 255);
			base.name = this.item.Config.items_id;
			this.lb_price.text = this.item.RealPrice.ToString();
		}
	}
}
