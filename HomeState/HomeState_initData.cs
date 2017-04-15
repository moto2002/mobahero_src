using System;

namespace HomeState
{
	internal class HomeState_initData : HomeStateBase
	{
		private bool bFinished;

		public HomeState_initData() : base(HomeStateCode.HomeState_initData)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.LoadData();
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

		public override void OnUpdate(long delta)
		{
			if (this.bFinished)
			{
				HomeManager.Instance.ChangeState(HomeStateCode.HomeState_requestData);
			}
		}

		private void LoadData()
		{
			if (GlobalSettings.useLocalData)
			{
				ResourceManager.AutoConfig();
			}
			else
			{
				ResourceManager.InitData(string.Empty, null);
			}
			this.bFinished = true;
		}
	}
}
