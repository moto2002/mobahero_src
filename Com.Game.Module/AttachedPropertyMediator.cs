using MobaHeros.Pvp;
using MobaHeros.Spawners;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class AttachedPropertyMediator : MonoBehaviour
	{
		private class GroupData
		{
			public int lm_lv;

			public int bl_lv;

			public int lm_kill;

			public int bl_kill;

			public float lm_ExpRatio;

			public float bl_ExpRatio;
		}

		private AttachedPropertyMediator.GroupData _data;

		private static AttachedPropertyMediator instance;

		public UILabel myLv;

		public UILabel enemyLv;

		public GameObject myEffect;

		public Animator myAnimator;

		public Animator enemyAnimator;

		public GameObject enemyEffect;

		public UISprite myFore;

		public UISprite enemyFore;

		public UILabel myTeamKill;

		public UILabel enemyTeamKill;

		public Transform selfAnchor;

		public Transform selfPos;

		private Transform anchor;

		private Transform replayAnchor;

		public UILabel timeLabel;

		private Dictionary<int, StatisticsManager> myHeroStatistics;

		private Dictionary<int, StatisticsManager> enemyHerosStatistics;

		private KillData cacheData;

		private int nowType = 1;

		private float interval = 0.5f;

		private float lastTime;

		private float _animationTime = 0.5f;

		public static AttachedPropertyMediator Instance
		{
			get
			{
				return AttachedPropertyMediator.instance;
			}
			set
			{
				AttachedPropertyMediator.instance = value;
			}
		}

		public Vector3 GetSelfPoint()
		{
			return this.selfPos.localPosition;
		}

		public void Start()
		{
			AttachedPropertyMediator.Instance = this;
			if (LevelManager.CurBattleType == 11)
			{
				this.SetType(2);
			}
			else
			{
				this.SetType(1);
			}
			this._data = new AttachedPropertyMediator.GroupData();
			this.SetMyForePercent(0f);
			this.SetEnemyForePercent(0f);
			this.SetMyTeamLv(1);
			this.SetEnemyTeamLv(1);
			this.SetTeamKill(true, 0);
			this.SetTeamKill(false, 0);
		}

		public void UpdateColor()
		{
			if (PlayerControlMgr.Instance.GetPlayer().teamType == 1)
			{
				Color color = this.enemyTeamKill.color;
				this.enemyTeamKill.color = this.myTeamKill.color;
				this.myTeamKill.color = color;
			}
		}

		public void SetType(int type)
		{
			if (this.anchor == null)
			{
				this.anchor = base.transform.Find("Anchor");
			}
			if (this.replayAnchor == null)
			{
				this.replayAnchor = base.transform.Find("ReplayAnchor");
			}
			if (type == 1)
			{
				this.myLv = base.transform.Find("Anchor/StateSelf/scale/teamLv").GetComponent<UILabel>();
				this.enemyLv = base.transform.Find("Anchor/StateEnemy/scale/teamLv").GetComponent<UILabel>();
				this.myFore = base.transform.Find("Anchor/StateSelf/fore").GetComponent<UISprite>();
				this.enemyFore = base.transform.Find("Anchor/StateEnemy/fore").GetComponent<UISprite>();
				this.myTeamKill = base.transform.Find("Anchor/StateSelf/kill").GetComponent<UILabel>();
				this.enemyTeamKill = base.transform.Find("Anchor/StateEnemy/kill").GetComponent<UILabel>();
				this.anchor.gameObject.SetActive(true);
				this.replayAnchor.gameObject.SetActive(false);
			}
			else
			{
				this.myLv = base.transform.Find("ReplayAnchor/StateSelf/scale/teamLv").GetComponent<UILabel>();
				this.enemyLv = base.transform.Find("ReplayAnchor/StateEnemy/scale/teamLv").GetComponent<UILabel>();
				this.myFore = base.transform.Find("ReplayAnchor/StateSelf/fore").GetComponent<UISprite>();
				this.enemyFore = base.transform.Find("ReplayAnchor/StateEnemy/fore").GetComponent<UISprite>();
				this.myTeamKill = base.transform.Find("ReplayAnchor/StateSelf/kill").GetComponent<UILabel>();
				this.enemyTeamKill = base.transform.Find("ReplayAnchor/StateEnemy/kill").GetComponent<UILabel>();
				this.anchor.gameObject.SetActive(false);
				this.replayAnchor.gameObject.SetActive(true);
			}
			this.SetMyForePercent(0f);
			this.SetEnemyForePercent(0f);
			this.SetMyTeamLv(1);
			this.SetEnemyTeamLv(1);
			this.SetTeamKill(true, 0);
			this.SetTeamKill(false, 0);
		}

		private void OnEnable()
		{
			Units.OnUnitsDead += new Action<Units, Units>(this.OnUnitsDead);
		}

		private void Update()
		{
			if (this.lastTime + this.interval > Time.time)
			{
				return;
			}
			this.lastTime = Time.time;
			this.UpdateKill();
		}

		private void OnDisable()
		{
			Units.OnUnitsDead -= new Action<Units, Units>(this.OnUnitsDead);
		}

		private void UpdateData()
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player == null)
			{
				return;
			}
			if (LevelManager.Instance.IsPvpBattleType)
			{
				PvpStatisticMgr.GroupData groupData = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(0);
				PvpStatisticMgr.GroupData groupData2 = Singleton<PvpManager>.Instance.StatisticMgr.GetGroupData(1);
				this._data.lm_kill = groupData.TeamKill;
				this._data.lm_lv = groupData.TeamLv;
				this._data.lm_ExpRatio = groupData.TeamExpRatio;
				this._data.bl_kill = groupData2.TeamKill;
				this._data.bl_lv = groupData2.TeamLv;
				this._data.bl_ExpRatio = groupData2.TeamExpRatio;
			}
			else
			{
				this._data.lm_kill = AttachedPropertyMediator.GetTotalHeroKill(TeamType.LM);
				this._data.bl_kill = AttachedPropertyMediator.GetTotalHeroKill(TeamType.BL);
				this._data.lm_lv = BattleAttrManager.Instance.LMAttrVale.TeamLevel;
				this._data.bl_lv = BattleAttrManager.Instance.BLAttrValue.TeamLevel;
			}
		}

		private void UpdateKill()
		{
			if (GameManager.IsGameOver())
			{
				return;
			}
			this.UpdateData();
			this.SetTeamKill(true, this._data.lm_kill);
			this.SetTeamKill(false, this._data.bl_kill);
			this.SetTeamLv(true, this._data.lm_lv);
			this.SetTeamLv(false, this._data.bl_lv);
		}

		private void FlyEffectToExpBar(Units unit)
		{
		}

		private void OnUnitsDead(Units unit, Units attacker)
		{
			if (unit && TeamManager.IsEnemy(unit) && SceneInfo.Current.IsOpenAdditionFactor)
			{
				this.FlyEffectToExpBar(unit);
			}
		}

		public static int GetTotalHeroKill(TeamType team)
		{
			int num = 0;
			if (GameManager.Instance.AchieveManager == null)
			{
				return num;
			}
			Dictionary<int, AchieveData> achieveDatasByTeam = GameManager.Instance.AchieveManager.GetAchieveDatasByTeam((int)team);
			if (achieveDatasByTeam != null)
			{
				foreach (KeyValuePair<int, AchieveData> current in achieveDatasByTeam)
				{
					num += current.Value.TotalKill;
				}
			}
			return num;
		}

		public int GetTotalHeroDeath(TeamType team)
		{
			int num = 0;
			Dictionary<int, AchieveData> achieveDatasByTeam = GameManager.Instance.AchieveManager.GetAchieveDatasByTeam((int)team);
			if (achieveDatasByTeam != null)
			{
				foreach (KeyValuePair<int, AchieveData> current in achieveDatasByTeam)
				{
					num += current.Value.SelfDeathTime;
				}
			}
			return num;
		}

		public void StartAnimateDate(int levelChange, int orginLevel, float originRate, float curRate, bool isMine, int maxLv)
		{
		}

		[DebuggerHidden]
		private IEnumerator AnimateDate(int levelChange, int orginLevel, float originRate, float curRate, bool isMine, int maxLv)
		{
			AttachedPropertyMediator.<AnimateDate>c__IteratorC5 <AnimateDate>c__IteratorC = new AttachedPropertyMediator.<AnimateDate>c__IteratorC5();
			<AnimateDate>c__IteratorC.levelChange = levelChange;
			<AnimateDate>c__IteratorC.originRate = originRate;
			<AnimateDate>c__IteratorC.curRate = curRate;
			<AnimateDate>c__IteratorC.isMine = isMine;
			<AnimateDate>c__IteratorC.orginLevel = orginLevel;
			<AnimateDate>c__IteratorC.maxLv = maxLv;
			<AnimateDate>c__IteratorC.<$>levelChange = levelChange;
			<AnimateDate>c__IteratorC.<$>originRate = originRate;
			<AnimateDate>c__IteratorC.<$>curRate = curRate;
			<AnimateDate>c__IteratorC.<$>isMine = isMine;
			<AnimateDate>c__IteratorC.<$>orginLevel = orginLevel;
			<AnimateDate>c__IteratorC.<$>maxLv = maxLv;
			<AnimateDate>c__IteratorC.<>f__this = this;
			return <AnimateDate>c__IteratorC;
		}

		private void ShowLevelUpEffect()
		{
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(TeamType.LM, TargetTag.Hero);
			if (mapUnits != null)
			{
				for (int i = 0; i < mapUnits.Count; i++)
				{
					mapUnits[i].effectManager.StartLevelUpEffect();
					if (mapUnits[i].isPlayer)
					{
					}
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator DoAnimation(float begin, float end, float time, bool isMine)
		{
			AttachedPropertyMediator.<DoAnimation>c__IteratorC6 <DoAnimation>c__IteratorC = new AttachedPropertyMediator.<DoAnimation>c__IteratorC6();
			<DoAnimation>c__IteratorC.end = end;
			<DoAnimation>c__IteratorC.begin = begin;
			<DoAnimation>c__IteratorC.time = time;
			<DoAnimation>c__IteratorC.isMine = isMine;
			<DoAnimation>c__IteratorC.<$>end = end;
			<DoAnimation>c__IteratorC.<$>begin = begin;
			<DoAnimation>c__IteratorC.<$>time = time;
			<DoAnimation>c__IteratorC.<$>isMine = isMine;
			<DoAnimation>c__IteratorC.<>f__this = this;
			return <DoAnimation>c__IteratorC;
		}

		public void SetTeamKill(bool isMine, int level)
		{
			if (isMine)
			{
				this.SetMyTeamKill(level);
			}
			else
			{
				this.SetEnemyTeamKill(level);
			}
		}

		public void SetTeamLv(bool isMine, int lv)
		{
			if (isMine)
			{
				this.SetMyTeamLv(lv);
			}
			else
			{
				this.SetEnemyTeamLv(lv);
			}
		}

		public void SetMyForePercent(float percent)
		{
			this.myFore.fillAmount = percent;
		}

		public void SetEnemyForePercent(float percent)
		{
			this.enemyFore.fillAmount = percent;
		}

		public void SetMyTeamKill(int level)
		{
			this.myTeamKill.text = level.ToString();
		}

		public void SetEnemyTeamKill(int level)
		{
			this.enemyTeamKill.text = level.ToString();
		}

		public void SetMyTeamLv(int val)
		{
			this.myLv.text = val.ToString("f0");
		}

		public void SetEnemyTeamLv(int val)
		{
			this.enemyLv.text = val.ToString("f0");
		}

		public void AnimateMyProperty()
		{
			this.myAnimator.SetTrigger("IsAnimating");
		}

		public void AnimateEnemyProperty()
		{
			this.enemyAnimator.SetTrigger("IsAnimating");
		}
	}
}
