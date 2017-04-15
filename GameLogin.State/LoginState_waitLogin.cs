using Com.Game.Module;
using MobaClient;
using MobaProtocol.Data;
using System;

namespace GameLogin.State
{
	internal class LoginState_waitLogin : LoginStateBase
	{
		public LoginState_waitLogin() : base(LoginStateCode.LoginState_waitLogin, MobaPeerType.C2Master)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			CtrlManager.OpenWindow(WindowID.LoginView_New, null);
			LoginStateManager.LoginLog("等待登陆状态");
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MobaMessageManager.RegistMessage(ClientV2C.LoginViewNew_Register, new MobaMessageFunc(this.On_Register));
			MobaMessageManager.RegistMessage(ClientV2C.LoginViewNew_Login, new MobaMessageFunc(this.On_login));
			MobaMessageManager.RegistMessage(ClientV2C.LoginViewNew_GuestUpgrade, new MobaMessageFunc(this.On_guestLogin));
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MobaMessageManager.UnRegistMessage(ClientV2C.LoginViewNew_Register, new MobaMessageFunc(this.On_Register));
			MobaMessageManager.UnRegistMessage(ClientV2C.LoginViewNew_Login, new MobaMessageFunc(this.On_login));
			MobaMessageManager.UnRegistMessage(ClientV2C.LoginViewNew_GuestUpgrade, new MobaMessageFunc(this.On_guestLogin));
		}

		private void On_Register(MobaMessage msg)
		{
			LoginStateManager.Instance.TempAccData = (msg.Param as AccountData);
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_masterRegist);
		}

		private void On_login(MobaMessage msg)
		{
			LoginStateManager.Instance.TempAccData = (msg.Param as AccountData);
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_masterLogin);
		}

		private void On_guestLogin(MobaMessage msg)
		{
			LoginStateManager.Instance.TempAccData = (msg.Param as AccountData);
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_masterGuestRegist);
		}
	}
}
