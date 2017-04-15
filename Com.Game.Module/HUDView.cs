using Assets.Scripts.Model;
using GUIFramework;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class HUDView : BaseView<HUDView>
	{
		public UILabel name_text;

		private Transform Item;

		private SummonerHeadItem summonerItem;

		public HUDView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "HUDView");
		}

		public override void Init()
		{
			base.Init();
			this.name_text = this.transform.Find("Anchor/NameBack/name").GetComponent<UILabel>();
			this.Item = this.transform.Find("Anchor/Item");
			this.summonerItem = this.Item.Find("SummonerItem").GetComponent<SummonerHeadItem>();
			UIEventListener.Get(this.Item.gameObject).onClick = new UIEventListener.VoidDelegate(this.openVIP);
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
		}

		private void ClearResources()
		{
			if (this.summonerItem != null)
			{
				this.summonerItem.ClearResources();
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.UpdateHUDData();
			this.UpdateHUDView();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdateHUDData()
		{
		}

		private void UpdateHUDView()
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (userData != null)
			{
				this.name_text.text = userData.NickName;
				this.summonerItem.Init(userData.Avatar.ToString(), userData.PictureFrame.ToString(), userData.Exp);
			}
		}

		private void openVIP(GameObject objct_1 = null)
		{
			string text = ModelManager.Instance.Get_userData_filed_X("UserId");
			CtrlManager.OpenWindow(WindowID.SummonerView, null);
		}
	}
}
