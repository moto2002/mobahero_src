using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_Coupon : ModelBase<List<DiscountCardData>>
	{
		public Model_Coupon()
		{
			base.Init(EModelType.Model_Coupon);
			base.Data = new List<DiscountCardData>();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetDiscountCard, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetDiscountCard, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>打折卡获取失败" : "===>打折卡获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					List<DiscountCardData> data = null;
					byte[] array = operationResponse.Parameters[127] as byte[];
					if (array != null)
					{
						data = SerializeHelper.Deserialize<List<DiscountCardData>>(array);
					}
					base.Data = data;
					base.DebugMessage = "===>OK " + operationResponse.OperationCode;
				}
				else
				{
					base.DebugMessage = "===>GetCoupon" + operationResponse.OperationCode;
				}
				base.Valid = (base.LastError == 0);
			}
		}
	}
}
