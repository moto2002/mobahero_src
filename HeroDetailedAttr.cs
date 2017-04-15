using MobaHeros;
using System;

public class HeroDetailedAttr
{
	private DataManager _data;

	public float power;

	public float extraPower;

	public float agility;

	public float extraAgility;

	public float intelligence;

	public float extraIntelligence;

	public float restore;

	public float extraRestore;

	public float armor;

	public float extraArmor;

	public float physicalPower;

	public float extraPhysicalPower;

	public float attackRange;

	public float extraAttackRange;

	public float physicalCritProp;

	public float extraPhysicalCritProp;

	public float physicalCritMag;

	public float extraPhysicalCritMag;

	public float armorCut;

	public float extraArmorCut;

	public float armorCutPercentage;

	public float extraArmorCutPercentage;

	public float dodgeProp;

	public float extraDodgeProp;

	public float atk;

	public float extraAtk;

	public float atkSpd;

	public float extraAtkSpd;

	public float spd;

	public float extraSpd;

	public float mpRestore;

	public float extraMpRestore;

	public float magicResist;

	public float extraMagicResist;

	public float magicPower;

	public float extraMagicPower;

	public float magicCritProp;

	public float extraMagicCritProp;

	public float magicCritMag;

	public float extraMagicCritMag;

	public float magicResistCut;

	public float extraMagicResistCut;

	public float magicResistCutPercentage;

	public float extraMagicResistCutPercentage;

	public void parseFrom(DataManager data)
	{
		this._data = data;
		this.getValuePair(AttrType.Power, ref this.power, ref this.extraPower);
		this.getValuePair(AttrType.Agile, ref this.agility, ref this.extraAgility);
		this.getValuePair(AttrType.Intelligence, ref this.intelligence, ref this.extraIntelligence);
		this.getValuePair(AttrType.HpRestore, ref this.restore, ref this.extraRestore);
		this.getValuePair(AttrType.Armor, ref this.armor, ref this.extraArmor);
		this.getValuePair(AttrType.PhysicPower, ref this.physicalPower, ref this.extraPhysicalPower);
		this.getValuePair(AttrType.PhysicCritProp, ref this.physicalCritProp, ref this.extraPhysicalCritProp);
		this.getValuePair(AttrType.PhysicCritMag, ref this.physicalCritMag, ref this.extraPhysicalCritMag);
		this.getValuePair(AttrType.ArmorCut, ref this.armorCut, ref this.extraArmorCut);
		this.getValuePair(AttrType.ArmorCut_Percentage, ref this.armorCutPercentage, ref this.extraArmorCutPercentage);
		this.getValuePair(AttrType.DodgeProp, ref this.dodgeProp, ref this.extraDodgeProp);
		this.getValuePair(AttrType.Attack, ref this.atk, ref this.extraAtk);
		this.getValuePair(AttrType.AttackSpeed, ref this.atkSpd, ref this.extraAtkSpd);
		this.getValuePair(AttrType.MoveSpeed, ref this.spd, ref this.extraSpd);
		this.getValuePair(AttrType.MpRestore, ref this.mpRestore, ref this.extraMpRestore);
		this.getValuePair(AttrType.MagicResist, ref this.magicResist, ref this.extraMagicResist);
		this.getValuePair(AttrType.MagicPower, ref this.magicPower, ref this.extraMagicPower);
		this.getValuePair(AttrType.MagicCritProp, ref this.magicCritProp, ref this.extraMagicCritProp);
		this.getValuePair(AttrType.MagicCritMag, ref this.magicCritMag, ref this.extraMagicCritMag);
		this.getValuePair(AttrType.MagicResistanceCut, ref this.magicResistCut, ref this.extraMagicResistCut);
		this.getValuePair(AttrType.MagicResistanceCut_Percentage, ref this.magicResistCutPercentage, ref this.extraMagicResistCutPercentage);
		this.getValuePair(AttrType.AttackRange, ref this.attackRange, ref this.extraAttackRange);
	}

	private void getValuePair(AttrType attr, ref float origin, ref float extra)
	{
		origin = this._data.GetBasicAttrInBattle(attr);
		extra = this._data.GetAttr(attr) - origin;
	}
}
