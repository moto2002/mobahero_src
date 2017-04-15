using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_allShopInfoList : ModelBase<ShopDataGroup>
	{
		public Model_allShopInfoList()
		{
			base.Init(EModelType.Model_allShopInfoList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetShopNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetShopVersion, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetShopNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetShopVersion, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				switch (MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID))
				{
				case MobaGameCode.GetShopVersion:
					this.OnGetMsg_GetShopVersion(operationResponse);
					break;
				case MobaGameCode.GetShopNew:
					this.OnGetMsg_GetShopNew(operationResponse);
					break;
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_GetShopNew(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[127] as byte[];
				byte[] buffer2 = res.Parameters[126] as byte[];
				int shopVersion = (int)res.Parameters[101];
				List<ShopDataNew> shopList = SerializeHelper.Deserialize<List<ShopDataNew>>(buffer);
				List<GoodsData> goodsList = SerializeHelper.Deserialize<List<GoodsData>>(buffer2);
				ShopDataGroup shopDataGroup = base.Data as ShopDataGroup;
				if (shopDataGroup == null)
				{
					shopDataGroup = new ShopDataGroup();
					base.Data = shopDataGroup;
				}
				shopDataGroup.shopList = shopList;
				shopDataGroup.goodsList = goodsList;
				shopDataGroup.shopVersion = shopVersion;
				base.DebugMessage = "====>GetShopNew:操作成功";
			}
			else
			{
				base.DebugMessage = "====>GetShopNew" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_GetShopVersion(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				object obj = 0;
				if (res.Parameters.TryGetValue(101, out obj))
				{
					ShopDataGroup shopDataGroup = base.Data as ShopDataGroup;
					if (shopDataGroup == null)
					{
						shopDataGroup = new ShopDataGroup();
						base.Data = shopDataGroup;
					}
					ModelManager.Instance.CheckShopVersion(shopDataGroup.shopVersion, (int)obj);
					base.DebugMessage = "=====>GetShopVersion:" + obj.ToString();
				}
				else
				{
					base.DebugMessage = "====>GetShopVersion" + res.OperationCode;
				}
			}
		}
	}
}
