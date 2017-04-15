using System;
using UnityEngine;

public class BattleEquipmentShopModel : MonoBehaviour
{
	[SerializeField]
	private ShopModelType _shopModelType = ShopModelType.LM;

	public ShopModelType ShopModelType
	{
		get
		{
			return this._shopModelType;
		}
	}
}
