using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	internal class LoginTask_initData : LoginTaskBase
	{
		private const string const_title = "资源下载";

		private bool bFinish;

		public LoginTask_initData() : base(ELoginTask.eInitData, new object[]
		{
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eBeginLoad
			}, new Action(this.LoadData));
		}

		public override void Destroy()
		{
			base.Destroy();
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
			base.DoAction(ELoginAction.eInitData);
			base.Valid = false;
		}
	}
}
