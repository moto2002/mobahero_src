using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	internal class ILoginTask_downLoadBindata : LoginTaskBase
	{
		private bool bFinish;

		public ILoginTask_downLoadBindata() : base(ELoginTask.eDownLoadBindata, new object[]
		{
			ClientC2C.DownLoadBinDataOver,
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eShowLoadView,
				ELoginAction.eCheckDownload
			}, new Action(this.CheckAndDownBinData));
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void CheckAndDownBinData()
		{
			if (!GlobalSettings.useLocalData)
			{
				ClientData clientData = ModelManager.Instance.Get_ClientData_X();
				ResourceManager.CheckAndDownloadBinData(clientData.AppUpgradeUrl, null);
			}
			else
			{
				this.bFinish = true;
				this.DownloadFinish();
			}
		}

		private void DownloadFinish()
		{
			base.DoAction(ELoginAction.eDownLoadBindataFinish);
			if (!GlobalSettings.useBundle)
			{
				base.DoAction(ELoginAction.eDownLoadResourceFinish);
				base.Valid = true;
			}
			else
			{
				base.Valid = false;
			}
		}

		private void OnMsg_DownLoadBinDataOver(MobaMessage msg)
		{
			if ((int)msg.Param == 0)
			{
				this.bFinish = true;
				this.DownloadFinish();
			}
			else
			{
				string str = ModelManager.Instance.Get_appVersionProperty();
				string content = "配置下载失败，请确认重试" + str + "，是否重试";
				CtrlManager.ShowMsgBox("糟糕", content, new Action(this.CheckAndDownBinData), PopViewType.PopOneButton, "确定", "取消", null);
			}
		}
	}
}
