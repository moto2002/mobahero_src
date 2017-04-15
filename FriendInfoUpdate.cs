using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FriendInfoUpdate : MonoBehaviour
{
	public int idx;

	public UISprite headTexture;

	public UILabel timeLabel;

	public UISprite maskSprite;

	public UISprite livePointSprite;

	public UISprite deadPointSprite;

	public UISprite hpSprite;

	private Units hero;

	private Task deathTimer;

	private CoroutineManager cMgr = new CoroutineManager();

	private float lastLeftTime;

	private bool isLive = true;

	private float lastHpPercent;

	private float lastTime;

	private bool inCd;

	private float cdTime;

	private bool skillLearned;

	private void onAwake()
	{
		this.deathTimer = null;
	}

	private void onDestroy()
	{
		this.deathTimer = null;
		this.cMgr.StopAllCoroutine();
	}

	private void Start()
	{
		this.Init();
	}

	private void Stop()
	{
		this.hero = null;
	}

	private void Update()
	{
		if (this.hero == null)
		{
			this.Init();
			return;
		}
		this.TryUpdateHp();
		if (this.isLive != this.hero.isLive)
		{
			if (this.hero.isLive)
			{
				this.SetLive();
			}
			else
			{
				this.SetDead();
			}
		}
		Skill skillByIndex = this.hero.getSkillByIndex(3);
		if (skillByIndex != null)
		{
			if (!this.skillLearned && this.hero.skillManager != null && this.hero.skillManager.GetSkillLevel(skillByIndex.skillMainId) > 0)
			{
				this.SetSkill4Ready();
				this.skillLearned = true;
			}
			if (this.skillLearned && this.IsSkillInCD(skillByIndex) != this.inCd)
			{
				if (this.inCd)
				{
					this.SetSkill4Ready();
				}
				else
				{
					this.SetSkill4NotReady();
				}
				this.inCd = this.IsSkillInCD(skillByIndex);
			}
		}
		if (!this.hero.isLive && this.deathTimer == null)
		{
			this.deathTimer = this.cMgr.StartCoroutine(this.DeathTimer(), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator DeathTimer()
	{
		FriendInfoUpdate.<DeathTimer>c__IteratorCE <DeathTimer>c__IteratorCE = new FriendInfoUpdate.<DeathTimer>c__IteratorCE();
		<DeathTimer>c__IteratorCE.<>f__this = this;
		return <DeathTimer>c__IteratorCE;
	}

	private void Init()
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (player == null)
		{
			return;
		}
		IList<Units> allHeroes = MapManager.Instance.GetAllHeroes();
		int num = 0;
		this.hero = null;
		foreach (Units current in allHeroes)
		{
			if (!current.MirrorState)
			{
				if (current.teamType == player.teamType)
				{
					if (!(current == player) || !(LevelManager.CurLevelId != "80007"))
					{
						if (num == this.idx)
						{
							this.hero = current;
							break;
						}
						num++;
					}
				}
			}
		}
		this.InitFriendInfo();
	}

	private void UpdateLeftTime(float leftTime)
	{
		this.timeLabel.text = ((int)leftTime).ToString();
		this.lastLeftTime = leftTime;
	}

	public void SetLive()
	{
		this.maskSprite.gameObject.SetActive(false);
		this.timeLabel.gameObject.SetActive(false);
		this.isLive = true;
		if (this.deathTimer != null)
		{
			this.cMgr.StopCoroutine(this.deathTimer);
			this.deathTimer = null;
		}
	}

	public void SetDead()
	{
		this.maskSprite.gameObject.SetActive(true);
		this.timeLabel.gameObject.SetActive(true);
		float playerDeathLastTime = BattleAttrManager.Instance.GetPlayerDeathLastTime(this.hero.unique_id);
		this.UpdateLeftTime(playerDeathLastTime);
		this.isLive = false;
	}

	public void TryUpdateHp()
	{
		float num = this.hero.hp / this.hero.hp_max;
		if (Time.realtimeSinceStartup - this.lastTime > 5f || (Math.Abs(this.lastHpPercent - num) > 0.02f && Time.realtimeSinceStartup - this.lastTime > 0.5f))
		{
			this.hpSprite.fillAmount = num;
			this.lastHpPercent = num;
			this.lastTime = Time.realtimeSinceStartup;
		}
	}

	private void InitFriendInfo()
	{
		if (this.hero == null)
		{
			return;
		}
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.hero.npc_id);
		if (heroMainData != null)
		{
			this.headTexture.spriteName = heroMainData.avatar_icon;
			this.hpSprite.fillAmount = this.hero.hp / this.hero.hp_max;
		}
		if (this.hero.isLive)
		{
			this.SetLive();
		}
		else
		{
			this.SetDead();
		}
		this.TryUpdateHp();
		Skill skillByIndex = this.hero.getSkillByIndex(3);
		if (skillByIndex != null && (!this.SkillLearned(skillByIndex) || this.IsSkillInCD(skillByIndex)))
		{
			this.SetSkill4NotReady();
		}
		else
		{
			this.SetSkill4Ready();
		}
	}

	private void SetSkill4Ready()
	{
		this.livePointSprite.gameObject.SetActive(true);
		this.deadPointSprite.gameObject.SetActive(false);
	}

	private void SetSkill4NotReady()
	{
		this.livePointSprite.gameObject.SetActive(false);
		this.deadPointSprite.gameObject.SetActive(true);
	}

	private bool SkillLearned(Skill skill)
	{
		return this.hero.skillManager != null && this.hero.skillManager.GetSkillLevel(skill.skillMainId) > 0;
	}

	private bool IsSkillInCD(Skill skill)
	{
		return this.hero.GetCDTime(skill.skillMainId) > 0f;
	}
}
