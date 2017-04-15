using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	internal class ILoginTask_checkVersion : LoginTaskBase
	{
		private bool bFinish;

		private bool bDoCheck;

		private int tryTime;

		public ILoginTask_checkVersion() : base(ELoginTask.eCheckVersion, new object[]
		{
			ClientNet.Connected_master,
			ClientNet.Disconnected_master,
			MobaMasterCode.UpgradeUrl,
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eConnectMaster
			}, new Action(this.CheckVersion));
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eCheckVersion
			}, new Action(this.ChangeState));
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void Register()
		{
			MVC_MessageManager.AddListener_view(MobaMasterCode.UpgradeUrl, new MobaMessageFunc(this.OnMsg_UpgradeUrl));
		}

		private void Unregister()
		{
			MVC_MessageManager.RemoveListener_view(MobaMasterCode.UpgradeUrl, new MobaMessageFunc(this.OnMsg_UpgradeUrl));
		}

		private void CheckVersion()
		{
			this.bDoCheck = true;
			if (GlobalSettings.CheckVersion)
			{
				LoginStateManager.LoginLog("检查版本...");
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获取版本信息...", false, 15f);
				bool flag = SendMsgManager.Instance.SendMsg(MobaMasterCode.UpgradeUrl, param, new object[0]);
			}
			else
			{
				this.OnCheckVersionFinish();
			}
		}

		private void ChangeState()
		{
			if (!ModelManager.Instance.Get_needDownloadAPK() || !GlobalSettings.CheckVersion)
			{
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_waitLogin);
			}
			else
			{
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_downLoadAPK);
			}
			base.Valid = false;
		}

		private void OnCheckVersionFinish()
		{
			LoginStateManager.LoginLog("检查版本完成");
			base.DoAction(ELoginAction.eCheckVersion);
		}

		private void OnMsg_UpgradeUrl(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				ClientData clientData = ModelManager.Instance.Get_ClientData_X();
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode != MobaErrorCode.SystemError)
					{
						string content = "版本验证：获取版本信息失败";
						CtrlManager.ShowMsgBox("版本验证", content, delegate
						{
							this.tryTime = 0;
							this.CheckVersion();
						}, PopViewType.PopOneButton, "确定", "取消", null);
					}
					else
					{
						this.tryTime++;
						ModelManager.Instance.Set_masterForceIp("ios.mobaapp.xiaomeng.cc");
						if (this.tryTime > 4)
						{
							string content2 = "服务器或者网络出现问题，请重试";
							CtrlManager.ShowMsgBox("版本验证", content2, delegate
							{
								this.tryTime = 0;
								NetWorkHelper.Instance.DisconnectFromMasterServer();
								NetWorkHelper.Instance.ConnectToMasterServer();
							}, PopViewType.PopOneButton, "确定", "取消", null);
						}
						else
						{
							NetWorkHelper.Instance.DisconnectFromMasterServer();
							NetWorkHelper.Instance.ConnectToMasterServer();
						}
					}
				}
				else
				{
					this.bFinish = true;
					this.tryTime = 0;
					this.OnCheckVersionFinish();
				}
			}
		}

		private void OnMsg_Connected_master(MobaMessage msg)
		{
			if (!this.bFinish && this.bDoCheck)
			{
				this.CheckVersion();
			}
		}

		private void OnMsg_Disconnected_master(MobaMessage msg)
		{
		}
	}
}
