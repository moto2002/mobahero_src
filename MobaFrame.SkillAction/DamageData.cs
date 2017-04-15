using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DamageData
	{
		public int damageId;

		private SysSkillDamageVo config;

		public int damageCalType;

		public float damageParam1;

		public float damageParam2;

		public float damageParam3;

		public float damageParam4;

		public float damageParam5;

		public float damageParam6;

		public EffectDataType DataType;

		public int damage_type
		{
			get
			{
				if (this.IsSkillData)
				{
					return (int)this.damageParam1;
				}
				return 0;
			}
		}

		public float damage_factor
		{
			get
			{
				if (this.IsSkillData)
				{
					return this.damageParam2;
				}
				return 0f;
			}
		}

		public float damage_value
		{
			get
			{
				if (this.IsSkillData)
				{
					return this.damageParam3;
				}
				return 0f;
			}
		}

		public bool IsProperty
		{
			get
			{
				return this.damageCalType == 4 || this.damageCalType == 5 || this.damageCalType == 27;
			}
		}

		public bool IsPropertyFormula
		{
			get
			{
				return this.damageCalType == 5;
			}
		}

		public bool IsPropertyValue
		{
			get
			{
				return this.damageCalType == 4;
			}
		}

		public AttrType property_key
		{
			get
			{
				if (this.IsProperty)
				{
					return (AttrType)this.damageParam1;
				}
				return AttrType.None;
			}
		}

		public float property_value
		{
			get
			{
				if (this.IsPropertyValue)
				{
					return this.damageParam2;
				}
				return 0f;
			}
		}

		public int property_formula
		{
			get
			{
				if (this.IsPropertyFormula)
				{
					return (int)this.damageParam2;
				}
				return 0;
			}
		}

		public bool property_percent
		{
			get
			{
				return this.IsProperty && this.damageParam3 == 1f;
			}
		}

		public int property_AddType
		{
			get
			{
				return (int)this.damageParam4;
			}
		}

		public bool CanRevert
		{
			get
			{
				return this.IsPropertyValue && this.property_key != AttrType.Hp && this.property_key != AttrType.Mp && this.property_key != AttrType.Shield;
			}
		}

		public bool IsSkillData
		{
			get
			{
				return this.IsAttackDamage || this.IsSkillDamage || this.IsTreatment;
			}
		}

		public bool IsAttackDamage
		{
			get
			{
				return this.damageCalType == 1 || this.damageCalType == 20;
			}
		}

		public bool IsSkillDamage
		{
			get
			{
				return this.damageCalType == 2 || this.damageCalType == 21 || this.damageCalType == 24 || this.damageCalType == 22;
			}
		}

		public bool IsTreatment
		{
			get
			{
				return this.damageCalType == 3;
			}
		}

		public bool IsPhysicDamage
		{
			get
			{
				return this.damage_type == 1;
			}
		}

		public bool IsMagicDamage
		{
			get
			{
				return this.damage_type == 2;
			}
		}

		public bool IsRealDamage
		{
			get
			{
				return this.damage_type == 3;
			}
		}

		public bool IsDistanceBuff
		{
			get
			{
				return this.damage_type == 6;
			}
		}

		public bool IsDistanceDamage
		{
			get
			{
				return this.damage_type == 22;
			}
		}

		public DamageData(string damage_id)
		{
			this.config = BaseDataMgr.instance.GetDataById<SysSkillDamageVo>(damage_id);
			this.damageId = this.config.damage_id;
			if (this.config == null)
			{
				Debug.LogError("Error damageId=" + this.damageId);
				return;
			}
			this.Parse(this.config);
		}

		public DamageData(string damage_id, SysSkillDamageVo damage_vo)
		{
			if (damage_vo == null)
			{
				Debug.LogError("Error damage_id=" + damage_id);
				return;
			}
			this.config = damage_vo;
			this.damageId = this.config.damage_id;
			this.Parse(this.config);
		}

		public void Parse(SysSkillDamageVo damage_vo)
		{
			if (damage_vo == null)
			{
				return;
			}
			try
			{
				if (StringUtils.CheckValid(damage_vo.formula))
				{
					string[] array = StringUtils.SplitVoString(damage_vo.formula, "|");
					this.damageCalType = int.Parse(array[0]);
					this.damageParam1 = ((array.Length <= 1) ? 0f : float.Parse(array[1]));
					this.damageParam2 = ((array.Length <= 2) ? 0f : float.Parse(array[2]));
					this.damageParam3 = ((array.Length <= 3) ? 0f : float.Parse(array[3]));
					this.damageParam4 = ((array.Length <= 4) ? 0f : float.Parse(array[4]));
					this.damageParam5 = ((array.Length <= 5) ? 0f : float.Parse(array[5]));
					this.damageParam6 = ((array.Length <= 6) ? 0f : float.Parse(array[6]));
				}
				if (StringUtils.CheckValid(damage_vo.effectGain))
				{
					string[] stringValue = StringUtils.GetStringValue(damage_vo.effectGain, '|');
					if (stringValue.Length > 0)
					{
						this.DataType.MagicType = (EffectMagicType)int.Parse(stringValue[0]);
					}
					if (stringValue.Length > 1)
					{
						this.DataType.GainType = (EffectGainType)int.Parse(stringValue[1]);
					}
					if (stringValue.Length > 2)
					{
						this.DataType.ImmuneType = (EffectImmuneType)int.Parse(stringValue[2]);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat(new object[]
				{
					" ==> DamageVo Parse Error : ",
					damage_vo.damage_id,
					" ",
					ex.Message
				}));
			}
		}
	}
}
