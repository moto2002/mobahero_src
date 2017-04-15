using Com.Game.Module;
using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	internal class LoginTask_downLoadResource : LoginTaskBase
	{
		private const string const_title = "资源下载";

		private bool bFinish;

		public LoginTask_downLoadResource() : base(ELoginTask.eDownLoadResource, new object[]
		{
			ClientC2C.DownLoadResourceOver,
			ClientC2C.Login_Action
		})
		{
			base.AddTask(new List<ELoginAction>
			{
				ELoginAction.eDownLoadBindataFinish
			}, new Action(this.DownloadAsset));
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void DownloadAsset()
		{
			if (GlobalSettings.useBundle && LoginStateManager.Instance.RSize > 0f)
			{
				ResourceManager.downLoadAssets();
			}
			else
			{
				this.DownLoadFinish();
			}
		}

		private void DownLoadFinish()
		{
			base.DoAction(ELoginAction.eDownLoadResourceFinish);
			base.Valid = false;
		}

		private void OnMsg_DownLoadResourceOver(MobaMessage msg)
		{
			object[] array = msg.Param as object[];
			string content = string.Empty;
			EAssetLoadError eAssetLoadError = EAssetLoadError.eUnknowReason;
			if (array != null && array.Length == 3)
			{
				eAssetLoadError = (EAssetLoadError)((int)array[0]);
			}
			EAssetLoadError eAssetLoadError2 = eAssetLoadError;
			switch (eAssetLoadError2)
			{
			case EAssetLoadError.eSuccess:
				this.DownLoadFinish();
				return;
			case EAssetLoadError.eDownLoadApk:
				IL_41:
				if (eAssetLoadError2 != EAssetLoadError.eUnknowReason)
				{
				}
				content = "资源下载失败，原因不明，是否重试？";
				this.ShowMsgBox("资源下载", content, new Action(this.DownloadAsset), new Action(this.Del_quit), "重试", "取消");
				return;
			case EAssetLoadError.eWWWError:
				content = "资源下载失败，www错误，是否重试？";
				this.ShowMsgBox("资源下载", content, new Action(this.DownloadAsset), new Action(this.Del_quit), "重试", "取消");
				return;
			}
			goto IL_41;
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
	}
}
