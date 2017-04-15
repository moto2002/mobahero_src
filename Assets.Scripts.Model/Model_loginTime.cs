using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_loginTime : ModelBase<LoginTime>
	{
		public Model_loginTime()
		{
			base.Init(EModelType.Model_loginTime);
			base.Data = new LoginTime();
			base.Valid = false;
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.Login, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ClientReportOnlineTime, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.RegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnGameStart));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.Login, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ClientReportOnlineTime, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.UnRegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnGameStart));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
			MobaGameCode mobaGameCode2 = mobaGameCode;
			if (mobaGameCode2 != MobaGameCode.Login)
			{
				if (mobaGameCode2 == MobaGameCode.ClientReportOnlineTime)
				{
					this.OnCorrect(msg);
				}
			}
			else
			{
				this.OnLogin(msg);
			}
			base.TriggerListners();
		}

		private void OnLogin(MobaMessage msg)
		{
			LoginTime loginTime = base.Data as LoginTime;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					loginTime.IsCorrected = false;
					byte[] buffer = operationResponse.Parameters[86] as byte[];
					UserData userData = SerializeHelper.Deserialize<UserData>(buffer);
					if (operationResponse.Parameters.ContainsKey(230))
					{
						List<string> freeHeros = SerializeHelper.Deserialize<List<string>>(operationResponse.Parameters[230] as byte[]);
						Singleton<PvpManager>.Instance.freeHeros = freeHeros;
					}
					if (userData != null)
					{
						loginTime.loginTime_Local = DateTime.Now;
						DateTime dateTime = DateTime.Parse(userData.ServerTime);
						loginTime.loginTime_Server = dateTime;
						loginTime.TimeSpan_login = dateTime - DateTime.Now;
						base.Valid = true;
					}
				}
			}
		}

		private void OnCorrect(MobaMessage msg)
		{
			LoginTime loginTime = base.Data as LoginTime;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 302)
				{
					DateTime serverTimeCorrected = new DateTime((long)operationResponse.Parameters[11]);
					loginTime.LocalTimeCorrected = DateTime.Now;
					loginTime.ServerTimeCorrected = serverTimeCorrected;
					loginTime.IsCorrected = true;
					base.Valid = true;
				}
			}
		}

		private void OnGameStart(MobaMessage msg)
		{
			LoginTime loginTime = base.Data as LoginTime;
			loginTime.TimeEnterBattle = ToolsFacade.ServerCurrentTime;
		}
	}
}
