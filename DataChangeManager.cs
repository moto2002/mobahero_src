using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataChangeManager : StaticUnitComponent
{
	private Dictionary<int, float> data_change_list = new Dictionary<int, float>();

	private Dictionary<int, float> damage_values = new Dictionary<int, float>();

	private Dictionary<short, float> attr_damages = new Dictionary<short, float>();

	private Dictionary<short, float> attr_rebounds = new Dictionary<short, float>();

	private List<int> insert_damage_list = new List<int>();

	private Units casterUnit;

	private System.Random ran = new System.Random();

	private System.Random ran2 = new System.Random();

	public override void OnInit()
	{
		this.ClearDataBag();
	}

	public override void OnStart()
	{
	}

	public override void OnStop()
	{
		this.ClearDataBag();
	}

	public override void OnDeath(Units attacker)
	{
	}

	public override void OnExit()
	{
	}

	public void InsertDataBag(int damage_id)
	{
		if (!this.insert_damage_list.Contains(damage_id))
		{
			this.insert_damage_list.Add(damage_id);
		}
	}

	public int[] GetInsertDataBag()
	{
		return this.insert_damage_list.ToArray();
	}

	public void ClearInsertDataBag()
	{
		this.insert_damage_list.Clear();
	}

	private void AddDataBag(int damage_id, float value)
	{
		if (this.data_change_list.ContainsKey(damage_id))
		{
			this.data_change_list[damage_id] = value;
		}
	}

	private void InitDataBag(int[] damage_ids)
	{
		if (damage_ids == null)
		{
			return;
		}
		for (int i = 0; i < damage_ids.Length; i++)
		{
			int key = damage_ids[i];
			if (!this.data_change_list.ContainsKey(key))
			{
				this.data_change_list.Add(key, 0f);
			}
		}
	}

	private float GetDataBagValue(int damage_id)
	{
		if (this.data_change_list.ContainsKey(damage_id))
		{
			return this.data_change_list[damage_id];
		}
		return 0f;
	}

	private void RemoveDataBag(int data_id)
	{
		if (this.data_change_list.ContainsKey(data_id))
		{
			this.data_change_list.Remove(data_id);
		}
	}

	private void ClearDataBag()
	{
		this.data_change_list.Clear();
		this.ClearDataValues();
	}

	private void ClearDataValues()
	{
		this.damage_values.Clear();
		this.attr_damages.Clear();
		this.attr_rebounds.Clear();
	}

	protected void AddDamageHP(DamageValueType damageType, float value)
	{
		if (value != 0f)
		{
			if (!this.damage_values.ContainsKey((int)damageType))
			{
				this.damage_values.Add((int)damageType, value);
			}
			else
			{
				this.damage_values[(int)damageType] = value;
			}
		}
	}

	protected void AddDamage(AttrType attrType, float value)
	{
		if (value != 0f)
		{
			if (this.attr_damages.ContainsKey((short)attrType))
			{
				this.attr_damages[(short)attrType] = value;
			}
			else
			{
				this.attr_damages.Add((short)attrType, value);
			}
		}
	}

	protected void AddRebound(AttrType attrType, float value)
	{
		if (value != 0f)
		{
			if (this.attr_rebounds.ContainsKey((short)attrType))
			{
				this.attr_rebounds[(short)attrType] = value;
			}
			else
			{
				this.attr_rebounds.Add((short)attrType, value);
			}
		}
	}

	protected bool CheckAttrEmpty(Dictionary<int, float> attrs)
	{
		return attrs == null || attrs.Count <= 0;
	}

	public void doBuffWoundAction(string buffId, Units casterUnit, bool isReverse = false)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		ActionManager.BuffChange(this.self, casterUnit, buffId, isReverse, true);
	}

	public Dictionary<short, float> doSkillWoundAction(int[] damage_ids, Units casterUnit, bool isJumpFont = true, params float[] extraParams)
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return this.attr_damages;
		}
		string arg_32_0 = (!(casterUnit != null)) ? string.Empty : casterUnit.name;
		this.casterUnit = casterUnit;
		this.ClearDataBag();
		this.InitDataBag(damage_ids);
		if (casterUnit != null)
		{
			this.InitDataBag(casterUnit.dataChange.GetInsertDataBag());
		}
		float final_damage = 0f;
		int num = 0;
		bool flag;
		bool flag2;
		int damageType;
		this.calculateDamage(this.self, casterUnit, damage_ids, out flag, out flag2, isJumpFont, out damageType, out num, extraParams);
		this.calculateDamageCut(this.self, damage_ids, flag, flag2, out final_damage);
		this.calculateDamageRebound(casterUnit, ref final_damage);
		this.calculateXiXue(casterUnit, final_damage);
		ActionManager.Wound(this.self, casterUnit, this.attr_damages.Keys.ToList<short>(), this.attr_damages.Values.ToList<float>(), flag, true, damageType);
		ActionManager.Wound(casterUnit, this.self, this.attr_rebounds.Keys.ToList<short>(), this.attr_rebounds.Values.ToList<float>(), false, true, damageType);
		if (flag && !flag2)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitCrit, casterUnit, null, null);
		}
		if (num == 1 && !flag2 && this.self && this.self.IsWildMonster)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitHitProp, casterUnit, null, null);
		}
		this.self.attackExtraDamage = 0f;
		this.self.attackMultipleDamage = 0f;
		this.self.beheadedCoefficient = 0f;
		this.self.attackReboundCoefficient = 0f;
		if (casterUnit != null)
		{
			casterUnit.dataChange.ClearInsertDataBag();
		}
		return this.attr_damages;
	}

	private void calculateDamage(Units target, Units casterUnit, int[] damage_ids, out bool crit, out bool miss, bool isJumpFont, out int damageType, out int damageCalType, params float[] extraParams)
	{
		crit = false;
		miss = false;
		damageType = 0;
		damageCalType = 0;
		if (target == null)
		{
			return;
		}
		if (damage_ids == null)
		{
			return;
		}
		for (int i = 0; i < damage_ids.Length; i++)
		{
			int id = damage_ids[i];
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(id);
			if (vo != null)
			{
				if (!SkillUtility.DoMoFaMianYi(target, vo))
				{
					damageCalType = vo.damageCalType;
					damageType = vo.damage_type;
					int damageCalType2 = vo.damageCalType;
					switch (damageCalType2)
					{
					case 1:
					{
						float value = this.GetAttackDamage(target, casterUnit, vo, out miss, out crit) * -1f;
						int debugDamage = GlobalSettings.Instance.PvpSetting.DebugDamage;
						if (debugDamage != 0)
						{
							value = (float)debugDamage;
						}
						this.doExtraDCA_Attack(casterUnit, target, ref value);
						this.AddDataBag(vo.damageId, value);
						this.AddDamageHP((DamageValueType)vo.damage_type, value);
						goto IL_53D;
					}
					case 2:
					{
						float value2 = this.GetSkillDamage(target, casterUnit, vo, out miss, out crit) * -1f;
						this.AddDataBag(vo.damageId, value2);
						this.AddDamageHP((DamageValueType)vo.damage_type, value2);
						goto IL_53D;
					}
					case 3:
					{
						float skillTreatment = this.GetSkillTreatment(target, casterUnit, vo, out miss, out crit);
						this.AddDataBag(vo.damageId, skillTreatment);
						this.AddDamageHP((DamageValueType)vo.damage_type, skillTreatment);
						goto IL_53D;
					}
					case 4:
					{
						float num = 0f;
						AttrType property_key = vo.property_key;
						if (property_key == AttrType.Hp)
						{
							float hp_max = target.hp_max;
							if (vo.property_percent)
							{
								num = hp_max * vo.property_value;
							}
							else
							{
								num = vo.property_value;
							}
							this.doDamageCalModifier(target, AttrType.Hp, ref num);
							this.ClampMinDamage(ref num);
							this.AddDamageHP(DamageValueType.RealDamage, num);
						}
						else if (property_key == AttrType.Mp)
						{
							float mp_max = target.mp_max;
							if (vo.property_percent)
							{
								num = mp_max * vo.property_value;
							}
							else
							{
								num = vo.property_value;
							}
							this.doDamageCalModifier(target, AttrType.Mp, ref num);
							if (isJumpFont)
							{
								target.jumpFontValue(AttrType.Mp, num, casterUnit, false, false, 0, 0);
							}
							this.AddDamage(property_key, num);
						}
						else
						{
							float attr = target.GetAttr(vo.property_key);
							if (vo.property_percent)
							{
								num = attr * vo.property_value;
							}
							else
							{
								num = vo.property_value;
							}
							this.AddDamage(property_key, num);
						}
						this.AddDataBag(vo.damageId, num);
						goto IL_53D;
					}
					case 5:
					{
						float num2 = 0f;
						AttrType property_key2 = vo.property_key;
						float formualValue = FormulaTool.GetFormualValue(vo.property_formula, target);
						if (vo.property_percent)
						{
							num2 = formualValue * vo.property_value;
						}
						else
						{
							num2 = formualValue;
						}
						if (property_key2 == AttrType.Hp)
						{
							this.doDamageCalModifier(target, AttrType.Hp, ref num2);
							if (isJumpFont)
							{
								target.jumpFontValue(AttrType.Hp, num2, casterUnit, false, false, 0, vo.damage_type);
							}
							this.AddDamageHP(DamageValueType.RealDamage, num2);
						}
						else if (property_key2 == AttrType.Mp)
						{
							this.doDamageCalModifier(target, AttrType.Mp, ref num2);
							if (isJumpFont)
							{
								target.jumpFontValue(AttrType.Mp, num2, casterUnit, false, false, 0, 0);
							}
							this.AddDamage(property_key2, num2);
						}
						else
						{
							this.AddDamage(property_key2, num2);
						}
						this.AddDataBag(vo.damageId, num2);
						goto IL_53D;
					}
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
						IL_C1:
						switch (damageCalType2)
						{
						case 22:
						{
							float num3 = vo.damageParam5 - Vector3.Distance(casterUnit.transform.position, target.transform.position);
							num3 = Mathf.Clamp(num3, 0f, vo.damageParam5);
							float value3 = -(vo.damageParam4 * num3 * this.GetDistanceBuffDamage(target, casterUnit, vo, out miss, out crit));
							this.AddDataBag(vo.damageId, value3);
							this.AddDamageHP((DamageValueType)vo.damage_type, value3);
							goto IL_53D;
						}
						case 23:
							goto IL_53D;
						case 24:
						{
							float num4 = this.GetSkillDamage(target, casterUnit, vo, out miss, out crit) * -1f;
							float num5 = 1f;
							if (extraParams.Length > 0 && extraParams[0] > 0f)
							{
								num5 = extraParams[0];
							}
							num4 += -num5 * vo.damageParam5;
							this.AddDataBag(vo.damageId, num4);
							this.AddDamageHP((DamageValueType)vo.damage_type, num4);
							goto IL_53D;
						}
						default:
							goto IL_53D;
						}
						break;
					case 13:
					{
						bool flag = (int)vo.damageParam2 != 0;
						float num6 = 0f;
						if (flag)
						{
							num6 = target.hp * vo.damageParam1;
						}
						else
						{
							num6 = vo.damageParam1;
						}
						this.doDamageCalModifier(target, AttrType.Hp, ref num6);
						this.AddDataBag(vo.damageId, num6);
						this.AddDamage(AttrType.Hp, num6);
						target.jumpFontValue(AttrType.Hp, num6, casterUnit, false, false, 0, vo.damage_type);
						goto IL_53D;
					}
					case 14:
					{
						bool flag2 = (int)vo.damageParam2 != 0;
						float num7 = 0f;
						if (flag2)
						{
							num7 = target.mp * vo.damageParam1;
						}
						else
						{
							num7 = vo.damageParam1;
						}
						this.doDamageCalModifier(target, AttrType.Mp, ref num7);
						this.AddDataBag(vo.damageId, num7);
						this.AddDamage(AttrType.Mp, num7);
						target.jumpFontValue(AttrType.Mp, num7, casterUnit, false, false, 0, 0);
						goto IL_53D;
					}
					}
					goto IL_C1;
				}
				target.jumpFont(JumpFontType.MoFaMianYi, string.Empty, null, true);
			}
			IL_53D:;
		}
	}

	private void doExtraDCA_Physical(Units casterUnit, Units target, ref float damage)
	{
		if (casterUnit == null)
		{
			return;
		}
		if (target == null)
		{
			return;
		}
		float num = -(casterUnit.physicDamageAdd - target.physicDamageCut);
		damage += num;
		float num2 = casterUnit.physicDamagePercent - target.physicDamagePercentCut;
		damage *= 1f + num2;
		if (damage < 0f)
		{
			damage = 0f;
		}
	}

	private void doExtraDCA_Magic(Units casterUnit, Units target, ref float damage)
	{
		if (casterUnit == null)
		{
			return;
		}
		if (target == null)
		{
			return;
		}
		float num = -(casterUnit.magicDamageAdd - target.magicDamageCut);
		damage += num;
		float num2 = casterUnit.magicDamagePercent - target.magicDamagePercentCut;
		damage *= 1f + num2;
		if (damage < 0f)
		{
			damage = 0f;
		}
	}

	private void doExtraDCA_Attack(Units casterUnit, Units target, ref float damage)
	{
		if (casterUnit == null)
		{
			return;
		}
		if (target == null)
		{
			return;
		}
		float num = -(casterUnit.attackDamageAdd - target.attackDamageCut);
		damage += num;
		float num2 = casterUnit.attackDamagePercent - target.attackDamagePercentCut;
		damage *= 1f + num2;
		if (damage > 0f)
		{
			damage = 0f;
		}
	}

	private void doDamageCalModifier(Units target, AttrType type, ref float damage)
	{
		if (type != AttrType.Hp)
		{
			if (type == AttrType.Mp)
			{
				if (damage + target.mp >= target.mp_max)
				{
					damage = target.mp_max - target.mp;
				}
				if (damage + target.mp <= 0f)
				{
					damage = -target.mp;
				}
			}
		}
		else
		{
			if (damage + target.hp >= target.hp_max)
			{
				damage = target.hp_max - target.hp;
			}
			if (damage + target.hp <= 0f)
			{
				damage = -target.hp;
			}
		}
	}

	private void calculateDamageCut(Units target, int[] damage_ids, bool crit, bool miss, out float final_damage)
	{
		final_damage = 0f;
		if (miss)
		{
			target.jumpFont(JumpFontType.Miss, string.Empty, this.casterUnit, false);
			return;
		}
		float hp = target.hp;
		float mp = target.mp;
		float shield = target.shield;
		Dictionary<int, float>.Enumerator enumerator = this.damage_values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, float> current = enumerator.Current;
			int key = current.Key;
			KeyValuePair<int, float> current2 = enumerator.Current;
			float value = current2.Value;
			if (!this.doExcuteSpecialState(target, ref value))
			{
				switch (key)
				{
				case 1:
					if (!this.doExcuteSMHuDun(target, ref value))
					{
						if (!this.doExcuteMFHuDun(target, ref value))
						{
							final_damage += value;
						}
					}
					break;
				case 2:
					if (!this.doExcuteMoFaMianYi(target, ref value))
					{
						if (!this.doExcuteSMHuDun(target, ref value))
						{
							if (!this.doExcuteMFHuDun(target, ref value))
							{
								final_damage += value;
							}
						}
					}
					break;
				case 3:
					if (!this.doExcuteSMHuDun(target, ref value))
					{
						if (!this.doExcuteMFHuDun(target, ref value))
						{
							final_damage += value;
						}
					}
					break;
				}
			}
		}
		if (this.casterUnit != null)
		{
			this.doExcuteTeamModifier(target, this.casterUnit, ref final_damage);
		}
		this.doDamageModifier(ref final_damage, target);
		this.doDamageCalModifier(target, AttrType.Hp, ref final_damage);
		if (final_damage != 0f)
		{
			this.ClampMinDamage(ref final_damage);
			if (SkillUtility.DoWuDi(target, final_damage))
			{
				final_damage = 0f;
			}
			float attr = target.GetAttr(AttrType.DamageMultiple);
			final_damage *= attr;
			this.AddDamage(AttrType.Hp, final_damage);
		}
		this.doExcuteZhansha(target, this.data_change_list, final_damage);
	}

	private void ClampMinDamage(ref float damage)
	{
		if (damage > 0f)
		{
			return;
		}
		damage = Mathf.Clamp(damage, -3.40282347E+38f, -1f);
	}

	private void doDamageModifier(ref float final_damage, Units target)
	{
		Dictionary<int, float>.Enumerator enumerator = this.data_change_list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, float> current = enumerator.Current;
			int key = current.Key;
			KeyValuePair<int, float> current2 = enumerator.Current;
			float value = current2.Value;
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(key);
			if (vo != null)
			{
				int damageCalType = vo.damageCalType;
				switch (damageCalType)
				{
				case 12:
				{
					float num;
					if (target.mp_max == 0f)
					{
						num = 0f;
					}
					else
					{
						num = target.mp_max - target.mp;
					}
					float num2 = -vo.damageParam4 * num - vo.damageParam3;
					final_damage = num2 + final_damage;
					continue;
				}
				case 13:
				case 14:
				case 15:
				case 16:
				{
					IL_88:
					if (damageCalType != 6)
					{
						continue;
					}
					float num3 = Vector3.Distance(this.casterUnit.transform.position, target.transform.position);
					bool flag;
					bool flag2;
					float num4 = -vo.damageParam4 * num3 * this.GetDistanceBuffDamage(target, this.casterUnit, vo, out flag, out flag2);
					final_damage = num4 + final_damage;
					continue;
				}
				case 17:
					final_damage = vo.damageParam1 * final_damage;
					continue;
				case 18:
				{
					float num5;
					if (target.mp_max == 0f)
					{
						num5 = 0f;
					}
					else
					{
						num5 = 1f - target.hp / target.hp_max;
					}
					bool flag3;
					bool flag4;
					float num6 = -vo.damageParam4 * num5 * this.GetDistanceBuffDamage(target, this.casterUnit, vo, out flag3, out flag4);
					final_damage = num6 + final_damage;
					continue;
				}
				case 19:
				{
					float num7 = Mathf.Clamp(this.casterUnit.magic_power - target.magic_power, 0f, 3.40282347E+38f);
					bool flag5;
					bool flag6;
					float num8 = -vo.damageParam4 * num7 * this.GetDistanceBuffDamage(target, this.casterUnit, vo, out flag5, out flag6);
					final_damage = num8 + final_damage;
					continue;
				}
				case 20:
				{
					bool flag7;
					bool flag8;
					float num9 = this.GetAttackDamage(target, this.casterUnit, vo, out flag7, out flag8) * -1f;
					final_damage = num9 + final_damage;
					continue;
				}
				case 21:
				{
					bool flag9;
					bool flag10;
					float num10 = this.GetSkillDamage(target, this.casterUnit, vo, out flag9, out flag10) * -1f;
					final_damage = num10 + final_damage;
					continue;
				}
				case 22:
				{
					float num11 = vo.damageParam5 - Vector3.Distance(this.casterUnit.transform.position, target.transform.position);
					num11 = Mathf.Clamp(num11, 0f, vo.damageParam5);
					bool flag11;
					bool flag12;
					float num12 = vo.damageParam4 * num11 * this.GetDistanceBuffDamage(target, this.casterUnit, vo, out flag11, out flag12);
					final_damage = -num12 + final_damage;
					continue;
				}
				}
				goto IL_88;
			}
		}
	}

	private void calculateXiXue(Units casterUnit, float final_damage)
	{
		if (casterUnit == null || casterUnit.data == null || this.self == null || this.self.data == null)
		{
			return;
		}
		int dataInt = this.self.data.GetDataInt(DataType.ItemType);
		if (dataInt == 16 || dataInt == 32)
		{
			return;
		}
		foreach (int current in this.data_change_list.Keys)
		{
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(current);
			if (vo != null && vo.damageCalType == 1)
			{
				float value = casterUnit.data.GetAttr(AttrType.LifeSteal) * -final_damage;
				this.AddRebound(AttrType.Hp, value);
			}
		}
	}

	private void calculateDamageRebound(Units casterUnit, ref float final_damage)
	{
		Dictionary<int, float>.Enumerator enumerator = this.data_change_list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, float> current = enumerator.Current;
			int key = current.Key;
			KeyValuePair<int, float> current2 = enumerator.Current;
			float value = current2.Value;
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(key);
			if (vo != null)
			{
				switch (vo.damageCalType)
				{
				case 13:
				{
					float num = -value;
					this.AddRebound(AttrType.Hp, num);
					casterUnit.jumpFontValue(AttrType.Hp, num, null, false, false, 0, vo.damage_type);
					break;
				}
				case 14:
				{
					float num2 = -value;
					this.AddRebound(AttrType.Mp, num2);
					casterUnit.jumpFontValue(AttrType.Mp, num2, null, false, false, 0, 0);
					break;
				}
				case 16:
					if (!this.self.isTower && !this.self.isHome && !this.self.isBuilding)
					{
						float value2 = vo.damageParam1 * -final_damage;
						this.AddRebound(AttrType.Hp, value2);
					}
					break;
				}
			}
		}
	}

	public float GetAttackDamage(Units target, Units casterUnit, DamageData data, out bool miss, out bool crit)
	{
		miss = false;
		crit = false;
		if (!data.IsAttackDamage)
		{
			Debug.LogError(" GetAttackDamage Error : 类型不匹配!!" + data.damageCalType);
			miss = true;
			crit = false;
			return 0f;
		}
		float num = 1f;
		this.GetTableValue(target, casterUnit, data, out miss, out crit, out num);
		if (miss)
		{
			return 0f;
		}
		float num2 = casterUnit.attack * data.damage_factor;
		float num3 = 1f - (target.armor * (1f - casterUnit.armor_cut_percentage) - casterUnit.armor_cut) / Mathf.Clamp(target.armor * (1f - casterUnit.armor_cut_percentage) - casterUnit.armor_cut + 100f, 50f, 3.40282347E+38f);
		float num4 = 1f - Mathf.Clamp(target.magic_resist * (1f - casterUnit.magic_resist_cut_percentage) - casterUnit.magic_resist_cut, 0f, 3.40282347E+38f) / Mathf.Clamp(target.magic_resist * (1f - casterUnit.magic_resist_cut_percentage) - casterUnit.magic_resist_cut + 100f, 100f, 3.40282347E+38f);
		float starModifier = this.GetStarModifier(target, casterUnit);
		float num5 = 0f;
		int damage_type = data.damage_type;
		if (damage_type != 1)
		{
			if (damage_type == 2)
			{
				num5 = (num2 * num4 * starModifier * (1f + casterUnit.additive_magic_attack_relative) + casterUnit.additive_magic_attack_abs) * UnityEngine.Random.Range(0.95f, 1.05f);
				this.doExtraDCA_Magic(casterUnit, target, ref num5);
			}
		}
		else
		{
			num5 = (num2 * num3 * starModifier * (1f + casterUnit.additive_physic_attack_relative) + casterUnit.additive_physic_attack_abs) * UnityEngine.Random.Range(0.95f, 1.05f);
			this.doExtraDCA_Physical(casterUnit, target, ref num5);
		}
		num5 += (float)casterUnit.level * data.damageParam4 + (float)casterUnit.quality * data.damageParam5;
		if (crit)
		{
			return Mathf.Clamp(num5 * num, 1f, 3.40282347E+38f);
		}
		return Mathf.Clamp(num5, 1f, 3.40282347E+38f);
	}

	public float GetSkillDamage(Units target, Units casterUnit, DamageData data, out bool miss, out bool crit)
	{
		miss = false;
		crit = false;
		if (!data.IsSkillDamage)
		{
			Debug.LogError(" GetSkillDamage Error : 类型不匹配!!" + data.damageCalType);
			return 0f;
		}
		float num = 1f;
		this.GetTableValue(target, casterUnit, data, out miss, out crit, out num);
		if (miss)
		{
			return 0f;
		}
		float starModifier = this.GetStarModifier(target, casterUnit);
		float num2 = UnityEngine.Random.Range(0.95f, 1.05f);
		float num3 = 0f;
		switch (data.damage_type)
		{
		case 1:
		{
			float num4 = 1f - (target.armor * (1f - casterUnit.armor_cut_percentage) - casterUnit.armor_cut) / Mathf.Clamp(target.armor * (1f - casterUnit.armor_cut_percentage) - casterUnit.armor_cut + 100f, 50f, 3.40282347E+38f);
			num3 = (this.GetAddBaseVale(1, casterUnit, data) * data.damage_factor + data.damage_value) * num4;
			num3 = (num3 * starModifier * (1f + casterUnit.additive_physic_attack_relative) + casterUnit.additive_physic_attack_abs) * num2;
			this.doExtraDCA_Physical(casterUnit, target, ref num3);
			break;
		}
		case 2:
		{
			float num5 = 1f - Mathf.Clamp(target.magic_resist * (1f - casterUnit.magic_resist_cut_percentage) - casterUnit.magic_resist_cut, 0f, 3.40282347E+38f) / Mathf.Clamp(target.magic_resist * (1f - casterUnit.magic_resist_cut_percentage) - casterUnit.magic_resist_cut + 100f, 100f, 3.40282347E+38f);
			num3 = (this.GetAddBaseVale(2, casterUnit, data) * data.damage_factor + data.damage_value) * num5;
			num3 = (num3 * starModifier * (1f + casterUnit.additive_magic_attack_relative) + casterUnit.additive_magic_attack_abs) * num2;
			this.doExtraDCA_Magic(casterUnit, target, ref num3);
			break;
		}
		case 3:
			num3 = ((this.GetAddBaseVale(3, casterUnit, data) * data.damage_factor + data.damage_value) * (1f + casterUnit.additive_real_attack_relative) + casterUnit.additive_real_attack_abs) * num2;
			break;
		}
		if (crit)
		{
			return Mathf.Clamp(num3 * num, 1f, 3.40282347E+38f);
		}
		return Mathf.Clamp(num3, 1f, 3.40282347E+38f);
	}

	private float GetAddBaseVale(int DamageType, Units casterUnit, DamageData data)
	{
		if (data.property_AddType == 0)
		{
			if (DamageType == 1)
			{
				return casterUnit.GetAttr(AttrType.ExtraAttack);
			}
			if (DamageType == 2)
			{
				return casterUnit.magic_power;
			}
			if (DamageType == 3)
			{
				return casterUnit.magic_power;
			}
		}
		else
		{
			if (data.property_AddType == 1)
			{
				return casterUnit.GetAttr(AttrType.ExtraAttack);
			}
			if (data.property_AddType == 2)
			{
				return casterUnit.magic_power;
			}
		}
		return casterUnit.attack;
	}

	public float GetDistanceBuffDamage(Units target, Units casterUnit, DamageData data, out bool miss, out bool crit)
	{
		miss = false;
		crit = false;
		float num = 1f;
		this.GetTableValue(target, casterUnit, data, out miss, out crit, out num);
		if (miss)
		{
			return 0f;
		}
		float starModifier = this.GetStarModifier(target, casterUnit);
		float num2 = UnityEngine.Random.Range(0.95f, 1.05f);
		float num3 = 0f;
		switch ((int)data.damageParam1)
		{
		case 1:
		{
			float num4 = 1f - (target.armor - casterUnit.armor_cut) / Mathf.Clamp(target.armor - casterUnit.armor_cut + 100f, 50f, 3.40282347E+38f);
			num3 = (casterUnit.attack * data.damageParam2 + data.damageParam3) * num4;
			num3 = (num3 * starModifier * (1f + casterUnit.additive_physic_attack_relative) + casterUnit.additive_physic_attack_abs) * num2;
			this.doExtraDCA_Physical(casterUnit, target, ref num3);
			break;
		}
		case 2:
		{
			float num5 = 1f - Mathf.Clamp(target.magic_resist - casterUnit.magic_resist_cut, 0f, 3.40282347E+38f) / Mathf.Clamp(target.magic_resist - casterUnit.magic_resist_cut + 100f, 100f, 3.40282347E+38f);
			num3 = (casterUnit.magic_power * data.damageParam2 + data.damageParam3) * num5;
			num3 = (num3 * starModifier * (1f + casterUnit.additive_magic_attack_relative) + casterUnit.additive_magic_attack_abs) * num2;
			this.doExtraDCA_Magic(casterUnit, target, ref num3);
			break;
		}
		case 3:
			num3 = ((casterUnit.magic_power * data.damageParam2 + data.damageParam3) * (1f + casterUnit.additive_real_attack_relative) + casterUnit.additive_real_attack_abs) * num2;
			break;
		}
		if (crit)
		{
			return Mathf.Clamp(num3 * num, 1f, 3.40282347E+38f);
		}
		return Mathf.Clamp(num3, 1f, 3.40282347E+38f);
	}

	public float GetSkillTreatment(Units target, Units casterUnit, DamageData data, out bool miss, out bool crit)
	{
		float num = 1f;
		this.GetTableValue(target, casterUnit, data, out miss, out crit, out num);
		if (miss)
		{
			return 0f;
		}
		float num2 = UnityEngine.Random.Range(0.95f, 1.05f);
		float num3 = 0f;
		switch (data.damage_type)
		{
		case 1:
			Debug.LogError(" GetSkillTreatment Error : 类型不匹配！！");
			break;
		case 2:
		{
			float num4 = casterUnit.magic_power * data.damage_factor + data.damage_value;
			num3 = (num4 * (1f + casterUnit.additive_treatment_relative) + casterUnit.additive_treatment_abs) * num2;
			break;
		}
		case 3:
		{
			float damage_value = data.damage_value;
			num3 = (damage_value * (1f + casterUnit.additive_treatment_relative) + casterUnit.additive_treatment_abs) * num2;
			break;
		}
		}
		if (crit)
		{
			return Mathf.Clamp(num3 * num, 1f, 3.40282347E+38f);
		}
		return Mathf.Clamp(num3, 1f, 3.40282347E+38f);
	}

	private float GetStarModifier(Units target, Units casterUnit)
	{
		float result = 1f;
		if (target == null)
		{
			return result;
		}
		if (casterUnit == null)
		{
			return result;
		}
		if (target.isHero && casterUnit.isHero)
		{
			result = (float)(casterUnit.star + 9) / (float)(target.star + 9);
		}
		return result;
	}

	private void GetTableValue(Units target, Units casterUnit, DamageData data, out bool miss, out bool crit, out float crit_mag)
	{
		miss = true;
		crit = false;
		crit_mag = 1f;
		if (data.IsRealDamage)
		{
			miss = false;
			crit = false;
			crit_mag = 1f;
			return;
		}
		if (!data.IsAttackDamage)
		{
			miss = false;
			crit = false;
			crit_mag = 1f;
		}
		if (!miss)
		{
			bool flag = false;
			if (data.IsPhysicDamage)
			{
				flag = this.GetPhysicCritProp(casterUnit, out crit_mag);
			}
			else if (data.IsMagicDamage)
			{
				flag = this.GetMagicCritProp(casterUnit, out crit_mag);
			}
			if (flag)
			{
				crit = true;
			}
			else
			{
				crit = false;
			}
		}
		else
		{
			bool hitProp = this.GetHitProp(casterUnit);
			if (hitProp && !this.GetDodgeProp(target))
			{
				bool flag2 = false;
				if (data.IsPhysicDamage)
				{
					flag2 = this.GetPhysicCritProp(casterUnit, out crit_mag);
				}
				else if (data.IsMagicDamage)
				{
					flag2 = this.GetMagicCritProp(casterUnit, out crit_mag);
				}
				if (flag2)
				{
					miss = false;
					crit = true;
				}
				else
				{
					miss = false;
					crit = false;
				}
			}
		}
	}

	private bool GetHitProp(Units casterUnit)
	{
		float num = casterUnit.hit_prop * (1f + casterUnit.additive_hitprop_relative) + casterUnit.additive_hitprop_abs;
		return num >= this.GetRand();
	}

	private bool GetDodgeProp(Units target)
	{
		if (target == null)
		{
			return false;
		}
		float num = target.dodge_prop * (1f + target.additive_dodgeprop_relative) + target.additive_dodgeprop_abs;
		return num > this.GetRand();
	}

	private bool GetPhysicCritProp(Units casterUnit, out float crit_mag)
	{
		if (casterUnit == null)
		{
			crit_mag = 1f;
			return false;
		}
		float num = casterUnit.physic_crit_prop * (1f + casterUnit.additive_physic_critprop_relative) + casterUnit.additive_physic_critprop_abs;
		if (num >= this.GetRand())
		{
			crit_mag = casterUnit.physic_crit_mag;
			return true;
		}
		crit_mag = 1f;
		return false;
	}

	private bool GetMagicCritProp(Units casterUnit, out float crit_mag)
	{
		if (casterUnit == null)
		{
			crit_mag = 1f;
			return false;
		}
		float num = casterUnit.magic_crit_prop * (1f + casterUnit.additive_physic_critprop_relative) + casterUnit.additive_physic_critprop_abs;
		if (num >= this.GetRand())
		{
			crit_mag = casterUnit.magic_crit_mag;
			return true;
		}
		crit_mag = 1f;
		return false;
	}

	private float GetRand()
	{
		int num = this.ran.Next(0, 100);
		return (float)num / 100f;
	}

	private float GetRand2()
	{
		int num = this.ran2.Next(80, 120);
		return (float)num / 100f;
	}

	private bool doExcuteSpecialState(Units target, ref float damage)
	{
		if (SkillUtility.DoWuDi(target, damage))
		{
			target.jumpFont(JumpFontType.WuDi, string.Empty, this.casterUnit, false);
			return true;
		}
		return false;
	}

	private bool doExcuteMoFaMianYi(Units target, ref float damage)
	{
		if (target.WuDi.IsInState)
		{
			damage = 0f;
			return true;
		}
		if (!TagManager.IsCharacterTarget(target.gameObject) || target is Home)
		{
			damage = 0f;
			return true;
		}
		return false;
	}

	private bool doExcuteMFHuDun(Units target, ref float damage)
	{
		if (target.HuiGuangFanZhao.IsInState)
		{
			return false;
		}
		if (damage < 0f)
		{
			if (target.MoFaHuDun.IsInState && target.mp > 0f && target.moFaHuDunCoverProportion > 0f)
			{
				float num = damage / target.moFaHuDunCoverProportion;
				if (target.mp + num >= 0f)
				{
					damage = 0f;
					this.AddDamage(AttrType.Mp, num);
				}
				else
				{
					float mp = target.mp;
					damage = mp + num;
					this.AddDamage(AttrType.Mp, -mp);
				}
			}
			if (damage >= 0f)
			{
				return true;
			}
		}
		return false;
	}

	private bool doExcuteSMHuDun(Units target, ref float damage)
	{
		if (target.HuiGuangFanZhao.IsInState)
		{
			return false;
		}
		float num = target.shield;
		if (damage < 0f)
		{
			num = Mathf.Clamp(num, 0f, 3.40282347E+38f);
			if (num >= 0f && num + damage >= 0f)
			{
				this.AddDamage(AttrType.Shield, damage);
				damage = 0f;
			}
			else
			{
				damage += num;
				this.AddDamage(AttrType.Shield, -num);
			}
			if (damage >= 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void doExcuteZhansha(Units target, Dictionary<int, float> damage_ids, float damage)
	{
		if (target == null || damage_ids == null)
		{
			return;
		}
		Dictionary<int, float>.Enumerator enumerator = damage_ids.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DamageDataManager arg_33_0 = Singleton<DamageDataManager>.Instance;
			KeyValuePair<int, float> current = enumerator.Current;
			DamageData vo = arg_33_0.GetVo(current.Key);
			if (vo != null)
			{
				if (vo.damageCalType == 10)
				{
					float num = target.hp + damage;
					if ((int)vo.damageParam2 == 0)
					{
						if (target.hp < vo.damageParam1)
						{
							damage = target.hp * -1f;
							this.AddDamage(AttrType.Hp, damage);
						}
					}
					else if (target.hp < target.hp_max * vo.damageParam1)
					{
						damage = target.hp * -1f;
						this.AddDamage(AttrType.Hp, damage);
					}
				}
			}
		}
	}

	private void doExcuteAdditiveDamage(Units target, float damageMag, float damageAbs, ref float damage)
	{
		damage = damage + damageMag + damageAbs;
	}

	private void doExucteAddDamageByDistance(Units target, ref float damage)
	{
	}

	private bool doExcuteHuiGuangFanZhao(Units target, ref float damage)
	{
		if (target.HuiGuangFanZhao.IsInState && damage < 0f)
		{
			damage = -damage;
			this.AddDamage(AttrType.Hp, damage);
			target.jumpFontValue(AttrType.Hp, damage, this.casterUnit, false, false, 0, 0);
			return true;
		}
		return false;
	}

	private void doExcuteFanTan(Units casterUnit, float damage)
	{
		if (casterUnit.attackReboundCoefficient > 0f)
		{
			damage *= casterUnit.attackReboundCoefficient;
			this.AddRebound(AttrType.Hp, damage);
			casterUnit.jumpFontValue(AttrType.Hp, damage, this.self, false, false, 0, 2);
		}
	}

	private void doExcuteTeamModifier(Units target, Units caster, ref float damage)
	{
		if (BattleAttrManager.isBattleAttrOpen)
		{
			damage = BattleAttrManager.Instance.AdjustFinalDamage(caster, target, damage);
		}
	}

	private void doExcuteDiZengWoJian(Units caster)
	{
	}

	private void DumpAttr(ref Dictionary<int, float> attrs)
	{
		if (attrs != null)
		{
			Dictionary<int, float>.Enumerator enumerator = attrs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, float> current = enumerator.Current;
				int key = current.Key;
				KeyValuePair<int, float> current2 = enumerator.Current;
				float value = current2.Value;
			}
		}
	}

	public void doDataChangeAction(AttrType type, float value, Units casterUnit, bool isReverse = false)
	{
		if (value != 0f)
		{
			this.self.ChangeAttr(type, OpType.Add, value);
			this.self.jumpFontValue(type, value, null, false, false, 0, 0);
		}
	}
}
