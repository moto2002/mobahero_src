using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using HUD.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class HUDModuleManager : BaseView<HUDModuleManager>
	{
		private enum ModuleCategory
		{
			LowFrequencyRefresh,
			Unstatic = 128,
			HighFrequencyRefresh = 255
		}

		private GameObject test1;

		private GameObject test2;

		private GameObject LowFrequencyRefreshPanel;

		private UIPanel lowFrequencyRefreshUIPanel;

		private GameObject UnStaticPanel;

		private GameObject HighFrequencyRefreshPanel;

		private CoroutineManager corManager;

		private Dictionary<EHUDModule, BaseModule> mModules = new Dictionary<EHUDModule, BaseModule>();

		public Dictionary<string, string> headIconDict = new Dictionary<string, string>();

		public SkillPanelPivot skillPanelPivot;

		private Task taskRecord;

		private bool isVastMap;

		public bool IsVastMap
		{
			get
			{
				return this.isVastMap;
			}
		}

		public HUDModuleManager()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/HUDModuleManager");
		}

		public override void Init()
		{
			this.LowFrequencyRefreshPanel = this.transform.Find("LowFrequencyRefreshPanel").gameObject;
			this.lowFrequencyRefreshUIPanel = this.LowFrequencyRefreshPanel.GetComponent<UIPanel>();
			this.UnStaticPanel = this.transform.Find("UnStaticPanel").gameObject;
			this.HighFrequencyRefreshPanel = this.transform.Find("HighFrequencyRefreshPanel").gameObject;
		}

		private void onTest1Click(GameObject obj = null)
		{
			this.FlyOut();
		}

		private void onTest2Click(GameObject obj = null)
		{
			this.FlyIn();
		}

		public override void HandleAfterOpenView()
		{
			this.mModules.Clear();
			this.JudgeIsVastMap();
			this.skillPanelPivot = (SkillPanelPivot)ModelManager.Instance.Get_SettingData().skillPanelPivot;
			bool isReplayStart = GameManager.Instance.ReplayController.IsReplayStart;
			if (!Singleton<PvpManager>.Instance.IsObserver)
			{
				if (!isReplayStart)
				{
					this.OpenModule(EHUDModule.FPS, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
					this.OpenModule(EHUDModule.ActionIndicator, 0);
					this.OpenModule(EHUDModule.FunctionBtns, 0);
				}
				this.OpenModule(EHUDModule.BattleExploit, 0);
				this.OpenModule(EHUDModule.DeathTimer, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
				this.OpenModule(EHUDModule.Buff, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
				if (LevelManager.CurLevelId != "80007")
				{
					this.OpenModule(EHUDModule.PlayersIndicator, HUDModuleManager.ModuleCategory.Unstatic);
				}
				else
				{
					this.OpenModule(EHUDModule.ChaosBattleExploit, HUDModuleManager.ModuleCategory.Unstatic);
				}
				this.OpenModule(EHUDModule.AttackIndicator, 0);
			}
			this.OpenModule(EHUDModule.SiderTips, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
			this.OpenModule(EHUDModule.BattleEvent, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
			if (GlobalSettings.Instance.PvpSetting.isDebugSpeed)
			{
				this.OpenModule(EHUDModule.Debug, 0);
			}
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			this.corManager.StartCoroutine(this.DelayControlStatic(2f), true);
		}

		[DebuggerHidden]
		private IEnumerator DelayControlStatic(float _delaySec)
		{
			HUDModuleManager.<DelayControlStatic>c__IteratorD9 <DelayControlStatic>c__IteratorD = new HUDModuleManager.<DelayControlStatic>c__IteratorD9();
			<DelayControlStatic>c__IteratorD._delaySec = _delaySec;
			<DelayControlStatic>c__IteratorD.<$>_delaySec = _delaySec;
			<DelayControlStatic>c__IteratorD.<>f__this = this;
			return <DelayControlStatic>c__IteratorD;
		}

		public override void HandleBeforeCloseView()
		{
			this.isVastMap = false;
			foreach (EHUDModule current in this.mModules.Keys)
			{
				this.CloseModule(current, false);
			}
			this.mModules.Clear();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnMsgHideUI));
			MobaMessageManager.RegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnMsgShowUI));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)26021, new MobaMessageFunc(this.OnMsgHideUI));
			MobaMessageManager.UnRegistMessage((ClientMsg)26022, new MobaMessageFunc(this.OnMsgShowUI));
		}

		private void OnMsgHideUI(MobaMessage msg)
		{
			this.FlyOut();
		}

		private void OnMsgShowUI(MobaMessage msg)
		{
			this.FlyIn();
		}

		public int GetDepth(byte _type)
		{
			if (this.lowFrequencyRefreshUIPanel == null)
			{
				return 120;
			}
			if (_type == 0)
			{
				return this.lowFrequencyRefreshUIPanel.depth;
			}
			return this.UnStaticPanel.GetComponent<UIPanel>().depth;
		}

		public T GetModule<T>(EHUDModule _mod) where T : BaseModule
		{
			if (this.mModules.ContainsKey(_mod))
			{
				return (T)((object)this.mModules[_mod]);
			}
			return (T)((object)null);
		}

		public void OpenModule(EHUDModule _mod, byte _refreshFrequency = 0)
		{
			this.OpenModule(_mod, (HUDModuleManager.ModuleCategory)_refreshFrequency);
		}

		private void OpenModule(EHUDModule _mod, HUDModuleManager.ModuleCategory refreshFrequency)
		{
			if (this.mModules.ContainsKey(_mod))
			{
				BaseModule baseModule = this.mModules[_mod];
				baseModule.HandleAfterOpenModule();
				baseModule.RegisterUpdateHandler();
			}
			else if (HUDModDefine.mDicModType.ContainsKey(_mod))
			{
				Type type = HUDModDefine.mDicModType[_mod];
				BaseModule baseModule = Activator.CreateInstance(type) as BaseModule;
				if (baseModule == null)
				{
					ClientLogger.Error("Invalid Module Type." + type.ToString());
					return;
				}
				this.mModules.Add(_mod, baseModule);
				GameObject parent;
				if (refreshFrequency == HUDModuleManager.ModuleCategory.LowFrequencyRefresh)
				{
					parent = this.LowFrequencyRefreshPanel;
				}
				else if (refreshFrequency == HUDModuleManager.ModuleCategory.Unstatic)
				{
					parent = this.UnStaticPanel;
				}
				else
				{
					parent = this.HighFrequencyRefreshPanel;
				}
				baseModule.gameObject = NGUITools.AddChild(parent, Resources.Load<GameObject>(baseModule.WinResCfg.Url));
				if (baseModule.gameObject == null)
				{
					ClientLogger.Error("Invalid Module Prefab Url." + baseModule.WinResCfg.Url);
					return;
				}
				baseModule.transform = baseModule.gameObject.transform;
				baseModule.Init();
				baseModule.AdaptSkillPanelPivot();
				baseModule.HandleAfterOpenModule();
				baseModule.RegisterUpdateHandler();
			}
		}

		public void ResetModulePivot(EHUDModule _mod)
		{
			if (this.mModules.ContainsKey(_mod))
			{
				BaseModule baseModule = this.mModules[_mod];
				baseModule.AdaptSkillPanelPivot();
				baseModule.HandleAfterOpenModule();
				baseModule.RegisterUpdateHandler();
			}
		}

		public void CloseModule(EHUDModule _mod, bool isDestoryFromDict = false)
		{
			if (this.mModules.ContainsKey(_mod))
			{
				BaseModule baseModule = this.mModules[_mod];
				baseModule.CancelUpdateHandler();
				baseModule.HandleBeforeCloseModule();
				if (isDestoryFromDict)
				{
					baseModule.Destroy();
					this.mModules.Remove(_mod);
				}
			}
		}

		public void FlyOut()
		{
			this.TemporaryCancelPanelStatic(0.3f);
			if (Singleton<BarrageEmitterView>.Instance.gameObject)
			{
				Singleton<BarrageEmitterView>.Instance.FlyOut();
			}
			if (Singleton<MiniMapView>.Instance.gameObject)
			{
				Singleton<MiniMapView>.Instance.FlyOut();
			}
			if (Singleton<SkillView>.Instance.gameObject)
			{
				Singleton<SkillView>.Instance.FlyOut();
			}
			foreach (BaseModule current in this.mModules.Values)
			{
				current.onFlyOut();
			}
		}

		public void FlyIn()
		{
			this.TemporaryCancelPanelStatic(0.3f);
			if (Singleton<BarrageEmitterView>.Instance.gameObject)
			{
				Singleton<BarrageEmitterView>.Instance.FlyIn();
			}
			if (Singleton<MiniMapView>.Instance.gameObject)
			{
				Singleton<MiniMapView>.Instance.FlyIn();
			}
			if (Singleton<SkillView>.Instance.gameObject)
			{
				Singleton<SkillView>.Instance.FlyIn();
			}
			foreach (BaseModule current in this.mModules.Values)
			{
				current.onFlyIn();
			}
		}

		public void TemporaryCancelPanelStatic(float _duration)
		{
			if (this.taskRecord != null)
			{
				this.corManager.StopCoroutine(this.taskRecord);
			}
			if (this.lowFrequencyRefreshUIPanel.widgetsAreStatic)
			{
				this.lowFrequencyRefreshUIPanel.widgetsAreStatic = false;
			}
			this.taskRecord = this.corManager.StartCoroutine(this.DelayControlStatic(_duration), true);
		}

		public void SetSkillPanelPivot(SkillPanelPivot _pivot)
		{
			if (this.skillPanelPivot == _pivot)
			{
				return;
			}
			this.skillPanelPivot = _pivot;
			this.CloseModule(EHUDModule.SiderTips, true);
			this.OpenModule(EHUDModule.SiderTips, HUDModuleManager.ModuleCategory.HighFrequencyRefresh);
			this.ResetModulePivot(EHUDModule.Buff);
			Singleton<SkillView>.Instance.SetSkillPanelPivot(_pivot);
			if (Singleton<SurrenderView>.Instance.IsOpen)
			{
				Singleton<SurrenderView>.Instance.SetPosition(_pivot == SkillPanelPivot.Right);
			}
		}

		public string GetSpriteNameById(string _id)
		{
			string result = string.Empty;
			if (!this.headIconDict.TryGetValue(_id, out result))
			{
				result = this.AddCfgDataToDict(_id);
			}
			return result;
		}

		private string AddCfgDataToDict(string _id)
		{
			string text = string.Empty;
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(_id);
			if (heroMainData != null)
			{
				text = heroMainData.avatar_icon;
			}
			else
			{
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(_id);
				if (monsterMainData != null)
				{
					text = monsterMainData.avatar_icon;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				ClientLogger.Error("SiderTipsModule gets an id that can't be found in 'hero_main' or in 'monster_main'.");
			}
			this.headIconDict.Add(_id, text);
			return text;
		}

		private void JudgeIsVastMap()
		{
			string level_id = LevelManager.m_CurLevel.level_id;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(level_id);
			if (dataById != null && (dataById.scene_map_id.Equals("Map21") || dataById.scene_map_id.Equals("Map22")))
			{
				this.isVastMap = true;
			}
		}
	}
}
