using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections.Generic;

public class MonsterDataManager : DataManager
{
	public override void OnInit()
	{
		this.SetConfigData();
		this.CalculateFinalData();
		this.InitDynamicData();
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public override void OnWound(Units attacker, float damage)
	{
	}

	private void SetConfigData()
	{
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(this.self.npc_id);
		base.SetData(DataType.NameId, monsterMainData.npc_id);
		base.SetData(DataType.DisplayName, monsterMainData.name);
		base.SetData(DataType.ModelId, monsterMainData.model_id);
		base.SetData(DataType.MusicId, monsterMainData.music_id);
		base.SetData(DataType.ItemType, monsterMainData.item_type);
		base.SetData(DataType.CharacterType, monsterMainData.character_type);
		base.SetData(DataType.AttackType, monsterMainData.atk_type);
		base.SetData(DataType.Skills, StringUtils.GetStringValue(monsterMainData.skill_id, ','));
		base.SetData(DataType.Attacks, StringUtils.GetStringValue(monsterMainData.attack_id, ','));
		base.SetData(DataType.EffectId, monsterMainData.effect_id);
		base.SetAttr(AttrType.Attack, monsterMainData.attack);
		base.SetAttr(AttrType.Armor, monsterMainData.armor);
		base.SetAttr(AttrType.MagicResist, monsterMainData.resistance);
		base.SetAttr(AttrType.HpMax, monsterMainData.hp);
		base.SetAttr(AttrType.MpMax, monsterMainData.mp);
		base.SetAttr(AttrType.HpRestore, monsterMainData.hp_restore);
		base.SetAttr(AttrType.MpRestore, monsterMainData.mp_restore);
		base.SetAttr(AttrType.PhysicPower, monsterMainData.pysic_power);
		base.SetAttr(AttrType.MagicPower, monsterMainData.magic_power);
		base.SetAttr(AttrType.HitProp, monsterMainData.hit_prop);
		base.SetAttr(AttrType.DodgeProp, monsterMainData.dodge_prop);
		base.SetAttr(AttrType.PhysicCritProp, monsterMainData.physic_crit_prop);
		base.SetAttr(AttrType.MagicCritProp, monsterMainData.magic_crit_prop);
		base.SetAttr(AttrType.AttackSpeed, monsterMainData.atk_speed);
		base.SetAttr(AttrType.MoveSpeed, monsterMainData.move_speed);
		base.SetAttr(AttrType.PhysicCritMag, monsterMainData.crit_mag);
		base.SetAttr(AttrType.MagicCritMag, monsterMainData.magic_crit_mag);
		base.SetAttr(AttrType.ArmorCut, monsterMainData.armor_cut);
		base.SetAttr(AttrType.ArmorCut_Percentage, 0f);
		base.SetAttr(AttrType.MagicResistanceCut, monsterMainData.resistance_cut);
		base.SetAttr(AttrType.MagicResistanceCut_Percentage, 0f);
		base.SetAttr(AttrType.AttackRange, monsterMainData.atk_range);
		base.SetAttr(AttrType.WarningRange, monsterMainData.warning_range);
		base.SetAttr(AttrType.Shield, 0f);
		base.SetAttr(AttrType.FogRange, monsterMainData.player_range);
		base.SetAttr(AttrType.RealSightRange, monsterMainData.eye_range);
		base.SetAttr(AttrType.PhysicDamageAdd, 0f);
		base.SetAttr(AttrType.PhysicDamagePercent, 0f);
		base.SetAttr(AttrType.MagicDamageAdd, 0f);
		base.SetAttr(AttrType.MagicDamagePercent, 0f);
		base.SetAttr(AttrType.AttackDamageAdd, 0f);
		base.SetAttr(AttrType.AttackDamagePercent, 0f);
		base.SetAttr(AttrType.PhysicDamageCut, 0f);
		base.SetAttr(AttrType.PhysicDamagePercentCut, 0f);
		base.SetAttr(AttrType.MagicDamageCut, 0f);
		base.SetAttr(AttrType.MagicDamagePercentCut, 0f);
		base.SetAttr(AttrType.AttackDamageCut, 0f);
		base.SetAttr(AttrType.AttackDamagePercentCut, 0f);
		base.SetAttr(AttrType.MoFaHuDunCoverProportion, 0f);
		base.SetAttr(AttrType.NormalSkillCooling, 0f);
		base.SetAttr(AttrType.SummonSkillCooling, 0f);
		base.SetAttr(AttrType.ItemSkillCooling, 0f);
		base.SetAttr(AttrType.DamageMultiple, 1f);
		this.self.sourceAnimSpeed = monsterMainData.atk_speed;
		this.self.sourceMoveSpeed = monsterMainData.move_speed;
	}

	private void CalculateFinalData()
	{
		float attr_factor = 1f;
		Dictionary<AttrType, float> monsterProperty = this.GetMonsterProperty(this.self.npc_id, attr_factor);
		foreach (KeyValuePair<AttrType, float> current in monsterProperty)
		{
			base.SetAttr(current.Key, current.Value);
		}
	}

	public Dictionary<AttrType, float> GetMonsterProperty(string npc_id, float attr_factor = 1f)
	{
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(npc_id);
		return new Dictionary<AttrType, float>
		{
			{
				AttrType.Hp,
				monsterMainData.hp * attr_factor
			},
			{
				AttrType.HpMax,
				monsterMainData.hp * attr_factor
			},
			{
				AttrType.Mp,
				monsterMainData.mp * attr_factor
			},
			{
				AttrType.MpMax,
				monsterMainData.mp * attr_factor
			},
			{
				AttrType.Armor,
				monsterMainData.armor * attr_factor
			}
		};
	}

	private void InitDynamicData()
	{
	}
}
