using Com.Game.Module;
using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace HUD.Module
{
	public class DeathTimerModule : BaseModule
	{
		private UILabel timeText;

		private UISlider timeSlider;

		private Units player;

		private UIBloodBar playerBloodBar;

		private CoroutineManager cMgr;

		public DeathTimerModule()
		{
			this.module = EHUDModule.DeathTimer;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/DeathTimerModule");
		}

		public override void Init()
		{
			base.Init();
			this.transform.GetComponent<TweenPosition>().ResetToBeginning();
			this.timeText = this.transform.FindChild("TimerLabel/Time").GetComponent<UILabel>();
			this.timeSlider = this.transform.FindChild("Slider").GetComponent<UISlider>();
		}

		public override void HandleAfterOpenModule()
		{
			if (this.cMgr == null)
			{
				this.cMgr = new CoroutineManager();
			}
			else
			{
				this.cMgr.StopAllCoroutine();
			}
			this.gameObject.SetActive(false);
			MobaMessageManager.RegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
			MobaMessageManager.RegistMessage((ClientMsg)25035, new MobaMessageFunc(this.OnUnitDeathTime));
		}

		public override void HandleBeforeCloseModule()
		{
			if (this.cMgr != null)
			{
				this.cMgr.StopAllCoroutine();
			}
			MobaMessageManager.UnRegistMessage((ClientMsg)25036, new MobaMessageFunc(this.OnPlayerAttached));
			MobaMessageManager.UnRegistMessage((ClientMsg)25035, new MobaMessageFunc(this.OnUnitDeathTime));
		}

		private void OnPlayerAttached(MobaMessage msg)
		{
			this.player = PlayerControlMgr.Instance.GetPlayer();
			if (this.player != null)
			{
				this.playerBloodBar = this.player.GetUnitComponent<SurfaceManager>().mHpBar;
			}
		}

		private void OnUnitDeathTime(MobaMessage msg)
		{
			ParamUnitDeathTime paramUnitDeathTime = msg.Param as ParamUnitDeathTime;
			if (paramUnitDeathTime.uniqueId == this.player.unique_id)
			{
				this.cMgr.StartCoroutine(this.Timer(paramUnitDeathTime.uniqueId, paramUnitDeathTime.reliveTime), true);
			}
		}

		[DebuggerHidden]
		private IEnumerator Timer(int _uniqId, float _time)
		{
			DeathTimerModule.<Timer>c__IteratorD7 <Timer>c__IteratorD = new DeathTimerModule.<Timer>c__IteratorD7();
			<Timer>c__IteratorD._time = _time;
			<Timer>c__IteratorD._uniqId = _uniqId;
			<Timer>c__IteratorD.<$>_time = _time;
			<Timer>c__IteratorD.<$>_uniqId = _uniqId;
			<Timer>c__IteratorD.<>f__this = this;
			return <Timer>c__IteratorD;
		}
	}
}
