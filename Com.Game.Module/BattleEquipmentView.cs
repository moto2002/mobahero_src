using Assets.Scripts.GUILogic.View.BattleEquipment;
using GUIFramework;
using Newbie;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class BattleEquipmentView : BaseView<BattleEquipmentView>
	{
		private BattleEquip_Top com_top;

		private BattleEquip_Bottom com_bottom;

		private BattleEquip_Center com_center;

		private BattleEquip_Left com_left;

		private BattleEquip_Right com_right;

		public BattleEquipmentView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/BattleEquipment/BattleEquipmentView");
		}

		public override void Init()
		{
			base.Init();
			Transform transform = this.transform.Find("BottomAnchor");
			this.com_bottom = transform.gameObject.AddComponent<BattleEquip_Bottom>();
			transform = this.transform.Find("CenterAnchor");
			this.com_center = transform.gameObject.AddComponent<BattleEquip_Center>();
			transform = this.transform.Find("LeftAnchor");
			this.com_left = transform.gameObject.AddComponent<BattleEquip_Left>();
			transform = this.transform.Find("RightAnchor");
			this.com_right = transform.gameObject.AddComponent<BattleEquip_Right>();
			transform = this.transform.FindChild("TopAnchor");
			this.com_top = transform.gameObject.AddComponent<BattleEquip_Top>();
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			MobaMessageManagerTools.SendClientMsg(ClientV2V.BattleShop_shopOpened, null, false);
			NewbieManager.Instance.TryTriggerBuyEquipHint();
			NewbieManager.Instance.TryHandleOpenShop();
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			MobaMessageManagerTools.SendClientMsg(ClientV2V.BattleShop_shopClosed, null, false);
			NewbieManager.Instance.TryHandleCloseShop();
		}

		public override void Destroy()
		{
			Transform transform = this.com_bottom.transform.Find("Coin");
			if (transform != null)
			{
				HomeGCManager.Instance.UnloadUISpriteAsset(transform.gameObject);
			}
			base.Destroy();
		}

		public override void RefreshUI()
		{
		}
	}
}
