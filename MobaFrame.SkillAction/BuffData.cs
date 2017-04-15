using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BuffData
	{
		public string buffId;

		public SysSkillBuffVo config;

		public bool clear_flag;

		public bool revert;

		public string[] buff_mutex_id;

		public string[] perform_ids;

		public string[] higheff_ids;

		public string[] buff_ids;

		public string[] end_attach_higheff_ids;

		public string[] end_attach_buff_ids;

		public int[] damage_ids;

		public EffectDataType DataType;

		public bool isStopEffectWhenFullHpAndMp;

		public int[] attachState;

		public bool isProperty;

		public int m_nBuffGroup = -1;

		public int m_OverlapPriority;

		public bool isNotClearResetBuffAndDeletAtMax;

		public bool isClearWhenDeath = true;

		public bool isRemoveLayer;

		public BuffData(string buff_id)
		{
			this.buffId = buff_id;
			this.config = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(buff_id);
			if (this.config == null)
			{
				Debug.LogError("Error buff_id=" + buff_id);
				return;
			}
			this.Parse(this.config);
		}

		public BuffData(string buff_id, SysSkillBuffVo buff_vo)
		{
			if (buff_vo == null)
			{
				Debug.LogError("Error buff_id=" + buff_id);
				return;
			}
			this.buffId = buff_id;
			this.config = buff_vo;
			this.Parse(this.config);
		}

		public void Parse(SysSkillBuffVo buff_vo)
		{
			this.clear_flag = (buff_vo.clear_flag == 1);
			this.revert = (buff_vo.revert == 1);
			this.isStopEffectWhenFullHpAndMp = false;
			if (StringUtils.CheckValid(buff_vo.buff_mutex_id))
			{
				this.buff_mutex_id = StringUtils.GetStringValue(buff_vo.buff_mutex_id, ',');
				for (int i = 0; i < this.buff_mutex_id.Length; i++)
				{
					string[] stringValue = StringUtils.GetStringValue(this.buff_mutex_id[i], '|');
					if (stringValue[0] == "1")
					{
						this.isStopEffectWhenFullHpAndMp = (stringValue[1] == "1");
					}
					if (stringValue[0] == "2")
					{
						this.isNotClearResetBuffAndDeletAtMax = (stringValue[1] == "0");
					}
					if (stringValue[0] == "3")
					{
						this.isClearWhenDeath = (stringValue[1] == "1");
					}
					if (stringValue[0] == "4")
					{
						this.isRemoveLayer = (stringValue[1] == "1");
					}
				}
			}
			if (StringUtils.CheckValid(buff_vo.perform_id))
			{
				this.perform_ids = StringUtils.GetStringValue(buff_vo.perform_id, ',');
			}
			if (StringUtils.CheckValid(buff_vo.damage_id))
			{
				this.damage_ids = StringUtils.GetStringToInt(buff_vo.damage_id, ',');
			}
			if (StringUtils.CheckValid(buff_vo.buff_type))
			{
				string[] stringValue2 = StringUtils.GetStringValue(buff_vo.buff_type, '|');
				if (stringValue2.Length > 0)
				{
					this.DataType.MagicType = (EffectMagicType)int.Parse(stringValue2[0]);
				}
				if (stringValue2.Length > 1)
				{
					this.DataType.GainType = (EffectGainType)int.Parse(stringValue2[1]);
				}
				if (stringValue2.Length > 2)
				{
					this.DataType.ImmuneType = (EffectImmuneType)int.Parse(stringValue2[2]);
				}
			}
			if (StringUtils.CheckValid(buff_vo.attach_higheff))
			{
				this.higheff_ids = StringUtils.GetStringValue(buff_vo.attach_higheff, ',');
			}
			if (StringUtils.CheckValid(buff_vo.attach_buff))
			{
				this.buff_ids = StringUtils.GetStringValue(buff_vo.attach_buff, ',');
			}
			if (StringUtils.CheckValid(buff_vo.end_attach_higheff))
			{
				this.end_attach_higheff_ids = StringUtils.GetStringValue(buff_vo.end_attach_higheff, ',');
			}
			if (StringUtils.CheckValid(buff_vo.end_attach_buff))
			{
				this.end_attach_buff_ids = StringUtils.GetStringValue(buff_vo.end_attach_buff, ',');
			}
			if (StringUtils.CheckValid(buff_vo.superposition))
			{
				string[] stringValue3 = StringUtils.GetStringValue(buff_vo.superposition, '|');
				if (stringValue3 != null && stringValue3.Length == 2)
				{
					this.m_nBuffGroup = int.Parse(stringValue3[0]);
					this.m_OverlapPriority = int.Parse(stringValue3[1]);
				}
			}
			string attach_states = buff_vo.attach_states;
			if (StringUtils.CheckValid(attach_states))
			{
				this.attachState = StringUtils.GetSampleArrayStringToInt(attach_states);
			}
			if (this.damage_ids != null)
			{
				for (int j = 0; j < this.damage_ids.Length; j++)
				{
					DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(this.damage_ids[j]);
					if (vo == null)
					{
						Debug.LogError("没有找到伤害包：错误id" + this.damage_ids[j]);
					}
					else if (vo.IsPropertyValue)
					{
						this.isProperty = true;
					}
					else if (vo.IsPropertyFormula)
					{
						this.isProperty = true;
					}
					else
					{
						this.isProperty = false;
					}
				}
			}
		}

		public bool IsEffective()
		{
			return this.config.effective_time > 0f;
		}
	}
}
