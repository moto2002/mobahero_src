using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class UIBloodBar : MonoBehaviour
	{
		private enum HpbarType
		{
			CurHero = 1,
			FriendHero,
			FriendSodior,
			FriendTower,
			FriendHome,
			EnemyHero,
			EnemySodior,
			EnemyTower,
			EnemyHome
		}

		private const float animTime = 0.3f;

		[SerializeField]
		private UISlider TimerLine;

		[SerializeField]
		private UISprite m_frontBloodSprite;

		[SerializeField]
		private UISprite m_bloodRuleSprite;

		[SerializeField]
		private GameObject m_protectObj;

		[SerializeField]
		private GameObject m_bloodContainer;

		private UISprite m_hudunSprite;

		private UIWidget m_bloodWidget;

		[SerializeField]
		private UISprite m_slideBloodSprite;

		[SerializeField]
		private UISprite m_blueBloodSprite;

		[SerializeField]
		private UISprite m_backgroundSprite;

		[SerializeField]
		private UILabel m_debuffName;

		[SerializeField]
		private UILabel m_SummonerName;

		[SerializeField]
		private UISprite m_AITime;

		[SerializeField]
		private UILabel m_AITimeLabel;

		[SerializeField]
		private UILabel m_LevelLabel;

		[SerializeField]
		private UISprite m_ExperienceBar;

		protected float high = 3f;

		protected float angle;

		protected float maxlife;

		protected float Dunlife;

		protected float camerasize;

		protected float scale;

		private float m_height;

		[NonSerialized]
		public bool isFollow;

		private Units m_targetUnit;

		private Transform m_targetUnitTrans;

		private Transform m_healthBarTrans;

		private bool m_bIsRegister;

		[NonSerialized]
		public bool m_IsActive = true;

		[NonSerialized]
		public bool m_isCurVisible = true;

		[NonSerialized]
		public bool isChangeVisible = true;

		private float m_curHP;

		private float m_maxHP;

		private float m_curMagic;

		private float m_maxMagic;

		private float m_timeInterval;

		private float m_originWidth = 159f;

		private float m_curHuDun;

		private List<GameObject> keduList;

		private List<GameObject> keduList2;

		private float bloodcount;

		private float prewid;

		private int keducount;

		private float tempBoold;

		private float oldMaxHp;

		private float oldHudun;

		private float oldHp;

		private float m_curShield;

		private int lv;

		private bool fxPlayerShowed;

		private GameObject Fx_player_arrow;

		private UIPanel m_panel;

		private CoroutineManager m_CoroutineManager = new CoroutineManager();

		private CoroutineManager m_CoroutineManagerMark = new CoroutineManager();

		private UISprite[] sprites;

		private UILabel nameLabel;

		private UILabel levelLabel;

		private int _lastHpWidth = -1;

		private float _lastMpWidth = -1f;

		private int _lastExpWidth = -1;

		private bool _lastUpdate;

		private bool _needUpdate;

		private int animCount;

		public string BloodType
		{
			get;
			set;
		}

		private void Awake()
		{
			this.m_originWidth = (float)this.m_frontBloodSprite.width;
			this.m_healthBarTrans = base.gameObject.transform;
			this.m_panel = base.GetComponent<UIPanel>();
		}

		private void Start()
		{
			this.Register();
			this.keduList = new List<GameObject>();
			this.keduList2 = new List<GameObject>();
		}

		public void SetLiveTime(float fValue)
		{
			if (this.TimerLine == null)
			{
				return;
			}
			this.TimerLine.value = fValue;
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
			if (PostMist.Instance != null && PostMist.Instance.doMist)
			{
				Transform transform = base.transform;
				int childCount = base.transform.childCount;
				List<UISprite> list = new List<UISprite>(childCount);
				for (int i = 0; i < childCount; i++)
				{
					UISprite component = base.transform.GetChild(i).GetComponent<UISprite>();
					if (component != null)
					{
						component.enabled = false;
						list.Add(component);
					}
				}
				this.sprites = list.ToArray();
				Transform transform2 = transform.Find("SummonerName");
				if (transform2)
				{
					this.nameLabel = transform2.gameObject.GetComponent<UILabel>();
					if (this.nameLabel)
					{
						this.nameLabel.enabled = false;
					}
				}
				Transform transform3 = transform.FindChild("FrontBlood/Level");
				if (transform3 != null)
				{
					this.levelLabel = transform3.gameObject.GetComponent<UILabel>();
					if (this.levelLabel)
					{
						this.levelLabel.enabled = false;
					}
				}
			}
		}

		private void UnRegister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
		}

		private void BattleEnd(MobaMessage msg)
		{
			this.UnRegisterEvent();
		}

		private void FixedUpdate()
		{
			if (this.m_targetUnitTrans == null && base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			this.UnRegister();
			this.UnRegisterEvent();
			if (this.keduList != null)
			{
				this.keduList.Clear();
			}
			if (this.keduList2 != null)
			{
				this.keduList2.Clear();
			}
		}

		private void RegisterEvent()
		{
			if (GlobalSettings.Instance.UIOpt)
			{
				return;
			}
			this.m_targetUnit.OnWoundCallback += new Callback<Units>(this.CallWhenInjured);
			this.m_targetUnit.OnDeathCallback += new Callback<Units>(this.CallWhenDeath);
			this.m_targetUnit.OnRebornCallback += new Callback<Units>(this.CallWhenInjured);
			if (this.m_targetUnit.isHero)
			{
				this.m_targetUnit.OnUseMagic += new Callback<float, float>(this.CallUseMagic);
			}
			this.m_bIsRegister = true;
		}

		public void UnRegisterEvent()
		{
			if (GlobalSettings.Instance.UIOpt)
			{
				return;
			}
			if (this.m_bIsRegister)
			{
				this.m_targetUnit.OnWoundCallback -= new Callback<Units>(this.CallWhenInjured);
				this.m_targetUnit.OnDeathCallback -= new Callback<Units>(this.CallWhenDeath);
				this.m_targetUnit.OnRebornCallback -= new Callback<Units>(this.CallWhenInjured);
				if (this.m_targetUnit.isHero)
				{
					this.m_targetUnit.OnUseMagic -= new Callback<float, float>(this.CallUseMagic);
				}
				this.m_bIsRegister = false;
			}
		}

		public void CallWhenInjured(Units owner)
		{
			this.m_curHP = owner.hp;
			this.m_maxHP = owner.hp_max;
			if (!this.m_isCurVisible)
			{
				return;
			}
			if (!this._needUpdate)
			{
				return;
			}
			if (GlobalSettings.Instance.UIOpt)
			{
				return;
			}
			if (this.m_targetUnit.isHero || TagManager.CheckTag(this.m_targetUnit, global::TargetTag.EyeUnit))
			{
				this.UpdateRule(this.m_maxHP);
			}
			if (!base.gameObject.activeSelf && this.m_IsActive)
			{
				base.gameObject.SetActive(true);
			}
			this.UpdateFrontHealthBar();
			this.UpdateHuDun(this.m_curShield, this.m_curHP, false);
			if (this.m_targetUnit.isHero || TagManager.CheckTag(this.m_targetUnit, global::TargetTag.EyeUnit))
			{
				this.UpdateRule(this.m_maxHP + this.m_curShield);
			}
		}

		public void CallUseMagic(float curMgic, float maxMgic)
		{
			this.m_curMagic = curMgic;
			this.m_maxMagic = maxMgic;
			if (!this.m_isCurVisible)
			{
				return;
			}
			if (!this._needUpdate)
			{
				return;
			}
			if (GlobalSettings.Instance.UIOpt)
			{
				return;
			}
			if (!base.gameObject.activeSelf && this.m_IsActive)
			{
				base.gameObject.SetActive(true);
			}
			this.UpdateFrontMagicBar();
		}

		private void CallWhenDeath(Units owner)
		{
			if (GlobalSettings.Instance.UIOpt)
			{
				this.setActive(false);
			}
			else if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void UpdateFrontHealthBar()
		{
			this.m_curHP = this.m_targetUnit.hp;
			this.m_maxHP = this.m_targetUnit.hp_max;
			this.m_curHuDun = this.m_targetUnit.shield;
			if (this.m_curHP > this.m_maxHP)
			{
				this.m_curHP = this.m_maxHP;
			}
			int num;
			if (this.m_curHP + this.m_curHuDun <= this.m_maxHP)
			{
				num = (int)(this.m_curHP / this.m_maxHP * this.m_originWidth);
			}
			else
			{
				num = (int)(this.m_curHP / (this.m_curHP + this.m_curHuDun) * this.m_originWidth);
			}
			if (this.m_frontBloodSprite && (this._lastHpWidth < 0 || (float)Mathf.Abs(this._lastHpWidth - num) > this.m_originWidth * 0.03f))
			{
				this._lastHpWidth = num;
				this.CheckSlidBloodShow(num);
				this.SetFrontBarWidth(num);
				if (!this.isChangeVisible)
				{
					this.UpdateHuDun(this.m_targetUnit.shield, this.m_targetUnit.hp, false);
				}
			}
			if (this.isChangeVisible && this.m_isCurVisible)
			{
				this.isChangeVisible = false;
				this.m_slideBloodSprite.width = this.m_frontBloodSprite.width;
				this.UpdateHuDun(this.m_targetUnit.shield, this.m_targetUnit.hp, true);
			}
		}

		private void CheckSlidBloodShow(int width)
		{
			this.m_frontBloodSprite.alpha = (float)((width > 0) ? 1 : 0);
			if (this.isChangeVisible || width <= 0)
			{
				this.m_slideBloodSprite.alpha = 0f;
			}
			else
			{
				this.m_slideBloodSprite.alpha = 1f;
			}
		}

		private void UpdateFrontMagicBar()
		{
			if (this.m_curMagic > this.m_maxMagic)
			{
				this.m_curMagic = this.m_maxMagic;
			}
			if (this.m_blueBloodSprite)
			{
				int num = (int)(this.m_curMagic / this.m_maxMagic * this.m_originWidth);
				if (this._lastMpWidth < 0f || Math.Abs((float)num - this._lastMpWidth) > 5f)
				{
					this._lastMpWidth = (float)num;
					this.m_blueBloodSprite.width = num;
				}
			}
		}

		private void UpdateExperienceBar()
		{
			if (this.m_targetUnit == null || this.m_targetUnit.tag != "Player" || LevelManager.CurBattleType == 6)
			{
				return;
			}
			float num = UtilManager.Instance.GetHeroExpRatio(this.m_targetUnit.unique_id);
			if (num > 1f)
			{
				num = 1f;
			}
			int num2 = (int)(this.m_originWidth * num);
			if (this._lastExpWidth < 0 || Math.Abs(num2 - this._lastExpWidth) > 5)
			{
				this._lastExpWidth = num2;
				this.m_ExperienceBar.SetDimensions(num2, 4);
			}
		}

		private void TryUpdateSlideHealthBarAnim()
		{
			if (this.m_slideBloodSprite.width > this.m_frontBloodSprite.width && this.m_slideBloodSprite.width > 0)
			{
				this.m_slideBloodSprite.width -= 5;
				if (this.m_slideBloodSprite.width <= this.m_frontBloodSprite.width)
				{
					this.m_slideBloodSprite.width = this.m_frontBloodSprite.width;
					if (this.m_slideBloodSprite.width < 0)
					{
						this.m_slideBloodSprite.width = 0;
					}
					this.CheckSlidBloodShow(this.m_slideBloodSprite.width);
				}
			}
		}

		public virtual void UpdateView()
		{
			if (this.m_targetUnit != null && this.m_targetUnit.isLive)
			{
				this.UpdateHudBarType(this.m_targetUnit);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		private void SetFrontBarWidth(int w)
		{
			if (this.m_frontBloodSprite)
			{
				this.m_frontBloodSprite.width = w;
			}
			if (this.m_slideBloodSprite && this.m_slideBloodSprite.width < w)
			{
				this.m_slideBloodSprite.width = w;
			}
		}

		private void DoUpdate(bool updateImmediately = false)
		{
			Vector3 hudbarWorldPos = this.m_targetUnit.surface.HudbarWorldPos;
			hudbarWorldPos.z = 0f;
			this.m_healthBarTrans.position = hudbarWorldPos;
			if (!updateImmediately && Time.frameCount % 2 == 0)
			{
				return;
			}
			this.UpdateFrontHealthBar();
			this.m_curShield = this.m_targetUnit.shield;
			this.m_curHP = this.m_targetUnit.hp;
			this.UpdateHuDun(this.m_targetUnit.shield, this.m_targetUnit.hp, false);
			if (this.m_targetUnit.isHero || TagManager.CheckTag(this.m_targetUnit, global::TargetTag.EyeUnit))
			{
				this.UpdateRule(this.m_maxHP + this.m_curShield);
			}
			int level = this.m_targetUnit.level;
			if (this.m_LevelLabel != null && this.lv != level)
			{
				this.lv = level;
				this.m_LevelLabel.text = level.ToString();
			}
			this.UpdateExperienceBar();
			if (this.m_blueBloodSprite != null)
			{
				this.m_curMagic = this.m_targetUnit.mp;
				this.m_maxMagic = this.m_targetUnit.mp_max;
				this.UpdateFrontMagicBar();
			}
			this.TryUpdateSlideHealthBarAnim();
		}

		public void SkipUpdate()
		{
			this._needUpdate = false;
			this._lastUpdate = false;
			this.m_healthBarTrans.position = new Vector3(10000f, 10000f, 0f);
		}

		public void OnUpdate(bool newVisible, bool updateImmediately = false)
		{
			this._needUpdate = true;
			if (this.m_targetUnit == null || !this.m_targetUnit.isLive || (!this.m_targetUnit.IsRebirthing && this.m_targetUnit.hp <= 0f))
			{
				this.SkipUpdate();
			}
			newVisible = (this.m_IsActive && newVisible);
			bool lastUpdate = newVisible;
			if (this.m_isCurVisible != newVisible)
			{
				this.m_isCurVisible = newVisible;
				if (this.m_isCurVisible)
				{
					this.isChangeVisible = true;
				}
				this.FastSetVisible(this.m_isCurVisible);
			}
			if (updateImmediately)
			{
				this.m_curHP = this.m_targetUnit.hp;
				this.m_maxHP = this.m_targetUnit.hp_max;
				this._lastUpdate = false;
			}
			this.DoUpdate(updateImmediately);
			if (!this._lastUpdate && (double)this.m_maxHP > 0.1 && (double)this.m_originWidth > 0.1)
			{
				this.m_CoroutineManager.StopAllCoroutine();
				int width = (int)(this.m_curHP * this.m_originWidth / this.m_maxHP);
				this.CheckSlidBloodShow(width);
				this.m_slideBloodSprite.width = width;
			}
			this._lastUpdate = lastUpdate;
		}

		public void SetTargetUnit(Units targetUnit)
		{
			this.m_targetUnit = targetUnit;
			this.m_targetUnitTrans = targetUnit.transform;
			this.UpdateHudBarType(targetUnit);
			this.SetFrontBarWidth((int)this.m_originWidth);
			this.UpdateHuDun(targetUnit.shield, targetUnit.hp, false);
			if (this.m_blueBloodSprite)
			{
				this.m_blueBloodSprite.width = (int)this.m_originWidth;
			}
			if (this.m_ExperienceBar)
			{
				this.m_ExperienceBar.width = (int)this.m_originWidth;
			}
			if (!base.gameObject.activeSelf && this.m_IsActive)
			{
				base.gameObject.SetActive(true);
			}
			this.RegisterEvent();
		}

		public virtual void UpdateHudBarType(Units targetUnit)
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				this.ShowObserverHpbarType(targetUnit);
			}
			else
			{
				this.ShowNormalHpbarType(targetUnit);
			}
		}

		private void ShowNormalHpbarType(Units targetUnit)
		{
			if (targetUnit.isMyTeam)
			{
				string tag = targetUnit.tag;
				switch (tag)
				{
				case "Player":
					this.ShowMainHeroMark();
					this.SetHPType(UIBloodBar.HpbarType.CurHero, targetUnit);
					break;
				case "Hero":
					this.SetHPType(UIBloodBar.HpbarType.FriendHero, targetUnit);
					break;
				case "Monster":
					this.SetHPType((!targetUnit.isCreep) ? UIBloodBar.HpbarType.FriendSodior : UIBloodBar.HpbarType.FriendTower, null);
					break;
				case "Building":
					this.SetHPType(UIBloodBar.HpbarType.FriendTower, null);
					break;
				case "Home":
					this.SetHPType(UIBloodBar.HpbarType.FriendHome, null);
					break;
				}
			}
			else
			{
				string tag = targetUnit.tag;
				switch (tag)
				{
				case "Hero":
					this.SetHPType(UIBloodBar.HpbarType.EnemyHero, targetUnit);
					break;
				case "Monster":
					this.SetHPType((!targetUnit.isCreep) ? UIBloodBar.HpbarType.EnemySodior : UIBloodBar.HpbarType.EnemyTower, null);
					break;
				case "Building":
					this.SetHPType(UIBloodBar.HpbarType.EnemyTower, null);
					break;
				case "Home":
					this.SetHPType(UIBloodBar.HpbarType.EnemyHome, null);
					break;
				}
			}
		}

		private void ShowObserverHpbarType(Units targetUnit)
		{
			if (targetUnit.TeamType == TeamType.LM)
			{
				string tag = targetUnit.tag;
				switch (tag)
				{
				case "Player":
				case "Hero":
					this.SetHPType(UIBloodBar.HpbarType.FriendHero, targetUnit);
					break;
				case "Monster":
					this.SetHPType((!targetUnit.isCreep) ? UIBloodBar.HpbarType.FriendSodior : UIBloodBar.HpbarType.FriendTower, null);
					break;
				case "Building":
					this.SetHPType(UIBloodBar.HpbarType.FriendTower, null);
					break;
				case "Home":
					this.SetHPType(UIBloodBar.HpbarType.FriendHome, null);
					break;
				}
			}
			else
			{
				string tag = targetUnit.tag;
				switch (tag)
				{
				case "Hero":
					this.SetHPType(UIBloodBar.HpbarType.EnemyHero, targetUnit);
					break;
				case "Monster":
					this.SetHPType((!targetUnit.isCreep) ? UIBloodBar.HpbarType.EnemySodior : UIBloodBar.HpbarType.EnemyTower, null);
					break;
				case "Building":
					this.SetHPType(UIBloodBar.HpbarType.EnemyTower, null);
					break;
				case "Home":
					this.SetHPType(UIBloodBar.HpbarType.EnemyHome, null);
					break;
				}
			}
		}

		public void SetBloodBar(bool inIsMyTeam)
		{
			this.SetHPType((!inIsMyTeam) ? UIBloodBar.HpbarType.EnemySodior : UIBloodBar.HpbarType.FriendSodior, null);
		}

		public void setActive(bool isActive)
		{
			this.m_IsActive = isActive;
			if (!GlobalSettings.Instance.UIOpt)
			{
				this.FastSetVisible(isActive);
				return;
			}
			if (this.m_isCurVisible != (this.m_IsActive && this.m_isCurVisible))
			{
				this.m_isCurVisible = (this.m_IsActive && this.m_isCurVisible);
				if (this.m_isCurVisible)
				{
					this.isChangeVisible = true;
				}
				this.FastSetVisible(this.m_isCurVisible);
			}
		}

		private void FastSetVisible(bool visible)
		{
			PerfTools.SetVisible(base.gameObject, visible);
		}

		private void SetHealthBarColor(Color frontColor, Color slideColor)
		{
			this.m_frontBloodSprite.color = frontColor;
			this.m_slideBloodSprite.color = slideColor;
		}

		private void SetSlideBarColor(Color color)
		{
			this.m_slideBloodSprite.color = new Color(color.r, color.g, color.b, this.m_slideBloodSprite.color.a);
		}

		private void SetHPType(UIBloodBar.HpbarType type, Units target = null)
		{
			if (target != null && LevelManager.m_CurLevel.Is3V3V3() && (type == UIBloodBar.HpbarType.CurHero || type == UIBloodBar.HpbarType.EnemyHero || type == UIBloodBar.HpbarType.FriendHero))
			{
				switch (target.TeamType)
				{
				case TeamType.LM:
					this.m_frontBloodSprite.spriteName = "Blood_3v3v3_bar_red";
					this.SetSlideBarColor(new Color32(182, 0, 0, 255));
					break;
				case TeamType.BL:
					this.m_frontBloodSprite.spriteName = "Blood_3v3v3_bar_blue";
					this.SetSlideBarColor(new Color32(182, 0, 0, 255));
					break;
				case TeamType.Team_3:
					this.m_frontBloodSprite.spriteName = "Blood_3v3v3_bar_yellow";
					this.SetSlideBarColor(new Color32(182, 0, 0, 255));
					break;
				}
				return;
			}
			switch (type)
			{
			case UIBloodBar.HpbarType.CurHero:
				this.m_frontBloodSprite.spriteName = "Hero_self_blodbar";
				this.SetSlideBarColor(new Color32(182, 0, 0, 255));
				break;
			case UIBloodBar.HpbarType.FriendHero:
				this.m_frontBloodSprite.spriteName = "Hero_friend_blodbar";
				this.SetSlideBarColor(new Color32(182, 0, 0, 255));
				break;
			case UIBloodBar.HpbarType.FriendSodior:
				this.m_frontBloodSprite.spriteName = "Selfsodior_blodbar";
				this.SetSlideBarColor(new Color32(182, 0, 0, 255));
				break;
			case UIBloodBar.HpbarType.FriendTower:
			case UIBloodBar.HpbarType.FriendHome:
				this.m_frontBloodSprite.spriteName = "Hero_friend_blodbar";
				this.SetSlideBarColor(new Color32(182, 0, 0, 255));
				break;
			case UIBloodBar.HpbarType.EnemyHero:
				this.m_frontBloodSprite.spriteName = "Enermy_bloodbar";
				this.SetSlideBarColor(new Color32(255, 198, 0, 255));
				break;
			case UIBloodBar.HpbarType.EnemySodior:
				this.m_frontBloodSprite.spriteName = "Sodior_enermy_bloodbar";
				this.SetSlideBarColor(new Color32(208, 142, 0, 255));
				break;
			case UIBloodBar.HpbarType.EnemyTower:
			case UIBloodBar.HpbarType.EnemyHome:
				this.m_frontBloodSprite.spriteName = "Enermy_bloodbar";
				this.SetSlideBarColor(new Color32(255, 198, 0, 255));
				break;
			}
		}

		public virtual void UpdateHeroLevel(int level)
		{
		}

		private void OnEnable()
		{
			this.m_isCurVisible = true;
			this.isChangeVisible = true;
			if (this.m_debuffName != null)
			{
				this.m_debuffName.alpha = 0f;
			}
		}

		private void OnDisable()
		{
			this.isChangeVisible = true;
			this.m_isCurVisible = false;
			this.m_CoroutineManagerMark.StopAllCoroutine();
			this.animCount = 0;
		}

		public void On_Spawn()
		{
			this.animCount = 0;
			this.m_isCurVisible = true;
			if (this.m_debuffName)
			{
				PerfTools.SetVisible(this.m_debuffName.gameObject, false);
			}
			if (this.m_SummonerName)
			{
				this.m_SummonerName.gameObject.SetActive(true);
			}
		}

		public void On_Despawn()
		{
			this.m_CoroutineManagerMark.StopAllCoroutine();
			this.animCount = 0;
			base.StopCoroutine("ShowDebuffAnim");
		}

		public void SetAIState(string str)
		{
		}

		public void SetBarAlpha(float alpha)
		{
			if (this.m_frontBloodSprite != null)
			{
				this.m_frontBloodSprite.alpha = alpha;
			}
			if (this.m_blueBloodSprite != null)
			{
				this.m_blueBloodSprite.alpha = alpha;
			}
			if (this.m_slideBloodSprite != null)
			{
				this.m_slideBloodSprite.alpha = alpha;
			}
			if (this.m_backgroundSprite != null)
			{
				this.m_backgroundSprite.alpha = alpha;
			}
			if (this.m_SummonerName != null)
			{
				this.m_SummonerName.alpha = alpha;
			}
			if (this.m_AITime != null)
			{
				this.m_AITime.alpha = alpha;
			}
			if (this.m_ExperienceBar != null)
			{
				this.m_ExperienceBar.alpha = alpha;
			}
		}

		public void SetDebuffIcon(bool active, string debuffName = "")
		{
			if (this.m_debuffName == null)
			{
				return;
			}
			if (active && !string.IsNullOrEmpty(debuffName))
			{
				PerfTools.SetVisible(this.m_debuffName.gameObject, true);
				this.m_debuffName.text = debuffName;
				if (this.m_SummonerName != null)
				{
					PerfTools.SetVisible(this.m_SummonerName.gameObject, false);
				}
				this.m_debuffName.animation.enabled = true;
				this.m_debuffName.animation.Play();
				this.animCount++;
				base.StopCoroutine("ShowDebuffAnim");
				if (base.gameObject == null)
				{
					return;
				}
				if (base.gameObject.activeSelf)
				{
					base.StartCoroutine("ShowDebuffAnim");
				}
			}
			else
			{
				this.animCount--;
				if (this.animCount <= 0)
				{
					PerfTools.SetVisible(this.m_debuffName.gameObject, false);
					this.m_debuffName.animation.Stop();
					this.m_debuffName.animation.enabled = false;
					TweenAlpha.Begin(this.m_debuffName.gameObject, 0.1f, 0.01f);
					if (this.m_SummonerName != null)
					{
						PerfTools.SetVisible(this.m_SummonerName.gameObject, true);
					}
					base.StopCoroutine("ShowDebuffAnim");
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator ShowDebuffAnim()
		{
			UIBloodBar.<ShowDebuffAnim>c__IteratorF0 <ShowDebuffAnim>c__IteratorF = new UIBloodBar.<ShowDebuffAnim>c__IteratorF0();
			<ShowDebuffAnim>c__IteratorF.<>f__this = this;
			return <ShowDebuffAnim>c__IteratorF;
		}

		public void ShowName(bool isShow, string name, int userid = 0)
		{
			if (this.m_SummonerName == null)
			{
				return;
			}
			PerfTools.SetVisible(this.m_SummonerName.gameObject, true);
			this.m_SummonerName.enabled = true;
			string text = name ?? string.Empty;
			this.m_SummonerName.text = text;
			if (userid < 0)
			{
				userid = -userid;
			}
			ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.TryFindPlayerInfo(userid, "GetHeroInfoData");
			if (readyPlayerSampleInfo != null)
			{
				this.m_SummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(readyPlayerSampleInfo.CharmRankvalue);
			}
		}

		public void ShowAITimer(int time, bool isActive)
		{
			if (!this.m_targetUnit.isPlayer)
			{
				return;
			}
			this.m_AITime.gameObject.SetActive(isActive);
			if (PerfTools.IsVisible(this.m_SummonerName.transform) && this.m_SummonerName.text != string.Empty)
			{
				this.m_AITime.gameObject.transform.localPosition = new Vector3(10f, 70f, 0f);
			}
			else
			{
				this.m_AITime.gameObject.transform.localPosition = new Vector3(10f, 48f, 0f);
			}
			this.m_AITimeLabel.text = time.ToString();
		}

		private void UpdateRule(float newMaxHp)
		{
			if (newMaxHp == this.oldMaxHp || newMaxHp > 10000f || newMaxHp < 0f)
			{
				return;
			}
			if (TagManager.CheckTag(this.m_targetUnit, global::TargetTag.EyeUnit) && (int)newMaxHp != (int)this.oldMaxHp)
			{
				int num = 90;
				this.m_bloodRuleSprite.spriteName = "BloodRule500";
				float num2 = 1.66666663f;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = 0.6f;
				this.m_bloodRuleSprite.fillAmount = num2;
				this.oldMaxHp = newMaxHp;
				return;
			}
			if ((int)newMaxHp / 100 == (int)this.oldMaxHp / 100)
			{
				return;
			}
			this.oldMaxHp = newMaxHp;
			this.SetBloodRuleSpriteInfo(newMaxHp);
		}

		private void SetBloodRuleSpriteInfo(float inMaxHp)
		{
			if (inMaxHp < 1f)
			{
				return;
			}
			int num = 158;
			if (inMaxHp <= 500.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule500";
				float num2 = 500f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 500f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 700.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule700";
				float num2 = 700f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 700f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 1000.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule1000";
				float num2 = 1000f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 1000f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 1500.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule1500";
				float num2 = 1500f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 1500f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 2000.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule2000";
				float num2 = 2000f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 2000f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 3000.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule3000";
				float num2 = 3000f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 3000f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else if (inMaxHp <= 4000.001f)
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule4000";
				float num2 = 4000f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = inMaxHp / 4000f;
				this.m_bloodRuleSprite.fillAmount = num2;
			}
			else
			{
				this.m_bloodRuleSprite.spriteName = "BloodRule5000";
				float num2 = 5000f / inMaxHp;
				this.m_bloodRuleSprite.width = (int)((float)num * num2);
				num2 = Mathf.Clamp01(inMaxHp / 5000f);
				this.m_bloodRuleSprite.fillAmount = num2;
			}
		}

		public void EnableLeader(bool enabled)
		{
			Transform transform = base.transform.Find("Leader");
			if (transform)
			{
				transform.gameObject.SetActive(enabled);
			}
		}

		public void UpdateHuDun(float HD, float HP, bool forceUpdate = false)
		{
			if (this.oldHudun == HD && this.oldHp == HP && !forceUpdate)
			{
				return;
			}
			this.oldHudun = HD;
			this.oldHp = HP;
			if (HD <= 0f)
			{
				if (this.m_hudunSprite != null && this.m_hudunSprite.gameObject.activeInHierarchy)
				{
					this.m_hudunSprite.gameObject.SetActive(false);
				}
				return;
			}
			if (this.m_hudunSprite != null && !this.m_hudunSprite.gameObject.activeInHierarchy)
			{
				this.m_hudunSprite.gameObject.SetActive(true);
			}
			if (this.m_bloodContainer == null)
			{
				return;
			}
			if (this.m_bloodWidget == null)
			{
				this.m_bloodWidget = this.m_bloodContainer.GetComponent<UIWidget>();
				ClientLogger.Assert(this.m_bloodWidget, this.m_bloodContainer + " has no UIWidget");
			}
			if (this.m_hudunSprite == null)
			{
				this.m_hudunSprite = this.m_protectObj.GetComponent<UISprite>();
			}
			if (this.m_curHP + HD <= this.m_maxHP)
			{
				if (this.m_curHP < 1f || this.m_maxHP < 1f)
				{
					this.m_curHP = 1f;
					this.m_maxHP = 1f;
					HD = 0f;
				}
				int num = (int)((float)this.m_frontBloodSprite.width / this.m_curHP * HD);
				if (num > 159)
				{
					num = 0;
				}
				this.m_hudunSprite.SetDimensions(num, this.m_hudunSprite.height);
			}
			else
			{
				if (this.m_curHP < 1f || this.m_maxHP < 1f)
				{
					this.m_curHP = 1f;
					this.m_maxHP = 1f;
					HD = 0f;
				}
				int num2 = (int)(HD / (this.m_curHP + HD) * this.m_originWidth);
				if (num2 > 159)
				{
					num2 = 0;
				}
				this.m_hudunSprite.SetDimensions(num2, this.m_hudunSprite.height);
			}
			this.m_hudunSprite.transform.localPosition = new Vector3((float)this.m_frontBloodSprite.width, 0f, 0f);
			this.m_hudunSprite.gameObject.SetActive(true);
			if (this.m_hudunSprite != null && this.m_panel != null && this.m_hudunSprite.panel != null)
			{
				this.m_panel.widgetsAreStatic = false;
				this.m_hudunSprite.UpdateTransform(0);
				this.m_panel.widgetsAreStatic = true;
			}
		}

		private void ShowMainHeroMark()
		{
		}

		private void ChangeLayer(GameObject objct, string layer = "Unit")
		{
			objct.layer = LayerMask.NameToLayer(layer);
			Transform[] componentsInChildren = objct.transform.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				transform.gameObject.layer = LayerMask.NameToLayer(layer);
			}
		}

		private void ChangeRender(GameObject obj)
		{
			ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].renderer.material.renderQueue = 3000;
				}
			}
		}
	}
}
