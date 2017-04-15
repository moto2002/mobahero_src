using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_getMagicBottleRankList : ModelBase<MagicBottleRankList>
	{
		public Model_getMagicBottleRankList()
		{
			base.Init(EModelType.Model_getMagicBottleRankList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetMagicBottleRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetMagicBottleRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>小魔瓶信息获取失败" : "===>小魔瓶信息取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetMagicBottleRankList)
					{
						this.GetMagicBottleRankList(operationResponse);
					}
				}
			}
			base.TriggerListners();
		}

		private void GetMagicBottleRankList(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MagicBottleRankList data = base.Data as MagicBottleRankList;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[238] as byte[];
				data = SerializeHelper.Deserialize<MagicBottleRankList>(buffer);
				base.Data = data;
			}
		}
	}
}
