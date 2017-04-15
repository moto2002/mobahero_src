using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class HighEffManager : UnitComponent
{
	private Dictionary<string, StartHighEffectAction> mActions = new Dictionary<string, StartHighEffectAction>();

	private Dictionary<string, HighEffVo> mHighEffList = new Dictionary<string, HighEffVo>();

	private Dictionary<string, int> mTriggers = new Dictionary<string, int>();

	private List<HighEffVo> higheffCDCache = new List<HighEffVo>();

	private List<HighEffectData> mForeverActions = new List<HighEffectData>();

	private BackHomeAction mBackHomeAction = new BackHomeAction();

	private CoroutineManager mCoroutineManager = new CoroutineManager();

	public HighEffManager()
	{
	}

	public HighEffManager(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		base.OnInit();
		if (!this.self.isHero)
		{
			this.donotUpdateByMonster = true;
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		this.UpdateHighEffCacheTime(deltaTime);
	}

	public override void OnStop()
	{
		this.self.SetLockCharaControl(false);
		this.Clear();
	}

	public override void OnDeath(Units attacker)
	{
		this.self.SetLockCharaControl(false);
	}

	public override void OnExit()
	{
		this.Clear();
	}

	private void Clear()
	{
		this.mCoroutineManager.StopAllCoroutine();
		this.DestroyHighEffects();
		this.DestroyActions();
		this.ClearAllHighEffCacheTime();
	}

	public HighEffVo AddHighEffect(string higheff_id, string skillId, Units casterUnit, Vector3? skillPosition = null, bool isDoHandle = true)
	{
		if (this.mHighEffList == null)
		{
			return null;
		}
		if (!this.mHighEffList.ContainsKey(higheff_id))
		{
			HighEffVo highEffVo = HighEffVo.Create(higheff_id, skillId, casterUnit, skillPosition, false, 1);
			this.mHighEffList.Add(higheff_id, highEffVo);
			if (isDoHandle)
			{
				this.DoHighEffectHandler(highEffVo);
			}
			return highEffVo;
		}
		HighEffVo highEffVo2;
		if (!this.mHighEffList.TryGetValue(higheff_id, out highEffVo2))
		{
			return null;
		}
		if (highEffVo2 == null)
		{
			ClientLogger.Error("没有这个高级效果，请检查配置表:" + higheff_id);
			return null;
		}
		highEffVo2.casterUnit = casterUnit;
		highEffVo2.skillPosition = skillPosition;
		highEffVo2.skillId = skillId;
		if (isDoHandle)
		{
			this.DoHighEffectHandler(highEffVo2);
		}
		return highEffVo2;
	}

	public void RemoveHighEffect(string higheff_id)
	{
		if (!StringUtils.CheckValid(higheff_id))
		{
			return;
		}
		if (this.mHighEffList.ContainsKey(higheff_id))
		{
			this.mHighEffList.Remove(higheff_id);
		}
		this.RemoveAction(higheff_id);
		this.RemoveTrigger(higheff_id);
	}

	public void DestroyHighEffects()
	{
		List<string> list = this.mHighEffList.Keys.ToList<string>();
		for (int i = 0; i < list.Count; i++)
		{
			this.RemoveHighEffect(list[i]);
		}
		this.mHighEffList.Clear();
		this.mActions.Clear();
		this.DestroyTriggers();
	}

	public bool HasHighEffect(string higheffect_id)
	{
		return this.mHighEffList.ContainsKey(higheffect_id);
	}

	public void AddAction(string higheff_id, StartHighEffectAction action)
	{
		if (action == null || action.isDestroyed)
		{
			return;
		}
		if (!this.mActions.ContainsKey(higheff_id))
		{
			this.mActions.Add(higheff_id, action);
		}
		else
		{
			this.mActions[higheff_id] = action;
		}
	}

	public void DestroySameAction(string higheff_id)
	{
		if (this.mActions.ContainsKey(higheff_id))
		{
			this.mActions[higheff_id].Destroy();
		}
	}

	public bool HaveSameAction(string higheff_id)
	{
		return this.mActions.ContainsKey(higheff_id);
	}

	public void RemoveAction(string higheff_id)
	{
		if (this.mActions.ContainsKey(higheff_id) && this.mActions[higheff_id] != null)
		{
			this.mActions[higheff_id].Destroy();
			this.mActions.Remove(higheff_id);
		}
	}

	private void DestroyActions()
	{
		List<string> list = this.mActions.Keys.ToList<string>();
		for (int i = 0; i < list.Count; i++)
		{
			this.RemoveAction(list[i]);
		}
		this.mActions.Clear();
	}

	public void AddTrigger(string higheffect_id, int trigger_id)
	{
		if (!this.mTriggers.ContainsKey(higheffect_id))
		{
			this.mTriggers.Add(higheffect_id, trigger_id);
		}
	}

	public void RemoveTrigger(string higheffect_id)
	{
		if (this.mTriggers.ContainsKey(higheffect_id))
		{
			this.mTriggers.Remove(higheffect_id);
		}
	}

	private string FindTrigger(int trigger_id)
	{
		string result = null;
		if (this.mTriggers != null)
		{
			Dictionary<string, int>.Enumerator enumerator = this.mTriggers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				if (current.Value.Equals(trigger_id))
				{
					KeyValuePair<string, int> current2 = enumerator.Current;
					result = current2.Key;
					break;
				}
			}
		}
		return result;
	}

	public void DestroyTriggers()
	{
		List<string> list = this.mTriggers.Keys.ToList<string>();
		for (int i = 0; i < list.Count; i++)
		{
			this.RemoveAction(list[i]);
		}
		this.mTriggers.Clear();
	}

	protected void DoHighEffectHandler(HighEffVo h)
	{
		if (h == null)
		{
			return;
		}
		switch (h.data.config.higheff_trigger)
		{
		case 0:
			if (!this.mForeverActions.Contains(h.data))
			{
				this.StartHighEffect(h);
				this.mForeverActions.Add(h.data);
			}
			break;
		case 1:
			this.StartHighEffect(h);
			break;
		case 2:
		{
			VTrigger vTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitHit, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger.trigger_id);
			}
			break;
		}
		case 3:
		{
			VTrigger vTrigger2 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitAttack, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger2 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger2.trigger_id);
			}
			break;
		}
		case 4:
		{
			VTrigger vTrigger3 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitConjure, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger3 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger3.trigger_id);
			}
			break;
		}
		case 5:
		{
			VTrigger vTrigger4 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitConjureR, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger4 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger4.trigger_id);
			}
			break;
		}
		case 6:
		{
			VTrigger vTrigger5 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitKillTarget, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger5 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger5.trigger_id);
			}
			break;
		}
		case 8:
		{
			VTrigger vTrigger6 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitAttackHitOther, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger6 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger6.trigger_id);
			}
			break;
		}
		case 9:
		{
			VTrigger vTrigger7 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitPrognosisDeath, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger7 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger7.trigger_id);
			}
			break;
		}
		case 10:
		{
			VTrigger vTrigger8 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitUnderGroud, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger8 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger8.trigger_id);
			}
			break;
		}
		case 11:
		{
			VTrigger vTrigger9 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitPreConjure, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger9 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger9.trigger_id);
			}
			break;
		}
		case 13:
		{
			VTrigger vTrigger10 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitPreAttack, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger10 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger10.trigger_id);
			}
			break;
		}
		case 14:
		{
			VTrigger vTrigger11 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitTriggerEnter, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger11 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger11.trigger_id);
			}
			break;
		}
		case 15:
		{
			VTrigger vTrigger12 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDespawn, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger12 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger12.trigger_id);
			}
			break;
		}
		case 16:
		{
			VTrigger vTrigger13 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitSkillHitOther, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger13 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger13.trigger_id);
			}
			break;
		}
		case 17:
		{
			VTrigger vTrigger14 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitBeAttackHit, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger14 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger14.trigger_id);
			}
			break;
		}
		case 18:
		{
			VTrigger vTrigger15 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitBeSkillHit, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger15 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger15.trigger_id);
			}
			break;
		}
		case 19:
		{
			VTrigger vTrigger16 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitConjureQWE_HitOther, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger16 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger16.trigger_id);
			}
			break;
		}
		case 21:
		{
			VTrigger vTrigger17 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitKillAndAssist, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger17 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger17.trigger_id);
			}
			break;
		}
		case 22:
		{
			VTrigger vTrigger18 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitCrit, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger18 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger18.trigger_id);
			}
			break;
		}
		case 23:
		{
			VTrigger vTrigger19 = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitHitProp, null, new TriggerAction(this.doHighEffect), this.self.unique_id);
			if (vTrigger19 != null)
			{
				this.AddTrigger(h.higheff_id, vTrigger19.trigger_id);
			}
			break;
		}
		}
	}

	private void doHighEffect()
	{
		VTrigger trigger = TriggerManager.GetTrigger(TriggerType.UnitTrigger);
		if (trigger != null)
		{
			string text = this.FindTrigger(trigger.trigger_id);
			if (text != null && this.HasHighEffect(text))
			{
				HighEffVo highEffVo = this.mHighEffList[text];
				if (highEffVo != null)
				{
					this.StartHighEffect(highEffVo);
				}
				else
				{
					UnityEngine.Debug.LogError("立即找陈宜明, mHighEffList[higheff_id] == null, higheff_id=" + text);
				}
			}
		}
	}

	private void StartHighEffect(HighEffVo h)
	{
		if (this.self == null || h == null)
		{
			return;
		}
		if (this.IsInHighEffCacheTime(h))
		{
			return;
		}
		if (!MathUtils.Rand(h.data.config.probability))
		{
			return;
		}
		if (this.mCoroutineManager == null)
		{
			return;
		}
		this.mCoroutineManager.StartCoroutine(this.StartHighEff_Coroutine(h), true);
	}

	public void stopHighEffeCoroutine()
	{
		this.mCoroutineManager.StopAllCoroutine();
	}

	[DebuggerHidden]
	private IEnumerator StartHighEff_Coroutine(HighEffVo h)
	{
		HighEffManager.<StartHighEff_Coroutine>c__Iterator42 <StartHighEff_Coroutine>c__Iterator = new HighEffManager.<StartHighEff_Coroutine>c__Iterator42();
		<StartHighEff_Coroutine>c__Iterator.h = h;
		<StartHighEff_Coroutine>c__Iterator.<$>h = h;
		<StartHighEff_Coroutine>c__Iterator.<>f__this = this;
		return <StartHighEff_Coroutine>c__Iterator;
	}

	private void AddHighEffCacheTime(HighEffVo h)
	{
		if (h == null || h.data.cdTime <= 0f)
		{
			return;
		}
		for (int i = 0; i < this.higheffCDCache.Count; i++)
		{
			if (this.higheffCDCache[i].data == h.data)
			{
				this.higheffCDCache[i].time = h.data.cdTime;
				return;
			}
		}
		h.time = h.data.cdTime;
		this.higheffCDCache.Add(h);
	}

	private bool IsInHighEffCacheTime(HighEffVo h)
	{
		for (int i = 0; i < this.higheffCDCache.Count; i++)
		{
			if (this.higheffCDCache[i].data == h.data)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateHighEffCacheTime(float deltaTime)
	{
		for (int i = 0; i < this.higheffCDCache.Count; i++)
		{
			this.higheffCDCache[i].time -= deltaTime;
			if (this.higheffCDCache[i].time < 0f)
			{
				this.higheffCDCache.RemoveAt(i);
			}
		}
	}

	public void ClearAllHighEffCacheTime()
	{
		this.higheffCDCache.Clear();
	}

	public void ClearAllDebuffHighEff()
	{
		if (this.mHighEffList == null)
		{
			return;
		}
		List<HighEffVo> list = this.mHighEffList.Values.ToList<HighEffVo>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && list[i].data != null && list[i].data.DataType.GainType == EffectGainType.negative)
			{
				this.RemoveHighEffect(list[i].higheff_id);
			}
		}
	}
}
