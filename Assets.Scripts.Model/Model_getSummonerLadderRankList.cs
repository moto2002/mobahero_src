using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_getSummonerLadderRankList : ModelBase<SummonerLadderData>
	{
		public Model_getSummonerLadderRankList()
		{
			base.Init(EModelType.Model_getSummonerLadderRankList);
			base.Data = new SummonerLadderData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetSummonerLadderRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetSummonerLadderRankList, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>召唤师天梯排行榜信息获取失败" : "===>召唤师天梯排行榜信息取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetSummonerLadderRankList)
					{
						this.GetSummonerLadderRankList(operationResponse);
					}
				}
			}
			base.TriggerListners();
		}

		private void GetSummonerLadderRankList(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			SummonerLadderData summonerLadderData = base.Data as SummonerLadderData;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[210] as byte[];
				byte[] buffer2 = operationResponse.Parameters[128] as byte[];
				summonerLadderData.rankList = SerializeHelper.Deserialize<List<SummonerLadderRankData>>(buffer);
				summonerLadderData.matchTimeData = SerializeHelper.Deserialize<MatchTimeData>(buffer2);
				base.Data = summonerLadderData;
			}
		}
	}
}
