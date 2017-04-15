using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using System;

namespace GameLogin.State
{
	internal class LoginState_selectServer : LoginStateBase
	{
		private bool bFinish;

		private object[] msgs = new object[]
		{
			ClientV2C.AreaViewNew_enterGame,
			ClientV2C.AreaViewNew_goback
		};

		public LoginState_selectServer() : base(LoginStateCode.LoginState_selectServer, MobaPeerType.C2Master)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			LoginStateManager.Instance.BLogin = true;
			this.GetAllGameServers();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MVC_MessageManager.AddListener_view(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnGetAllGameServers));
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.GetAllGameServers, new MobaMessageFunc(this.OnGetAllGameServers));
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void GetAllGameServers()
		{
			if (NetWorkHelper.Instance.MasterServerFlag)
			{
				CtrlManager.CloseWindow(WindowID.NewWaitingView);
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "获取服务器列表...", false, 3f);
				SendMsgManager.Instance.SendMsg(MobaMasterCode.GetAllGameServers, param, new object[0]);
			}
		}

		private void OnGetAllGameServers(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("更新服务器列表失败:系统错误", 1f);
				this.GetAllGameServers();
			}
			else
			{
				short returnCode = operationResponse.ReturnCode;
				if (returnCode != 0)
				{
					Singleton<TipView>.Instance.ShowViewSetText("更新服务器列表失败:系统错误", 1f);
					this.GetAllGameServers();
				}
				else
				{
					CtrlManager.CloseWindow(WindowID.LoginView_New);
					CtrlManager.OpenWindow(WindowID.AreaView_New, null);
					this.bFinish = true;
				}
			}
		}

		private void OnMsg_AreaViewNew_enterGame(MobaMessage msg)
		{
			CtrlManager.CloseWindow(WindowID.AreaView_New);
			LoginStateManager.Instance.AddTasks(new LoginTaskBase[]
			{
				new LoginTask_loadView(),
				new ILoginTask_downLoadBindata(),
				new LoginTask_checkDowload(),
				new LoginTask_initData()
			});
		}

		private void OnMsg_AreaViewNew_goback(MobaMessage msg)
		{
			Singleton<LoginView_New>.Instance.isCancelAccount = true;
			CtrlManager.CloseWindow(WindowID.AreaView_New);
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
		}

		protected override void OnConnected_master(MobaMessage msg)
		{
			if (!this.bFinish)
			{
				this.GetAllGameServers();
			}
		}

		protected override void OnDisconnected_master(MobaMessage msg)
		{
		}
	}
}
