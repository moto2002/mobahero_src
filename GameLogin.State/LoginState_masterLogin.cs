using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace GameLogin.State
{
	public class LoginState_masterLogin : LoginStateBase
	{
		private bool bFinish;

		public LoginState_masterLogin() : base(LoginStateCode.LoginState_masterLogin, MobaPeerType.C2Master)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.Login();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MVC_MessageManager.AddListener_view(MobaMasterCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.AddListener_view(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.AddListener_view(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnLogin));
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnLogin));
		}

		private void Login()
		{
			AccountData accountData = LoginStateManager.Instance.TempAccData;
			if (accountData == null)
			{
				accountData = new AccountData();
			}
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在登录服务器...", false, 15f);
			bool flag = SendMsgManager.Instance.SendMsg(MobaMasterCode.Login, param, new object[]
			{
				accountData
			});
		}

		private void OnLogin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("登陆失败...系统错误", 1f);
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
			}
			else
			{
				int num = (int)operationResponse.Parameters[1];
				string debugMessage = operationResponse.DebugMessage;
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.InvalidOperation)
				{
					if (mobaErrorCode != MobaErrorCode.InvalidParameter)
					{
						if (mobaErrorCode != MobaErrorCode.UserExist)
						{
							if (mobaErrorCode != MobaErrorCode.UserNotExist)
							{
								if (mobaErrorCode != MobaErrorCode.Ok)
								{
									if (mobaErrorCode != MobaErrorCode.SystemError)
									{
										Singleton<TipView>.Instance.ShowViewSetText("登陆失败..不明原因，请重试", 1f);
										LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
									}
									else
									{
										Singleton<TipView>.Instance.ShowViewSetText("传送门被调皮的小精灵搞坏了，正在抢修中", 1f);
										LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
									}
								}
								else
								{
									AccountData accountData = ModelManager.Instance.Get_accountData_X();
									if (accountData != null)
									{
										AnalyticsToolManager.SetAccountId(accountData.AccountId);
									}
									Singleton<TipView>.Instance.ShowViewSetText((!(LanguageManager.Instance.GetStringById("ChooseServerUI_LoginSuccess") != string.Empty)) ? "登录成功" : LanguageManager.Instance.GetStringById("ChooseServerUI_LoginSuccess"), 1f);
									this.bFinish = true;
									LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_selectServer);
								}
							}
							else
							{
								CtrlManager.ShowMsgBox((!(LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError") != string.Empty)) ? "登录错误" : LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError"), "该账号不存在，请确认后重新输入", null, PopViewType.PopOneButton, "确定", "取消", null);
								LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
							}
						}
						else
						{
							Singleton<TipView>.Instance.ShowViewSetText("登陆失败...玩家已经存在，重复登录", 1f);
							LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
						}
					}
					else
					{
						Singleton<TipView>.Instance.ShowViewSetText("登陆失败...参数错误", 1f);
						LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
					}
				}
				else
				{
					CtrlManager.ShowMsgBox("登录错误", "账号或密码错误，请确认后重新输入", null, PopViewType.PopOneButton, "确定", "取消", null);
					LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
				}
			}
		}

		protected override void OnConnected_master(MobaMessage msg)
		{
			if (!this.bFinish)
			{
				this.Login();
			}
		}

		protected override void OnDisconnected_master(MobaMessage msg)
		{
		}
	}
}
