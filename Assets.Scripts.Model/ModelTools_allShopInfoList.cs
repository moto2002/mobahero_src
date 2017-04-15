using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public static class ModelTools_allShopInfoList
	{
		public static List<ShopData> Get_allShopList_X(this ModelManager mmng)
		{
			return mmng.GetAllShopDataList();
		}

		private static List<ShopData> GetAllShopDataList(this ModelManager mmng)
		{
			List<ShopData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_allShopInfoList))
			{
				result = mmng.GetData<List<ShopData>>(EModelType.Model_allShopInfoList);
			}
			return result;
		}

		private static ShopDataGroup GetShopDataGroup(this ModelManager mmng)
		{
			ShopDataGroup result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_allShopInfoList))
			{
				result = mmng.GetData<ShopDataGroup>(EModelType.Model_allShopInfoList);
			}
			return result;
		}

		public static List<ShopDataNew> Get_ShopList(this ModelManager mmng)
		{
			ShopDataGroup shopDataGroup = mmng.GetShopDataGroup();
			if (shopDataGroup == null)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetShopNew, null, new object[0]);
				return null;
			}
			return shopDataGroup.shopList;
		}

		public static List<GoodsData> Get_ShopGoodsList(this ModelManager mmng)
		{
			ShopDataGroup shopDataGroup = mmng.GetShopDataGroup();
			if (shopDataGroup == null)
			{
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetShopNew, null, new object[0]);
				return null;
			}
			return shopDataGroup.goodsList;
		}

		public static Dictionary<byte, float[]> Get_ShopGoodsPrice(this ModelManager mmng, GoodsData _data, Callback _repeatCall = null, Callback<float> _discountCall = null, Callback<byte, int, DiscountCardData> _couponCall = null)
		{
			if (_data == null)
			{
				return null;
			}
			Dictionary<byte, float[]> dictionary = new Dictionary<byte, float[]>();
			if (_data.IsSingle > 0)
			{
				if (_data.Type == 1)
				{
					if (_repeatCall != null && CharacterDataMgr.instance.OwenHeros.Contains(_data.ElementId))
					{
						_repeatCall();
					}
				}
				else if (_data.Type == 2)
				{
					if (_repeatCall != null && ModelManager.Instance.IsPossessSkin(Convert.ToInt32(_data.ElementId)))
					{
						_repeatCall();
					}
				}
				else if (_data.Type == 5)
				{
					if (_repeatCall != null && ModelManager.Instance.IsRepeatAvatar("3", _data.ElementId))
					{
						_repeatCall();
					}
				}
				else if (_data.Type == 6 && _repeatCall != null && ModelManager.Instance.IsRepeatAvatar("4", _data.ElementId))
				{
					_repeatCall();
				}
			}
			string[] array = _data.Price.Split(new char[]
			{
				','
			});
			float num = 1f;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string[] array3 = text.Split(new char[]
				{
					'|'
				});
				if (array3 != null && array3.Length == 3)
				{
					float[] array4 = new float[2];
					array4[0] = float.Parse(array3[1]);
					float num2 = float.Parse(array3[2]);
					array4[1] = (float)Mathf.CeilToInt(num2 * array4[0]);
					byte b = byte.Parse(array3[0]);
					dictionary.Add(b, array4);
					if (num2 < 1f)
					{
						dictionary.Add(b + 10, null);
					}
					if (num2 < num || b == 2)
					{
						num = num2;
					}
				}
			}
			dictionary.Add(100, new float[]
			{
				(float)array.Length
			});
			if (_discountCall != null && num < 1f)
			{
				_discountCall(num);
			}
			if (_data.Type == 1 || _data.Type == 2)
			{
				List<DiscountCardData> list = ModelManager.Instance.Get_GivenItemCouponList(_data.ElementId);
				if (list == null || list.Count == 0)
				{
					return dictionary;
				}
				Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysCouponVo>();
				DiscountCardData discountCardData = list[0];
				SysCouponVo sysCouponVo = (SysCouponVo)dicByType[discountCardData.modelid.ToString()];
				for (int j = 1; j < list.Count; j++)
				{
					SysCouponVo sysCouponVo2 = (SysCouponVo)dicByType[list[j].modelid.ToString()];
					if (sysCouponVo2.other_coupon_convert.ToString().Length < sysCouponVo.other_coupon_convert.ToString().Length)
					{
						discountCardData = list[j];
						sysCouponVo = sysCouponVo2;
					}
				}
				if (dictionary.ContainsKey((byte)sysCouponVo.currency_type))
				{
					dictionary[(byte)sysCouponVo.currency_type][1] = (float)Mathf.CeilToInt(dictionary[(byte)sysCouponVo.currency_type][1] * (float)sysCouponVo.off_number * 0.1f);
					if (_couponCall != null)
					{
						_couponCall((byte)sysCouponVo.currency_type, sysCouponVo.off_number, discountCardData);
					}
					dictionary.Add((byte)(sysCouponVo.currency_type + 20), null);
				}
			}
			return dictionary;
		}

		public static Dictionary<byte, float[]> Get_ShopGoodsPrice(this ModelManager mmng, int _id, Callback _repeatCall = null, Callback<float> _discountCall = null, Callback<byte, int, DiscountCardData> _couponCall = null)
		{
			Dictionary<byte, float[]> dictionary = new Dictionary<byte, float[]>();
			GoodsData goodsData = mmng.Get_ShopGoodsList().Find((GoodsData obj) => obj.Id == _id);
			if (goodsData == null)
			{
				dictionary.Add(0, new float[1]);
				return dictionary;
			}
			return mmng.Get_ShopGoodsPrice(goodsData, _repeatCall, _discountCall, _couponCall);
		}

		public static void CheckShopVersion(this ModelManager mmng, int _record, int _new)
		{
			if (_new > _record)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "获取商店数据更新…", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetShopNew, param, new object[0]);
			}
		}
	}
}
