using System;

namespace GameLogin.State
{
	public class LoginState_Init : LoginStateBase
	{
		private float starTime;

		private float waitTime;

		private bool haveShowVideo = true;

		public LoginState_Init() : base(LoginStateCode.LoginState_Init)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.Init();
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

		private void Init()
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2C.SceneManagerReady, null, true);
			this.BeginLogin();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}

		private void BeginLogin()
		{
			LoginStateManager.Instance.AddTasks(new LoginTaskBase[]
			{
				new LoginTask_playLogoMovie(),
				new LoginTask_ConnectMaster(),
				new LoginTask_playVedio(),
				new ILoginTask_checkVersion(),
				new LoginTask_uiOpenTimer()
			});
		}
	}
}
