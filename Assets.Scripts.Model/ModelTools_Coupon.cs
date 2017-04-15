using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_Coupon
	{
		private static List<DiscountCardData> GetCouponList(this ModelManager mmng)
		{
			List<DiscountCardData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_Coupon))
			{
				result = mmng.GetData<List<DiscountCardData>>(EModelType.Model_Coupon);
			}
			return result;
		}

		public static List<DiscountCardData> Get_CouponList(this ModelManager mmng)
		{
			List<DiscountCardData> list = mmng.GetCouponList();
			if (list == null)
			{
				list = mmng.GetData<List<DiscountCardData>>(EModelType.Model_Coupon);
			}
			return list;
		}

		public static List<DiscountCardData> Get_GivenItemCouponList(this ModelManager mmng, string _id)
		{
			List<DiscountCardData> list = mmng.Get_CouponList();
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<DiscountCardData> list2 = new List<DiscountCardData>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].typemodelid == _id && !ToolsFacade.Instance.IsPastTime(list[i].endtime))
				{
					list2.Add(list[i]);
				}
			}
			return list2;
		}

		public static bool IsCouponRepeated(this ModelManager mmng, int _modelId)
		{
			ModelTools_Coupon.<IsCouponRepeated>c__AnonStorey2B1 <IsCouponRepeated>c__AnonStorey2B = new ModelTools_Coupon.<IsCouponRepeated>c__AnonStorey2B1();
			<IsCouponRepeated>c__AnonStorey2B._modelId = _modelId;
			SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(<IsCouponRepeated>c__AnonStorey2B._modelId.ToString());
			if (dataById == null)
			{
				return false;
			}
			if (dataById.mother_type == 1)
			{
				if (CharacterDataMgr.instance.OwenHeros.Contains(dataById.mother_id))
				{
					return true;
				}
			}
			else if (dataById.mother_type == 2 && mmng.IsPossessSkin(int.Parse(dataById.mother_id)))
			{
				return true;
			}
			List<DiscountCardData> list = mmng.Get_GivenItemCouponList(dataById.mother_id);
			if (list == null || list.Count == 0)
			{
				return false;
			}
			if (list.Find((DiscountCardData obj) => obj.modelid == <IsCouponRepeated>c__AnonStorey2B._modelId) != null)
			{
				return true;
			}
			<IsCouponRepeated>c__AnonStorey2B._triggerCouponList = dataById.other_coupon_convert.Split(new char[]
			{
				','
			});
			int i;
			for (i = 0; i < <IsCouponRepeated>c__AnonStorey2B._triggerCouponList.Length; i++)
			{
				if (list.Find((DiscountCardData obj) => obj.modelid.ToString() == <IsCouponRepeated>c__AnonStorey2B._triggerCouponList[i]) != null)
				{
					return true;
				}
			}
			return false;
		}

		public static void GetNewCoupon(this ModelManager mmng, string _modelId)
		{
			List<DiscountCardData> list = mmng.Get_CouponList();
			if (list == null)
			{
				list = new List<DiscountCardData>();
			}
			SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(_modelId);
			if (dataById == null)
			{
				return;
			}
			List<DiscountCardData> list2 = mmng.Get_GivenItemCouponList(dataById.mother_id);
			if (list2 == null || list2.Count == 0 || list2.Find((DiscountCardData obj) => obj.modelid.ToString() == _modelId) == null)
			{
				list.Add(new DiscountCardData
				{
					modelid = Convert.ToInt32(_modelId),
					endtime = ToolsFacade.ServerCurrentTime + new TimeSpan(7, 0, 0, 0),
					typemodelid = dataById.mother_id
				});
			}
		}
	}
}
