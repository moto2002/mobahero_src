using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EffectManager : StaticUnitComponent
{
	private int startCount;

	private string trilEffect = string.Empty;

	private string birthEffect = string.Empty;

	private string deathEffect = string.Empty;

	private string backEffect = string.Empty;

	private string levelEffect = "Fx_levelup";

	private List<BaseAction> mEffects = new List<BaseAction>();

	private readonly CoroutineManager _coroutineManager = new CoroutineManager();

	public override void OnInit()
	{
	}

	private void AssinPersonalData()
	{
		if (this.self is Hero)
		{
			Hero hero = this.self as Hero;
			if (hero.heroData == null)
			{
				return;
			}
			if (hero.heroData.tailEffectId != 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.tailEffectId.ToString());
				if (dataById != null)
				{
					this.trilEffect = dataById.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制：道具没找到 error id=" + hero.heroData.tailEffectId);
				}
			}
			if (hero.heroData.birthEffectId != 0)
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.birthEffectId.ToString());
				if (dataById2 != null)
				{
					this.birthEffect = dataById2.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制：道具没找到 error id=" + hero.heroData.birthEffectId);
				}
			}
			if (hero.heroData.backEffectId != 0)
			{
				SysGameItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.backEffectId.ToString());
				if (dataById3 != null)
				{
					this.backEffect = dataById3.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制：道具没找到 error id=" + hero.heroData.backEffectId);
				}
			}
			if (hero.heroData.deathEffectId != 0)
			{
				SysGameItemsVo dataById4 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.deathEffectId.ToString());
				if (dataById4 != null)
				{
					this.deathEffect = dataById4.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制：道具没找到 error id=" + hero.heroData.deathEffectId);
				}
			}
			if (hero.heroData.levelEffectId != 0)
			{
				SysGameItemsVo dataById5 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.levelEffectId.ToString());
				if (dataById5 != null)
				{
					this.levelEffect = dataById5.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制：道具没找到 error id=" + hero.heroData.levelEffectId);
				}
			}
		}
	}

	public override void OnStart()
	{
		this.AssinPersonalData();
		if (this.startCount == 0)
		{
			TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRebirthAgain, null, new TriggerAction(this.OnUnitRebirthAgain), this.self.unique_id);
		}
		else if (this.startCount > 0)
		{
			this.ShowReBirthEffect();
			this.ShowBodyEffect();
		}
		this.startCount++;
	}

	public void OnFadeOut()
	{
		if (Singleton<PvpManager>.Instance.IsGlobalObserver)
		{
			if (this.self.gameObject.activeSelf && this.startCount <= 0)
			{
				this.self.StartCoroutine(this.DelayShowBirthEffect());
			}
		}
		else if (this.self.gameObject.activeSelf)
		{
			this.self.StartCoroutine(this.DelayShowBirthEffect());
		}
		this.ShowBodyEffect();
	}

	[DebuggerHidden]
	private IEnumerator DelayShowBirthEffect()
	{
		EffectManager.<DelayShowBirthEffect>c__Iterator3A <DelayShowBirthEffect>c__Iterator3A = new EffectManager.<DelayShowBirthEffect>c__Iterator3A();
		<DelayShowBirthEffect>c__Iterator3A.<>f__this = this;
		return <DelayShowBirthEffect>c__Iterator3A;
	}

	public void SetVisible(bool bShow)
	{
		if (this.mEffects != null)
		{
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				if (this.mEffects[i] != null)
				{
					if (!bShow || this.mEffects[i].gameObject != null)
					{
					}
					if (bShow || !this.mEffects[i].IsForceDisplay())
					{
						this.SetParticlesVisible(this.mEffects[i].gameObject, bShow);
					}
				}
			}
		}
	}

	public void SetParticlesVisible(GameObject obj, bool isShow)
	{
		if (obj == null)
		{
			return;
		}
		ParticleAdapter[] componentsInChildren = obj.GetComponentsInChildren<ParticleAdapter>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null)
				{
					componentsInChildren[i].ShowRenders(isShow);
				}
			}
		}
		EffectPlayer[] componentsInChildren2 = obj.GetComponentsInChildren<EffectPlayer>();
		if (componentsInChildren2 != null)
		{
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j] != null)
				{
					componentsInChildren2[j].ShowRenders(isShow);
				}
			}
		}
		if (componentsInChildren == null && componentsInChildren2 == null)
		{
			Renderer[] componentsInChildren3 = obj.GetComponentsInChildren<Renderer>();
			if (componentsInChildren3 != null)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					if (componentsInChildren3[k] != null)
					{
						componentsInChildren3[k].enabled = isShow;
					}
				}
			}
		}
	}

	private void OnUnitRebirthAgain()
	{
		if (this.mEffects == null)
		{
			return;
		}
		List<CharacterEffectAction> list = new List<CharacterEffectAction>();
		foreach (BaseAction current in this.mEffects)
		{
			if (current is CharacterEffectAction && (current as CharacterEffectAction).IsDeathEffect())
			{
				list.Add((CharacterEffectAction)current);
			}
		}
		foreach (CharacterEffectAction current2 in list)
		{
			current2.Destroy();
		}
	}

	public override void OnStop()
	{
		this.DestroyEffects();
	}

	public override void OnDeath(Units attacker)
	{
		this.ShowDeathEffect();
	}

	public void ShowBodyEffect()
	{
		this.AddEffect(ActionManager.PlayCharacterEffect(this.self.effect_id, this.self, 7));
		if (this.trilEffect != string.Empty)
		{
			this.AddEffect(ActionManager.PlayEffect(this.trilEffect, this.self, null, null, true, string.Empty, null));
		}
	}

	private void ShowBirthEffect()
	{
		if (this.birthEffect != string.Empty)
		{
			this.AddEffect(ActionManager.PlayEffect(this.birthEffect, this.self, null, null, true, string.Empty, null));
		}
		else
		{
			this.AddEffect(ActionManager.PlayCharacterEffect(this.self.effect_id, this.self, 1));
		}
		if (this.self.isPlayer && LevelManager.m_CurLevel.Is3V3V3())
		{
			this._coroutineManager.StartCoroutine(this.Entry(), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator Entry()
	{
		return new EffectManager.<Entry>c__Iterator3B();
	}

	private void ShowReBirthEffect()
	{
		if (this.birthEffect == string.Empty)
		{
			this.AddEffect(ActionManager.PlayCharacterEffect(this.self.effect_id, this.self, 6));
		}
		else
		{
			this.AddEffect(ActionManager.PlayEffect(this.birthEffect, this.self, null, null, true, string.Empty, null));
		}
	}

	private void ShowDeathEffect()
	{
		if (this.deathEffect != string.Empty)
		{
			this.AddEffect(ActionManager.PlayEffect(this.deathEffect, this.self, null, null, true, string.Empty, null));
		}
		else if (this.self.isHero && !this.self.MirrorState)
		{
			this.AddEffect(ActionManager.PlayEffect("Perform_900", this.self, null, null, true, string.Empty, null));
		}
		this.AddEffect(ActionManager.PlayCharacterEffect(this.self.effect_id, this.self, 2));
	}

	public PlayEffectAction StartEffect(string perform_id)
	{
		PlayEffectAction playEffectAction = null;
		if (StringUtils.CheckValid(perform_id))
		{
			playEffectAction = ActionManager.PlayEffect(perform_id, this.self, null, null, true, string.Empty, null);
		}
		this.AddEffect(playEffectAction);
		return playEffectAction;
	}

	public PlayEffectAction StartLevelUpEffect()
	{
		PlayEffectAction playEffectAction = null;
		if (StringUtils.CheckValid(this.levelEffect))
		{
			playEffectAction = ActionManager.PlayEffect(this.levelEffect, this.self, null, null, true, string.Empty, null);
		}
		this.AddEffect(playEffectAction);
		return playEffectAction;
	}

	public void AddEffect(BaseAction action)
	{
		if (action != null)
		{
			this.mEffects.Add(action);
		}
	}

	public void RemoveEffect(BaseAction action)
	{
		if (action != null && this.mEffects.Contains(action))
		{
			this.mEffects.Remove(action);
		}
	}

	public void DestroyEffects()
	{
		if (this.mEffects != null)
		{
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				if (this.mEffects[i] != null)
				{
					this.mEffects[i].Destroy();
				}
			}
			this.mEffects.Clear();
		}
	}
}
