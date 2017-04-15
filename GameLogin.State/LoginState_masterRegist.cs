using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace GameLogin.State
{
	internal class LoginState_masterRegist : LoginStateBase
	{
		private bool bFinish;

		public LoginState_masterRegist() : base(LoginStateCode.LoginState_masterRegist, MobaPeerType.C2Master)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.Regist();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MVC_MessageManager.AddListener_view(MobaMasterCode.Register, new MobaMessageFunc(this.On_server_Register));
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.Register, new MobaMessageFunc(this.On_server_Register));
		}

		private void Regist()
		{
			AccountData accountData = LoginStateManager.Instance.TempAccData;
			if (accountData == null)
			{
				accountData = new AccountData();
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "注册...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaMasterCode.Register, param, new object[]
			{
				accountData
			});
		}

		private void On_server_Register(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("注册失败...系统错误", 1f);
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
						Singleton<TipView>.Instance.ShowViewSetText("注册失败...系统错误", 1f);
						LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
					}
					else
					{
						if (!AutoTestController.IsEnable(AutoTestTag.Login))
						{
							CtrlManager.ShowMsgBox("注册失败", "该账号已经被注册，请更换一个账号重新注册", null, PopViewType.PopOneButton, "确定", "取消", null);
						}
						LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
						AutoTestController.InvokeTestLogic(AutoTestTag.Login, delegate
						{
							Singleton<LoginView_New>.Instance.TestRegisterAgain();
						}, 1f);
					}
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText("注册成功", 1f);
					this.bFinish = true;
					LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_masterLogin);
				}
			}
		}

		protected override void OnConnected_master(MobaMessage msg)
		{
			if (!this.bFinish)
			{
				this.Regist();
			}
		}

		protected override void OnDisconnected_master(MobaMessage msg)
		{
		}
	}
}
