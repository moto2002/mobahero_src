using GUIFramework;
using System;

namespace Com.Game.Module
{
	public class MessageView : BaseView<MessageView>
	{
		private UIMessageBox message_box;

		public VTrigger ReplayGame;

		public MessageView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MessageView");
		}

		public override void Init()
		{
			base.Init();
			this.message_box = this.transform.GetComponent<UIMessageBox>();
			if (this.message_box == null)
			{
				this.message_box = this.gameObject.AddComponent<UIMessageBox>();
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.RegisterTrigger();
		}

		public override void RefreshUI()
		{
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
