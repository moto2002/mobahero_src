using Assets.Scripts.GUILogic.View.BottleSystemView;
using Assets.Scripts.GUILogic.View.HomeChatView;
using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.GUILogic.View.Runes;
using MobaClient;
using System;

namespace HomeState
{
	internal class HomeState_menu : HomeStateBase
	{
		private bool bFinished;

		private BottleViewCtrl bottleins;

		private SacrificialCtrl sacrificialins;

		private HomeChatCtrl homechatins;

		private RunesCtrl runesins;

		public HomeState_menu() : base(HomeStateCode.HomeState_menu, MobaPeerType.C2GateServer)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.InitBottleViewCtrl(true);
			this.InitSacrificialCtrl(true);
			this.InitHomeChatCtrl(true);
			this.IniRunesCtrl(true);
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25011, null, 0f);
			MobaMessageManager.DispatchMsg(message);
			this.bottleins.Init();
			this.sacrificialins.Init();
			this.homechatins.Init();
			this.runesins.Init();
		}

		public override void OnExit()
		{
			base.OnExit();
			this.bottleins.UnInit();
			this.sacrificialins.UnInit();
			this.homechatins.UnInit();
			this.runesins.UnInit();
			this.InitBottleViewCtrl(false);
			this.InitSacrificialCtrl(false);
			this.InitHomeChatCtrl(false);
			this.IniRunesCtrl(false);
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
		}

		public override void OnUpdate(long delta)
		{
		}

		protected override void OnConnected_game(MobaMessage msg)
		{
		}

		protected override void OnDisconnected_game(MobaMessage msg)
		{
		}

		private void InitBottleViewCtrl(bool isTrue = true)
		{
			if (!isTrue)
			{
				this.bottleins = null;
			}
			else
			{
				this.bottleins = BottleViewCtrl.GetInstance();
			}
		}

		private void InitSacrificialCtrl(bool isTrue = true)
		{
			if (!isTrue)
			{
				this.sacrificialins = null;
			}
			else
			{
				this.sacrificialins = SacrificialCtrl.GetInstance();
			}
		}

		private void InitHomeChatCtrl(bool isTrue = true)
		{
			if (!isTrue)
			{
				this.homechatins = null;
			}
			else
			{
				this.homechatins = HomeChatCtrl.GetInstance();
			}
		}

		private void IniRunesCtrl(bool isTrue = true)
		{
			if (!isTrue)
			{
				this.runesins = null;
			}
			else
			{
				this.runesins = RunesCtrl.GetInstance();
			}
		}
	}
}
