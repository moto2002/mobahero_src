using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MovingEntity
{
	[NonSerialized]
	public bool herovoiceloaded;

	[NonSerialized]
	public string musicid = string.Empty;

	[NonSerialized]
	public eventPlayerSound m_eventPlayerSound;

	public int InitCount;

	private RecieverObjCtrl roc;

	private bool started;

	private Animator ani;

	private Dictionary<AttrType, float> _inBattleEquipAdd = new Dictionary<AttrType, float>();

	private Dictionary<AttrType, float> _inBattleEquipMul = new Dictionary<AttrType, float>();

	public HeroInfoData heroData
	{
		get;
		private set;
	}

	public HeroEquipPackage EquipPackage
	{
		get;
		private set;
	}

	public string getMusicID()
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.npc_id);
		this.musicid = heroMainData.music_id;
		return this.musicid;
	}

	protected override void OnCreate()
	{
		this.data = base.AddUnitComponent<HeroDataManager>(!Singleton<PvpManager>.Instance.IsInPvp);
		this.EquipPackage = base.AddUnitComponent<HeroEquipPackage>();
		this.atkController = base.AddUnitComponent<HeroAttackController>();
		base.OnCreate();
		base.ChangeLayer("Unit");
		this.musicid = this.data.GetData<string>(DataType.MusicId);
		this.m_hv = base.gameObject.GetComponent<HeroVoicePlayer>();
		if (this.m_hv == null)
		{
			this.m_hv = base.gameObject.AddComponent<HeroVoicePlayer>();
		}
		if (this.m_model != null)
		{
			eventPlayerSound eventPlayerSound = this.m_model.gameObject.GetComponent<eventPlayerSound>();
			if (eventPlayerSound == null)
			{
				eventPlayerSound = this.m_model.gameObject.AddComponent<eventPlayerSound>();
			}
			eventPlayerSound.units = this;
			this.m_eventPlayerSound = eventPlayerSound;
			HeroVoicePlayer component = this.m_model.gameObject.GetComponent<HeroVoicePlayer>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		if (RecieverObjCtrl.usefakeshadow)
		{
			if (this.roc == null)
			{
				this.roc = base.GetComponentInChildren<RecieverObjCtrl>();
			}
			if (this.roc == null)
			{
				GameObject gameObject = new GameObject("recieverObj_auto");
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.enabled = false;
				MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.mesh = null;
				meshFilter.sharedMesh = null;
				gameObject.transform.parent = base.transform;
				this.roc = gameObject.AddComponent<RecieverObjCtrl>();
				this.roc.skintarget = base.GetComponentInChildren<SkinnedMeshRenderer>();
				this.roc.updatamesh3();
				this.roc.doStart();
			}
			else
			{
				this.roc.skintarget = base.GetComponentInChildren<SkinnedMeshRenderer>();
				this.roc.updatamesh3();
				this.roc.doStart();
			}
		}
	}

	protected override void OnInit(bool isRebirth = false)
	{
		if (!base.MirrorState)
		{
			this.heroData = Singleton<PvpManager>.Instance.GetHeroInfoData(this.unique_id);
		}
		base.OnInit(isRebirth);
		this.m_fLiveTime = -1f;
		this.m_fLeftTime = 0f;
		this.musicid = this.data.GetData<string>(DataType.MusicId);
		if (this.m_hv != null)
		{
			this.m_hv.init(this.musicid);
		}
		if (RecieverObjCtrl.usefakeshadow)
		{
			if (this.roc == null)
			{
				this.roc = base.GetComponentInChildren<RecieverObjCtrl>();
			}
			if (this.roc != null)
			{
				string[] array = base.name.Split(new char[]
				{
					'+'
				});
				this.roc.setnpcid(array[0]);
			}
		}
		if (this.InitCount == 0)
		{
			AudioMgr.loadSoundBank_Skill(this.musicid, false, HeroSkins.GetRealHeroSkin(base.TeamType, base.model_id));
		}
		if (this.m_eventPlayerSound != null)
		{
			this.m_eventPlayerSound.isplayer = this.isPlayer;
		}
		if (this.isPlayer)
		{
			base.controllerType = 1;
			AudioMgr.AddLisener(base.gameObject);
			AudioMgr.loadLanguageSoundBank(this.musicid, HeroSkins.GetRealHeroSkin(base.TeamType, base.model_id));
		}
		else
		{
			base.controllerType = 0;
		}
		if (!base.MirrorState)
		{
		}
		this.InitCount++;
		this.TryInitHeroVisibleOnReplay(isRebirth);
	}

	private void TryInitHeroVisibleOnReplay(bool inIsRebirth)
	{
		if (GameManager.Instance.ReplayController.IsReplayStart && !inIsRebirth)
		{
			this.m_nVisibleState = 2;
			this.m_nServerVisibleState = 0;
		}
	}

	protected override void OnStart()
	{
		this.started = false;
		base.OnStart();
		if (this.EquipPackage != null)
		{
			this.EquipPackage.ApplyChange();
		}
	}

	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		this.UpdateMirrorState(delta);
		if (this.ani != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = this.ani.GetCurrentAnimatorStateInfo(0);
			float num = currentAnimatorStateInfo.normalizedTime;
			num -= (float)((int)currentAnimatorStateInfo.normalizedTime);
			if (this.roc != null)
			{
				this.roc.setframe(num * 64f);
			}
		}
		if (!this.started)
		{
			this.started = true;
		}
		if (this.ani == null)
		{
			this.ani = base.GetComponentInChildren<Animator>();
		}
		AnimationInfo[] currentAnimationClipState = this.ani.GetCurrentAnimationClipState(0);
		if (currentAnimationClipState != null && currentAnimationClipState.Length >= 1 && this.roc != null)
		{
			this.roc.setclip(currentAnimationClipState[0].clip.name);
		}
		if (Input.GetKeyUp(KeyCode.F))
		{
			if (this.roc == null)
			{
				this.roc = base.GetComponentInChildren<RecieverObjCtrl>();
			}
			if (this.roc == null)
			{
				if (this.heroData != null)
				{
				}
				return;
			}
			this.roc.setvis(false);
		}
	}

	protected override void OnExit()
	{
		if (this.isPlayer)
		{
			AudioMgr.unloadLanguageSoundBank(this.musicid, 0);
		}
		base.OnExit();
	}

	private void UpdateMirrorState(float delta)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		if (!base.MirrorState)
		{
			return;
		}
		this.m_fLeftTime += delta;
		if (this.m_fLeftTime > this.m_fLiveTime && base.isLive)
		{
			this.data.SetHp(0f);
			this.TryDeath(this);
		}
	}

	protected override void DeathIncome(Units attacker)
	{
		if (base.MirrorState)
		{
			return;
		}
		UtilManager.Instance.AddDeathGold(attacker, this);
		UtilManager.Instance.AddExp(attacker, this);
		UtilManager.Instance.UpdateKillState(attacker, this);
	}

	public override void Wound(Units attacker, float damage)
	{
		base.Wound(attacker, damage);
	}

	public override void RealDeath(Units attacker)
	{
		if (this.isPlayer)
		{
		}
		base.RealDeath(attacker);
		if (this.IsMaster)
		{
		}
	}

	public HeroDetailedAttr getDetailedAttr()
	{
		HeroDetailedAttr heroDetailedAttr = new HeroDetailedAttr();
		heroDetailedAttr.parseFrom(this.data);
		return heroDetailedAttr;
	}

	public void ChangePropFromEquip(Dictionary<AttrType, float> add, Dictionary<AttrType, float> mul)
	{
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		Dictionary<AttrType, float> dictionary2 = new Dictionary<AttrType, float>();
		this.CompleteNature(ref dictionary, this._inBattleEquipAdd, false);
		this.CompleteNature(ref dictionary2, this._inBattleEquipMul, false);
		this._inBattleEquipAdd = add;
		this._inBattleEquipMul = mul;
		this.CompleteNature(ref dictionary, add, true);
		this.CompleteNature(ref dictionary2, mul, true);
		foreach (KeyValuePair<AttrType, float> current in dictionary)
		{
			this.data.ChangeAttr(current.Key, OpType.Add, current.Value);
		}
		foreach (KeyValuePair<AttrType, float> current2 in dictionary2)
		{
			this.data.ChangeAttr(current2.Key, OpType.Mul, current2.Value);
		}
	}

	private void CompleteNature(ref Dictionary<AttrType, float> change, Dictionary<AttrType, float> target, bool isAdd)
	{
		if (target == null || target.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<AttrType, float> current in target)
		{
			if (!change.ContainsKey(current.Key))
			{
				change[current.Key] = 0f;
			}
			if (isAdd)
			{
				Dictionary<AttrType, float> dictionary;
				Dictionary<AttrType, float> expr_54 = dictionary = change;
				AttrType key;
				AttrType expr_5D = key = current.Key;
				float num = dictionary[key];
				expr_54[expr_5D] = num + current.Value;
			}
			else
			{
				Dictionary<AttrType, float> dictionary2;
				Dictionary<AttrType, float> expr_7E = dictionary2 = change;
				AttrType key;
				AttrType expr_88 = key = current.Key;
				float num = dictionary2[key];
				expr_7E[expr_88] = num - current.Value;
			}
		}
	}
}
