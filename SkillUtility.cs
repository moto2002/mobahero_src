using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillUtility
{
	private static string sLevelPreFix = "_lv";

	private static string sSkinPreFix = "_skin";

	private static HashSet<string> summonerSkill = new HashSet<string>
	{
		"Summoner_Ghost",
		"Summoner_Heal",
		"Summoner_Clarity",
		"Summoner_Cleanse",
		"Summoner_Smite",
		"Summoner_Barrier",
		"Summoner_Ignite",
		"Summoner_Exhaust",
		"Summoner_Flash",
		"Skill_GoTown"
	};

	public static SysSkillMainVo GetSkillData(string skillID, int level = -1, int skin = -1)
	{
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillID);
		SysSkillLevelupVo skillLevelupData = SkillUtility.GetSkillLevelupData(skillID, level);
		SysSkillSkinVo skillSkinData = SkillUtility.GetSkillSkinData(skillID, skin);
		return SkillUtility.GetSkillData(skillMainData, skillLevelupData, skillSkinData);
	}

	public static SysSkillMainVo GetSkillData(SysSkillMainVo mainVo, SysSkillLevelupVo levelupVo, SysSkillSkinVo skinVo)
	{
		if (mainVo == null)
		{
			return null;
		}
		SysSkillMainVo result = SkillUtility.CopySkillData(ref mainVo);
		if (levelupVo != null)
		{
			SkillUtility.LevelUpImpact(ref result, ref levelupVo);
		}
		if (skinVo != null)
		{
			SkillUtility.SkinImpact(ref result, ref skinVo);
		}
		return result;
	}

	public static SysSkillLevelupVo GetSkillLevelupData(string skillID, int level)
	{
		if (level < 0)
		{
			return null;
		}
		string skillLvUpId = SkillUtility.getSkillLvUpId(skillID, level);
		return BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillLvUpId);
	}

	public static string getSkillLvUpId(string skillId, int level)
	{
		return skillId + SkillUtility.sLevelPreFix + SkillUtility.IntToString(level);
	}

	public static SysSkillSkinVo GetSkillSkinData(string skillID, int skin)
	{
		if (skin < 0)
		{
			return null;
		}
		string unikey = skillID + SkillUtility.sSkinPreFix + SkillUtility.IntToString(skin);
		return BaseDataMgr.instance.GetDataById<SysSkillSkinVo>(unikey);
	}

	public static bool IsSkillCanLevelUp(string skillID)
	{
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillID);
		return skillMainData != null && skillMainData.skill_levelmax > 0;
	}

	public static bool IsSkillPassive(string skillID)
	{
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillID);
		return skillMainData != null && skillMainData.skill_trigger == 3;
	}

	public static int GetSkillLevelMax(string skillID)
	{
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillID);
		if (skillMainData != null)
		{
			return skillMainData.skill_levelmax;
		}
		return 0;
	}

	private static void LevelUpImpact(ref SysSkillMainVo baseData, ref SysSkillLevelupVo levelupData)
	{
		if (!SkillUtility.CheckStringDefault(levelupData.skill_name))
		{
			baseData.skill_name = levelupData.skill_name;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_description))
		{
			baseData.skill_description = levelupData.skill_description;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_description2))
		{
			baseData.skill_description2 = levelupData.skill_description2;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_description3))
		{
			baseData.skill_description3 = levelupData.skill_description3;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_icon))
		{
			baseData.skill_icon = levelupData.skill_icon;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.skill_index))
		{
			baseData.skill_index = levelupData.skill_index;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.skill_type))
		{
			baseData.skill_type = levelupData.skill_type;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.skill_logic_type))
		{
			baseData.skill_logic_type = levelupData.skill_logic_type;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.skill_trigger))
		{
			baseData.skill_trigger = levelupData.skill_trigger;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.skill_prop))
		{
			baseData.skill_prop = levelupData.skill_prop;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.distance))
		{
			baseData.distance = levelupData.distance;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.target_type))
		{
			baseData.target_type = levelupData.target_type;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.effective_range))
		{
			baseData.effective_range = levelupData.effective_range;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.max_num))
		{
			baseData.max_num = levelupData.max_num;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.need_target))
		{
			baseData.need_target = levelupData.need_target;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.sing_time))
		{
			baseData.sing_time = levelupData.sing_time;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.guide_time))
		{
			baseData.guide_time = levelupData.guide_time;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.interrupt))
		{
			baseData.interrupt = levelupData.interrupt;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.cost))
		{
			baseData.cost = levelupData.cost;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.cost_upgrade))
		{
			baseData.cost_upgrade = levelupData.cost_upgrade;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_phase))
		{
			baseData.skill_phase = levelupData.skill_phase;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.skill_interval))
		{
			baseData.skill_interval = levelupData.skill_interval;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.cd))
		{
			baseData.cd = levelupData.cd;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.cd_upgrade))
		{
			baseData.cd_upgrade = levelupData.cd_upgrade;
		}
		if (!SkillUtility.CheckFloatDefault(levelupData.hard_cd))
		{
			baseData.hard_cd = levelupData.hard_cd;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.skill_mutex))
		{
			baseData.skill_mutex = levelupData.skill_mutex;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.ready_action_ids))
		{
			baseData.ready_action_ids = levelupData.ready_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.start_action_ids))
		{
			baseData.start_action_ids = levelupData.start_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.hit_action_ids))
		{
			baseData.hit_action_ids = levelupData.hit_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.end_action_ids))
		{
			baseData.end_action_ids = levelupData.end_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.init_higheff_ids))
		{
			baseData.init_higheff_ids = levelupData.init_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.start_higheff_ids))
		{
			baseData.start_higheff_ids = levelupData.start_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.hit_higheff_ids))
		{
			baseData.hit_higheff_ids = levelupData.hit_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.start_buff_ids))
		{
			baseData.start_buff_ids = levelupData.start_buff_ids;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.hit_buff_ids))
		{
			baseData.hit_buff_ids = levelupData.hit_buff_ids;
		}
		if (!SkillUtility.CheckIntDefault(levelupData.hit_trigger_type))
		{
			baseData.hit_trigger_type = levelupData.hit_trigger_type;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.hit_time))
		{
			baseData.hit_time = levelupData.hit_time;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.damage_id))
		{
			baseData.damage_id = levelupData.damage_id;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.ConjureRangetype))
		{
			baseData.ConjureRangetype = levelupData.ConjureRangetype;
		}
		if (!SkillUtility.CheckStringDefault(levelupData.SkillExtraParam))
		{
			baseData.SkillExtraParam = levelupData.SkillExtraParam;
		}
	}

	private static void SkinImpact(ref SysSkillMainVo baseData, ref SysSkillSkinVo skinData)
	{
		if (!SkillUtility.CheckStringDefault(skinData.skill_name))
		{
			baseData.skill_name = skinData.skill_name;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_description))
		{
			baseData.skill_description = skinData.skill_description;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_description2))
		{
			baseData.skill_description2 = skinData.skill_description2;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_description3))
		{
			baseData.skill_description3 = skinData.skill_description3;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_icon))
		{
			baseData.skill_icon = skinData.skill_icon;
		}
		if (!SkillUtility.CheckIntDefault(skinData.skill_index))
		{
			baseData.skill_index = skinData.skill_index;
		}
		if (!SkillUtility.CheckIntDefault(skinData.skill_type))
		{
			baseData.skill_type = skinData.skill_type;
		}
		if (!SkillUtility.CheckIntDefault(skinData.skill_logic_type))
		{
			baseData.skill_logic_type = skinData.skill_logic_type;
		}
		if (!SkillUtility.CheckIntDefault(skinData.skill_trigger))
		{
			baseData.skill_trigger = skinData.skill_trigger;
		}
		if (!SkillUtility.CheckIntDefault(skinData.skill_prop))
		{
			baseData.skill_prop = skinData.skill_prop;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.distance))
		{
			baseData.distance = skinData.distance;
		}
		if (!SkillUtility.CheckStringDefault(skinData.target_type))
		{
			baseData.target_type = skinData.target_type;
		}
		if (!SkillUtility.CheckStringDefault(skinData.effective_range))
		{
			baseData.effective_range = skinData.effective_range;
		}
		if (!SkillUtility.CheckIntDefault(skinData.max_num))
		{
			baseData.max_num = skinData.max_num;
		}
		if (!SkillUtility.CheckStringDefault(skinData.need_target))
		{
			baseData.need_target = skinData.need_target;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.sing_time))
		{
			baseData.sing_time = skinData.sing_time;
		}
		if (!SkillUtility.CheckStringDefault(skinData.guide_time))
		{
			baseData.guide_time = skinData.guide_time;
		}
		if (!SkillUtility.CheckIntDefault(skinData.interrupt))
		{
			baseData.interrupt = skinData.interrupt;
		}
		if (!SkillUtility.CheckStringDefault(skinData.cost))
		{
			baseData.cost = skinData.cost;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.cost_upgrade))
		{
			baseData.cost_upgrade = skinData.cost_upgrade;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_phase))
		{
			baseData.skill_phase = skinData.skill_phase;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.skill_interval))
		{
			baseData.skill_interval = skinData.skill_interval;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.cd))
		{
			baseData.cd = skinData.cd;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.cd_upgrade))
		{
			baseData.cd_upgrade = skinData.cd_upgrade;
		}
		if (!SkillUtility.CheckFloatDefault(skinData.hard_cd))
		{
			baseData.hard_cd = skinData.hard_cd;
		}
		if (!SkillUtility.CheckStringDefault(skinData.skill_mutex))
		{
			baseData.skill_mutex = skinData.skill_mutex;
		}
		if (!SkillUtility.CheckStringDefault(skinData.ready_action_ids))
		{
			baseData.ready_action_ids = skinData.ready_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.start_action_ids))
		{
			baseData.start_action_ids = skinData.start_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.hit_action_ids))
		{
			baseData.hit_action_ids = skinData.hit_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.end_action_ids))
		{
			baseData.end_action_ids = skinData.end_action_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.init_higheff_ids))
		{
			baseData.init_higheff_ids = skinData.init_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.start_higheff_ids))
		{
			baseData.start_higheff_ids = skinData.start_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.hit_higheff_ids))
		{
			baseData.hit_higheff_ids = skinData.hit_higheff_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.start_buff_ids))
		{
			baseData.start_buff_ids = skinData.start_buff_ids;
		}
		if (!SkillUtility.CheckStringDefault(skinData.hit_buff_ids))
		{
			baseData.hit_buff_ids = skinData.hit_buff_ids;
		}
		if (!SkillUtility.CheckIntDefault(skinData.hit_trigger_type))
		{
			baseData.hit_trigger_type = skinData.hit_trigger_type;
		}
		if (!SkillUtility.CheckStringDefault(skinData.hit_time))
		{
			baseData.hit_time = skinData.hit_time;
		}
		if (!SkillUtility.CheckStringDefault(skinData.damage_id))
		{
			baseData.damage_id = skinData.damage_id;
		}
	}

	private static SysSkillMainVo CopySkillData(ref SysSkillMainVo baseData)
	{
		return new SysSkillMainVo
		{
			unikey = baseData.unikey,
			skill_id = baseData.skill_id,
			skill_name = baseData.skill_name,
			skill_description = baseData.skill_description,
			skill_description2 = baseData.skill_description2,
			skill_description3 = baseData.skill_description3,
			skill_icon = baseData.skill_icon,
			skill_index = baseData.skill_index,
			skill_type = baseData.skill_type,
			skill_logic_type = baseData.skill_logic_type,
			skill_trigger = baseData.skill_trigger,
			skill_prop = baseData.skill_prop,
			distance = baseData.distance,
			target_type = baseData.target_type,
			effective_range = baseData.effective_range,
			max_num = baseData.max_num,
			need_target = baseData.need_target,
			sing_time = baseData.sing_time,
			guide_time = baseData.guide_time,
			interrupt = baseData.interrupt,
			cost = baseData.cost,
			cost_upgrade = baseData.cost_upgrade,
			skill_phase = baseData.skill_phase,
			skill_interval = baseData.skill_interval,
			cd = baseData.cd,
			cd_upgrade = baseData.cd_upgrade,
			hard_cd = baseData.hard_cd,
			skill_mutex = baseData.skill_mutex,
			ready_action_ids = baseData.ready_action_ids,
			start_action_ids = baseData.start_action_ids,
			hit_action_ids = baseData.hit_action_ids,
			end_action_ids = baseData.end_action_ids,
			init_higheff_ids = baseData.init_higheff_ids,
			start_higheff_ids = baseData.start_higheff_ids,
			hit_higheff_ids = baseData.hit_higheff_ids,
			start_buff_ids = baseData.start_buff_ids,
			hit_buff_ids = baseData.hit_buff_ids,
			hit_trigger_type = baseData.hit_trigger_type,
			hit_time = baseData.hit_time,
			damage_id = baseData.damage_id,
			skill_levelmax = baseData.skill_levelmax,
			skill_unique = baseData.skill_unique,
			is_autoattackseltarget_onskillend = baseData.is_autoattackseltarget_onskillend,
			ConjureRangetype = baseData.ConjureRangetype,
			SkillExtraParam = baseData.SkillExtraParam
		};
	}

	private static string IntToString(int index)
	{
		string result = string.Empty;
		if (index >= 100 || index < 0)
		{
			Debug.LogError("Convert integate value [" + index + "] is out of range[0:100]");
			return string.Empty;
		}
		if (index < 10)
		{
			result = '0' + index.ToString();
		}
		else
		{
			result = index.ToString();
		}
		return result;
	}

	private static bool CheckStringDefault(string str)
	{
		bool result = false;
		if (str == "[]")
		{
			result = true;
		}
		return result;
	}

	private static bool CheckIntDefault(int i)
	{
		bool result = false;
		if (i == -999)
		{
			result = true;
		}
		return result;
	}

	private static bool CheckFloatDefault(float f)
	{
		bool result = false;
		if (Mathf.Abs(f - -999f) < 0.0001f)
		{
			result = true;
		}
		return result;
	}

	public static bool IsSummonerSkill(Skill skill)
	{
		return skill != null && SkillUtility.summonerSkill.Contains(skill.skillMainId);
	}

	public static bool IsBackHomeSkill(Skill skill)
	{
		return skill != null && skill.skillIndex == 7;
	}

	public static bool IsSummonerCleanseSkill(Skill skill)
	{
		return skill != null && skill.skillMainId.Equals("Summoner_Cleanse");
	}

	public static bool IsBurnUnitSkill(Skill skill)
	{
		return skill != null && skill.skillMainId.Equals("Skill_Timo_04");
	}

	public static bool DoMoFaMianYi(Units target, DamageData data)
	{
		return target.ImmunityManager.IsImmunity(data.DataType);
	}

	public static bool DoWuDi(Units target, DamageData data)
	{
		return data.DataType.GainType == EffectGainType.negative && target.WuDi.IsInState;
	}

	public static bool DoWuDi(Units target, float damage)
	{
		return damage < 0f && target.WuDi.IsInState;
	}

	public static bool IsImmunityBuff(Units target, string buffId)
	{
		if (target == null)
		{
			return false;
		}
		BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(buffId);
		if (vo == null)
		{
			return false;
		}
		if (target.WuDi.IsInState && vo.DataType.GainType == EffectGainType.negative)
		{
			target.jumpFont(JumpFontType.WuDi, string.Empty, null, true);
			return true;
		}
		if (target.ImmunityManager.IsImmunity(vo.DataType))
		{
			target.jumpFont(JumpFontType.MoFaMianYi, string.Empty, null, true);
			return true;
		}
		return false;
	}

	public static bool IsImmunityHighEff(Units target, string highEffId)
	{
		if (target == null)
		{
			return false;
		}
		HighEffectData vo = Singleton<HighEffectDataManager>.Instance.GetVo(highEffId);
		if (vo == null)
		{
			return false;
		}
		if (target.WuDi.IsInState && vo.DataType.GainType == EffectGainType.negative)
		{
			target.jumpFont(JumpFontType.WuDi, string.Empty, null, true);
			return true;
		}
		if (target.ImmunityManager.IsImmunity(vo.DataType))
		{
			target.jumpFont(JumpFontType.MoFaMianYi, string.Empty, null, true);
			return true;
		}
		return false;
	}

	public static void AddHighEff(Units selfUnit, SkillDataKey skill_key, SkillPhrase skillPhrase, List<Units> targets = null, Vector3? skillPosition = null)
	{
		if (!StringUtils.CheckValid(skill_key.SkillID))
		{
			return;
		}
		SkillData data = GameManager.Instance.SkillData.GetData(skill_key);
		if (data == null)
		{
			return;
		}
		string[] highEffects = data.GetHighEffects(skillPhrase);
		if (highEffects != null)
		{
			for (int i = 0; i < highEffects.Length; i++)
			{
				string text = highEffects[i];
				switch (skillPhrase)
				{
				case SkillPhrase.Start:
				case SkillPhrase.Init:
					if (!SkillUtility.IsImmunityHighEff(selfUnit, text))
					{
						ActionManager.AddHighEffect(text, skill_key.SkillID, selfUnit, selfUnit, skillPosition, true);
					}
					break;
				case SkillPhrase.Hit:
					for (int j = 0; j < targets.Count; j++)
					{
						if (targets[j] != null && targets[j].isLive && !SkillUtility.IsImmunityHighEff(targets[j], text))
						{
							ActionManager.AddHighEffect(text, skill_key.SkillID, targets[j], selfUnit, skillPosition, true);
						}
					}
					break;
				}
			}
		}
	}

	public static void AddBuff(Units selfUnit, SkillDataKey skill_key, SkillPhrase trigger, List<Units> targets = null)
	{
		if (!StringUtils.CheckValid(skill_key.SkillID))
		{
			return;
		}
		SkillData data = GameManager.Instance.SkillData.GetData(skill_key);
		if (data == null)
		{
			return;
		}
		string[] buffs = data.GetBuffs(trigger);
		if (buffs != null)
		{
			for (int i = 0; i < buffs.Length; i++)
			{
				string text = buffs[i];
				switch (trigger)
				{
				case SkillPhrase.Start:
				case SkillPhrase.Init:
					if (!SkillUtility.IsImmunityBuff(selfUnit, text))
					{
						ActionManager.AddBuff(text, selfUnit, selfUnit, true, string.Empty);
					}
					break;
				case SkillPhrase.Hit:
					for (int j = 0; j < targets.Count; j++)
					{
						if (targets[j] != null && targets[j].isLive && !SkillUtility.IsImmunityBuff(targets[j], text))
						{
							ActionManager.AddBuff(text, targets[j], selfUnit, true, string.Empty);
						}
					}
					break;
				}
			}
		}
	}
}
