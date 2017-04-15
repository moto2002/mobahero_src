using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_getSummSkinList : ModelBase<List<SummSkinData>>
	{
		public Model_getSummSkinList()
		{
			base.Init(EModelType.Model_getSummSkinList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetSummSkinList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.SignDay, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetSummSkinList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SignDay, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
				MobaGameCode mobaGameCode2 = mobaGameCode;
				if (mobaGameCode2 != MobaGameCode.BuyShopGoodsNew)
				{
					if (mobaGameCode2 != MobaGameCode.SignDay)
					{
						if (mobaGameCode2 == MobaGameCode.GetSummSkinList)
						{
							this.OnGetMsg_GetSummSkinList(operationResponse);
						}
					}
					else
					{
						this.OnGetMsg_SignDay(operationResponse);
					}
				}
				else
				{
					this.OnGetMsg_BuyShopGoodsNew(operationResponse);
				}
			}
		}

		private void OnGetMsg_GetSummSkinList(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[209] as byte[];
				List<SummSkinData> collection = SerializeHelper.Deserialize<List<SummSkinData>>(buffer);
				List<SummSkinData> list = base.Data as List<SummSkinData>;
				if (list == null)
				{
					list = new List<SummSkinData>();
					base.Data = list;
				}
				list.Clear();
				list.AddRange(collection);
				base.DebugMessage = "====>GetSummSkinList:操作成功";
			}
			else
			{
				base.DebugMessage = "====>GetSummSkinList" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
			base.TriggerListners();
		}

		private void OnGetMsg_BuyShopGoodsNew(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[242];
				if (num == 2)
				{
					byte[] buffer = res.Parameters[209] as byte[];
					SummSkinData item = SerializeHelper.Deserialize<SummSkinData>(buffer);
					List<SummSkinData> list = base.Data as List<SummSkinData>;
					if (list == null)
					{
						list = new List<SummSkinData>();
						base.Data = list;
					}
					list.Add(item);
					base.DebugMessage = "====>BuyShopGoodsNew:操作成功";
				}
				else if (num == 2)
				{
					base.DebugMessage = "====>BuyShopGoodsNew" + res.OperationCode;
				}
			}
			base.TriggerListners();
		}

		private void OnGetMsg_SignDay(OperationResponse res)
		{
			if (res == null)
			{
				return;
			}
			if ((int)res.Parameters[1] == 0)
			{
				object obj3 = null;
				res.Parameters.TryGetValue(246, out obj3);
				object obj2 = null;
				res.Parameters.TryGetValue(146, out obj2);
				List<DropItemData> list = SerializeHelper.Deserialize<List<DropItemData>>((byte[])obj3);
				List<DropItemData> list2 = SerializeHelper.Deserialize<List<DropItemData>>((byte[])obj2);
				DropItemData dropItemData = null;
				if (list != null && list.Count > 0)
				{
					dropItemData = list.Find((DropItemData obj) => obj.itemType == 3 && obj.itemId == 2);
				}
				if (dropItemData != null)
				{
					ModelManager.Instance.GetNewHeroSkin(dropItemData.itemCount);
				}
			}
		}
	}
}
