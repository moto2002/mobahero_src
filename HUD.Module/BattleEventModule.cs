using GUIFramework;
using System;

namespace HUD.Module
{
	public class BattleEventModule : BaseModule
	{
		private UIMessageBox message_box;

		private TweenPosition tPos;

		public VTrigger ReplayGame;

		public BattleEventModule()
		{
			this.module = EHUDModule.BattleEvent;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/BattleEventModule");
		}

		public override void Init()
		{
			base.Init();
			this.message_box = this.transform.GetComponent<UIMessageBox>();
			this.tPos = this.transform.GetComponent<TweenPosition>();
			if (this.message_box == null)
			{
				this.message_box = this.gameObject.AddComponent<UIMessageBox>();
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.RegisterTrigger();
		}

		public override void onFlyOut()
		{
			this.tPos.PlayForward();
		}

		public override void onFlyIn()
		{
			this.tPos.PlayReverse();
		}

		public override void Destroy()
		{
			if (this.message_box != null)
			{
				this.message_box.StopAllCoroutines();
				this.message_box = null;
			}
			base.Destroy();
		}

		private void RegisterTrigger()
		{
			this.ReplayGame = TriggerManager.CreateGameEventTrigger(GameEvent.CleanAllData, null, new TriggerAction(this.ReUpdate));
		}

		private void ReUpdate()
		{
		}
	}
}
