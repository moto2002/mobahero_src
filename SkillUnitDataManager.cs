using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using System;

public class SkillUnitDataManager : DataManager
{
	public SkillUnitData data;

	public SkillUnitDataManager()
	{
	}

	public SkillUnitDataManager(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		this.SetConfigData();
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
	}

	private void SetConfigData()
	{
		this.data = Singleton<SkillUnitDataMgr>.Instance.GetVo(this.self.npc_id);
		base.SetData(DataType.NameId, this.data.config.unit_id);
		base.SetData(DataType.DisplayName, this.data.config.unit_name);
		base.SetData(DataType.ModelId, this.data.config.model_id);
		base.SetData(DataType.CharacterType, this.data.config.character_type);
		base.SetData(DataType.AttackType, this.data.config.atk_type);
		base.SetData(DataType.Skills, StringUtils.GetStringValue(this.data.config.skill_id, ','));
		base.SetData(DataType.Attacks, StringUtils.GetStringValue(this.data.config.attack_id, ','));
		base.SetAttr(AttrType.AttackSpeed, this.GetData(this.data.config.atk_speed));
		base.SetAttr(AttrType.MoveSpeed, this.GetData(this.data.config.move_speed));
		base.SetAttr(AttrType.AttackRange, this.GetData(this.data.config.atk_range));
		base.SetAttr(AttrType.WarningRange, this.GetData(this.data.config.warning_range));
		base.SetAttr(AttrType.Attack, this.GetData(this.data.config.attack));
		base.SetAttr(AttrType.Armor, this.GetData(this.data.config.armor));
		base.SetAttr(AttrType.MagicResist, this.GetData(this.data.config.resistance));
		base.SetAttr(AttrType.HpMax, this.GetData(this.data.config.hp));
		base.SetAttr(AttrType.MpMax, this.GetData(this.data.config.mp));
		base.SetAttr(AttrType.HpRestore, this.GetData(this.data.config.hp_restore));
		base.SetAttr(AttrType.MpRestore, this.GetData(this.data.config.mp_restore));
		base.SetAttr(AttrType.PhysicPower, this.GetData(this.data.config.pysic_power));
		base.SetAttr(AttrType.MagicPower, this.GetData(this.data.config.magic_power));
		base.SetAttr(AttrType.HitProp, this.GetData(this.data.config.hit_prop));
		base.SetAttr(AttrType.DodgeProp, this.GetData(this.data.config.dodge_prop));
		base.SetAttr(AttrType.PhysicCritProp, this.GetData(this.data.config.physic_crit_prop));
		base.SetAttr(AttrType.MagicCritProp, this.GetData(this.data.config.magic_crit_prop));
		base.SetAttr(AttrType.PhysicCritMag, this.GetData(this.data.config.crit_mag));
		base.SetAttr(AttrType.MagicCritMag, this.GetData(this.data.config.magic_crit_mag));
		base.SetAttr(AttrType.ArmorCut, this.GetData(this.data.config.armor_cut));
		base.SetAttr(AttrType.MagicResistanceCut, this.GetData(this.data.config.resistance_cut));
	}

	private float GetData(string data)
	{
		string[] stringValue = StringUtils.GetStringValue(data, '|');
		if (stringValue != null)
		{
			int num = int.Parse(stringValue[0]);
			int num2 = num;
			if (num2 == 1)
			{
				return (stringValue.Length <= 1) ? 0f : float.Parse(stringValue[1]);
			}
			if (num2 == 2)
			{
				int formula_id = (stringValue.Length <= 1) ? 0 : int.Parse(stringValue[1]);
				return FormulaTool.GetFormualValue(formula_id, this.self.ParentUnit);
			}
		}
		return 0f;
	}
}
