using Com.Game.Data;
using MobaMessageData;
using System;
using System.IO;
using UnityEngine;

namespace Com.Game.Module
{
	public class UIBannerItem : MonoBehaviour
	{
		public int id;

		public UILabel bannerName;

		public DownloadTexture downTex;

		[HideInInspector]
		public SysMainuiRollingadvertisementVo bannerInfo;

		public BannerInfo info;

		private void Awake()
		{
			this.info = ActivityTools.GetBannerActivityByID(this.id - 1);
			if (this.info != null)
			{
				UIEventListener.Get(this.downTex.uiTex.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBannerItem);
				this.bannerName.text = LanguageManager.Instance.GetStringById(this.info.activityConfig.banner_title);
				this.CheckTexUrl();
			}
		}

		private void ClickBannerItem(GameObject obj)
		{
			if (this.info == null)
			{
				return;
			}
			CtrlManager.OpenWindow(WindowID.ActivityView, null);
			MsgData_Activity_setCurActivity param = new MsgData_Activity_setCurActivity
			{
				activity_typeID = this.info.activityConfig.activity_type_id,
				activity_id = this.info.activityConfig.id
			};
			MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_setCurActivity, param, false);
		}

		private void CheckTexUrl()
		{
			string[] array = this.info.picModuleConfig.content_first.Split(new char[]
			{
				'/'
			});
			string text = array[array.Length - 1];
			this.downTex.texName = text;
			if (File.Exists(Application.persistentDataPath + "/" + text))
			{
				this.downTex.url = Application.persistentDataPath + "/" + text;
				this.downTex.isFromLocal = true;
			}
			else
			{
				this.downTex.url = this.info.picModuleConfig.content_first;
				this.downTex.isFromLocal = false;
			}
		}
	}
}
