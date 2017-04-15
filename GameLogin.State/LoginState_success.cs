using MobaClient;
using System;

namespace GameLogin.State
{
	internal class LoginState_success : LoginStateBase
	{
		public LoginState_success() : base(LoginStateCode.LoginState_success, MobaPeerType.C2GateServer)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.ChangeState();
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

		private void ChangeState()
		{
			MobaMessageManagerTools.VedioPlay_Vedio_creatPlayer(2, false);
			this.NormalLogin();
		}

		private void NormalLogin()
		{
			SceneManager.Instance.ChangeScene(SceneType.Home, true);
		}
	}
}
