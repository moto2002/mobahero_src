using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PriceTagController : MonoBehaviour
{
	[SerializeField]
	private UICenterHelper _singleWidget;

	[SerializeField]
	private UISprite _single_sprite;

	[SerializeField]
	private UILabel _single_label;

	[SerializeField]
	private UICenterHelper _coupleWidget;

	[SerializeField]
	private UISprite _couple_1_sprite;

	[SerializeField]
	private UILabel _couple_1_label;

	[SerializeField]
	private UISprite _couple_2_sprite;

	[SerializeField]
	private UILabel _couple_2_label;

	[SerializeField]
	private UICenterHelper _discountWidget;

	[SerializeField]
	private UISprite _discount_sprite;

	[SerializeField]
	private UILabel _discount_labelRaw;

	[SerializeField]
	private UILabel _discount_label;

	[SerializeField]
	private GameObject _unsell_tipLabel;

	private string COIN_SPRITENAME = "Store_icons_gold";

	private string DIAMOND_SPRITENAME = "Store_icons_diamond";

	private string CAP_SPRITENAME = "Store_icons_smallcap";

	private string RMB_SPRITENAME = " ";

	private string YELLOW_FONTCOLOR = "[FDC847]";

	private void SetMoneyIcon(byte _moneyCode, UISprite _comp)
	{
		if (_moneyCode != 1)
		{
			if (_moneyCode != 2)
			{
				if (_moneyCode != 9)
				{
					if (_moneyCode == 10)
					{
						_comp.spriteName = this.RMB_SPRITENAME;
						_comp.SetDimensions(1, 1);
						return;
					}
				}
				else
				{
					_comp.spriteName = this.CAP_SPRITENAME;
				}
			}
			else
			{
				_comp.spriteName = this.DIAMOND_SPRITENAME;
			}
		}
		else
		{
			_comp.spriteName = this.COIN_SPRITENAME;
		}
		_comp.MakePixelPerfect();
	}

	private void Activate(GameObject targetObj)
	{
		if (this._unsell_tipLabel != null)
		{
			if (targetObj == null)
			{
				this._unsell_tipLabel.SetActive(true);
			}
			else
			{
				this._unsell_tipLabel.SetActive(false);
			}
		}
		this._singleWidget.gameObject.SetActive(targetObj == this._singleWidget.gameObject);
		this._coupleWidget.gameObject.SetActive(targetObj == this._coupleWidget.gameObject);
		this._discountWidget.gameObject.SetActive(targetObj == this._discountWidget.gameObject);
	}

	public Dictionary<byte, float[]> SetData(GoodsSubject _type, string _itemId, int _itemNum, Callback _repeatCall = null, Callback<float> _discountCall = null, Callback<byte, int, DiscountCardData> _couponCall = null)
	{
		GoodsData goodsData = ModelManager.Instance.Get_ShopGoodsList().Find((GoodsData obj) => obj.Type == (int)_type && obj.ElementId == _itemId && obj.Count == _itemNum);
		if (goodsData == null)
		{
			ClientLogger.Error("Can't find shop goods:" + _itemId);
			this.Activate(null);
			return new Dictionary<byte, float[]>
			{
				{
					0,
					new float[1]
				}
			};
		}
		return this.SetData(goodsData.Id, _repeatCall, _discountCall, _couponCall);
	}

	public Dictionary<byte, float[]> SetData(int _id, Callback _repeatCall = null, Callback<float> _discountCall = null, Callback<byte, int, DiscountCardData> _couponCall = null)
	{
		Dictionary<byte, float[]> dictionary = ModelManager.Instance.Get_ShopGoodsPrice(_id, _repeatCall, _discountCall, _couponCall);
		if (dictionary.ContainsKey(0))
		{
			ClientLogger.Error("Can't find shop goods:" + _id.ToString());
			this.Activate(null);
			return dictionary;
		}
		bool flag = dictionary.ContainsKey(1);
		bool flag2 = dictionary.ContainsKey(2);
		bool flag3 = dictionary.ContainsKey(9);
		bool flag4 = dictionary.ContainsKey(10);
		byte b = (!flag2) ? ((!flag3) ? ((!flag) ? ((!flag4) ? 0 : 10) : 1) : 9) : 2;
		bool flag5 = dictionary.ContainsKey(11) || dictionary.ContainsKey(21);
		bool flag6 = dictionary.ContainsKey(12) || dictionary.ContainsKey(22);
		bool flag7 = dictionary.ContainsKey(19) || dictionary.ContainsKey(29);
		bool flag8 = dictionary.ContainsKey(20) || dictionary.ContainsKey(30);
		byte b2 = (!flag6) ? ((!flag7) ? ((!flag5) ? ((!flag8) ? 0 : 10) : 1) : 9) : 2;
		if (b2 != 0)
		{
			this.Activate(this._discountWidget.gameObject);
			this.SetMoneyIcon(b2, this._discount_sprite);
			if (b2 == 10)
			{
				this._discount_labelRaw.text = "原价 " + dictionary[b2][0].ToString() + " 元";
				this._discount_label.text = this.YELLOW_FONTCOLOR + dictionary[b2][1].ToString() + "[-] 元";
			}
			else
			{
				this._discount_labelRaw.text = "原价 " + dictionary[b2][0].ToString();
				this._discount_label.text = this.YELLOW_FONTCOLOR + dictionary[b2][1].ToString() + "[-]";
			}
			this._discountWidget.Reposition();
			return dictionary;
		}
		if (dictionary.ContainsKey(100) && dictionary[100][0] > 1f)
		{
			this.Activate(this._coupleWidget.gameObject);
			this.SetMoneyIcon(1, this._couple_1_sprite);
			this._couple_1_label.text = dictionary[1][0].ToString();
			this.SetMoneyIcon(2, this._couple_2_sprite);
			this._couple_2_label.text = dictionary[2][0].ToString();
			this._coupleWidget.Reposition();
			return dictionary;
		}
		this.Activate(this._singleWidget.gameObject);
		this.SetMoneyIcon(b, this._single_sprite);
		if (b == 10)
		{
			this._single_label.text = this.YELLOW_FONTCOLOR + dictionary[b][0].ToString() + "[-] 元";
		}
		else
		{
			this._single_label.text = this.YELLOW_FONTCOLOR + dictionary[b][0].ToString() + "[-]";
		}
		this._singleWidget.Reposition();
		return dictionary;
	}
}
