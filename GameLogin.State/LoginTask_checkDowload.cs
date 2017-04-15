using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol.Data;
using System;

namespace GameLogin.State
{
	internal class LoginTask_checkDowload : LoginTaskBase
	{
		private const string const_title = "资源下载";

		private float rSize;

		private float freeSize;

		private float bSize;

		private float threshold = 8f;

		public LoginTask_checkDowload() : base(ELoginTask.eCheckDownload, new object[]
		{
			ClientC2C.CheckResourceOver,
			ClientC2C.Login_Action
		})
		{
			this.rSize = 0f;
			this.bSize = 0f;
			this.freeSize = 0f;
			base.AddTask(null, new Action(this.CheckResource));
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void CheckResource()
		{
			if (GlobalSettings.useBundle)
			{
				this.CheckAsset();
			}
			else
			{
				this.CheckWIFI();
			}
		}

		private void CheckAsset()
		{
			ResourceManager.CheckDownLoadAssets();
		}

		private void CheckWIFI()
		{
			if (NetWorkHelper.Instance.IsAvailable() && NetWorkHelper.Instance.IsWifi())
			{
				this.CheckFinish();
			}
			else
			{
				float num = this.rSize + this.bSize;
				if (num > this.threshold)
				{
					string content = string.Format("需要更新{0}M,非WIFI环境下更新会造成付费流量，是否继续?", num.ToString("F1"));
					this.ShowMsgBox("资源下载", content, new Action(this.CheckFinish), new Action(this.Del_quit), "更新", "退出");
				}
				else
				{
					this.CheckFinish();
				}
			}
		}

		private void CheckFinish()
		{
			base.DoAction(ELoginAction.eCheckDownload);
			base.Valid = false;
		}

		private void OnMsg_CheckResourceOver(MobaMessage msg)
		{
			object[] array = msg.Param as object[];
			string content = string.Empty;
			EAssetLoadError eAssetLoadError = EAssetLoadError.eUnknowReason;
			if (array != null && array.Length == 3)
			{
				eAssetLoadError = (EAssetLoadError)((int)array[0]);
			}
			switch (eAssetLoadError)
			{
			case EAssetLoadError.eWWWError:
				content = "资源下载失败，www错误，是否重试？";
				this.ShowMsgBox("资源下载", content, new Action(this.CheckAsset), new Action(this.Del_quit), "重试", "取消");
				break;
			case EAssetLoadError.eInsufficientSpace:
				this.HandleParams(array[1]);
				content = "磁盘空间不足，是否重试？" + this.GetSpaceNotice();
				this.ShowMsgBox("资源下载", content, new Action(this.CheckAsset), new Action(this.Del_quit), "重试", "取消");
				break;
			case EAssetLoadError.eCheckDownLoadOK:
				this.HandleParams(array[1]);
				content = "请确认下载资源" + this.GetSpaceNotice();
				this.CheckWIFI();
				break;
			case EAssetLoadError.eNoNeedToDownloadAsset:
				this.HandleParams(array[1]);
				this.CheckFinish();
				break;
			}
		}

		private void ShowMsgBox(string title, string content, Action del_ok, Action del_cancel, string ok = "确认", string cancel = "取消")
		{
			CtrlManager.ShowMsgBox(title, content, delegate(bool ret)
			{
				if (ret)
				{
					del_ok();
				}
				else
				{
					del_cancel();
				}
			}, PopViewType.PopTwoButton, ok, cancel, null);
		}

		private void Del_quit()
		{
			GlobalObject.QuitApp();
		}

		private void HandleParams(object extra)
		{
			object[] array = extra as object[];
			if (array != null && array.Length == 2)
			{
				this.rSize = (float)((long)array[0]) / 1024f;
				this.freeSize = (float)((long)array[1]) / 1024f;
			}
			else
			{
				this.rSize = 0f;
			}
			ServerInfo serverInfo = ModelManager.Instance.Get_curLoginServerInfo();
			if (serverInfo != null)
			{
				this.bSize = (float)serverInfo.bindataZipSize / 1024f / 1024f;
			}
			else
			{
				this.bSize = 0f;
			}
			LoginStateManager.Instance.RSize = this.rSize;
			LoginStateManager.Instance.BSize = this.bSize;
		}

		private string GetSpaceNotice()
		{
			return string.Format("需要更新（{0}M）", this.rSize.ToString("0.0"));
		}
	}
}
