using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Com.Game.Module
{
	public class SkillView : BaseView<SkillView>
	{
		private Transform SkillPanel;

		private Transform trBar;

		private Transform trGuidBar;

		private TweenFill guidTweenFill;

		private UILabel guidInfo;

		private UIGrid m_uiGrid;

		private UIProgressBar hpBar;

		private UIProgressBar mpBar;

		private UILabel m_propmpt;

		private WaterWave m_waterWaveScript;

		public List<SkillItem> mSkillItems = new List<SkillItem>();

		private Transform ExtraSkillPanel;

		public MoveLanByBar lanTiao;

		public MoveLanByBar xueTiao;

		private UIPanel _cdPanel;

		private List<string> mSkillIds;

		private Units m_Player;

		private List<Skill> coolDownList = new List<Skill>();

		private CoroutineManager m_CoroutineManager = new CoroutineManager();

		public VTrigger ReplayGame;

		private int ExtraSkillCount;

		public int canUseLevelUpPoints = 1;

		public int usedSkillPoint;

		private SkillItem selectSkill;

		private AudioClipInfo mpNotEnoughClipInfo;

		private UIPanel panel;

		private Task changePanelTask;

		private SkillPanelPivot skillPanelPivot = SkillPanelPivot.none;

		private string[] skillHelp;

		private string AnchorPath = "Anchor_b";

		private bool bimmiadiatlySkill;

		public bool isShowNextLevel;

		private SimpleObjectPool<StringBuilder> _sbPool = new SimpleObjectPool<StringBuilder>(1, delegate(StringBuilder x)
		{
			x.Length = 0;
		}, null, string.Empty);

		private VTrigger ChangePlayerTrigger;

		private VTrigger UnitRebirthAgain;

		private SkillItem tempSkillItem;

		private bool widgetState;

		private SkillItem skillItemScript;

		private int childCount;

		private int activeIndex;

		private int ExtraSkillIndex;

		private SysSkillMainVo skillVo;

		private bool countChange;

		private SkillItem _skillItem;

		private bool mplayerIsLive = true;

		private int needUpdateMpValue;

		private float mpBarvalue = 1f;

		private int oldHp;

		private int oldMaxHp;

		private int oldMp;

		private int oldMaxMp;

		private List<string> _skillNames;

		private Skill _tmp_skill;

		private Task waitforskill;

		public SkillView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "SkillView");
		}

		public void setImmidiatlySkillFlag(bool immidiatly)
		{
			this.bimmiadiatlySkill = immidiatly;
		}

		public bool getImmidiatlySkillFlag()
		{
			return this.bimmiadiatlySkill;
		}

		public override void Init()
		{
			base.Init();
			this.panel = this.transform.GetComponent<UIPanel>();
			this.SkillPanel = this.transform.FindChild(this.AnchorPath + "/Panel/SkillPanel");
			this.ExtraSkillPanel = this.transform.FindChild(this.AnchorPath + "/ExtraSkillPanel");
			this.trBar = this.transform.FindChild(this.AnchorPath + "/Panel/BarPanel/LanTiao");
			this.lanTiao = this.trBar.GetComponent<MoveLanByBar>();
			this.xueTiao = this.transform.FindChild(this.AnchorPath + "/Panel/BarPanel/XueTiao").GetComponent<MoveLanByBar>();
			this.trGuidBar = this.transform.FindChild("Prompt/SkillGuidBar");
			this.guidTweenFill = this.trGuidBar.Find("Slider/Foreground").GetComponent<TweenFill>();
			this.guidTweenFill.ignoreTimeScale = false;
			this.guidTweenFill.SetOnFinished(new EventDelegate.Callback(this.OnGuideBarFinished));
			this.trGuidBar.gameObject.SetActive(false);
			this.mpBar = this.trBar.GetComponentInChildren<UIProgressBar>();
			this.hpBar = this.xueTiao.GetComponentInChildren<UIProgressBar>();
			this.m_uiGrid = this.SkillPanel.GetComponent<UIGrid>();
			this.m_propmpt = this.transform.Find("Prompt/prompt").GetComponent<UILabel>();
			this.m_propmpt.gameObject.SetActive(false);
			this.guidInfo = this.transform.Find("Prompt/SkillGuidBar/prompt").GetComponent<UILabel>();
			this._cdPanel = this.transform.TryGetComp(this.AnchorPath + "/Panel/CdPanel");
		}

		public override void RegisterUpdateHandler()
		{
			this.RegisterTrigger();
		}

		public override void CancelUpdateHandler()
		{
			this.m_CoroutineManager.StopAllCoroutine();
			this.UnRegisterTrigger();
		}

		public override void RefreshUI()
		{
			this.UpdateSkillView();
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void Destroy()
		{
			this.mSkillIds = null;
			this.m_Player = null;
			this.skillHelp = null;
			this.mSkillItems.Clear();
			this.UnRegisterTrigger();
			base.Destroy();
		}

		private void RegisterTrigger()
		{
			this.ClearData();
			this.InitSkillPanelPivot((SkillPanelPivot)ModelManager.Instance.Get_SettingData().skillPanelPivot);
			if (this.ChangePlayerTrigger == null)
			{
				this.ChangePlayerTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.ChangePlayer));
			}
			TriggerManager.CreateGameEventTrigger(GameEvent.GamePause, null, new TriggerAction(this.OnGamePause));
			TriggerManager.CreateGameEventTrigger(GameEvent.GameResume, null, new TriggerAction(this.OnGameResume));
			MobaMessageManager.RegistMessage((ClientMsg)21022, new MobaMessageFunc(this.OnItemSkill));
		}

		private void OnItemSkill(MobaMessage msg)
		{
			if (this.m_Player == null)
			{
				return;
			}
			ItemInfo itemInfo = msg.Param as ItemInfo;
			if (itemInfo == null)
			{
				return;
			}
			if (!this.UseItemSkill(itemInfo, true))
			{
				return;
			}
			if (LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsServerZyBattleType)
			{
				BattleEquipTools_op.DoPvpUseitem(itemInfo);
			}
			else
			{
				this.UseItemSkill(itemInfo, false);
				SysBattleItemsVo sysBattleItemsVo;
				if (BattleEquipTools_config.GetBattleItemVo(itemInfo.ID, out sysBattleItemsVo) && sysBattleItemsVo.consume > 0)
				{
					List<ItemInfo> possessItemsP = ModelManager.Instance.Get_BattleShop_pItems();
					BattleEquipTools_op.DoPveUseItem(this.m_Player, itemInfo, possessItemsP);
				}
			}
		}

		private bool UseItemSkill(ItemInfo info, bool isTry)
		{
			SysBattleItemsVo sysBattleItemsVo;
			if (BattleEquipTools_config.GetBattleItemVo(info.ID, out sysBattleItemsVo))
			{
				string[] array = StringUtils.SplitVoString(sysBattleItemsVo.skill_id, ",");
				if (array != null)
				{
					int num = 0;
					if (num < array.Length)
					{
						Skill skillById = this.m_Player.getSkillById(array[num]);
						if (skillById == null)
						{
							return false;
						}
						if (!skillById.CheckCD(true))
						{
							return false;
						}
						if (isTry)
						{
							return true;
						}
						this.m_Player.OnSkill(array[num]);
						return true;
					}
				}
			}
			return true;
		}

		public void SetGameObjectActive(bool isShow)
		{
			if (this.gameObject != null)
			{
				this.gameObject.SetActive(isShow);
				if (isShow)
				{
					this.RefreshUI();
				}
			}
		}

		private void ClearData()
		{
			this.canUseLevelUpPoints = 1;
		}

		public bool CanAddLevel()
		{
			return this.canUseLevelUpPoints > 0;
		}

		private void UnRegisterTrigger()
		{
			TriggerManager.DestroyTrigger(this.ChangePlayerTrigger);
			this.ChangePlayerTrigger = null;
			TriggerManager.DestroyTrigger(this.UnitRebirthAgain);
			MobaMessageManager.UnRegistMessage((ClientMsg)21022, new MobaMessageFunc(this.OnItemSkill));
		}

		public void ChangePlayer()
		{
			this.ShowGuideBar(false, 1f, "回城");
			this.skillHelp = null;
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
			if (this.m_Player == null)
			{
				return;
			}
			string npc_id = this.m_Player.npc_id;
			if (this.UnitRebirthAgain != null)
			{
				TriggerManager.DestroyTrigger(this.UnitRebirthAgain);
			}
			this.UnitRebirthAgain = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRebirthAgain, null, new TriggerAction(this.OnPlayerRebirthAgain), this.m_Player.unique_id);
			this.m_Player.OnDeathCallback -= new Callback<Units>(this.CheckIconToGrayByCanUseAll);
			this.m_Player.OnDeathCallback += new Callback<Units>(this.CheckIconToGrayByCanUseAll);
			this.mSkillIds = this.m_Player.GetSkills();
			List<string> list = new List<string>();
			for (int i = 0; i < this.mSkillIds.Count; i++)
			{
				Skill skillById = this.m_Player.getSkillById(this.mSkillIds[i]);
				if (skillById != null)
				{
					if (!skillById.isTalentSkill)
					{
						list.Add(this.mSkillIds[i]);
					}
				}
			}
			this.mSkillIds = list;
			this.ExtraSkillCount = 0;
			if ((LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsZyBattleType || LevelManager.CurBattleType == 2) && !string.IsNullOrEmpty(Singleton<PvpManager>.Instance.GetSummonerSkillId(this.m_Player.unique_id)))
			{
				this.ExtraSkillCount = 1;
			}
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
			if (this.ExtraSkillCount == 0 && (dataById == null || dataById.backHomeSkill == string.Empty || dataById.backHomeSkill == "[]"))
			{
				this.ExtraSkillCount = this.ExtraSkillCount;
				this.ExtraSkillPanel.gameObject.SetActive(false);
			}
			else
			{
				if (dataById != null && !(dataById.backHomeSkill == string.Empty) && !(dataById.backHomeSkill == "[]"))
				{
					this.ExtraSkillCount++;
				}
				this.ExtraSkillPanel.gameObject.SetActive(true);
			}
			this.RefreshUI();
			this.ShowCanLevelUp();
		}

		private void OnPlayerRebirthAgain()
		{
			this.UpdateSkillView();
			this.UpdateSelfValue(this.m_Player, false, false);
			this.CheckIconToGrayByCanUseAll(null);
		}

		private void OnGamePause()
		{
			this.PauseGuideBar();
			this.PauseAllCD();
		}

		private void OnGameResume()
		{
			this.ResumeGuideBar();
			this.ResumeAllCD();
		}

		public void AddSkillPoint(int value)
		{
			if (this.m_Player == null)
			{
				return;
			}
			if (this.m_Player.skillManager == null)
			{
				return;
			}
			this.canUseLevelUpPoints += value;
			this.m_Player.skillManager.SkillPointsLeft += value;
			this.ShowCanLevelUp();
		}

		public void SetSkillUILevel(string skillId, int level)
		{
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skillId);
			if (skillItem != null)
			{
				skillItem.SetLevel(level);
			}
		}

		public void SetSkillPointLeft(int skillPointLeft)
		{
			this.canUseLevelUpPoints = skillPointLeft;
			this.ShowCanLevelUp();
		}

		public void UseSkillPoint(int value, string skillName, int skillLevel)
		{
			if (!this.CanAddLevel())
			{
				return;
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				this.m_Player.skillManager.UpgradeSkillLevel(skillName, skillLevel);
			}
			else
			{
				this.m_Player.skillManager.UpgradeSkillLevel(skillName, skillLevel);
				this.m_Player.skillManager.SkillPointsLeft -= value;
				this.canUseLevelUpPoints -= value;
			}
			this.ShowCanLevelUp();
		}

		public void GetSkillLevelUpCallBack(string skillName, int skillLevel)
		{
			this.tempSkillItem = null;
			this.tempSkillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skillName);
			if (this.tempSkillItem != null)
			{
				this.tempSkillItem.AddLevelGetCallBack(skillLevel);
			}
		}

		public void UpdateSkillItem(int skillIndex)
		{
			if (skillIndex >= this.mSkillItems.Count)
			{
				return;
			}
			if (skillIndex < 0)
			{
				return;
			}
			SkillItem skillItem = this.mSkillItems[skillIndex];
			Skill skillByIndex = this.m_Player.skillManager.getSkillByIndex(skillIndex);
			if (skillByIndex == null)
			{
				return;
			}
			skillItem.level = skillByIndex.skillLevel;
			skillItem.UpdateSkillStateFX();
			skillItem.UpdateLevelIcon(false);
		}

		public void UpdateSkillStateFX()
		{
			if (this.m_Player == null)
			{
				return;
			}
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				SkillItem skillItem = this.mSkillItems[i];
				skillItem.UpdateSkillStateFX();
			}
		}

		public void ShowCanLevelUp()
		{
			if (this.m_Player == null)
			{
				return;
			}
			this.usedSkillPoint = 0;
			int num = 0;
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				SkillItem skillItem = this.mSkillItems[i];
				Skill skillByIndex = this.m_Player.getSkillByIndex(i);
				if (skillByIndex != null)
				{
					this.UpdateSkillLock(i);
					this.UpdateSkillActive(i);
					this.usedSkillPoint += skillItem.GetLevel();
					num += skillItem.GetMaxLevel();
					if (this.canUseLevelUpPoints > 0 && BattleAttrManager.Instance.IsCurSkillCanAdd(i, this.m_Player, skillItem.GetLevel(), skillItem.GetMaxLevel()) && !skillByIndex.IsSummonerSkill && skillByIndex.CanLevelUp())
					{
						skillItem.SetCanLevelUp(true);
					}
					else
					{
						skillItem.SetCanLevelUp(false);
					}
					if (skillByIndex.skillLevel > 0)
					{
						skillItem.SetMagicLabel((int)Math.Abs(skillByIndex.GetCostValue(AttrType.Mp)), false);
					}
					else if (skillItem.isSummerSkill)
					{
						skillItem.SetSummerSkillStyle(skillItem.name);
					}
				}
			}
			int num2 = this.m_Player.level;
			if (num < this.m_Player.level)
			{
				num2 = num;
			}
			this.canUseLevelUpPoints = num2 - this.usedSkillPoint;
			this.m_Player.skillManager.SkillPointsLeft = this.canUseLevelUpPoints;
			if (this.usedSkillPoint == 0 && this.m_Player.level == 1 && this.m_Player.skillManager.SkillPointsLeft == 0)
			{
				this.m_Player.skillManager.SkillPointsLeft = 1;
				this.ShowCanLevelUp();
			}
		}

		public void NewbieEleBatOneOneLearnSkill(int inIdx)
		{
			this.mSkillItems[inIdx].AddLevel();
		}

		public void NewbieEleBatOneOneUseSkill(int inIdx)
		{
			string skill_id = string.Empty;
			if (inIdx == 0)
			{
				skill_id = "Skill_Liaonida_01";
			}
			else if (inIdx == 1)
			{
				skill_id = "Skill_Liaonida_02";
			}
			else if (inIdx == 2)
			{
				skill_id = "Skill_Liaonida_03";
			}
			else
			{
				skill_id = "Skill_Liaonida_04";
			}
			this.m_Player.OnSkill(skill_id);
		}

		public void NewbieEleBatFiveUseExtraSkill(string inSkillId)
		{
			if (!string.IsNullOrEmpty(inSkillId))
			{
				this.m_Player.OnSkill(inSkillId);
			}
		}

		public bool NewbieCheckIsSkillCanLevelUp(int inIndex)
		{
			return this.mSkillItems != null && inIndex >= 0 && inIndex < this.mSkillItems.Count && this.mSkillItems[inIndex].NewbieCheckIsCanLevelUp();
		}

		public int GetSkillLevel(int index)
		{
			if (this.mSkillItems.Count == 0)
			{
				return 0;
			}
			return this.mSkillItems[index].GetLevel();
		}

		public int GetCanUseLevelUpPoints()
		{
			return this.canUseLevelUpPoints;
		}

		private bool CheckWidgeReady()
		{
			return this.widgetState;
		}

		private void UpdateSkillView()
		{
			if (this.m_Player == null)
			{
				return;
			}
			this.childCount = this.mSkillItems.Count;
			this.activeIndex = 0;
			this.ExtraSkillCount = this.mSkillIds.Count - 4;
			this.ExtraSkillIndex = 0;
			for (int i = 0; i < this.mSkillIds.Count; i++)
			{
				this.skillVo = SkillUtility.GetSkillData(this.mSkillIds[i], -1, -1);
				string text = null;
				Units player = PlayerControlMgr.Instance.GetPlayer();
				if (player != null)
				{
					Skill skillById = player.skillManager.getSkillById(this.skillVo.skill_id);
					if (skillById != null && skillById.SkillIcon != null)
					{
						text = skillById.SkillIcon;
					}
				}
				if (this.skillVo == null)
				{
					ClientLogger.Error("skill is null");
				}
				else
				{
					if (i >= this.childCount)
					{
						if (i >= this.mSkillIds.Count - this.ExtraSkillCount)
						{
							if (this.skillVo.skill_index == 7)
							{
								this.ExtraSkillPanel.Find("ExtraSkill0").gameObject.SetActive(true);
								this.skillItemScript = NGUITools.AddChild(this.ExtraSkillPanel.Find("ExtraSkill0").gameObject, base.LoadPrefabCache("SkillItem")).GetComponent<SkillItem>();
								this.skillItemScript.SetCdParent(this._cdPanel);
								this.skillItemScript.transform.Find("State1").GetComponent<BoxCollider>().size = new Vector3(188f, 248f, 0f);
							}
							else
							{
								this.ExtraSkillIndex++;
								this.skillItemScript = NGUITools.AddChild(this.ExtraSkillPanel.Find("ExtraSkill" + this.ExtraSkillIndex.ToString()).gameObject, base.LoadPrefabCache("SkillItem")).GetComponent<SkillItem>();
								this.skillItemScript.SetCdParent(this._cdPanel);
								this.skillItemScript.transform.Find("State1").GetComponent<BoxCollider>().size = new Vector3(188f, 248f, 0f);
							}
							this.skillItemScript.transform.Find("Point").gameObject.SetActive(false);
						}
						else
						{
							this.skillItemScript = NGUITools.AddChild(this.SkillPanel.gameObject, base.LoadPrefabCache("SkillItem")).GetComponent<SkillItem>();
							this.skillItemScript.SetCdParent(this._cdPanel);
							this.skillItemScript.SetPointNum(this.skillVo.skill_levelmax);
						}
						if (this.skillItemScript != null)
						{
							this.mSkillItems.Add(this.skillItemScript);
							this.skillItemScript.gameObject.SetActive(true);
						}
						this.countChange = true;
						if (this.skillVo.skill_trigger == 3)
						{
							this.skillItemScript.SetSkillType(SkillItemType.passive);
						}
						else
						{
							this.skillItemScript.SetSkillType(SkillItemType.active);
						}
						this.skillItemScript.Rename(this.mSkillIds[i]);
						this.skillItemScript.SkillItemIndex = i;
						this.skillItemScript.AddSkillKey(i);
						string skill_icon = this.skillVo.skill_icon;
						if (this.skillVo.skill_index == 4 || this.skillVo.skill_index == 5 || this.skillVo.skill_index == 8)
						{
							this.skillItemScript.SetSkillSpriteIcon((text == null) ? this.skillVo.skill_icon : text, true);
						}
						else
						{
							this.skillItemScript.SetSkillSpriteIcon((text == null) ? this.skillVo.skill_icon : text, false);
						}
					}
					else
					{
						this.skillItemScript = this.mSkillItems[i];
					}
					this.UpdateSkillLock(i);
					this.UpdateSkillActive(i);
					this.activeIndex++;
				}
			}
			for (int j = this.mSkillIds.Count; j < this.childCount; j++)
			{
				GameObject gameObject = this.SkillPanel.GetChild(j).gameObject;
				gameObject.SetActive(false);
			}
			if (this.countChange && this.gameObject.activeInHierarchy)
			{
				this.countChange = false;
				this.m_uiGrid.Reposition();
				for (int k = 0; k < this.mSkillItems.Count; k++)
				{
					this.mSkillItems[k].SetLevelUpToCdParent(this._cdPanel);
				}
			}
			this.widgetState = true;
		}

		public void RefreshIcon(int index, string iconStr)
		{
			this.skillItemScript = this.mSkillItems[index];
			if (index == 4 || index == 5 || index == 8)
			{
				this.skillItemScript.SetSkillSpriteIcon(iconStr, true);
			}
			else
			{
				this.skillItemScript.SetSkillSpriteIcon(iconStr, false);
			}
		}

		public void UpdateSkillActive(int item_index)
		{
			if (this.m_Player == null || this.mSkillIds == null)
			{
				return;
			}
			if (this.mSkillIds.Count < item_index + 1)
			{
				return;
			}
			string skillName = this.mSkillIds[item_index];
			Skill skillById = this.m_Player.getSkillById(skillName);
			if (skillById != null && skillById.IsPassive)
			{
				SkillItem skillItem = this.mSkillItems[item_index];
				GameObject objectState = skillItem.GetObjectState1();
				UIEventListener.Get(objectState).onPress = new UIEventListener.BoolDelegate(this.OnNoLockSkill);
				return;
			}
			if (skillById != null && !skillById.IsPassive)
			{
				bool flag = BattleAttrManager.Instance.IsCurSkillUnlock(item_index, this.m_Player);
				if (!skillById.IsCDTimeOver || !flag)
				{
					this.SetActiveSkill(item_index, false);
				}
				else
				{
					this.SetActiveSkill(item_index, true);
				}
			}
		}

		public void UpdateSkillLock(int skill_index)
		{
			if (skill_index < 0 || skill_index >= this.mSkillItems.Count || this.mSkillItems.Count <= 0)
			{
				return;
			}
			SkillItem skillItem = this.mSkillItems[skill_index];
			if (BattleAttrManager.Instance.IsCurSkillUnlock(skill_index, this.m_Player))
			{
				skillItem.SetActiveState1(true);
				skillItem.SetActiveState2(false);
			}
			else if (skillItem.isLock)
			{
				skillItem.SetActiveState1(false);
				skillItem.SetActiveState2(true);
			}
		}

		private void UpdateSkillButton(GameObject state, bool isclickable)
		{
			if (isclickable)
			{
				UIEventListener.Get(state).onDoubleClick = null;
				UIEventListener.Get(state).onPress = new UIEventListener.BoolDelegate(this.OnSkill0);
			}
			else
			{
				UIEventListener.Get(state).onPress = null;
				UIEventListener.Get(state).onDoubleClick = null;
			}
		}

		public void CheckIconToGrayByCanUseAll(Units target = null)
		{
			this.CheckIconToGrayByCanUse(target, -1);
		}

		public void CheckIconToGrayByCanUse(Units target = null, int m_skillIndex = -1)
		{
			if (this.m_Player == null)
			{
				this.m_Player = PlayerControlMgr.Instance.GetPlayer();
				if (this.m_Player == null)
				{
					return;
				}
			}
			if (this.mSkillItems == null)
			{
				return;
			}
			if (!this.m_Player.CanSkill || !this.m_Player.isLive)
			{
				if (m_skillIndex == -1)
				{
					for (int i = 0; i < this.mSkillItems.Count; i++)
					{
						this._skillItem = this.mSkillItems[i];
						if (this._skillItem != null)
						{
							if (this.m_Player.getSkillByIndex(i).CheckSkillCanUseSpecial && this.m_Player.isLive)
							{
								this._skillItem.ChangeIconToGrayByCanUse(true);
							}
							else
							{
								this._skillItem.ChangeIconToGrayByCanUse(false);
								if (!this.m_Player.isLive)
								{
									this._skillItem.IsShowNeedMagic(false);
									this.hpBar.value = 0f / this.m_Player.hp_max;
									this.xueTiao.SetCurrentAndMax(0, (int)this.m_Player.hp_max);
								}
							}
						}
					}
				}
				else
				{
					this._skillItem = this.mSkillItems[m_skillIndex];
					if (this._skillItem != null)
					{
						if (this.m_Player.getSkillByIndex(m_skillIndex).CheckSkillCanUseSpecial && this.m_Player.isLive)
						{
							this._skillItem.ChangeIconToGrayByCanUse(true);
						}
						else
						{
							if (!this.m_Player.isLive)
							{
								this._skillItem.IsShowNeedMagic(false);
							}
							this._skillItem.ChangeIconToGrayByCanUse(false);
						}
					}
				}
			}
			else
			{
				int j = 0;
				while (j < this.mSkillItems.Count)
				{
					if (m_skillIndex == -1)
					{
						goto IL_1E3;
					}
					if (m_skillIndex == j)
					{
						goto IL_1E3;
					}
					IL_36C:
					j++;
					continue;
					IL_1E3:
					this._skillItem = this.mSkillItems[j];
					if (this._skillItem == null)
					{
						goto IL_36C;
					}
					if (this._skillItem.GetSkillType() == SkillItemType.passive)
					{
						if (this.m_Player.isLive)
						{
							this._skillItem.ChangeIconToGrayByCanUse(true);
						}
						goto IL_36C;
					}
					if (this.m_Player.getSkillByIndex(j) == null)
					{
						goto IL_36C;
					}
					if (!BattleAttrManager.Instance.IsCurSkillUnlock(j, this.m_Player))
					{
						goto IL_36C;
					}
					Skill skillByIndex = this.m_Player.getSkillByIndex(j);
					if (Math.Abs(skillByIndex.GetCostValue(AttrType.Mp)) <= this.m_Player.mp)
					{
						if (this._skillItem.timeNum <= 0f)
						{
							this._skillItem.ChangeIconToGrayByCanUse(true);
							this._skillItem.isQueLan = false;
							this._skillItem.IsShowNeedMagic(false);
						}
					}
					else if (!this.m_Player.IsSkillCanTriggerBornPowerObj(skillByIndex.skillMainId))
					{
						this._skillItem.ChangeIconToGrayByCanUse(false);
						this._skillItem.isQueLan = true;
						if (this._skillItem.timeNum <= 0f && this.m_Player.isLive)
						{
							this._skillItem.IsShowNeedMagic(true);
						}
					}
					else
					{
						this._skillItem.ChangeIconToGrayByCanUse(true);
						this._skillItem.IsShowNeedMagic(false);
					}
					this.UpdateSelfValue(this.m_Player, false, false);
					this._skillItem.CheckCanPress();
					goto IL_36C;
				}
			}
		}

		private void SetCD(int mask_type, int skill_index, bool isactive, float skillCDTime = -1f, bool isReset = false)
		{
			if (this.m_Player == null)
			{
				return;
			}
			if (skill_index >= 0 && this.mSkillItems.Count > 0 && skill_index < this.mSkillItems.Count && mask_type == 1)
			{
				SkillItem skillItem = this.mSkillItems[skill_index];
				string name = skillItem.gameObject.name;
				float num = (skillCDTime != -1f) ? skillCDTime : this.m_Player.GetSkillCDTime(name);
				float cDTime = this.m_Player.GetCDTime(name);
				if (isactive && num >= 0f)
				{
					if (cDTime <= 0f || isReset)
					{
						skillItem.SetActiveMask(true);
						skillItem.SetFillAmount(1f);
						skillItem.SetTweenCD(num, 1f);
						this.m_Player.SetCDTime(name, num);
						skillItem.StartTextTimer(this.m_Player, num);
					}
					else
					{
						skillItem.SetActiveMask(true);
						skillItem.SetFillAmount(cDTime / num);
						skillItem.SetTweenCD(cDTime, cDTime / num);
						skillItem.StartTextTimer(this.m_Player, cDTime);
					}
					skillItem.SetCanPress(false, string.Empty);
					skillItem.ChangeIconToGrayByCanUse(false);
				}
				else if (this.m_Player.isLive)
				{
					if (skillItem.timeNum <= 0f)
					{
						skillItem.SetActiveMask(false);
						skillItem.SetCanPress(true, name);
					}
				}
				else
				{
					if (skillItem.timeNum <= 0f)
					{
						skillItem.SetActiveMask(false);
					}
					skillItem.SetCanPress(false, name);
				}
			}
		}

		public void UpdateChargeCD(int skillIndex, float percent, float chargeCD, int count)
		{
			if (this.mSkillItems.Count <= skillIndex)
			{
				return;
			}
			if (skillIndex < 0)
			{
				return;
			}
			bool flag = (float)count > 0f;
			this.mSkillItems[skillIndex].UpdateChargeCD(percent, ((float)count != 0f) ? 0f : chargeCD);
			this.mSkillItems[skillIndex].SetChargeCount(count);
			if (flag)
			{
				this.mSkillItems[skillIndex].SetChargeAlpha(0.5f);
			}
			else
			{
				this.mSkillItems[skillIndex].SetChargeAlpha(0.8f);
			}
			this.mSkillItems[skillIndex].CdItems1.Show(true);
		}

		public void UpdateSkillItemCDTime(float cdTime, string skillId)
		{
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skillId);
			if (skillItem != null)
			{
				skillItem.SetTweenCD(cdTime, 1f);
				skillItem.StartTextTimer(this.m_Player, cdTime);
			}
		}

		public void ShowTriggerBornPowerObjHint(string inSkillId)
		{
			if (this.m_Player == null)
			{
				return;
			}
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == inSkillId);
			if (skillItem != null)
			{
				skillItem.ShowTriggerBornPowerObjHint();
			}
		}

		public void HideTriggerBornPowerObjHint(string inSkillId)
		{
			if (this.m_Player == null)
			{
				return;
			}
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == inSkillId);
			if (skillItem != null)
			{
				skillItem.HideTriggerBornPowerObjHint();
			}
		}

		public void StartSkillCD(string skill_id, bool isactive, float cdTime = -1f, bool isReset = false)
		{
			if (this.m_Player == null)
			{
				return;
			}
			int skill_index = this.mSkillIds.IndexOf(skill_id);
			this.SetCD(1, skill_index, isactive, cdTime, isReset);
			this.UpdateSkillView(skill_id, true);
			if (this.m_Player.getSkillByIndex(skill_index) == null)
			{
				return;
			}
		}

		public void SetActiveSkill(int skill_index, bool isactive)
		{
			if (this.mSkillItems.Count <= 0)
			{
				return;
			}
			SkillItem skillItem = this.mSkillItems[skill_index];
			GameObject objectState = skillItem.GetObjectState1();
			if (isactive)
			{
				this.UpdateSkillButton(objectState, true);
			}
			else
			{
				this.UpdateSkillButton(objectState, false);
			}
		}

		public void UpdateSkillView(string skill_id, bool isAll = false)
		{
			if (this.m_Player == null || this.mSkillIds == null)
			{
				return;
			}
			int num = this.mSkillIds.IndexOf(skill_id);
			if (num >= 0)
			{
				this.UpdateSkillView_Coroutinue(num, isAll);
			}
		}

		private void UpdateSkillView_Coroutinue(int skill_index, bool isAll = false)
		{
			this.UpdateSkillLock(skill_index);
			this.UpdateSkillActive(skill_index);
			if (isAll)
			{
				this.CheckIconToGrayByCanUse(this.m_Player, -1);
			}
			else
			{
				this.CheckIconToGrayByCanUse(this.m_Player, skill_index);
			}
		}

		public void UpdateSkillByIndex(int skill_index)
		{
			this.UpdateSkillLock(skill_index);
			this.UpdateSkillActive(skill_index);
		}

		public void UpdateSelfValue(Units self, bool istween = false, bool isForce = false)
		{
			if (self == null)
			{
				this.m_Player = PlayerControlMgr.Instance.GetPlayer();
				self = this.m_Player;
				if (self == null)
				{
					return;
				}
			}
			if (this.hpBar == null)
			{
				return;
			}
			if (self.isLive)
			{
				this.mplayerIsLive = true;
				if (this.oldHp != (int)self.hp || this.oldMaxHp != (int)self.hp_max || isForce)
				{
					this.oldHp = (int)self.hp;
					this.oldMaxHp = (int)self.hp_max;
					this.hpBar.value = self.hp / self.hp_max;
					this.xueTiao.SetCurrentAndMax((int)this.m_Player.hp, (int)this.m_Player.hp_max);
				}
				this.xueTiao.SetRestoreSpeed(this.m_Player.hp_restore);
				if (this.oldMp != (int)self.mp || this.oldMaxMp != (int)self.mp_max || isForce)
				{
					this.oldMp = (int)self.mp;
					this.oldMaxMp = (int)self.mp_max;
					this.mpBarvalue = self.mp / self.mp_max;
					if (this.mpBar.value > this.mpBarvalue)
					{
						this.needUpdateMpValue = this.CheckNextSkillCostLine((int)this.m_Player.mp);
					}
					this.mpBar.value = this.mpBarvalue;
					this.lanTiao.SetCurrentAndMax((int)this.m_Player.mp, (int)this.m_Player.mp_max);
					if ((int)this.m_Player.mp >= this.needUpdateMpValue)
					{
						if (this.needUpdateMpValue != 0)
						{
							this.CheckIconToGrayByCanUse(null, -1);
						}
						this.needUpdateMpValue = this.CheckNextSkillCostLine((int)this.m_Player.mp);
					}
				}
				this.lanTiao.SetRestoreSpeed(this.m_Player.mp_restore);
			}
			else if (this.mplayerIsLive)
			{
				this.mplayerIsLive = false;
			}
		}

		private int CheckNextSkillCostLine(int nowValue)
		{
			int num = 9999;
			bool flag = true;
			if (this.m_Player == null)
			{
				this.m_Player = PlayerControlMgr.Instance.GetPlayer();
			}
			if (this.m_Player.skillManager == null)
			{
				return 0;
			}
			this._skillNames = this.m_Player.skillManager.GetSkills();
			if (this._skillNames == null)
			{
				return 0;
			}
			for (int i = 0; i < this._skillNames.Count; i++)
			{
				this._tmp_skill = this.m_Player.skillManager.getSkillById(this._skillNames[i]);
				if (this._tmp_skill != null)
				{
					int num2 = -(int)this._tmp_skill.GetCostValue(AttrType.Mp);
					if (nowValue < num2 && num > num2)
					{
						num = num2;
						flag = false;
					}
				}
			}
			if (flag)
			{
				num = (int)PlayerControlMgr.Instance.GetPlayer().mp_max;
			}
			return num;
		}

		public void SetSelectState()
		{
			this.ClearSelectState();
			if (this.selectSkill != null)
			{
				this.selectSkill.m_selectSprite.SetActive(true);
			}
		}

		public void ClearSelectState()
		{
			foreach (SkillItem current in this.mSkillItems)
			{
				current.m_selectSprite.SetActive(false);
			}
		}

		public void SetForbidMask(bool isShow, string text = "")
		{
			bool flag = isShow;
			foreach (SkillItem current in this.mSkillItems)
			{
				if (this.m_Player.getSkillByIndex(current.SkillItemIndex) != null)
				{
					if (!current.isLock && current.skillItemType != SkillItemType.passive && !this.m_Player.getSkillByIndex(current.SkillItemIndex).CheckSkillCanUseSpecial)
					{
						isShow = flag;
						current.m_forbidLabel.text = text;
						if (current.isSkillUIForbid && !isShow)
						{
							isShow = true;
						}
						PerfTools.SetVisible(current.m_forbidLabel.transform.parent, isShow);
						current.SetCanPress(!isShow, string.Empty);
					}
				}
			}
		}

		public void SetSkillUIForbidMask(int skillIdx, bool isShow)
		{
			SkillItem skillItem = this.mSkillItems[skillIdx];
			skillItem.isSkillUIForbid = isShow;
			if (skillItem.m_forbidLabel.text != string.Empty)
			{
				return;
			}
			PerfTools.SetVisible(skillItem.m_forbidLabel.transform.parent, isShow);
			skillItem.SetCanPress(!isShow, string.Empty);
		}

		public void ExternalCall(GameObject objct_1 = null, bool ispress = false)
		{
			this.OnSkill0(objct_1, ispress);
		}

		[DebuggerHidden]
		private IEnumerator Coroutine_wait()
		{
			return new SkillView.<Coroutine_wait>c__IteratorEE();
		}

		private void OnSkill_DoubleClick(GameObject objct_1 = null, bool ispress = false)
		{
			if (this.getImmidiatlySkillFlag())
			{
				return;
			}
			if (LevelManager.CurBattleType != 6 && StrategyManager.Instance != null)
			{
				StrategyManager.Instance.UpdateInputState(true);
			}
			this.selectSkill = objct_1.GetComponentInParent<SkillItem>();
			Skill skillByIndex = this.m_Player.getSkillByIndex(this.selectSkill.SkillItemIndex);
			if (skillByIndex == null)
			{
				return;
			}
			string skill_id = skillByIndex.skillMainId;
			if (Math.Abs(skillByIndex.GetCostValue(AttrType.Mp)) > this.m_Player.mp)
			{
				return;
			}
			if (skillByIndex.needTarget && skillByIndex.Data.skill_prop != 4 && skillByIndex.Data.skill_prop != 5 && PlayerControlMgr.Instance.GetSelectedTarget() == null)
			{
				return;
			}
			int num = this.mSkillIds.IndexOf(skill_id);
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skill_id);
			if (this.m_Player.GetCDTime(skill_id) <= 0f && this.m_Player.CanClickSkillUI(skill_id) && !this.m_Player.LockInputState)
			{
				this.setImmidiatlySkillFlag(true);
				if (this.waitforskill != null)
				{
					this.m_CoroutineManager.StopCoroutine(this.waitforskill);
				}
				this.waitforskill = this.m_CoroutineManager.StartCoroutine(this.Coroutine_wait(), false);
				this.waitforskill.Start();
				this.m_Player.OnSkill(skill_id);
				if (!this.m_Player.isLive)
				{
					if (this.mpNotEnoughClipInfo.clipName != "sd_int_close")
					{
						this.mpNotEnoughClipInfo = default(AudioClipInfo);
						this.mpNotEnoughClipInfo.clipName = "sd_int_close";
						this.mpNotEnoughClipInfo.audioSourceType = eAudioSourceType.UI;
						this.mpNotEnoughClipInfo.audioPriority = 128;
						this.mpNotEnoughClipInfo.volume = 1f;
					}
					AudioMgr.Play(this.mpNotEnoughClipInfo, null);
				}
			}
			skillItem.SetActiveMask1(false);
			skillItem.SetTweenCD1(0f, 1f);
		}

		public static string FixSkillTxtInfo(string txt, Units _m_Player)
		{
			string text = string.Empty;
			string[] array = txt.Split(new char[]
			{
				'{'
			});
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length; i++)
				{
					text += SkillView.GetSkillInfoAndValue(array[i], _m_Player);
				}
			}
			else
			{
				text = array[0];
			}
			if (text.Contains("$"))
			{
				string text2 = string.Empty;
				string[] array2 = text.Split(new char[]
				{
					'$'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					if (j % 2 == 0)
					{
						text2 += array2[j];
					}
					else
					{
						text2 += SkillView.GetSkillValueNew(array2[j], _m_Player);
					}
				}
				text = text2;
			}
			return text;
		}

		public static string GetSkillInfoAndValue(string txt, Units _m_Player)
		{
			string text = string.Empty;
			string[] array = txt.Split(new char[]
			{
				'}'
			});
			if (array.Length > 1)
			{
				text = text + SkillView.GetSkillValue(array[0], _m_Player) + array[1];
			}
			else
			{
				text = array[0];
			}
			return text;
		}

		public static string GetSkillValue(string txt, Units _m_Player)
		{
			string text = string.Empty;
			float num = 0f;
			string[] array = txt.Split(new char[]
			{
				'+'
			});
			string[] array2;
			if (array.Length > 1)
			{
				num += float.Parse(array[0]);
				array2 = array[1].Split(new char[]
				{
					'*'
				});
			}
			else
			{
				num = 0f;
				array2 = array[0].Split(new char[]
				{
					'*'
				});
			}
			float num2 = float.Parse(array2[1]);
			string[] array3 = array2[0].Split(new char[]
			{
				'|'
			});
			int num3 = int.Parse(array3[0]);
			if (num3 == 1)
			{
				if (_m_Player != null)
				{
					num += _m_Player.data.GetBasicAttrInBattle((AttrType)int.Parse(array3[1])) * num2;
				}
				text += num.ToString("0");
			}
			else if (num3 == 2)
			{
				if (_m_Player != null)
				{
					num += (_m_Player.data.GetAttr((AttrType)int.Parse(array3[1])) - _m_Player.data.GetBasicAttrInBattle((AttrType)int.Parse(array3[1]))) * num2;
				}
				text = text + "[1aef29]" + num.ToString("0") + "[-]";
			}
			return text;
		}

		public static string GetSkillValueNew(string txt, Units _m_Player)
		{
			string text = string.Empty;
			float num = 0f;
			string text2 = string.Empty;
			float num2 = 1f;
			string text3 = string.Empty;
			float num3 = 0f;
			string a = string.Empty;
			if (txt.Contains("Dam"))
			{
				SysSkillDamageVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDamageVo>(txt.Split(new char[]
				{
					'_'
				})[1]);
				if (dataById == null)
				{
					return "?(出错)";
				}
				string formula = dataById.formula;
				string[] array = formula.Split(new char[]
				{
					'|'
				});
				if (array[0] == "0")
				{
					return "0";
				}
				if (array[0] == "1" || array[0] == "2" || array[0] == "3" || array[0] == "21" || array[0] == "24")
				{
					if (array[0] == "2" || array.Length < 5)
					{
						text2 = array[1];
						if (array.Length >= 5)
						{
							a = array[4];
						}
					}
					else
					{
						text2 = array[4];
						a = text2;
					}
					num2 = float.Parse(array[2]);
					if (array.Length > 3)
					{
						num = float.Parse(array[3]);
					}
				}
				else if (array[0] == "25")
				{
					text2 = array[1];
					num2 = float.Parse(array[2]);
					num = float.Parse(array[3]);
					a = array[4];
				}
				else if (array[0] == "26")
				{
					text2 = array[1];
					a = text2;
					num2 = float.Parse(array[2]);
					num = float.Parse(array[3]);
					text3 = "+[1aef29]" + float.Parse(array[5]).ToString("0") + "*[-]";
				}
				else if (array[0] == "4")
				{
					text2 = array[1];
					if (array.Length > 3 && array[3] == "1")
					{
						return (Math.Abs(float.Parse(array[2])) * 100f).ToString("0") + "%";
					}
					return Math.Abs(float.Parse(array[2])).ToString();
				}
				else if (array[0] == "5")
				{
					text2 = array[1];
					return FormulaTool.GetFormualValueForIntroduce(int.Parse(array[2]), _m_Player);
				}
				if (text2 == "1")
				{
					text = text + "[ffa810]" + num.ToString("0") + "[-]";
					if (_m_Player != null)
					{
						if (a == "1")
						{
							float attr = _m_Player.data.GetAttr(AttrType.ExtraAttack);
							num = attr * num2 + num3;
							if (num2 != 0f)
							{
								if (num >= 0f)
								{
									text = text + "[ffa810](+" + num.ToString("0") + ")[-]";
								}
								else
								{
									text = text + "[ffa810](" + num.ToString("0") + ")[-]";
								}
							}
						}
						else if (a == "2")
						{
							int type = 25;
							float attr2 = _m_Player.data.GetAttr((AttrType)type);
							float basicAttrInBattle = _m_Player.data.GetBasicAttrInBattle((AttrType)type);
							num = (attr2 - basicAttrInBattle) * num2 + num3;
							if (num2 != 0f)
							{
								if (num >= 0f)
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](+",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
								else
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
							}
						}
					}
				}
				else if (text2 == "2")
				{
					text = text + "[29caff]" + num.ToString("0") + "[-]";
					if (_m_Player != null)
					{
						if (a == "1")
						{
							float attr3 = _m_Player.data.GetAttr(AttrType.ExtraAttack);
							num = attr3 * num2 + num3;
							if (num2 != 0f && num2 != 0f)
							{
								if (num >= 0f)
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[ffa810](+",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
								else
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[ffa810](",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
							}
						}
						else if (a == "2")
						{
							int type2 = 25;
							float attr4 = _m_Player.data.GetAttr((AttrType)type2);
							float basicAttrInBattle2 = _m_Player.data.GetBasicAttrInBattle((AttrType)type2);
							num = (attr4 - basicAttrInBattle2) * num2 + num3;
							if (num2 != 0f)
							{
								if (num >= 0f)
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](+",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
								else
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
							}
						}
					}
				}
				else if (text2 == "3")
				{
					text = text + "[ff1010]" + num.ToString("0") + "[-]";
					if (_m_Player != null)
					{
						if (a == "1")
						{
							float attr5 = _m_Player.data.GetAttr(AttrType.ExtraAttack);
							num = attr5 * num2 + num3;
							if (num2 != 0f)
							{
								if (num >= 0f)
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[ffa810](+",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
								else
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[ffa810](",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
							}
						}
						else if (a == "2")
						{
							int type3 = 25;
							float attr6 = _m_Player.data.GetAttr((AttrType)type3);
							float basicAttrInBattle3 = _m_Player.data.GetBasicAttrInBattle((AttrType)type3);
							num = (attr6 - basicAttrInBattle3) * num2 + num3;
							if (num2 != 0f)
							{
								if (num >= 0f)
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](+",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
								else
								{
									string text4 = text;
									text = string.Concat(new string[]
									{
										text4,
										"[29caff](",
										num.ToString("0"),
										")[-]",
										text3,
										"[-]"
									});
								}
							}
						}
					}
				}
			}
			else if (txt.Contains("Higheff"))
			{
				string text5 = txt.Split(new char[]
				{
					','
				})[0];
				string text6 = string.Empty;
				string[] array2 = text5.Split(new char[]
				{
					'_'
				});
				for (int i = 0; i < array2.Length - 1; i++)
				{
					if (text6 != string.Empty)
					{
						text6 += "_";
					}
					text6 += array2[i + 1];
				}
				string higheff_type = BaseDataMgr.instance.GetDataById<SysSkillHigheffVo>(text6).higheff_type;
				if (higheff_type == "[]")
				{
					return "0";
				}
				string[] array = higheff_type.Split(new char[]
				{
					'|'
				});
				return array[1];
			}
			else if (txt.Contains("Buff"))
			{
				string text7 = txt.Split(new char[]
				{
					','
				})[0];
				string text8 = string.Empty;
				string[] array3 = text7.Split(new char[]
				{
					'_'
				});
				for (int j = 0; j < array3.Length - 1; j++)
				{
					if (text8 != string.Empty)
					{
						text8 += "_";
					}
					text8 += array3[j + 1];
				}
				SysSkillBuffVo dataById2 = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(text8);
				if (dataById2 == null)
				{
					return "?(出错)";
				}
				string result = string.Empty;
				if (txt.Contains("buff_time"))
				{
					result = dataById2.buff_time.ToString();
				}
				else if (txt.Contains("max_layers"))
				{
					result = dataById2.max_layers.ToString();
				}
				return result;
			}
			return text;
		}

		public void stopdoubleclickCD(string id)
		{
			int num = this.mSkillIds.IndexOf(id);
			if (num < 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("告诉陈宜明：" + id + "在mSkillIds中找不到", 1f);
				return;
			}
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == id);
			if (skillItem == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("告诉陈宜明：" + id + "在mSkillItems中找不到", 1f);
				return;
			}
			skillItem.SetActiveMask1(false);
			skillItem.SetTweenCD1(0f, 1f);
		}

		private void OnSkill0(GameObject objct_1 = null, bool ispress = false)
		{
			if (ispress)
			{
				if (LevelManager.CurBattleType != 6 && StrategyManager.Instance != null)
				{
					StrategyManager.Instance.UpdateInputState(true);
				}
				this.selectSkill = objct_1.GetComponentInParent<SkillItem>();
				Skill skillByIndex = this.m_Player.getSkillByIndex(this.selectSkill.SkillItemIndex);
				if (skillByIndex == null)
				{
					return;
				}
				string skill_id = skillByIndex.skillMainId;
				this.selectSkill.CheckCanPress();
				if (this.m_Player.GetCDTime(skill_id) <= 0f && (skillByIndex.CheckSkillCanUseSpecial || (this.m_Player.CanClickSkillUI(skill_id) && !this.m_Player.LockInputState)) && this.selectSkill.canPress)
				{
					int num = this.mSkillIds.IndexOf(skill_id);
					SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skill_id);
					if (!skillByIndex.CheckCondition() && !this.m_Player.IsSkillCanTriggerBornPowerObj(skill_id))
					{
						return;
					}
					if (this.m_Player.isLive && !this.m_Player.isShowSkillPointer())
					{
						skillItem.SetActiveMask1(true);
						skillItem.SetTweenCD1(0.5f, 1f);
					}
					else if (!this.m_Player.isLive)
					{
						skillItem.SetActiveMask1(false);
						skillItem.SetTweenCD1(0f, 1f);
					}
					this.m_Player.OnSkill(skill_id);
					if (!this.m_Player.isLive)
					{
						if (this.mpNotEnoughClipInfo.clipName != "sd_int_close")
						{
							this.mpNotEnoughClipInfo = default(AudioClipInfo);
							this.mpNotEnoughClipInfo.clipName = "sd_int_close";
							this.mpNotEnoughClipInfo.audioSourceType = eAudioSourceType.UI;
							this.mpNotEnoughClipInfo.audioPriority = 128;
							this.mpNotEnoughClipInfo.volume = 1f;
						}
						AudioMgr.Play(this.mpNotEnoughClipInfo, null);
					}
					NewbieManager.Instance.TryHandleUseSkill(this.selectSkill.SkillItemIndex);
				}
			}
		}

		private void OnSkill(GameObject objct_1 = null, bool ispress = false)
		{
			if (this.getImmidiatlySkillFlag())
			{
				return;
			}
			if (ispress)
			{
				if (LevelManager.CurBattleType != 6 && StrategyManager.Instance != null)
				{
					StrategyManager.Instance.UpdateInputState(true);
				}
				this.selectSkill = objct_1.GetComponentInParent<SkillItem>();
				Skill skillByIndex = this.m_Player.getSkillByIndex(this.selectSkill.SkillItemIndex);
				if (skillByIndex == null)
				{
					return;
				}
				string skill_id = skillByIndex.skillMainId;
				if (this.m_Player.GetCDTime(skill_id) <= 0f && (skillByIndex.CheckSkillCanUseSpecial || (this.m_Player.CanClickSkillUI(skill_id) && !this.m_Player.LockInputState)))
				{
					int num = this.mSkillIds.IndexOf(skill_id);
					SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.name == skill_id);
					if (skillItem.GetTweenCD1() > 0f)
					{
						return;
					}
					if (skillByIndex.needTarget && PlayerControlMgr.Instance.GetSelectedTarget() == null && skillItem.GetTweenCD1() > 0f)
					{
						return;
					}
					this.m_Player.OnSkill(skill_id);
					if (!this.m_Player.isShowSkillPointer() && this.m_Player.isLive)
					{
						skillItem.SetActiveMask1(true);
						skillItem.SetTweenCD1(0.5f, 1f);
					}
					else if (!this.m_Player.isLive)
					{
						skillItem.SetActiveMask1(false);
						skillItem.SetTweenCD1(0f, 1f);
					}
					if (!this.m_Player.isLive)
					{
						if (this.mpNotEnoughClipInfo.clipName != "sd_int_close")
						{
							this.mpNotEnoughClipInfo = default(AudioClipInfo);
							this.mpNotEnoughClipInfo.clipName = "sd_int_close";
							this.mpNotEnoughClipInfo.audioSourceType = eAudioSourceType.UI;
							this.mpNotEnoughClipInfo.audioPriority = 128;
							this.mpNotEnoughClipInfo.volume = 1f;
						}
						AudioMgr.Play(this.mpNotEnoughClipInfo, null);
					}
				}
			}
		}

		private void OnLockSkill(GameObject objct_1 = null, bool ispress = false)
		{
			if (ispress)
			{
				this.m_propmpt.gameObject.SetActive(true);
				this.m_propmpt.text = "技能未解锁";
				if (this.mpNotEnoughClipInfo.clipName != "sd_int_close")
				{
					this.mpNotEnoughClipInfo = default(AudioClipInfo);
					this.mpNotEnoughClipInfo.clipName = "sd_int_close";
					this.mpNotEnoughClipInfo.audioSourceType = eAudioSourceType.UI;
					this.mpNotEnoughClipInfo.audioPriority = 128;
					this.mpNotEnoughClipInfo.volume = 1f;
				}
				AudioMgr.Play(this.mpNotEnoughClipInfo, null);
			}
			else
			{
				this.m_propmpt.gameObject.SetActive(false);
			}
		}

		public void ShowSkillWarn(string str)
		{
			this.m_propmpt.gameObject.SetActive(true);
			this.m_propmpt.enabled = true;
			this.m_propmpt.text = str;
		}

		public void HideSkillWarn()
		{
			this.m_propmpt.gameObject.SetActive(false);
		}

		private void OnNoLockSkill(GameObject objct_1 = null, bool ispress = false)
		{
			if (ispress)
			{
				SkillItemType skillType = objct_1.GetComponentInParent<SkillItem>().GetSkillType();
				if (skillType == SkillItemType.passive)
				{
					this.m_propmpt.gameObject.SetActive(true);
					this.m_propmpt.text = LanguageManager.Instance.GetStringById("BattlePopText_PassiveSkillNONeedToUse");
				}
			}
			else
			{
				this.m_propmpt.gameObject.SetActive(false);
			}
		}

		public void ShowGuideBar(bool b, float dur = 1f, string name = "回城")
		{
			if (!this.transform)
			{
				return;
			}
			if (this.trGuidBar == null)
			{
				return;
			}
			if (this.trGuidBar.gameObject.activeSelf != b)
			{
				this.trGuidBar.gameObject.SetActive(b);
			}
			if (b)
			{
				this.guidInfo.text = name;
				this.guidTweenFill.duration = dur;
				this.guidTweenFill.Begin();
			}
		}

		private void PauseGuideBar()
		{
			if (this.trGuidBar.gameObject.activeSelf)
			{
				this.guidTweenFill.Pause();
			}
		}

		private void ResumeGuideBar()
		{
			if (this.trGuidBar != null && this.trGuidBar.gameObject.activeSelf)
			{
				this.guidTweenFill.Resume();
			}
		}

		private void OnGuideBarFinished()
		{
			this.ShowGuideBar(false, 1f, "回城");
		}

		private void PauseAllCD()
		{
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				this.mSkillItems[i].Pause();
			}
		}

		private void ResumeAllCD()
		{
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				this.mSkillItems[i].Resume();
			}
		}

		public void GetMessages(object args)
		{
			MessageEventArgs messageEventArgs = (MessageEventArgs)args;
			switch (int.Parse(messageEventArgs.GetMessage("type")))
			{
			case 256:
			{
				string message = messageEventArgs.GetMessage("id");
				Singleton<SkillView>.Instance.UpdateSkillView(message, false);
				break;
			}
			}
		}

		public void SetLevelUpBtns(bool isShow)
		{
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				this.mSkillItems[i].SetLeveupBtn(isShow);
			}
		}

		public void FlyOut()
		{
			this.transform.Find(this.AnchorPath + "/Panel").GetComponent<TweenPosition>().PlayForward();
			this.transform.Find(this.AnchorPath + "/ExtraSkillPanel").GetComponent<TweenPosition>().PlayForward();
		}

		public void FlyIn()
		{
			this.UpdateSelfValue(this.m_Player, false, true);
			this.transform.Find(this.AnchorPath + "/Panel").GetComponent<TweenPosition>().PlayReverse();
			this.transform.Find(this.AnchorPath + "/ExtraSkillPanel").GetComponent<TweenPosition>().PlayReverse();
		}

		private void InitSkillPanelPivot(SkillPanelPivot pivot)
		{
			this.skillPanelPivot = pivot;
			if (pivot == SkillPanelPivot.Bottom)
			{
				this.AnchorPath = "Anchor_b";
			}
			else if (pivot == SkillPanelPivot.Left)
			{
				this.AnchorPath = "Anchor_l";
			}
			else if (pivot == SkillPanelPivot.Right)
			{
				this.AnchorPath = "Anchor_r";
			}
			this.transform.Find("Anchor_b").gameObject.SetActive(false);
			this.transform.Find("Anchor_l").gameObject.SetActive(false);
			this.transform.Find("Anchor_r").gameObject.SetActive(false);
			this.transform.Find(this.AnchorPath).gameObject.SetActive(true);
			this.transform.Find(this.AnchorPath).GetComponent<UIAnchor>().enabled = this.transform;
			this.Init();
		}

		public void SetSkillPanelPivot(SkillPanelPivot pivot)
		{
			if (this.skillPanelPivot == pivot)
			{
				return;
			}
			this.InitSkillPanelPivot(pivot);
			this.ChangeSkillItem();
		}

		public void SetTransSkillPo(Vector3 v3)
		{
			SkillItem skillItem = this.mSkillItems.Find((SkillItem obj) => obj.gameObject == this.ExtraSkillPanel.Find("ExtraSkill0").gameObject);
			if (skillItem != null)
			{
				skillItem.transform.localPosition = v3;
			}
		}

		private void ChangeSkillItem()
		{
			this.childCount = 0;
			this.ExtraSkillCount = this.mSkillIds.Count - 4;
			this.ExtraSkillIndex = 0;
			for (int i = 0; i < this.mSkillItems.Count; i++)
			{
				this.skillItemScript = this.mSkillItems[i];
				this.skillVo = SkillUtility.GetSkillData(this.mSkillIds[this.mSkillItems[i].SkillItemIndex], -1, -1);
				if (this.skillVo == null)
				{
					ClientLogger.Error("skill is null");
				}
				else if (i >= this.childCount)
				{
					if (i >= this.mSkillIds.Count - this.ExtraSkillCount)
					{
						if (this.skillVo.skill_index == 7)
						{
							this.ExtraSkillPanel.Find("ExtraSkill0").gameObject.SetActive(true);
							this.skillItemScript.transform.parent = this.ExtraSkillPanel.Find("ExtraSkill0");
							this.skillItemScript.transform.localPosition = Vector3.zero;
						}
						else
						{
							this.ExtraSkillIndex++;
							this.skillItemScript.transform.parent = this.ExtraSkillPanel.Find("ExtraSkill" + this.ExtraSkillIndex.ToString());
							this.skillItemScript.transform.localPosition = Vector3.zero;
						}
					}
					else
					{
						this.skillItemScript.transform.parent = this.SkillPanel;
					}
					this.countChange = true;
					this.skillItemScript.SetCdParent(this._cdPanel);
					this.skillItemScript.SetLevelUpToCdParent(this._cdPanel);
					this.skillItemScript.SetSkillPivot(this.skillPanelPivot);
					if (this.skillVo.skill_trigger == 3)
					{
						this.skillItemScript.SetSkillType(SkillItemType.passive);
					}
					else
					{
						this.skillItemScript.SetSkillType(SkillItemType.active);
					}
				}
			}
			this.CheckIconToGrayByCanUse(null, -1);
			if (this.countChange && this.gameObject.activeInHierarchy)
			{
				this.countChange = false;
				this.m_uiGrid.Reposition();
				for (int j = 0; j < this.mSkillItems.Count; j++)
				{
					this.mSkillItems[j].SetLevelUpToCdParent(this._cdPanel);
				}
			}
			this.ShowCanLevelUp();
		}

		public SkillPanelPivot GetSkillPanelPivot()
		{
			return this.skillPanelPivot;
		}

		public bool CheckSkillHelp(int skillIndex)
		{
			if (this.skillHelp == null && this.m_Player != null)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.m_Player.npc_id);
				if (heroMainData != null && heroMainData.skill_help != null && heroMainData.skill_help != string.Empty && heroMainData.skill_help != "[]")
				{
					this.skillHelp = heroMainData.skill_help.Split(new char[]
					{
						','
					});
				}
			}
			return this.skillHelp != null && !(this.m_Player == null) && (this.skillHelp.Length > this.m_Player.level - this.canUseLevelUpPoints && this.skillHelp[this.m_Player.level - this.canUseLevelUpPoints] == (skillIndex + 1).ToString());
		}
	}
}
