using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObserveHeadItem : MonoBehaviour
{
	public int UniqueId;

	public UITexture Face;

	public UISprite FaceMask;

	public UILabel FaceReliveTime;

	public AnimateHpBar Hp;

	public UISlider Mp;

	public UISlider Xp;

	public UILabel Level;

	public UILabel HeroName;

	public UILabel KillText;

	public UILabel DeathText;

	public UILabel GoldText;

	public UIGrid Skills;

	public UIGrid Equips;

	public UISprite Back;

	public ObserveHeroSkill SpecialSkill;

	private Units _hero;

	private readonly List<ObserveHeroSkill> _skillComps = new List<ObserveHeroSkill>();

	private readonly List<UITexture> _equipIcons = new List<UITexture>();

	private VTrigger _playerDeath;

	private VTrigger _playerRebirth;

	private readonly CoroutineManager _coroutineManager = new CoroutineManager();

	private void Awake()
	{
		UIEventListener.Get(this.Back.gameObject).onClick = delegate
		{
			this.OnClickHero();
		};
		this.ResetReliveState();
		for (int i = 0; i < 4; i++)
		{
			ObserveHeroSkill component = this.Skills.transform.GetChild(i).GetComponent<ObserveHeroSkill>();
			this._skillComps.Add(component);
		}
		for (int j = 0; j < 6; j++)
		{
			UITexture component2 = this.Equips.transform.GetChild(j).Find("icon").GetComponent<UITexture>();
			this._equipIcons.Add(component2);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("RepeatRefresh");
		this._coroutineManager.StopAllCoroutine();
		this.ClearCallbacks();
	}

	private void ResetReliveState()
	{
		if (this.FaceMask)
		{
			this.FaceMask.fillAmount = 0f;
		}
		if (this.FaceReliveTime)
		{
			this.FaceReliveTime.text = string.Empty;
		}
	}

	public void Refresh()
	{
		this.RegisterCallbacks();
		this._hero = MapManager.Instance.GetUnit(this.UniqueId);
		if (this._hero)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this._hero.npc_id);
			this.Face.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
			base.CancelInvoke("RepeatRefresh");
			base.InvokeRepeating("RepeatRefresh", 0f, 0.5f);
		}
		this.RepeatRefresh();
		this.RefreshEquips();
	}

	private void RefreshStats()
	{
		PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(this.UniqueId);
		this.KillText.text = string.Format("{0}/{1}/{2}", heroData.HeroKill, heroData.Death, heroData.Assist);
		this.DeathText.text = heroData.MonsterKill.ToString();
		this.GoldText.text = heroData.CurGold.ToString();
		if (!this._hero.isLive)
		{
			this.OnHeroDead();
		}
	}

	private void RefreshBasicInfo()
	{
		try
		{
			this.HeroName.text = Singleton<PvpManager>.Instance.GetSummonerName(-this.UniqueId);
			this.Level.text = UtilManager.Instance.GetHerolv(this.UniqueId).ToString();
			this.Hp.value = this._hero.hp / this._hero.hp_max;
			this.Mp.value = this._hero.mp / this._hero.mp_max;
			this.Xp.value = UtilManager.Instance.GetHeroExpRatio(this.UniqueId);
		}
		catch (Exception e)
		{
			ClientLogger.LogException(e);
		}
	}

	public void RefreshEquips()
	{
		List<ItemDynData> list = null;
		Units unit = MapManager.Instance.GetUnit(this.UniqueId);
		if (unit)
		{
			list = ((Hero)unit).EquipPackage.EquipList;
		}
		for (int i = 0; i < 6; i++)
		{
			this._equipIcons[i].mainTexture = null;
			if (i < list.Count)
			{
				string typeId = list[i].typeId;
				SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(typeId);
				if (dataById == null)
				{
					ClientLogger.Error("item not found for id=" + typeId);
					return;
				}
				this._equipIcons[i].mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
			}
		}
	}

	private void SetMaskEnableStatus(UISprite inMask, float inFillAmount)
	{
		if (inMask == null)
		{
			return;
		}
		if (inFillAmount < 0.01f)
		{
			if (inMask.enabled)
			{
				inMask.enabled = false;
			}
			if (inMask.gameObject.activeInHierarchy)
			{
				inMask.gameObject.SetActive(false);
			}
		}
		else
		{
			if (!inMask.gameObject.activeInHierarchy)
			{
				inMask.gameObject.SetActive(true);
			}
			if (!inMask.enabled)
			{
				inMask.enabled = true;
			}
		}
	}

	private void RefreshSkills()
	{
		List<string> skills = this._hero.GetSkills();
		for (int i = 0; i < 4; i++)
		{
			this.BindSkill(skills[i], this._skillComps[i]);
		}
		if (skills.Count > 4)
		{
			this.BindSkill(skills[4], this.SpecialSkill);
		}
	}

	private void BindSkill(string skill, ObserveHeroSkill skillComp)
	{
		try
		{
			SysSkillMainVo skillData = SkillUtility.GetSkillData(skill, -1, -1);
			string skill_icon = skillData.skill_icon;
			Texture mainTexture = ResourceManager.Load<Texture>(skill_icon, true, true, null, 0, false);
			skillComp.Texture.mainTexture = mainTexture;
			if (skillComp.Level)
			{
				int skillLevel = this._hero.skillManager.GetSkillLevel(skill);
				skillComp.Level.text = skillLevel.ToString();
			}
			float num = 0f;
			float skillCDTime = this._hero.GetSkillCDTime(skill);
			if (skillCDTime > 0f)
			{
				num = this._hero.GetCDTime(skill) / skillCDTime;
			}
			skillComp.Mask.fillAmount = num;
			this.SetMaskEnableStatus(skillComp.Mask, num);
		}
		catch (Exception e)
		{
			ClientLogger.LogException(e);
		}
	}

	private void RegisterCallbacks()
	{
		this.ClearCallbacks();
		this._playerDeath = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.OnHeroDead), this.UniqueId);
		this._playerRebirth = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRebirthAgain, null, new TriggerAction(this.OnHeroRelive), this.UniqueId);
		MobaMessageManager.RegistMessage((ClientMsg)25043, new MobaMessageFunc(this.OnHeroEquipChanged));
	}

	private void ClearCallbacks()
	{
		if (this._playerDeath != null)
		{
			TriggerManager.DestroyTrigger(this._playerDeath);
		}
		if (this._playerRebirth != null)
		{
			TriggerManager.DestroyTrigger(this._playerRebirth);
		}
		MobaMessageManager.UnRegistMessage((ClientMsg)25043, new MobaMessageFunc(this.OnHeroEquipChanged));
		this._playerRebirth = null;
		this._playerDeath = null;
	}

	private void OnHeroEquipChanged(MobaMessage msg)
	{
		HeroItemsChangedData heroItemsChangedData = msg.Param as HeroItemsChangedData;
		if (heroItemsChangedData != null && heroItemsChangedData._uid == this.UniqueId)
		{
			this.RefreshEquips();
		}
	}

	private void OnClickHero()
	{
		PvpObserveMgr.ObserveUser(-this.UniqueId);
	}

	protected void RepeatRefresh()
	{
		if (this.UniqueId == 0)
		{
			return;
		}
		this.RefreshBasicInfo();
		this.RefreshSkills();
		this.RefreshStats();
	}

	private void OnHeroDead()
	{
		this.Hp.value = 0f;
		this.Mp.value = 0f;
		this._coroutineManager.StopAllCoroutine();
		this._coroutineManager.StartCoroutine(this.UpdateFaceMask(), true);
	}

	[DebuggerHidden]
	private IEnumerator UpdateFaceMask()
	{
		ObserveHeadItem.<UpdateFaceMask>c__IteratorE4 <UpdateFaceMask>c__IteratorE = new ObserveHeadItem.<UpdateFaceMask>c__IteratorE4();
		<UpdateFaceMask>c__IteratorE.<>f__this = this;
		return <UpdateFaceMask>c__IteratorE;
	}

	private void OnHeroRelive()
	{
		this.ResetReliveState();
		this._coroutineManager.StopAllCoroutine();
	}
}
