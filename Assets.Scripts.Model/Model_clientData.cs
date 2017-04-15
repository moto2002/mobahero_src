using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Assets.Scripts.Model
{
	internal class Model_clientData : ModelBase<ClientModelData>
	{
		public Model_clientData()
		{
			base.Init(EModelType.Model_clientData);
			base.Data = new ClientModelData();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaMasterCode.UpgradeUrl, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.UpgradeUrl, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			base.DebugMessage = ((base.LastError != 0) ? "===>天赋的信息获取失败" : "===>天赋的信息获取成功");
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					MobaMasterCode mobaMasterCode = MVC_MessageManager.NotifyModel_to_Master((ClientMsg)msg.ID);
					base.LastMsgType = (int)mobaMasterCode;
					MobaMasterCode mobaMasterCode2 = mobaMasterCode;
					if (mobaMasterCode2 == MobaMasterCode.UpgradeUrl)
					{
						this.UpgradeUrl(operationResponse);
					}
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
			base.TriggerListners();
		}

		private void UpgradeUrl(OperationResponse operationResponse)
		{
			Log.debug(" MobaClient : 响应更新 " + operationResponse.ReturnCode);
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				string text = operationResponse.Parameters[32] as string;
				byte[] buffer = operationResponse.Parameters[84] as byte[];
				ClientData clientData = SerializeHelper.Deserialize<ClientData>(buffer);
				ClientModelData clientModelData = base.Data as ClientModelData;
				clientModelData.clientData = clientData;
				string appUpgradeUrl = clientModelData.clientData.AppUpgradeUrl;
				clientModelData.downLoadAPK = !clientModelData.clientData.AppVersion.Equals(GlobalSettings.AppVersion);
				clientModelData.appNetworkUrl_property = appUpgradeUrl;
				if (clientModelData.downLoadAPK)
				{
					ClientModelData expr_C2 = clientModelData;
					expr_C2.appNetworkUrl_property += "APK/Android.apk";
				}
			}
			base.Valid = (base.LastError == 0);
		}
	}
}
