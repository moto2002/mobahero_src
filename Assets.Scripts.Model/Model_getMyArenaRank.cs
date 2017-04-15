using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_getMyArenaRank : ModelBase<ArenaData>
	{
		public Model_getMyArenaRank()
		{
			base.Init(EModelType.Model_getMyArenaRank);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetMyArenaRank, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetMyArenaRank, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>我的排行榜信息获取失败" : "===>我的排行榜信息取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetMyArenaRank)
					{
						this.GetMyArenaRank(operationResponse);
					}
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
			base.TriggerListners();
		}

		private void GetMyArenaRank(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			ArenaData data = base.Data as ArenaData;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[205] as byte[];
				ArenaData arenaData = SerializeHelper.Deserialize<ArenaData>(buffer);
				data = arenaData;
				base.Data = data;
			}
			base.Valid = (base.LastError == 0);
		}
	}
}
