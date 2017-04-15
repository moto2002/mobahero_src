using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class SkillData
	{
		public string skillId;

		public SysSkillMainVo config;

		public SkillLogicType logicType;

		public bool needTarget;

		public int skillCastingType;

		public int skill_prop;

		public string[] skillMutexs;

		public bool isGuide;

		public float guideTime;

		public float guideInterval;

		public bool isShowGuideBar = true;

		public bool interrupt;

		public int[] cost_ids;

		public SkillPointerType SkillPointerType = SkillPointerType.CirclePointer;

		public float PointerRadius;

		public float SectorPointerAngle;

		public float PointerLength;

		public float PointerWidth;

		public bool IsUseSkillPointer;

		public SkillTargetCamp targetCamp;

		public TargetTag targetTag;

		public EffectiveRangeType effectiveRangeType;

		public float effectRange1;

		public float effectRange2;

		public EffectiveRangeType selectRangeType;

		public float selectRange1;

		public float selectRange2;

		public bool isAbsorbMp;

		public bool isCanMoveInSkillCastin;

		public bool isMoveSkill;

		public bool continueMoveAfterSkillEnd;

		public bool isDiJianWoZeng;

		public float damageProbability;

		public float[] hitTimes;

		public string[] ready_actions;

		public string[] start_actions;

		public string[] hit_actions;

		public string[] crit_start_actions;

		public string[] crit_hit_actions;

		public string[] friend_hit_actions;

		public string[] end_actions;

		public int[] damage_ids;

		public string[] init_higheff_ids;

		public string[] start_higheff_ids;

		public string[] hit_higheff_ids;

		public string[] start_buff_ids;

		public string[] hit_buff_ids;

		public float castBefore;

		public float castIn;

		public float castEnd;

		public SkillInterruptType interruptBefore;

		public SkillInterruptType interruptIn;

		public SkillInterruptType interruptEnd;

		public int m_nSkillUnique;

		public int m_nPriority;

		public float FullAnimTime
		{
			get
			{
				return this.castBefore + this.castIn + this.castEnd;
			}
		}

		public bool CanBeInterruptInCastManual
		{
			get
			{
				return this.config.interrupt == 1 || this.config.interrupt == 3;
			}
		}

		public SkillData(string skill_id)
		{
			this.skillId = skill_id;
			this.config = SkillUtility.GetSkillData(this.skillId, -1, -1);
			if (this.config == null)
			{
				return;
			}
			this.Parse(this.config);
		}

		public SkillData(string skill_id, int level, int skin)
		{
			SysSkillMainVo skillData = SkillUtility.GetSkillData(skill_id, level, skin);
			this.skillId = skill_id;
			this.config = skillData;
			if (this.config == null)
			{
				return;
			}
			this.Parse(this.config);
		}

		public void Parse(SysSkillMainVo skillAttr)
		{
			if (StringUtils.CheckValid(skillAttr.ConjureRangetype))
			{
				this.IsUseSkillPointer = true;
				string[] array = skillAttr.ConjureRangetype.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					string text = array[0];
					switch (text)
					{
					case "1":
						if (array.Length == 3)
						{
							this.SkillPointerType = SkillPointerType.RectanglePointer;
							this.PointerLength = float.Parse(array[1]);
							this.PointerWidth = float.Parse(array[2]);
						}
						break;
					case "2":
						if (array.Length == 2)
						{
							this.SkillPointerType = SkillPointerType.CirclePointer;
							this.PointerRadius = float.Parse(array[1]);
						}
						break;
					case "3":
						if (array.Length == 3)
						{
							this.SkillPointerType = SkillPointerType.SectorPointer;
							this.PointerRadius = float.Parse(array[1]);
							this.SectorPointerAngle = float.Parse(array[2]);
						}
						break;
					}
				}
			}
			this.logicType = (SkillLogicType)skillAttr.skill_logic_type;
			string str = Convert.ToString(skillAttr.need_target);
			int[] stringToInt = StringUtils.GetStringToInt(str, '|');
			if (stringToInt != null && stringToInt.Length > 0)
			{
				this.needTarget = (stringToInt[0] != 0);
			}
			if (stringToInt != null && stringToInt.Length > 1)
			{
				this.skillCastingType = stringToInt[1];
			}
			this.skill_prop = skillAttr.skill_prop;
			if (StringUtils.CheckValid(skillAttr.skill_mutex))
			{
				this.skillMutexs = StringUtils.GetStringValue(skillAttr.skill_mutex, ',');
			}
			if (StringUtils.CheckValid(skillAttr.guide_time))
			{
				string[] stringValue = StringUtils.GetStringValue(skillAttr.guide_time, '|');
				this.isGuide = (stringValue != null && float.Parse(stringValue[0]) > 0f);
				this.guideTime = ((!this.isGuide) ? 0f : float.Parse(stringValue[1]));
				this.guideInterval = ((!this.isGuide) ? 0f : float.Parse(stringValue[2]));
				if (stringValue.Length > 3 && this.isGuide)
				{
					this.isShowGuideBar = (int.Parse(stringValue[3]) != 0);
				}
				this.interrupt = (skillAttr.interrupt != 0);
			}
			if (StringUtils.CheckValid(skillAttr.cost))
			{
				this.cost_ids = StringUtils.GetStringToInt(skillAttr.cost, ',');
			}
			if (StringUtils.CheckValid(skillAttr.target_type))
			{
				int[] stringToInt2 = StringUtils.GetStringToInt(skillAttr.target_type, '|');
				this.targetCamp = (SkillTargetCamp)((stringToInt2 == null) ? 0 : stringToInt2[0]);
				this.targetTag = (TargetTag)((stringToInt2.Length <= 1) ? 0 : stringToInt2[1]);
			}
			if (StringUtils.CheckValid(skillAttr.effective_range))
			{
				string[] stringValue2 = StringUtils.GetStringValue(skillAttr.effective_range, ',');
				if (stringValue2.Length > 0)
				{
					float[] stringToFloat = StringUtils.GetStringToFloat(stringValue2[0], '|');
					this.effectiveRangeType = (this.selectRangeType = ((stringToFloat == null) ? EffectiveRangeType.None : ((EffectiveRangeType)stringToFloat[0])));
					this.effectRange1 = (this.selectRange1 = ((stringToFloat.Length <= 1) ? 0f : stringToFloat[1]));
					this.effectRange2 = (this.selectRange2 = ((stringToFloat.Length <= 2) ? 0f : stringToFloat[2]));
				}
				if (stringValue2.Length > 1)
				{
					float[] stringToFloat2 = StringUtils.GetStringToFloat(stringValue2[1], '|');
					this.selectRangeType = ((stringToFloat2 == null) ? EffectiveRangeType.None : ((EffectiveRangeType)stringToFloat2[0]));
					this.selectRange1 = ((stringToFloat2.Length <= 1) ? 0f : stringToFloat2[1]);
					this.selectRange2 = ((stringToFloat2.Length <= 2) ? 0f : stringToFloat2[2]);
				}
			}
			this.isAbsorbMp = false;
			this.isDiJianWoZeng = false;
			this.damageProbability = 100f;
			this.isCanMoveInSkillCastin = false;
			this.isMoveSkill = false;
			if (StringUtils.CheckValid(skillAttr.skill_mutex))
			{
				string[] stringValue3 = StringUtils.GetStringValue(skillAttr.skill_mutex, ',');
				for (int i = 0; i < stringValue3.Length; i++)
				{
					string[] stringValue4 = StringUtils.GetStringValue(stringValue3[i], '|');
					if (stringValue4[0] == "1")
					{
						this.damageProbability = (float)int.Parse(stringValue4[1]);
					}
					else if (stringValue4[0] == "2")
					{
						if (stringValue4[1] == "1")
						{
							this.isAbsorbMp = true;
						}
						else if (stringValue4[1] == "0")
						{
							this.isAbsorbMp = false;
						}
					}
					else if (stringValue4[0] == "3")
					{
						if (stringValue4[1] == "1")
						{
							this.isDiJianWoZeng = true;
						}
						else if (stringValue4[1] == "0")
						{
							this.isDiJianWoZeng = false;
						}
					}
					else if (stringValue4[0] == "4")
					{
						if (stringValue4[1] == "1")
						{
							this.isCanMoveInSkillCastin = true;
						}
						else if (stringValue4[1] == "0")
						{
							this.isCanMoveInSkillCastin = false;
						}
					}
					else if (stringValue4[0] == "6")
					{
						if (stringValue4[1] == "1")
						{
							this.continueMoveAfterSkillEnd = true;
						}
						else if (stringValue4[1] == "0")
						{
							this.continueMoveAfterSkillEnd = false;
						}
					}
					else if (stringValue4[0] == "7")
					{
						if (stringValue4[1] == "1")
						{
							this.isMoveSkill = true;
						}
						else if (stringValue4[1] == "0")
						{
							this.isMoveSkill = false;
						}
					}
				}
			}
			if (StringUtils.CheckValid(skillAttr.ready_action_ids))
			{
				this.ready_actions = StringUtils.GetStringValue(skillAttr.ready_action_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.start_action_ids))
			{
				string[] stringValue5 = StringUtils.GetStringValue(skillAttr.start_action_ids, ',');
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				for (int j = 0; j < stringValue5.Length; j++)
				{
					string[] stringValue6 = StringUtils.GetStringValue(stringValue5[j], '|');
					if (stringValue6.Length == 2)
					{
						if (stringValue6[0] == "1")
						{
							list.Add(stringValue6[1]);
						}
						else if (stringValue6[0] == "2")
						{
							list2.Add(stringValue6[1]);
						}
					}
					else if (stringValue6.Length == 1)
					{
						list.Add(stringValue6[0]);
					}
				}
				this.start_actions = list.ToArray();
				this.crit_start_actions = list2.ToArray();
			}
			if (StringUtils.CheckValid(skillAttr.hit_action_ids))
			{
				string[] stringValue7 = StringUtils.GetStringValue(skillAttr.hit_action_ids, ',');
				List<string> list3 = new List<string>();
				List<string> list4 = new List<string>();
				List<string> list5 = new List<string>();
				for (int k = 0; k < stringValue7.Length; k++)
				{
					string[] stringValue8 = StringUtils.GetStringValue(stringValue7[k], '|');
					if (stringValue8.Length == 2)
					{
						if (stringValue8[0] == "1")
						{
							list3.Add(stringValue8[1]);
						}
						else if (stringValue8[0] == "2")
						{
							list4.Add(stringValue8[1]);
						}
						else if (stringValue8[0] == "3")
						{
							list5.Add(stringValue8[1]);
						}
					}
					else if (stringValue8.Length == 1)
					{
						list3.Add(stringValue8[0]);
					}
				}
				this.hit_actions = list3.ToArray();
				this.crit_hit_actions = list4.ToArray();
				this.friend_hit_actions = list5.ToArray();
			}
			if (StringUtils.CheckValid(skillAttr.end_action_ids))
			{
				this.end_actions = StringUtils.GetStringValue(skillAttr.end_action_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.init_higheff_ids))
			{
				this.init_higheff_ids = StringUtils.GetStringValue(skillAttr.init_higheff_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.start_higheff_ids))
			{
				this.start_higheff_ids = StringUtils.GetStringValue(skillAttr.start_higheff_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.hit_higheff_ids))
			{
				this.hit_higheff_ids = StringUtils.GetStringValue(skillAttr.hit_higheff_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.start_buff_ids))
			{
				this.start_buff_ids = StringUtils.GetStringValue(skillAttr.start_buff_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.hit_buff_ids))
			{
				this.hit_buff_ids = StringUtils.GetStringValue(skillAttr.hit_buff_ids, ',');
			}
			if (StringUtils.CheckValid(skillAttr.skill_phase))
			{
				string[] stringValue9 = StringUtils.GetStringValue(skillAttr.skill_phase, ',');
				if (stringValue9 != null && stringValue9.Length == 3)
				{
					this.castBefore = float.Parse(stringValue9[0]);
					this.castIn = float.Parse(stringValue9[1]);
					this.castEnd = float.Parse(stringValue9[2]);
				}
			}
			if (this.config.skill_type == 1)
			{
				this.interruptBefore = SkillInterruptType.Force;
			}
			else if (this.config.skill_type == 2)
			{
				this.interruptBefore = SkillInterruptType.Passive;
			}
			this.interruptIn = (SkillInterruptType)this.config.interrupt;
			this.interruptEnd = SkillInterruptType.Force;
			if (StringUtils.CheckValid(skillAttr.hit_time))
			{
				this.hitTimes = StringUtils.GetStringToFloat(skillAttr.hit_time, ',');
			}
			if (StringUtils.CheckValid(skillAttr.damage_id))
			{
				this.damage_ids = StringUtils.GetStringToInt(skillAttr.damage_id, ',');
			}
			if (StringUtils.CheckValid(skillAttr.skill_unique))
			{
				int[] stringToInt3 = StringUtils.GetStringToInt(skillAttr.skill_unique, '|');
				if (stringToInt3 != null && stringToInt3.Length == 2)
				{
					this.m_nSkillUnique = stringToInt3[0];
					this.m_nPriority = stringToInt3[1];
				}
			}
		}

		public string[] GetHighEffects(SkillPhrase phrase)
		{
			switch (phrase)
			{
			case SkillPhrase.Start:
				return this.start_higheff_ids;
			case SkillPhrase.Hit:
				return this.hit_higheff_ids;
			case SkillPhrase.Init:
				return this.init_higheff_ids;
			}
			return null;
		}

		public string[] GetBuffs(SkillPhrase phrase)
		{
			switch (phrase)
			{
			case SkillPhrase.Start:
				return this.start_buff_ids;
			case SkillPhrase.Hit:
				return this.hit_buff_ids;
			}
			return null;
		}
	}
}
