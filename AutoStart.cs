using Com.Game.Module;
using MobaHeros;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AutoStart : MonoBehaviour
{
	public enum UnitType
	{
		Entity,
		SkillUnit
	}

	public static int assign_id = 1;

	public Units self;

	public int unique_id;

	public string npc_id;

	public string m_tag = "Hero";

	public int level = 10;

	public int star = 1;

	public int quality = 1;

	public TeamType teamType;

	public bool autoStart = true;

	public bool autoAtk;

	public bool useControl;

	public bool isPlayer;

	public bool useKey;

	public bool drawGizmos;

	private bool isInit;

	private bool deathAndRebirth;

	private bool rebirth;

	private bool noDeath;

	private CoroutineManager mCoroutineManager = new CoroutineManager();

	private Task mRebirthTask;

	private Task mDeathTask;

	public AutoStart.UnitType mUnitType;

	private bool autoatk_;

	private void Start()
	{
	}

	private void OnDisable()
	{
		if (this.autoStart && this.self != null)
		{
			this.self.UnitStop();
		}
	}

	private void OnDestroy()
	{
		if (this.autoStart)
		{
			this.mCoroutineManager.StopAllCoroutine();
			if (this.self != null)
			{
				this.self.OnWoundCallback -= new Callback<Units>(this.CallWhenInjured);
				this.self.UnitStop();
				this.self.UnitDestroy();
			}
			this.isInit = false;
			this.deathAndRebirth = false;
			this.rebirth = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator Rebirth_Coroutine()
	{
		AutoStart.<Rebirth_Coroutine>c__Iterator0 <Rebirth_Coroutine>c__Iterator = new AutoStart.<Rebirth_Coroutine>c__Iterator0();
		<Rebirth_Coroutine>c__Iterator.<>f__this = this;
		return <Rebirth_Coroutine>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator Death_Coroutine()
	{
		AutoStart.<Death_Coroutine>c__Iterator1 <Death_Coroutine>c__Iterator = new AutoStart.<Death_Coroutine>c__Iterator1();
		<Death_Coroutine>c__Iterator.<>f__this = this;
		return <Death_Coroutine>c__Iterator;
	}

	private void CreateUnit()
	{
		if (this.mUnitType == AutoStart.UnitType.Entity)
		{
			this.InitUnit();
		}
		else if (this.mUnitType == AutoStart.UnitType.SkillUnit)
		{
			this.InitSkillUnit();
		}
	}

	private void InitUnit()
	{
		if (this.self != null && this.self.isLive)
		{
			return;
		}
		this.deathAndRebirth = false;
		this.rebirth = false;
		if (this.self == null)
		{
			this.self = base.GetComponent<Units>();
			this.self.npc_id = this.npc_id;
			this.self.unique_id = AutoStart.assign_id++;
			this.self.teamType = (int)this.teamType;
			this.self.level = this.level;
			this.self.star = this.star;
			this.self.quality = this.quality;
			this.self.attr_factor = 1f;
			this.self.isLive = true;
			this.self.name = base.gameObject.name + "+" + this.self.team.name;
			this.self.isPlayer = this.isPlayer;
			if (this.isPlayer)
			{
				this.self.tag = "Player";
			}
			this.self.UnitInit(false);
			this.self.UnitStart();
		}
		else
		{
			this.self.teamType = (int)this.teamType;
			this.self.level = this.level;
			this.self.star = this.star;
			this.self.quality = this.quality;
			this.self.attr_factor = 1f;
			this.self.isLive = true;
			this.self.isPlayer = this.isPlayer;
			if (this.isPlayer)
			{
				this.self.tag = "Player";
			}
			this.self.UnitInit(false);
			this.self.UnitStart();
		}
		MapManager.Instance.AddToMap(this.self);
		if (this.autoAtk)
		{
			this.self.SetCanAIControl(true);
		}
		else
		{
			this.self.SetCanAIControl(false);
		}
		if (this.useControl)
		{
			this.self.controllerType = 1;
		}
		this.self.OnWoundCallback += new Callback<Units>(this.CallWhenInjured);
		if (this.noDeath)
		{
			this.self.ChangeAttr(AttrType.Hp, OpType.Add, 99999f);
			this.self.ChangeAttr(AttrType.Mp, OpType.Add, 99999f);
		}
		if (this.isPlayer)
		{
			PlayerControlMgr.Instance.ChangePlayer(this.self, false);
			Hero component = base.gameObject.GetComponent<Hero>();
			if (component != null)
			{
				component.isPlayer = this.isPlayer;
				AudioMgr.loadSoundBank_Skill(component.musicid, false, 0);
				AudioMgr.loadLanguageSoundBank(component.musicid, 0);
			}
			Monster component2 = base.gameObject.GetComponent<Monster>();
			if (component2 != null)
			{
				UnityEngine.Debug.Log("monster..");
			}
		}
		CtrlManager.OpenWindow(WindowID.SkillView, null);
	}

	private void InitSkillUnit()
	{
		if (this.self != null && this.self.isLive)
		{
			return;
		}
		this.deathAndRebirth = false;
		this.rebirth = false;
		if (this.self == null)
		{
			this.self = base.GetComponent<SkillUnit>();
			this.self.npc_id = this.npc_id;
			this.self.unique_id = AutoStart.assign_id++;
			this.self.tag = this.m_tag;
			this.self.teamType = (int)this.teamType;
			this.self.level = this.level;
			this.self.star = this.star;
			this.self.quality = this.quality;
			this.self.attr_factor = 1f;
			this.self.isLive = true;
			this.self.name = base.gameObject.name + "+" + this.self.team.name;
			this.self.UnitInit(false);
			this.self.UnitStart();
		}
		else
		{
			this.self.isLive = true;
			this.self.UnitInit(false);
			this.self.UnitStart();
		}
		MapManager.Instance.AddToMap(this.self);
	}

	public void CallWhenInjured(Units owner)
	{
		base.StartCoroutine(this.CallWhenInjured_Coroutinue(owner));
	}

	[DebuggerHidden]
	private IEnumerator CallWhenInjured_Coroutinue(Units owner)
	{
		AutoStart.<CallWhenInjured_Coroutinue>c__Iterator2 <CallWhenInjured_Coroutinue>c__Iterator = new AutoStart.<CallWhenInjured_Coroutinue>c__Iterator2();
		<CallWhenInjured_Coroutinue>c__Iterator.owner = owner;
		<CallWhenInjured_Coroutinue>c__Iterator.<$>owner = owner;
		return <CallWhenInjured_Coroutinue>c__Iterator;
	}
}
