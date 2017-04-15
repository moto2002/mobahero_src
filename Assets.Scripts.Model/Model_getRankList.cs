using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_getRankList : ModelBase<MyRankList>
	{
		public Model_getRankList()
		{
			base.Init(EModelType.Model_getRankList);
			base.Data = new MyRankList();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetCharmRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetCharmRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>排行榜信息获取失败" : "===>排行榜信息取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetCharmRankList)
					{
						this.GetRankList_Charm(operationResponse);
					}
				}
			}
			base.TriggerListners();
		}

		private void GetRankList_Charm(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MyRankList myRankList = base.Data as MyRankList;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[210] as byte[];
				myRankList.CharmRankList = SerializeHelper.Deserialize<List<CharmRankData>>(buffer);
				base.Data = myRankList;
			}
		}
	}
}
