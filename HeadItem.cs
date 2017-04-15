using Com.Game.Data;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class HeadItem : MonoBehaviour
{
	private UITexture Icon1;

	private Transform State1;

	private UILabel deathTimeLbl;

	private UISlider hp;

	private UISlider mp;

	public int player_unique_id;

	public string npc_id;

	private VTrigger PlayerDeath;

	private VTrigger PlayerReBrith;

	private VTrigger PlayerTimer;

	private Units m_units;

	private List<string> mSkillIds;

	private Transform SkillIcons;

	private UILabel level;

	private void Awake()
	{
		this.State1 = base.transform.FindChild("State1");
		this.Icon1 = this.State1.FindChild("Icon").GetComponent<UITexture>();
		this.hp = this.State1.FindChild("hp").GetComponent<UISlider>();
		this.deathTimeLbl = this.State1.FindChild("deathTime").GetComponent<UILabel>();
		this.mp = this.State1.FindChild("mp").GetComponent<UISlider>();
		this.SkillIcons = this.State1.FindChild("Skills");
		this.level = this.State1.Find("Anchor/level").GetComponent<UILabel>();
		UIEventListener.Get(this.Icon1.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChangePlayer);
		this.NormalHeroHead();
	}

	public void UpdateItem(string hero_icon, bool ismaster, string heroName = null)
	{
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(TeamType.None, TargetTag.Hero);
		for (int i = 0; i < mapUnits.Count; i++)
		{
			if (mapUnits[i].npc_id == heroName)
			{
				this.player_unique_id = mapUnits[i].unique_id;
				this.npc_id = mapUnits[i].npc_id;
				base.name = this.player_unique_id.ToString();
				this.m_units = mapUnits[i];
				break;
			}
		}
		this.RegisterTrigger();
		this.Icon1.mainTexture = ResourceManager.Load<Texture>(hero_icon, true, true, null, 0, false);
		if (ismaster)
		{
		}
	}

	private void UpdateSkills()
	{
		for (int i = 0; i < this.mSkillIds.Count; i++)
		{
			SysSkillMainVo skillData = SkillUtility.GetSkillData(this.mSkillIds[i], -1, -1);
			UITexture component = this.SkillIcons.GetChild(i).GetComponent<UITexture>();
			component.name = this.mSkillIds[i];
			string skill_icon = skillData.skill_icon;
			Texture mainTexture = ResourceManager.Load<Texture>(skill_icon, true, true, null, 0, false);
			component.mainTexture = mainTexture;
		}
	}

	public void UpdateItemById(int uid, string hero_icon, string nid)
	{
		this.player_unique_id = uid;
		base.name = this.player_unique_id.ToString();
		this.npc_id = nid;
		this.RegisterTrigger();
		this.Icon1.mainTexture = ResourceManager.Load<Texture>(hero_icon, true, true, null, 0, false);
		this.m_units = MapManager.Instance.GetUnit(uid);
	}

	private void RegisterTrigger()
	{
		if (this.PlayerDeath != null)
		{
			TriggerManager.DestroyTrigger(this.PlayerDeath);
		}
		if (this.PlayerReBrith != null)
		{
			TriggerManager.DestroyTrigger(this.PlayerReBrith);
		}
		if (this.PlayerTimer != null)
		{
			TriggerManager.DestroyTrigger(this.PlayerTimer);
		}
		this.PlayerDeath = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitDeath, null, new TriggerAction(this.GrayHeroHead), this.player_unique_id);
		this.PlayerReBrith = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRebirthAgain, null, new TriggerAction(this.NormalHeroHead), this.player_unique_id);
		this.PlayerTimer = TriggerManager.CreateUnitEventTrigger(UnitEvent.HeroDeathTimer, null, new TriggerAction(this.DeathTimer), this.player_unique_id);
	}

	public void UpdateValue(float hp_value, float mp_value)
	{
		if (this.hp != null)
		{
			this.hp.value = hp_value;
		}
		if (this.mp != null)
		{
			this.mp.value = mp_value;
		}
	}

	private void ChangePlayer(GameObject obj)
	{
	}

	private void FixedUpdate()
	{
		if (this.mSkillIds == null && this.mSkillIds == null && MapManager.Instance != null && MapManager.Instance.GetUnit(this.player_unique_id) != null && MapManager.Instance.GetUnit(this.player_unique_id).skills.Length > 0)
		{
			this.m_units = MapManager.Instance.GetUnit(this.player_unique_id);
			this.mSkillIds = this.m_units.skills.ToList<string>();
			this.UpdateSkills();
			this.level.text = this.m_units.level.ToString();
		}
	}

	private void GrayHeroHead()
	{
		this.Icon1.material = CharacterDataMgr.instance.ReturnMaterialType(9);
		this.hp.value = 0f;
		this.mp.value = 0f;
	}

	private void DeathTimer()
	{
		float playerDeathTime = BattleAttrManager.Instance.GetPlayerDeathTime(this.player_unique_id);
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(this.UpdateTimer(playerDeathTime));
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateTimer(float length)
	{
		HeadItem.<UpdateTimer>c__IteratorDF <UpdateTimer>c__IteratorDF = new HeadItem.<UpdateTimer>c__IteratorDF();
		<UpdateTimer>c__IteratorDF.length = length;
		<UpdateTimer>c__IteratorDF.<$>length = length;
		<UpdateTimer>c__IteratorDF.<>f__this = this;
		return <UpdateTimer>c__IteratorDF;
	}

	private void SetDeathTime(float time)
	{
		if (time <= 0f)
		{
			this.deathTimeLbl.text = string.Empty;
		}
		else
		{
			this.deathTimeLbl.text = time.ToString("F0");
		}
	}

	public void NormalHeroHead()
	{
		this.Icon1.material = CharacterDataMgr.instance.ReturnMaterialType(1);
		this.SetDeathTime(0f);
	}
}
