using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_getPlayerInfo : ModelBase<PlayerData>
	{
		public Model_getPlayerInfo()
		{
			base.Init(EModelType.Model_getPlayerInfo);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>排行榜个人信息获取失败" : "===>排行榜个人信息获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaGameCode;
					MobaGameCode mobaGameCode2 = mobaGameCode;
					if (mobaGameCode2 == MobaGameCode.GetPlayerData)
					{
						this.GetPlayerInfo(operationResponse);
					}
				}
			}
			base.TriggerListners();
		}

		private void GetPlayerInfo(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			PlayerData data = base.Data as PlayerData;
			base.Valid = true;
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				byte[] buffer = operationResponse.Parameters[86] as byte[];
				data = SerializeHelper.Deserialize<PlayerData>(buffer);
				base.Data = data;
			}
		}
	}
}
