using Com.Game.Module;
using System;

namespace GameLogin.State
{
	public class LoginState_downLoadAPK : LoginStateBase
	{
		public LoginState_downLoadAPK() : base(LoginStateCode.LoginState_downLoadAPK)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			CtrlManager.OpenWindow(WindowID.DownLoadApkView, null);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
		}

		public override void OnUpdate()
		{
		}
	}
}
