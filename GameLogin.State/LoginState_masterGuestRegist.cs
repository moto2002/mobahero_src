using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using System;

namespace GameLogin.State
{
	internal class LoginState_masterGuestRegist : LoginStateBase
	{
		private bool bFinish;

		public LoginState_masterGuestRegist() : base(LoginStateCode.LoginState_masterGuestRegist, MobaPeerType.C2Master)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.GuestRegist();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MVC_MessageManager.AddListener_view(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.On_server_GuestUpgrade));
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.On_server_GuestUpgrade));
		}

		private void GuestRegist()
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在登录服务器...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaMasterCode.GuestUpgrade, param, new object[0]);
		}

		private void On_server_GuestUpgrade(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("注册失败：系统错误", 1f);
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
			}
			else
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode != MobaErrorCode.UserExist)
					{
						Singleton<TipView>.Instance.ShowViewSetText("游客登录失败:系统错误", 1f);
						LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
					}
					else
					{
						LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
						Singleton<TipView>.Instance.ShowViewSetText("游客登录失败:该账号已经被注册，请更换一个账号重新注册", 1f);
					}
				}
				else
				{
					LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_selectServer);
					Singleton<TipView>.Instance.ShowViewSetText("游客登录成功", 1f);
					this.bFinish = true;
				}
			}
		}

		protected override void OnConnected_master(MobaMessage msg)
		{
			if (!this.bFinish)
			{
				this.GuestRegist();
			}
		}

		protected override void OnDisconnected_master(MobaMessage msg)
		{
		}
	}
}
