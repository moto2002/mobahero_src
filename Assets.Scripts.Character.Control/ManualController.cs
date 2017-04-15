using Assets.MobaTools.TriggerPlugin.Scripts;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Character.Control
{
	public class ManualController : StaticUnitComponent
	{
		private ManualControlTarget targetCom;

		private ManualControlSignalMng signalCom;

		private List<TriggerEvent2> listTrigger2 = new List<TriggerEvent2>();

		private List<ManualControllerCom> listManuControllerCom = new List<ManualControllerCom>();

		private TouchEventDebug touchDebugger;

		private bool pause;

		private bool initial;

		private bool debugFlag;

		private long controllerUnlockTime;

		private int controllerLock;

		public ManualControlSkillCrazy mcSkillCrazy;

		public ManualControlSkillNormal mcSkillNormal;

		public Units ControlUnit
		{
			get
			{
				return this.self;
			}
		}

		public ManualControlTarget TargetCom
		{
			get
			{
				return this.targetCom;
			}
		}

		public ManualControlSignalMng SignalCom
		{
			get
			{
				return this.signalCom;
			}
		}

		public CoroutineManager CoroutineMng
		{
			get
			{
				return this.m_CoroutineManager;
			}
		}

		private void Init()
		{
			if (this.debugFlag)
			{
				this.touchDebugger = new TouchEventDebug();
				if (this.touchDebugger != null)
				{
					this.touchDebugger.OnInit();
				}
			}
			this.signalCom = new ManualControlSignalMng(this);
			this.targetCom = new ManualControlTarget(this);
			this.RegisterTrigger();
			this.InvokeEveryCom("OnInit", null);
		}

		private void OnControllerClose(MobaMessage msg)
		{
			this.controllerLock++;
		}

		private void OnControllerOpen(MobaMessage msg)
		{
			this.controllerUnlockTime = DateTime.Now.Ticks;
			this.controllerLock = 0;
		}

		public void SetCrazyMode(bool isInit = false)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillControlReset, this.self, null, null);
			this.RemoveNormalModeNew();
			this.RemoveCrazyMode();
			this.listManuControllerCom.Add(this.mcSkillCrazy = new ManualControlSkillCrazy(this));
			if (!isInit)
			{
				this.mcSkillCrazy.OnInit();
			}
			this.self.AddCrazyController(false);
		}

		public void SetNormalModeNew(bool isInit = false)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillControlReset, this.self, null, null);
			this.RemoveCrazyMode();
			this.RemoveNormalModeNew();
			this.listManuControllerCom.Add(this.mcSkillNormal = new ManualControlSkillNormal(this));
			if (!isInit)
			{
				this.mcSkillNormal.OnInit();
			}
			this.self.AddNormalControllerNew(false);
		}

		private void RemoveCrazyMode()
		{
			this.listManuControllerCom.Remove(this.mcSkillCrazy);
			this.self.RemoveCrazyController();
		}

		private void RemoveNormalModeNew()
		{
			this.listManuControllerCom.Remove(this.mcSkillNormal);
			this.self.RemoveNormalController();
		}

		public override void OnInit()
		{
			base.OnInit();
			if (!this.initial)
			{
				this.Init();
				this.initial = true;
			}
		}

		public override void OnStart()
		{
			this.pause = false;
			base.OnStart();
			this.InvokeEveryCom("OnStart", null);
		}

		public override void OnStop()
		{
			this.pause = true;
			base.OnStop();
			this.InvokeEveryCom("OnStop", null);
		}

		public override void OnExit()
		{
			base.OnExit();
			this.InvokeEveryCom("OnExit", null);
			this.UnRegisterTrigger();
			this.m_CoroutineManager.StopAllCoroutine();
			if (this.debugFlag && this.touchDebugger != null)
			{
				this.touchDebugger.OnExit();
			}
		}

		private void OnDown(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			TriggerParamTouch triggerParamTouch;
			if (!this.HandleTouchParam(param, out triggerParamTouch))
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnDown", new object[]
			{
				triggerParamTouch
			});
		}

		private void OnPress(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			TriggerParamTouch triggerParamTouch;
			if (!this.HandleTouchParam(param, out triggerParamTouch))
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnPress", new object[]
			{
				triggerParamTouch
			});
		}

		private void OnUp(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			TriggerParamTouch triggerParamTouch;
			if (!this.HandleTouchParam(param, out triggerParamTouch))
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnUp", new object[]
			{
				triggerParamTouch
			});
		}

		private void OnMoveEnd(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			TriggerParamTouch triggerParamTouch;
			if (!this.HandleTouchParam(param, out triggerParamTouch))
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnMoveEnd", new object[]
			{
				triggerParamTouch
			});
		}

		private void OnSkillEnd(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnSkillEnd", new object[]
			{
				param
			});
		}

		private void OnSkill(ITriggerDoActionParam param)
		{
			if (!this.CanHandleControl())
			{
				return;
			}
			this.signalCom.Init();
			this.InvokeEveryCom("OnSkill", new object[]
			{
				param
			});
		}

		private void OnNavigateEnd(ITriggerDoActionParam param = null)
		{
			this.signalCom.Init();
			this.InvokeEveryCom("OnNavigateEnd", null);
		}

		private void InvokeEveryCom(string methodName, object[] param = null)
		{
			if (string.IsNullOrEmpty(methodName))
			{
				return;
			}
			this.ShowEventDebug(methodName);
			foreach (ManualControllerCom current in this.listManuControllerCom)
			{
				Type type = current.GetType();
				MethodInfo method = type.GetMethod(methodName);
				if (method != null)
				{
					method.Invoke(current, param);
				}
			}
		}

		public void ShowEventDebug(string str)
		{
			if (this.debugFlag)
			{
				this.touchDebugger.AddEventInfo(str);
			}
		}

		public void ShowHandlerDebug(string str)
		{
			if (this.debugFlag)
			{
				this.touchDebugger.AddHandlerInfo(str);
			}
		}

		private void RegisterTrigger()
		{
			this.RegisterEvent2(EEventID2_touch.eDown, new Callback<ITriggerDoActionParam>(this.OnDown));
			this.RegisterEvent2(EEventID2_touch.ePress, new Callback<ITriggerDoActionParam>(this.OnPress));
			this.RegisterEvent2(EEventID2_touch.eUp, new Callback<ITriggerDoActionParam>(this.OnUp));
			this.RegisterEvent2(EEventID2_touch.eMoveEnd, new Callback<ITriggerDoActionParam>(this.OnMoveEnd));
			this.RegisterEvent2(EEventID2_skillControl.ePressSkill, new Callback<ITriggerDoActionParam>(this.OnSkill));
			this.RegisterEvent2(EEventID2_navigation.eOnStopPath, new Callback<ITriggerDoActionParam>(this.OnNavigateEnd));
			this.RegisterEvent2(EEventID2_skillControl.eKillEnd, new Callback<ITriggerDoActionParam>(this.OnSkillEnd));
			MobaMessageManager.RegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnControllerClose));
			MobaMessageManager.RegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnControllerOpen));
			MobaMessageManager.RegistMessage((ClientMsg)26024, new MobaMessageFunc(this.OnControllerOpen));
			MobaMessageManager.RegistMessage((ClientMsg)26023, new MobaMessageFunc(this.OnControllerClose));
		}

		private void UnRegisterTrigger()
		{
			for (int i = 0; i < this.listTrigger2.Count; i++)
			{
				TriggerManager2.Instance.RemoveListner(this.listTrigger2[i]);
			}
			MobaMessageManager.UnRegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnControllerClose));
			MobaMessageManager.UnRegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnControllerOpen));
			MobaMessageManager.UnRegistMessage((ClientMsg)26024, new MobaMessageFunc(this.OnControllerOpen));
			MobaMessageManager.UnRegistMessage((ClientMsg)26023, new MobaMessageFunc(this.OnControllerClose));
		}

		private void RegisterEvent2(EEventID2_touch eventID, Callback<ITriggerDoActionParam> fun_action)
		{
			TriggerCreateParamTouch param = new TriggerCreateParamTouch();
			this.RecordTrigger(param, (int)eventID, fun_action, null);
		}

		private void RegisterEvent2(EEventID2_skillControl eventID, Callback<ITriggerDoActionParam> fun_action)
		{
			TriggerCreateParamSkillControl param = new TriggerCreateParamSkillControl();
			this.RecordTrigger(param, (int)eventID, fun_action, null);
		}

		private void RegisterEvent2(EEventID2_navigation eventID, Callback<ITriggerDoActionParam> fun_action)
		{
			TriggerCreateParamNavigation param = new TriggerCreateParamNavigation();
			this.RecordTrigger(param, (int)eventID, fun_action, new List<TriggerCondition<ITriggerDoActionParam>>
			{
				new TriggerCondition<ITriggerDoActionParam>(this.IsPlayer)
			});
		}

		private void RecordTrigger(ITriggerCreatorParam param, int eventID, Callback<ITriggerDoActionParam> fun_action, List<TriggerCondition<ITriggerDoActionParam>> conditions = null)
		{
			if (param != null)
			{
				param.EventID = eventID;
				param.TriggerID = TriggerManager2.assign_trigger_id();
				param.Func_actions = fun_action;
				param.Func_conditions = conditions;
				TriggerEvent2 triggerEvent = TriggerManager2.CreateTriggerEvent2(param);
				this.listTrigger2.Add(triggerEvent);
				TriggerManager2.Instance.AddListener(triggerEvent);
			}
		}

		private bool IsPlayer(ITriggerDoActionParam param = null)
		{
			TriggerParamNavigation triggerParamNavigation = param as TriggerParamNavigation;
			return triggerParamNavigation != null && triggerParamNavigation.IsPlayer;
		}

		private bool HandleTouchParam(ITriggerDoActionParam param, out TriggerParamTouch info)
		{
			info = (param as TriggerParamTouch);
			return info != null;
		}

		private bool CanHandleControl()
		{
			if (this.controllerUnlockTime > 0L)
			{
				if (DateTime.Now.Ticks - this.controllerUnlockTime < 1000000L)
				{
					return false;
				}
				this.controllerUnlockTime = 0L;
			}
			return null != this.self && this.self.isLive && !this.self.IsLockCharaControl && !this.self.LockInputState && this.controllerLock == 0;
		}

		public void ShowDebugMsg(string s)
		{
		}
	}
}
