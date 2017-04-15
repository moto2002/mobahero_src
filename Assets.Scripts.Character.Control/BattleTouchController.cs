using Assets.MobaTools.TriggerPlugin.Scripts;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Character.Control
{
	public class BattleTouchController : UnitComponent
	{
		protected bool isActive = true;

		private ControlEventManager tEventManager;

		private List<TriggerEvent2> listTrigger;

		private bool inital;

		public BattleTouchController()
		{
		}

		public BattleTouchController(Units self) : base(self)
		{
		}

		public void Enable(bool b)
		{
			this.isActive = b;
		}

		public override void OnInit()
		{
			if (!this.inital)
			{
				this.Init();
				this.inital = true;
			}
		}

		public override void OnStart()
		{
			this.isActive = true;
		}

		public void OnUpdateByFrame(float delta = 0f)
		{
			if (this.isActive)
			{
				this.TriggerEvent(this.tEventManager.UpdateControl(delta));
				this.DebugInput();
			}
		}

		public override void OnUpdate(float delta = 0f)
		{
		}

		public override void OnStop()
		{
			this.isActive = false;
		}

		public override void OnExit()
		{
			this.tEventManager.OpenMultiTouch(false);
			this.isActive = false;
			this.UnRegisterEvent();
			this.tEventManager.OnExit();
		}

		private void TriggerEvent(ControlEvent tEvent)
		{
			if (tEvent == null)
			{
				return;
			}
			TriggerParamTouch triggerParamTouch = new TriggerParamTouch();
			triggerParamTouch.EventID = (int)tEvent.type;
			triggerParamTouch.Trigger = this;
			triggerParamTouch.Pos = tEvent.position;
			triggerParamTouch.FingerID = tEvent.id;
			TriggerManager2.Instance.Trigger2(triggerParamTouch);
		}

		private void Init()
		{
			this.listTrigger = new List<TriggerEvent2>();
			this.tEventManager = new ControlEventManager();
			this.tEventManager.OnInit();
			this.tEventManager.OpenMultiTouch(true);
		}

		private void RegisterEvent()
		{
			TriggerEvent2 triggerEvent = TriggerManager2.CreateTriggerEvent2(new TriggerCreateParam_touchController
			{
				EventID = 1,
				TriggerID = TriggerManager2.assign_trigger_id(),
				Func_actions = new Callback<ITriggerDoActionParam>(this.OnStopOrStart),
				Func_conditions = null
			});
			TriggerManager2.Instance.AddListener(triggerEvent);
			this.listTrigger.Add(triggerEvent);
		}

		private void UnRegisterEvent()
		{
			TriggerManager2.Instance.RemoveListner(ETriggerType2.TriggerEvent2_manulController);
			foreach (TriggerEvent2 current in this.listTrigger)
			{
				TriggerManager2.Instance.RemoveListner(current);
			}
		}

		private void OnStopOrStart(ITriggerDoActionParam param)
		{
			TriggerParam_touchController triggerParam_touchController = param as TriggerParam_touchController;
			if (triggerParam_touchController == null)
			{
				return;
			}
			if (triggerParam_touchController.Start)
			{
				this.OnStart();
			}
			else
			{
				this.OnStop();
			}
		}

		private void DebugInput()
		{
		}
	}
}
