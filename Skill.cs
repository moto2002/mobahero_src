using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Skill : StaticUnitComponent
{
	private enum ESkillTargetStatus
	{
		TargetSuccess,
		TargetNotFound,
		TargetInvalid
	}

	public Callback<string, int> DestroyAction;

	public int BulletIndex;

	private bool _isTalentSkill;

	protected string skillID = string.Empty;

	protected List<SkillData> skillDataList = new List<SkillData>();

	public Units unit;

	public Units attackTarget;

	public Vector3? attackPosition;

	public Vector3? externalPostion;

	public List<Units> attackTargets;

	public Task endTask;

	public Dictionary<int, List<string>> higheff_ids;

	public Dictionary<int, List<string>> buff_ids;

	protected new CoroutineManager m_CoroutineManager = new CoroutineManager();

	private Task m_SkillPhaseTask;

	private Task m_GrassTask;

	public int skillSubIdx;

	public int skillLevel;

	public int skillLevelMax;

	public float publicCD;

	private bool isStart;

	private bool isStop;

	private bool isEnd;

	private bool isInterrupt;

	private bool isMiss;

	private bool isCrit;

	public Callback<Skill> OnSkillStartCallback;

	public Callback<Skill> OnSkillEndCallback;

	public Callback<Skill> OnSkillInterruptCallback;

	public Callback<Skill> OnSkillDamageCallback;

	public Callback<Skill> OnSkillFailedBeforeStartCallback;

	public static Action<Skill, Units> OnCustomDamage;

	private Dictionary<int, List<BaseAction>> mActions = new Dictionary<int, List<BaseAction>>();

	public Callback<List<Units>> OnTargetHitCallback;

	public Callback<string, int> OnPerformHitCallback;

	private List<string> _bighEffList = new List<string>();

	private List<string> _buffList = new List<string>();

	private SimpleSkillAction _passiveSimple = new SimpleSkillAction();

	public bool isTalentSkill
	{
		get
		{
			return this._isTalentSkill;
		}
		set
		{
			this._isTalentSkill = value;
		}
	}

	public virtual List<SkillData> SkillDataList
	{
		get
		{
			return this.skillDataList;
		}
	}

	public SkillData data
	{
		get
		{
			int num = (this.skillLevel <= 0) ? this.skillLevel : (this.skillLevel - 1);
			if (num >= 0 && num < this.SkillDataList.Count)
			{
				return this.SkillDataList[num];
			}
			ClientLogger.Error(string.Concat(new object[]
			{
				"无法获取相应等级的技能数据 请检查表！！ cur Level ",
				this.skillLevel,
				" cur level index ",
				num,
				" SkillId=",
				this.skillMainId
			}));
			return null;
		}
	}

	public SkillData Data
	{
		get
		{
			return this.data;
		}
	}

	public virtual string SkillIcon
	{
		get
		{
			return null;
		}
	}

	public string skillMainId
	{
		get
		{
			return this.skillID;
		}
	}

	public string realSkillMainId
	{
		get;
		set;
	}

	public virtual SkillDataKey skillKey
	{
		get
		{
			return new SkillDataKey(this.skillID, this.skillLevel, 0);
		}
	}

	public int skillIndex
	{
		get
		{
			return this.data.config.skill_index;
		}
	}

	public string skillName
	{
		get
		{
			return this.data.config.skill_name;
		}
	}

	public SkillType skillType
	{
		get
		{
			return (SkillType)this.data.config.skill_type;
		}
	}

	public float public_cd
	{
		get
		{
			return this.data.castEnd;
		}
	}

	public float distance
	{
		get
		{
			return this.data.config.distance;
		}
	}

	public int maxNum
	{
		get
		{
			return this.data.config.max_num;
		}
	}

	public int castingType
	{
		get
		{
			return this.data.skillCastingType;
		}
	}

	public bool needTarget
	{
		get
		{
			return this.data.needTarget;
		}
	}

	public EffectiveRangeType errect_range_type
	{
		get
		{
			return this.data.effectiveRangeType;
		}
	}

	public float effect_range1
	{
		get
		{
			return this.data.effectRange1;
		}
	}

	public float effect_range2
	{
		get
		{
			return this.data.effectRange2;
		}
	}

	public float CD
	{
		get
		{
			if (this.data == null)
			{
				return 1f;
			}
			float cd = this.data.config.cd;
			if (this.IsNormalSkill)
			{
				return cd / (1f + Mathf.Clamp(this.self.GetAttr(AttrType.NormalSkillCooling), 0f, 3.40282347E+38f));
			}
			if (this.IsSummonerSkill)
			{
				return cd / (1f + Mathf.Clamp(this.self.GetAttr(AttrType.SummonSkillCooling), 0f, 3.40282347E+38f));
			}
			if (this.IsItemSkill)
			{
				return cd / (1f + Mathf.Clamp(this.self.GetAttr(AttrType.ItemSkillCooling), 0f, 3.40282347E+38f));
			}
			return cd;
		}
	}

	public SkillCastPhase CastPhase
	{
		get;
		set;
	}

	public bool IsInSkillCastNone
	{
		get
		{
			return this.CastPhase == SkillCastPhase.Cast_None;
		}
	}

	public bool IsInSkillCastBefore
	{
		get
		{
			return this.CastPhase == SkillCastPhase.Cast_Before;
		}
	}

	public bool IsInSkillCastIn
	{
		get
		{
			return this.CastPhase == SkillCastPhase.Cast_In;
		}
	}

	public bool IsInSkillCastEnd
	{
		get
		{
			return this.CastPhase == SkillCastPhase.Cast_End;
		}
	}

	public bool IsCDTimeOver
	{
		get
		{
			float cDTime = this.unit.GetCDTime(this.skillMainId);
			return cDTime <= 0f;
		}
	}

	public bool IsCastting
	{
		get
		{
			return this.m_SkillPhaseTask != null && this.m_SkillPhaseTask.Running;
		}
	}

	public bool IsGuide
	{
		get
		{
			return this.data.isGuide;
		}
	}

	public bool IsShowGuideBar
	{
		get
		{
			return this.data.isShowGuideBar;
		}
	}

	public bool IsSkillOver
	{
		get
		{
			return this.isInterrupt || this.isStop || this.isEnd;
		}
	}

	public bool IsInstance
	{
		get
		{
			return this.data.config.skill_trigger == 1;
		}
	}

	public bool IsAttack
	{
		get
		{
			return this.data.config.skill_type == 1;
		}
	}

	public bool IsSkill
	{
		get
		{
			return !this.IsAttack;
		}
	}

	public bool IsNormalSkill
	{
		get
		{
			return this.skillIndex < 4;
		}
	}

	public bool IsSummonerSkill
	{
		get
		{
			return SkillUtility.IsSummonerSkill(this);
		}
	}

	public bool IsItemSkill
	{
		get
		{
			return this.skillIndex == 6;
		}
	}

	public bool IsFocus
	{
		get
		{
			return this.data.config.skill_trigger == 4;
		}
	}

	public bool IsPassive
	{
		get
		{
			return SkillUtility.IsSkillPassive(this.skillID);
		}
	}

	public bool IsDrag
	{
		get
		{
			return this.data.config.skill_trigger == 2;
		}
	}

	public bool IsInitiative
	{
		get
		{
			return !this.IsPassive && this.data.config.skill_trigger != 0;
		}
	}

	public bool IsInitiativeSkill
	{
		get
		{
			return !this.IsAttack && !this.IsPassive && this.data.config.skill_trigger != 0;
		}
	}

	public bool CheckSkillCanUseSpecial
	{
		get
		{
			return this.skillMainId == "Summoner_Cleanse" || this.skillMainId == "Skill_Guanyu_04" || this.skillMainId == "Skill_DK_04";
		}
	}

	public bool CanUseMoveSkill
	{
		get
		{
			return this.self.CanMove || !this.data.isMoveSkill;
		}
	}

	public bool IsSingleAttack
	{
		get
		{
			return this.data.effectiveRangeType == EffectiveRangeType.Single;
		}
	}

	public bool IsSelectTarget
	{
		get
		{
			return this.data.config.hit_trigger_type == 1 && this.data.needTarget;
		}
	}

	public Skill()
	{
	}

	public Skill(string skill_id, Units self)
	{
		this.self = self;
		this.init(self, skill_id);
	}

	public bool IsSkillTarget(GameObject hit, SkillTargetCamp targetCampType, out Units unit)
	{
		unit = null;
		if (hit != null)
		{
			if (!TagManager.IsAttackTarget(hit))
			{
				return false;
			}
			if (!TeamManager.CheckTeam(unit.gameObject, hit.gameObject, targetCampType, null))
			{
				return false;
			}
			Units component = hit.gameObject.GetComponent<Units>();
			if (component)
			{
				unit = component;
				return true;
			}
		}
		return false;
	}

	public virtual bool CanInterrupt(SkillInterruptType type)
	{
		if (this.IsSkillOver)
		{
			return false;
		}
		if (type == SkillInterruptType.None)
		{
			return false;
		}
		if (this.IsInitiativeSkill)
		{
			return (this.IsInSkillCastBefore && (type == this.data.interruptBefore || type == SkillInterruptType.Force || this.data.interruptBefore == SkillInterruptType.Force)) || (this.IsInSkillCastIn && (type == this.data.interruptIn || type == SkillInterruptType.Force || this.data.interruptIn == SkillInterruptType.Force)) || (this.IsInSkillCastEnd && (type == this.data.interruptEnd || type == SkillInterruptType.Force || this.data.interruptEnd == SkillInterruptType.Force));
		}
		return this.IsAttack && ((this.IsInSkillCastBefore && (type == this.data.interruptBefore || type == SkillInterruptType.Force || this.data.interruptBefore == SkillInterruptType.Force)) || (this.IsInSkillCastIn && (type == this.data.interruptIn || type == SkillInterruptType.Force || this.data.interruptIn == SkillInterruptType.Force)) || (this.IsInSkillCastEnd && (type == this.data.interruptEnd || type == SkillInterruptType.Force || this.data.interruptEnd == SkillInterruptType.Force)));
	}

	public virtual void OnReadySkill()
	{
	}

	public virtual void OnStartSkill()
	{
	}

	public virtual void SynInfo(SynSkillInfo info)
	{
	}

	public virtual void RefreshSkillIcon()
	{
	}

	private void init(Units theurgist, string mainId)
	{
		this.skillDataList.Clear();
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(mainId);
		if (skillMainData != null)
		{
			this.skillLevelMax = skillMainData.skill_levelmax;
			if (this.skillLevelMax <= 0)
			{
				this.skillDataList.Add(GameManager.Instance.SkillData.GetData(mainId, 0, 0));
				this.skillLevel = 0;
			}
			else
			{
				for (int i = 1; i <= this.skillLevelMax; i++)
				{
					this.skillDataList.Add(GameManager.Instance.SkillData.GetData(mainId, i, 0));
				}
				if ((LevelManager.IsTestingLevel && !LevelManager.Instance.CheckSceneIsTest()) || GlobalSettings.isSkillUnlock)
				{
					this.skillLevel = 1;
				}
				else
				{
					this.skillLevel = 0;
				}
			}
		}
		this.unit = theurgist;
		this.skillID = mainId;
		this.realSkillMainId = mainId;
		this.mActions.Clear();
	}

	public bool isSkillVisible()
	{
		if (GlobalSettings.FogMode <= 1)
		{
			return true;
		}
		Vector3? skillPosition = this.GetSkillPosition();
		Vector3 vector = this.unit.transform.position;
		Vector3 vector2 = this.unit.transform.forward;
		if (skillPosition.HasValue)
		{
			vector2 = skillPosition.Value - vector;
		}
		vector2.Normalize();
		Vector3 side = Vector3.Cross(Vector3.up, vector2);
		if (this.data.effectiveRangeType == EffectiveRangeType.JuXing)
		{
			return FOWSystem.Instance.IsVisible_Rect(vector, vector2, side, this.data.effectRange1, this.data.effectRange2, false);
		}
		if (this.data.effectiveRangeType == EffectiveRangeType.ShanXing)
		{
			return FOWSystem.Instance.IsVisible_Fan(vector, vector2, side, this.data.effectRange1, this.data.effectRange2 * 0.0174532924f);
		}
		if (this.data.effectiveRangeType == EffectiveRangeType.YuanXing)
		{
			if (skillPosition.HasValue)
			{
				vector = skillPosition.Value;
			}
			return FOWSystem.Instance.IsVisible_Circle(vector, vector2, side, this.data.effectRange1, this.data.effectRange2);
		}
		if (this.data.effectiveRangeType == EffectiveRangeType.Single)
		{
			float len = this.distance;
			if (skillPosition.HasValue)
			{
				len = Vector3.Distance(skillPosition.Value, vector);
			}
			return FOWSystem.Instance.IsVisible_Rect(vector, vector2, side, len, 0f, false);
		}
		return FOWSystem.Instance.IsVisible(skillPosition.Value);
	}

	public List<Units> GetSkillTargets()
	{
		List<Units> list = null;
		SkillType skillType = this.skillType;
		if (skillType != SkillType.Attck)
		{
			if (skillType == SkillType.Skill)
			{
				Vector3? skillPosition = this.GetSkillPosition();
				if (skillPosition.HasValue)
				{
					list = FindTargetHelper.findTargets(this.unit, skillPosition.Value, this.data.targetCamp, this.data.targetTag, this.data.selectRangeType, this.data.selectRange1, this.data.selectRange2, this.data.config.max_num);
				}
				if (list == null)
				{
					list = new List<Units>();
					if (this.attackTarget != null && TagManager.CheckTag(this.attackTarget, this.data.targetTag) && TeamManager.CheckTeam(this.self.gameObject, this.attackTarget.gameObject, this.data.targetCamp, null))
					{
						list.Add(this.attackTarget);
					}
				}
				else if (list.Count == 0)
				{
					if (this.attackTarget != null && TagManager.CheckTag(this.attackTarget, this.data.targetTag) && TeamManager.CheckTeam(this.self.gameObject, this.attackTarget.gameObject, this.data.targetCamp, null) && this.attackTarget.isLive)
					{
						list.Add(this.attackTarget);
					}
				}
				else if (!this.IsInstance)
				{
					if (this.attackTarget != null)
					{
						if (this.attackTarget.teamType != this.self.teamType)
						{
							if (list.Contains(this.attackTarget))
							{
								list.Remove(this.attackTarget);
								list.Insert(0, this.attackTarget);
							}
							else if (this.attackTarget.isLive)
							{
								list.Insert(0, this.attackTarget);
							}
						}
					}
					else if (this.castingType == 1 && this.self.isPlayer)
					{
						list.Clear();
					}
				}
			}
		}
		else if (this.IsSingleAttack)
		{
			if (this.attackTarget != null)
			{
				list = new List<Units>();
				list.Add(this.attackTarget);
			}
		}
		else
		{
			list = FindTargetHelper.findTargets(this.unit, this.attackTarget.mTransform.position, this.data.targetCamp, this.data.targetTag, this.data.selectRangeType, this.data.selectRange1, this.data.selectRange2, this.data.config.max_num);
			if (list != null)
			{
				if (this.attackTarget != null && list.Contains(this.attackTarget))
				{
					list.Remove(this.attackTarget);
					list.Insert(0, this.attackTarget);
				}
			}
			else if (this.attackTarget != null)
			{
				list = new List<Units>();
				list.Add(this.attackTarget);
			}
		}
		return list;
	}

	protected virtual Vector3? GetSkillPosition()
	{
		Vector3? vector = this.externalPostion;
		if (vector.HasValue)
		{
			return this.externalPostion;
		}
		int skill_trigger = this.data.config.skill_trigger;
		if (skill_trigger == 1)
		{
			Vector3 position = this.unit.transform.position;
			return new Vector3?(position);
		}
		if (skill_trigger != 2)
		{
			return null;
		}
		SkillPointer skillPointer = null;
		if (this.unit.mSkillPointer.IsValid<SkillPointer>())
		{
			skillPointer = this.unit.mSkillPointer.Component;
		}
		Vector3 value = (!skillPointer) ? (this.unit.transform.position + this.unit.transform.forward * this.data.config.distance) : skillPointer.endPosition;
		return new Vector3?(value);
	}

	private void AssianAttackPosition(Vector3? pos)
	{
		this.attackPosition = pos;
	}

	private bool CheckNormalAttackTargets(List<Units> inTargets)
	{
		if (!this.data.needTarget)
		{
			return true;
		}
		Skill.ESkillTargetStatus eSkillTargetStatus = Skill.ESkillTargetStatus.TargetSuccess;
		if (inTargets == null || inTargets.Count <= 0)
		{
			eSkillTargetStatus = Skill.ESkillTargetStatus.TargetNotFound;
		}
		else if (inTargets[0] != null && this.unit.teamType == inTargets[0].teamType)
		{
			eSkillTargetStatus = Skill.ESkillTargetStatus.TargetInvalid;
		}
		if (eSkillTargetStatus != Skill.ESkillTargetStatus.TargetSuccess)
		{
			if (this.unit.isPlayer)
			{
				if (eSkillTargetStatus == Skill.ESkillTargetStatus.TargetNotFound)
				{
					UIMessageBox.ShowMessage("未找到目标，不能释放！", 1.5f, 0);
				}
				this.OnSkillEnd();
				if (this.unit.UnitController != null)
				{
					this.unit.UnitController.OnSkillNotFindTargets();
				}
			}
			return false;
		}
		return true;
	}

	public void AssianAttackTarget(Units target = null)
	{
		if (target != null)
		{
			this.attackTarget = target;
		}
		this.attackTargets = this.GetSkillTargets();
	}

	public bool CheckTargets()
	{
		this.AssianAttackTarget(null);
		this.AssianAttackPosition(this.GetSkillPosition());
		if (this.IsAttack)
		{
			return this.CheckNormalAttackTargets(this.attackTargets);
		}
		if (this.data.needTarget)
		{
			if (this.attackTargets == null || this.attackTargets.Count <= 0)
			{
				if (this.unit.isPlayer)
				{
					UIMessageBox.ShowMessage("未找到目标，不能释放！", 1.5f, 0);
					this.OnSkillEnd();
					if (this.unit.UnitController != null)
					{
						this.unit.UnitController.OnSkillNotFindTargets();
					}
				}
				return false;
			}
			if (this.unit.isPlayer && !this.IsAttack && this.data.targetCamp != SkillTargetCamp.Self)
			{
				float num = Vector3.Distance(this.attackTargets[0].transform.position, this.self.transform.position);
				if (num > this.distance)
				{
					UIMessageBox.ShowMessage("超出施法范围，不能释放！", 1.5f, 0);
					this.OnSkillEnd();
					if (this.unit.UnitController != null)
					{
						this.unit.UnitController.OnSkillNotFindTargets();
					}
					return false;
				}
			}
		}
		return true;
	}

	public virtual bool CheckCondition()
	{
		return this.CheckState() && this.CheckCD(true) && this.CheckCostValue();
	}

	protected bool CheckState()
	{
		if (this.data.config.skill_type == 1 && this.unit.ZhiMang.IsInState && this.IsAttack)
		{
			if (this.unit.isPlayer)
			{
				UIMessageBox.ShowMessage("无法攻击！", 1.5f, 0);
			}
			return false;
		}
		return true;
	}

	public virtual bool CheckCD(bool showMsg = true)
	{
		if (!this.IsCDTimeOver && this.IsInitiativeSkill)
		{
			if (showMsg && this.unit.isPlayer)
			{
				UIMessageBox.ShowMessage("技能冷却中", 1.5f, 0);
			}
			return false;
		}
		return true;
	}

	protected bool CheckCostValue()
	{
		if (GlobalSettings.NoCost)
		{
			return true;
		}
		if (this.data == null)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				this.skillMainId,
				", level:",
				this.skillLevel,
				", data == null"
			}));
			return false;
		}
		if (this.data.cost_ids != null)
		{
			for (int i = 0; i < this.data.cost_ids.Length; i++)
			{
				DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(this.data.cost_ids[i]);
				if (vo == null)
				{
					UnityEngine.Debug.LogError(string.Concat(new object[]
					{
						this.skillMainId,
						", level:",
						this.skillLevel,
						", cost_ids[i](",
						this.data.cost_ids[i],
						") is null"
					}));
				}
				else
				{
					float num = Mathf.Abs(this.GetCostValue(vo.property_key));
					AttrType property_key = vo.property_key;
					if (property_key != AttrType.Hp)
					{
						if (property_key == AttrType.Mp)
						{
							bool flag = true;
							if (this.unit.mp < num)
							{
								flag = false;
							}
							if (!flag)
							{
								if (this.unit.isPlayer && LevelManager.CurBattleType != 6)
								{
									Singleton<SkillView>.Instance.lanTiao.ShowQueLan();
								}
								return false;
							}
						}
					}
					else
					{
						bool flag2 = true;
						if (this.unit.hp <= num)
						{
							flag2 = false;
						}
						if (!flag2)
						{
							if (this.unit.isPlayer)
							{
								UIMessageBox.ShowMessage("生命值不足", 1.5f, 0);
							}
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	public virtual float GetCostValue(AttrType type)
	{
		if (GlobalSettings.NoCost)
		{
			return 0f;
		}
		if (this.data.cost_ids != null)
		{
			for (int i = 0; i < this.data.cost_ids.Length; i++)
			{
				DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(this.data.cost_ids[i]);
				if (vo == null)
				{
					UnityEngine.Debug.LogError("伤害包没找到 Error id=" + this.data.cost_ids[i]);
					return 0f;
				}
				if (vo.property_key == type)
				{
					if (vo.property_percent)
					{
						AttrType property_key = vo.property_key;
						if (property_key == AttrType.Hp)
						{
							return -vo.property_value * this.unit.hp_max;
						}
						if (property_key == AttrType.Mp)
						{
							return -vo.property_value * this.unit.mp_max;
						}
					}
					else
					{
						if (vo.IsPropertyFormula)
						{
							return FormulaTool.GetFormualValue(vo.property_formula, this.self);
						}
						return vo.property_value;
					}
				}
			}
		}
		return 0f;
	}

	[DebuggerHidden]
	private IEnumerator doSkillPhase()
	{
		Skill.<doSkillPhase>c__Iterator95 <doSkillPhase>c__Iterator = new Skill.<doSkillPhase>c__Iterator95();
		<doSkillPhase>c__Iterator.<>f__this = this;
		return <doSkillPhase>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator doAttackPhase()
	{
		Skill.<doAttackPhase>c__Iterator96 <doAttackPhase>c__Iterator = new Skill.<doAttackPhase>c__Iterator96();
		<doAttackPhase>c__Iterator.<>f__this = this;
		return <doAttackPhase>c__Iterator;
	}

	public void ProcessAfterSkillEnd()
	{
		this.TryAttackSelectTargetAfterSkillEnd();
		this.TryClearSelectTargetAfterSkillEnd();
	}

	private void TryAttackSelectTargetAfterSkillEnd()
	{
		if (!this.IsStartAttackSelectedTargetOnSkillEnd())
		{
			return;
		}
		if (this.m_SkillPhaseTask != null)
		{
			this.m_CoroutineManager.StopCoroutine(this.m_SkillPhaseTask);
			this.m_SkillPhaseTask = null;
		}
		this.self.TryAttackSelectTargetAfterSkillEnd();
	}

	private void TryClearSelectTargetAfterSkillEnd()
	{
		if (this.IsClearSelectedTargetOnSkillEnd())
		{
			this.self.SetSelectTarget(null);
		}
	}

	private bool IsStartAttackSelectedTargetOnSkillEnd()
	{
		return (this.data.config.is_autoattackseltarget_onskillend & 1) != 0;
	}

	private bool IsClearSelectedTargetOnSkillEnd()
	{
		return (this.data.config.is_autoattackseltarget_onskillend & 2) != 0;
	}

	public virtual bool NeedResetTargetPos()
	{
		return false;
	}

	public virtual Vector3 GetExtraTargetPos(Units target, Vector3 targetPos, bool isCrazyMode = true)
	{
		return targetPos;
	}

	private bool IsNormalAttackTargetConditionSatisfied()
	{
		if (!this.self.isTower && !this.IsSelectTarget)
		{
			return true;
		}
		if (this.attackTargets == null || this.attackTargets.Count <= 0 || !(this.attackTargets[0] != null))
		{
			return false;
		}
		if (this.attackTargets[0].isLive)
		{
			float attr = this.self.GetAttr(AttrType.AttackRange);
			return (this.attackTargets[0].mTransform.position - this.unit.mTransform.position).sqrMagnitude <= attr * attr;
		}
		return false;
	}

	private bool IsNormalAttackCasterStatusConditionSatisfied()
	{
		return this.self.isTower || !this.unit.ZhiMang.IsInState;
	}

	private bool CheckNormalAttackCastCondition()
	{
		bool flag = true;
		flag = (flag && this.IsNormalAttackTargetConditionSatisfied());
		return flag && this.IsNormalAttackCasterStatusConditionSatisfied();
	}

	private void TryAssignAttackPosition()
	{
		if ((this.self.isTower || this.IsSelectTarget) && this.attackTargets != null && this.attackTargets.Count > 0 && this.attackTargets[0] != null)
		{
			this.AssianAttackPosition(new Vector3?(this.attackTargets[0].transform.position));
		}
	}

	private void TryShowNormalAttackCastFailHint()
	{
		if (!this.self.isTower && this.unit.isPlayer && this.unit.ZhiMang.IsInState)
		{
			UIMessageBox.ShowMessage("无法攻击！", 1.5f, 0);
		}
	}

	protected virtual void OnSkillStart()
	{
		if (this.IsInitiativeSkill && !this.IsSummonerSkill)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitPreConjure, this.unit, null, null);
		}
		if (this.IsAttack)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitPreAttack, this.unit, null, null);
		}
	}

	protected void OnSkillPhase1(SkillDataKey skill_key)
	{
		if (!Application.loadedLevelName.Contains("Test"))
		{
			this.OnAiEvadeSkill();
		}
		this.ReadyCastSkill(skill_key);
	}

	protected void OnAiEvadeSkill()
	{
		if (this.IsSummonerSkill || this.IsPassive || !this.IsAttack)
		{
		}
	}

	protected bool OnSkillPhase1End(SkillDataKey skill_key)
	{
		if (this.isInterrupt)
		{
			return false;
		}
		if (this.IsSkill)
		{
			if ((this.IsSelectTarget || this.self.isTower) && this.attackTargets != null && this.attackTargets.Count > 0)
			{
				float num = this.data.config.distance;
				if (this.IsAttack)
				{
					num = this.self.GetAttr(AttrType.AttackRange);
				}
				if (Vector3.Distance(this.attackTargets[0].mTransform.position, this.unit.mTransform.position) > num)
				{
					if (this.unit.isPlayer)
					{
					}
					ActionManager.StopSkill(skill_key, this.unit, SkillInterruptType.Force, true);
					return false;
				}
				this.AssianAttackPosition(new Vector3?(this.attackTargets[0].transform.position));
			}
			if (!this.CheckCondition())
			{
				ActionManager.StopSkill(skill_key, this.unit, SkillInterruptType.Force, true);
				return false;
			}
			if (!this.IsSummonerSkill)
			{
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitConjure, this.unit, null, null);
			}
			if (this.skillIndex == 3 && !this.IsSummonerSkill)
			{
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitConjureR, this.unit, null, null);
			}
			this.genericSet();
			if (this.IsGuide && this.unit.isPlayer && this.IsShowGuideBar)
			{
				Singleton<SkillView>.Instance.ShowGuideBar(true, this.data.guideTime, LanguageManager.Instance.GetStringById(this.skillName));
			}
			if (this.unit.isPlayer && this.unit.UnitController != null)
			{
				this.unit.UnitController.OnSkillCastBeforeEnd();
			}
			return true;
		}
		else
		{
			if (this.IsAttack)
			{
				Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAttack, this.unit, null, null);
				return true;
			}
			return true;
		}
	}

	public virtual void OnSkillReadyBegin(ReadySkillAction action)
	{
	}

	public virtual void OnSkillStartBegin(StartSkillAction action)
	{
	}

	public virtual void OnSkillHitBegin(HitSkillAction action)
	{
	}

	protected void OnSkillPhase2(SkillDataKey skill_key)
	{
		this.StartCastSkill(skill_key);
		if (this.OnSkillStartCallback != null)
		{
			this.OnSkillStartCallback(this);
		}
	}

	protected void OnSkillPhase2End(SkillDataKey skill_key)
	{
		if (this.self.isPlayer)
		{
			this.self.OnSkillEnd(skill_key.SkillID);
		}
	}

	protected void OnSkillPhase3(SkillDataKey skill_key)
	{
		Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdEnd, this.unit, this);
	}

	protected virtual void OnSkillPhase3End(SkillDataKey skill_key)
	{
	}

	protected void OnSkillEnd()
	{
		if (this.OnSkillEndCallback != null)
		{
			this.OnSkillEndCallback(this);
		}
	}

	protected void OnFailedBeforeStartSkill()
	{
		if (this.OnSkillFailedBeforeStartCallback != null)
		{
			this.OnSkillFailedBeforeStartCallback(this);
		}
	}

	protected virtual void OnSkillInterrupt()
	{
		if (this.IsSkill)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillFailed, this.unit, null, null);
		}
		Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdFailed, this.unit, this);
		if (this.unit.isPlayer)
		{
		}
		if (this.OnSkillInterruptCallback != null)
		{
			this.OnSkillInterruptCallback(this);
		}
		this.StopSkillPhase();
	}

	public virtual void ParseExtraParam()
	{
	}

	public virtual void OnSkillDamage(BaseSkillAction action, List<Units> targets)
	{
		SkillDataKey skillKey = action.skillKey;
		if (targets != null && targets.Count > 0)
		{
			for (int i = 0; i < targets.Count; i++)
			{
				if (i >= this.maxNum)
				{
					break;
				}
				if (!(targets[i] == null) && targets[i].isLive)
				{
					if (Skill.OnCustomDamage != null)
					{
						Skill.OnCustomDamage(this, targets[i]);
					}
					else
					{
						this.Damage(skillKey, targets[i], targets.Count);
					}
				}
			}
		}
		if (this.OnSkillDamageCallback != null)
		{
			this.OnSkillDamageCallback(this);
		}
		new HitUnitMsg(this.self);
	}

	protected void OnStartSkillActionEnd(BaseAction action)
	{
		if (this.IsSkillOver)
		{
			return;
		}
	}

	public void InitSkill()
	{
		this.isStart = false;
		this.isStop = false;
		this.isEnd = false;
		this.isInterrupt = false;
		this.isMiss = false;
		this.isCrit = false;
		this.CastPhase = SkillCastPhase.Cast_None;
		this.mActions.Clear();
	}

	public void InitReadySkill()
	{
		this.isStart = false;
		this.isStop = false;
		this.isEnd = false;
	}

	public virtual void Start()
	{
		this.InitSkill();
		if (!this.IsInitiative)
		{
			this.OnFailedBeforeStartSkill();
			return;
		}
		this.SetSkillState();
		this.m_CoroutineManager.StopCoroutine(this.m_SkillPhaseTask);
		if (this.IsAttack)
		{
			this.m_SkillPhaseTask = this.m_CoroutineManager.StartCoroutine(this.doAttackPhase(), false);
			this.m_SkillPhaseTask.Finished += delegate(bool b)
			{
				Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitAttackOver, this.unit, this);
			};
			this.m_SkillPhaseTask.Finished += delegate(bool b)
			{
				Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdOver, this.unit, this);
			};
		}
		else
		{
			this.m_SkillPhaseTask = this.m_CoroutineManager.StartCoroutine(this.doSkillPhase(), false);
			if (this.unit.isPlayer)
			{
				this.m_SkillPhaseTask.Finished += delegate(bool b)
				{
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillOver, this.unit, null, null);
				};
				this.m_SkillPhaseTask.Finished += delegate(bool b)
				{
					Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSkillCmdOver, this.unit, null, null);
				};
			}
		}
		this.m_SkillPhaseTask.Start();
	}

	public void End()
	{
		if (!this.isEnd)
		{
			this.isEnd = true;
			this.OnSkillEnd();
			this.DestroyActions(SkillCastPhase.Cast_None);
			this.Destroy();
		}
	}

	public void Interrupt(SkillInterruptType type)
	{
		if (this.CanInterrupt(type))
		{
			ActionManager.StopSkill(this.skillKey, this.unit, type, true);
		}
	}

	public void Interrupt(bool isForce = false)
	{
		this.isInterrupt = true;
		this.ResetSkillState();
		this.OnSkillInterrupt();
		if (isForce)
		{
			this.DestroyActions(SkillCastPhase.Cast_None);
		}
		else
		{
			this.DestroyActions(this.CastPhase);
		}
		this.unit.ClearReplacePerform();
		this.Destroy();
	}

	public void Interrupt_PVP(SkillCastPhase phase = SkillCastPhase.Cast_None)
	{
		this.isInterrupt = true;
		this.ResetSkillState();
		this.OnSkillInterrupt();
		this.DestroyActions(phase);
		if (phase != SkillCastPhase.Cast_End)
		{
			this.unit.ClearReplacePerform();
		}
		this.Destroy();
	}

	private void StopSkillPhase()
	{
		if (!this.unit.isPlayer || this.skillMainId == "Skill_ZS_04")
		{
		}
		if (this.m_SkillPhaseTask != null)
		{
			this.m_CoroutineManager.StopCoroutine(this.m_SkillPhaseTask);
		}
	}

	protected void Destroy()
	{
		if (!this.isStop)
		{
			this.isStop = true;
			if (this.m_CoroutineManager != null)
			{
				this.m_CoroutineManager.StopAllCoroutine();
			}
			if (this.unit != null && this.unit.isLive)
			{
				if (this.IsAttack)
				{
					this.unit.PlayAnim(AnimationType.ComboAttack, false, 0, true, false);
				}
				else
				{
					this.unit.PlayAnim(AnimationType.Conjure, false, 0, true, false);
				}
			}
			this.OnSkillStartCallback = null;
			this.OnSkillInterruptCallback = null;
			this.OnSkillEndCallback = null;
			this.OnSkillDamageCallback = null;
			this.attackTarget = null;
			this.attackPosition = null;
			this.attackTargets = null;
			this.externalPostion = null;
		}
	}

	public void ResetSkillState()
	{
		if (this.IsAttack)
		{
			this.unit.SetAttackCool(0f, true);
			if (this.IsInSkillCastBefore)
			{
				this.unit.SetAttackTimeLengh(0f, true);
			}
		}
		else
		{
			this.unit.SetActionCool(0f, true);
			this.unit.SetAICool(0f, true);
			this.unit.SetWaitCool(0f, true);
			this.unit.SetSkillCool(0f, true);
			this.unit.SetAttackCool(0f, true);
		}
	}

	public void SetSkillState()
	{
		if (this.IsAttack)
		{
			this.unit.SetAttackCool(0.01f, true);
			this.unit.SetAICool(0.01f, true);
			if (!this.data.isCanMoveInSkillCastin)
			{
				this.unit.SetWaitCool(0.01f, true);
				this.unit.StopMove();
			}
		}
		else if (this.IsInitiativeSkill)
		{
			this.unit.SetAICool(0.5f, false);
			if (!this.data.isCanMoveInSkillCastin)
			{
				this.unit.SetWaitCool(0.1f, false);
				this.unit.SetAttackCool(0.1f, false);
				this.unit.StopMove();
			}
		}
	}

	private void Damage(SkillDataKey skill_key, Units target, int targetsCount = 1)
	{
		if (target != null && target.isLive)
		{
			SkillData data = GameManager.Instance.SkillData.GetData(skill_key);
			target.dataChange.doSkillWoundAction(data.damage_ids, this.unit, true, new float[]
			{
				(float)targetsCount
			});
		}
	}

	public virtual void genericSet()
	{
		this.unit.dataChange.doSkillWoundAction(this.data.cost_ids, this.unit, false, new float[0]);
		this.StartCDTime(-1f, false);
	}

	public virtual bool IsCanTriggerBornPowerObj()
	{
		return false;
	}

	public virtual void OnBornPowerObj()
	{
	}

	public virtual void OnBornPowerObjTriggered()
	{
	}

	public virtual void OnSynced(byte inUseState)
	{
	}

	private bool IsCDConditionSatisfied()
	{
		return this.CD > 0f;
	}

	public bool IsHaveBornPowerObj()
	{
		return this.Data.config.skill_logic_type == 17;
	}

	public virtual void StartCDTime(float cd = -1f, bool isReset = false)
	{
		if (this.IsCDConditionSatisfied())
		{
			if (GlobalSettings.IsNoSkillCD)
			{
				return;
			}
			if (this.unit.isPlayer)
			{
				Singleton<SkillView>.Instance.StartSkillCD(this.skillMainId, true, cd, isReset);
			}
			else
			{
				this.unit.SetCDTime(this.skillMainId, (cd != -1f) ? cd : this.CD);
			}
		}
	}

	private void ReadyCastSkill(SkillDataKey skill_key)
	{
		ActionManager.ReadySkill(this.skillKey, this.unit, this.attackTargets, this.attackPosition, this, true);
	}

	public virtual void StartCastSkill(SkillDataKey skill_key)
	{
		this.AddHighEff(skill_key, SkillPhrase.Start, this.attackTargets, this.GetSkillPosition());
		this.AddBuff(skill_key, SkillPhrase.Start, this.attackTargets);
		StartSkillAction startSkillAction = ActionManager.StartSkill(this.skillKey, this.unit, this.attackTargets, this.attackPosition, true, null);
		if (startSkillAction != null)
		{
			startSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
			startSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnStartSkillActionEnd);
		}
	}

	protected virtual void AddHighEff(SkillDataKey skill_key, SkillPhrase skillPhrase, List<Units> targets = null, Vector3? skillPosition = null)
	{
		SkillUtility.AddHighEff(this.unit, this.skillKey, skillPhrase, targets, skillPosition);
	}

	protected virtual void AddBuff(SkillDataKey skill_key, SkillPhrase trigger, List<Units> targets = null)
	{
		SkillUtility.AddBuff(this.unit, this.skillKey, trigger, targets);
	}

	public virtual void OnSkillReadyPvp()
	{
	}

	public virtual void OnSkillStartPvp()
	{
	}

	public bool SetLevel(int level)
	{
		bool result = false;
		int num = SkillUtility.GetSkillLevelMax(this.skillID);
		if (num > 0 && level >= 0 && level <= num)
		{
			this.skillLevel = level;
			result = true;
		}
		this.OnSkillLevelUp();
		return result;
	}

	public virtual void AddAction(SkillCastPhase phase, BaseAction action)
	{
		if (action != null)
		{
			if (!this.mActions.ContainsKey((int)phase))
			{
				this.mActions.Add((int)phase, new List<BaseAction>());
			}
			this.mActions[(int)phase].Add(action);
		}
	}

	public virtual void RemAction(BaseAction action)
	{
		Dictionary<int, List<BaseAction>>.Enumerator enumerator = this.mActions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, List<BaseAction>> current = enumerator.Current;
			current.Value.Remove(action);
		}
	}

	public virtual BaseAction GetAction(SkillCastPhase inSkillCastPhase, int inActionId)
	{
		List<BaseAction> list = null;
		if (!this.mActions.TryGetValue((int)inSkillCastPhase, out list))
		{
			return null;
		}
		if (list == null)
		{
			return null;
		}
		if (list.Count <= 0)
		{
			return null;
		}
		BaseAction result = null;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				if (list[i].GetActionById(inActionId, out result))
				{
					break;
				}
			}
		}
		return result;
	}

	public virtual Units ReselectTarget(Units target, bool isCrazyMode = true)
	{
		return target;
	}

	public virtual bool NeedAutoLaunchToHero()
	{
		return true;
	}

	public virtual Units CustomTargetInCrazy()
	{
		return null;
	}

	public virtual bool NeedCustomTargetInCrazy()
	{
		return false;
	}

	public void DestroyActions(SkillCastPhase phase = SkillCastPhase.Cast_None)
	{
		if (phase == SkillCastPhase.Cast_None)
		{
			if (this.mActions.Count > 0)
			{
				List<int> list = this.mActions.Keys.ToList<int>();
				for (int l = 0; l < list.Count; l++)
				{
					Skill.<DestroyActions>c__AnonStorey207 <DestroyActions>c__AnonStorey = new Skill.<DestroyActions>c__AnonStorey207();
					List<BaseAction> list2 = this.mActions[list[l]];
					<DestroyActions>c__AnonStorey.clearList = new List<int>();
					for (int j = 0; j < list2.Count; j++)
					{
						<DestroyActions>c__AnonStorey.clearList.Add(list2[j].actionId);
					}
					int i;
					for (i = 0; i < <DestroyActions>c__AnonStorey.clearList.Count; i++)
					{
						list2.Find((BaseAction p) => p.actionId == <DestroyActions>c__AnonStorey.clearList[i]).Destroy();
					}
				}
				this.mActions.Clear();
			}
		}
		else if (this.mActions.ContainsKey((int)phase))
		{
			Skill.<DestroyActions>c__AnonStorey209 <DestroyActions>c__AnonStorey3 = new Skill.<DestroyActions>c__AnonStorey209();
			List<BaseAction> list3 = this.mActions[(int)phase];
			<DestroyActions>c__AnonStorey3.clearList = new List<int>();
			for (int k = 0; k < list3.Count; k++)
			{
				<DestroyActions>c__AnonStorey3.clearList.Add(list3[k].actionId);
			}
			int i;
			for (i = 0; i < <DestroyActions>c__AnonStorey3.clearList.Count; i++)
			{
				list3.Find((BaseAction p) => p.actionId == <DestroyActions>c__AnonStorey3.clearList[i]).Destroy();
			}
			this.mActions.Remove((int)phase);
		}
	}

	private void AssianHigheffAndBuff()
	{
		SkillData data = GameManager.Instance.SkillData.GetData(this.skillKey);
		if (data == null)
		{
			return;
		}
		if (this.IsPassive)
		{
			string[] highEffects = data.GetHighEffects(SkillPhrase.Start);
			string[] buffs = data.GetBuffs(SkillPhrase.Start);
			string[] highEffects2 = data.GetHighEffects(SkillPhrase.Init);
			string[] buffs2 = data.GetBuffs(SkillPhrase.Init);
			if (highEffects != null)
			{
				this._bighEffList = new List<string>(highEffects);
			}
			if (buffs != null)
			{
				this._buffList = new List<string>(buffs);
			}
			if (highEffects2 != null)
			{
				this._bighEffList.AddRange(new List<string>(highEffects2));
			}
			if (buffs2 != null)
			{
				this._buffList.AddRange(new List<string>(buffs2));
			}
		}
		if (this.IsInitiativeSkill)
		{
			string[] highEffects3 = data.GetHighEffects(SkillPhrase.Init);
			string[] buffs3 = data.GetBuffs(SkillPhrase.Init);
			if (highEffects3 != null)
			{
				this._bighEffList = new List<string>(highEffects3);
			}
			if (buffs3 != null)
			{
				this._buffList = new List<string>(buffs3);
			}
		}
	}

	public void RemoveHighEffAndBuff()
	{
		for (int i = 0; i < this._bighEffList.Count; i++)
		{
			this.self.highEffManager.RemoveHighEffect(this._bighEffList[i]);
		}
		for (int j = 0; j < this._buffList.Count; j++)
		{
			this.self.buffManager.RemoveBuff(this._buffList[j], -1);
		}
		if (this._passiveSimple != null)
		{
			this._passiveSimple.Destroy();
		}
	}

	public void RemoveHighEffectAndBuffBySkillPhase(SkillPhrase inPhase)
	{
		SkillData data = GameManager.Instance.SkillData.GetData(this.skillKey);
		if (data == null)
		{
			return;
		}
		string[] buffs = data.GetBuffs(inPhase);
		if (buffs != null && buffs.Length > 0)
		{
			for (int i = 0; i < buffs.Length; i++)
			{
				this.self.buffManager.RemoveBuff(buffs[i], -1);
			}
		}
		string[] highEffects = data.GetHighEffects(inPhase);
		if (highEffects != null && highEffects.Length > 0)
		{
			for (int j = 0; j < highEffects.Length; j++)
			{
				this.self.highEffManager.RemoveHighEffect(highEffects[j]);
			}
		}
	}

	public virtual void DoSkillLevelUp()
	{
		if (this.self.isHero && this.skillLevel < 0)
		{
			return;
		}
		if (this.skillLevel == 0 && this.skillLevelMax > 0)
		{
			return;
		}
		this.RemoveHighEffAndBuff();
		List<Units> list = new List<Units>
		{
			this.self
		};
		this.AddHighEff(this.skillKey, SkillPhrase.Init, list, null);
		this.AddBuff(this.skillKey, SkillPhrase.Init, list);
		if (this.IsPassive)
		{
			this.AddHighEff(this.skillKey, SkillPhrase.Start, list, null);
			this.AddBuff(this.skillKey, SkillPhrase.Start, list);
			this._passiveSimple = ActionManager.SimpleSkill(this.skillKey, this.self, list, null);
		}
		this.AssianHigheffAndBuff();
	}

	public void OnSkillLevelUp()
	{
		this.ParseExtraParam();
	}

	public float GetCastBefore()
	{
		return this.data.castBefore * this.unit.attack_timelenth / (this.data.castBefore + this.data.castIn + this.data.castEnd);
	}

	public virtual bool CanLevelUp()
	{
		return true;
	}

	public void StartEndTask()
	{
		this.endTask = this.m_CoroutineManager.StartCoroutine(this.PvpEndSkill(this.self), true);
	}

	public void InterruptEndTask()
	{
		if (this.endTask != null)
		{
			this.m_CoroutineManager.StopCoroutine(this.endTask);
			this.endTask = null;
		}
	}

	[DebuggerHidden]
	private IEnumerator PvpEndSkill(Units units)
	{
		Skill.<PvpEndSkill>c__Iterator97 <PvpEndSkill>c__Iterator = new Skill.<PvpEndSkill>c__Iterator97();
		<PvpEndSkill>c__Iterator.units = units;
		<PvpEndSkill>c__Iterator.<$>units = units;
		<PvpEndSkill>c__Iterator.<>f__this = this;
		return <PvpEndSkill>c__Iterator;
	}

	public virtual void OnPerFormeCreate(BasePerformAction obj)
	{
	}

	public virtual void OnHighEffStart(BaseHighEffAction highEff)
	{
	}
}
