using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	public class LoginTask_ConnectMaster : LoginTaskBase
	{
		public LoginTask_ConnectMaster() : base(ELoginTask.eConnectMaster, new object[]
		{
			ClientNet.Connected_master,
			ClientNet.Disconnected_master,
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eVedio2Start
			}, new Action(this.ConnectMaster));
		}

		private void ConnectMaster()
		{
			LoginStateManager.LoginLog("尝试连接服务器");
			NetWorkHelper.Instance.ConnectToMasterServer();
		}

		private void OnMsg_Connected_master(MobaMessage msg)
		{
			LoginStateManager.LoginLog("连接到服务器了");
			base.DoAction(ELoginAction.eConnectMaster);
			base.Valid = false;
		}

		private void OnMsg_Disconnected_master(MobaMessage msg)
		{
		}
	}
}
