using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaHeros;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class HighEffectData
	{
		public string higheffId;

		public SysSkillHigheffVo config;

		public SkillTargetCamp targetCamp;

		public TargetTag targetTag;

		public float rangeRadius;

		public int maxNum;

		public int higheffTrigerCount;

		public HighEffType higheffType;

		public float highEffTrigerParam1;

		public float highEffTrigerParam2;

		public float cdTime;

		public float delayTime;

		public string[] attachHighEffs;

		public string[] attachBuffs;

		public string[] performIds;

		public string[] attachSelfHighEffs;

		public string unit_id;

		public float param1;

		public float param2;

		public float param3;

		public float param4;

		public float param5;

		public float param6;

		public float param7;

		public int target_type;

		public string strParam1;

		public string strParam2;

		public string[] strParams1;

		public int[] damage_ids;

		public bool isAutoDestroy;

		public EffectDataType DataType;

		public HighEffectData(string higheff_id)
		{
			this.higheffId = higheff_id;
			this.config = BaseDataMgr.instance.GetDataById<SysSkillHigheffVo>(higheff_id);
			if (this.config == null)
			{
				Debug.LogError("Error higheff_id=" + higheff_id);
				return;
			}
			this.Parse(this.config);
		}

		public HighEffectData(string higheff_id, SysSkillHigheffVo higheff_vo)
		{
			this.higheffId = higheff_id;
			this.config = higheff_vo;
			this.Parse(this.config);
		}

		private void Parse(SysSkillHigheffVo higheff_vo)
		{
			if (StringUtils.CheckValid(higheff_vo.target_type))
			{
				int[] stringToInt = StringUtils.GetStringToInt(higheff_vo.target_type, '|');
				this.targetCamp = (SkillTargetCamp)((stringToInt == null) ? 0 : stringToInt[0]);
				this.targetTag = (TargetTag)((stringToInt.Length <= 1) ? 0 : stringToInt[1]);
			}
			if (StringUtils.CheckValid(higheff_vo.effective_range))
			{
				float[] stringToFloat = StringUtils.GetStringToFloat(higheff_vo.effective_range, '|');
				this.rangeRadius = ((stringToFloat == null) ? 0f : stringToFloat[0]);
				this.maxNum = ((stringToFloat.Length <= 1) ? 2147483647 : ((int)stringToFloat[1]));
				this.higheffTrigerCount = ((stringToFloat.Length <= 2) ? 2147483647 : ((int)stringToFloat[2]));
			}
			if (StringUtils.CheckValid(higheff_vo.higheff_type))
			{
				string[] stringValue = StringUtils.GetStringValue(higheff_vo.higheff_type, '|');
				this.higheffType = (HighEffType)int.Parse(stringValue[0]);
				this.SetHighEffParam(this.higheffType, stringValue);
			}
			if (StringUtils.CheckValid(higheff_vo.cd_time))
			{
				string[] stringValue2 = StringUtils.GetStringValue(higheff_vo.cd_time, ',');
				this.cdTime = ((stringValue2.Length <= 0) ? 0f : float.Parse(stringValue2[0]));
				this.delayTime = ((stringValue2.Length <= 1) ? 0f : float.Parse(stringValue2[1]));
			}
			if (StringUtils.CheckValid(higheff_vo.perform_id))
			{
				this.performIds = StringUtils.GetStringValue(higheff_vo.perform_id, ',');
			}
			if (StringUtils.CheckValid(higheff_vo.attach_higheff))
			{
				this.attachHighEffs = StringUtils.GetStringValue(higheff_vo.attach_higheff, ',');
			}
			if (StringUtils.CheckValid(higheff_vo.attach_buff))
			{
				this.attachBuffs = StringUtils.GetStringValue(higheff_vo.attach_buff, ',');
			}
			if (StringUtils.CheckValid(higheff_vo.attach_self_higheff))
			{
				this.attachSelfHighEffs = StringUtils.GetStringValue(higheff_vo.attach_self_higheff, ',');
			}
			this.isAutoDestroy = (higheff_vo.isAutoDestroy != 0);
			if (StringUtils.CheckValid(higheff_vo.damage_id))
			{
				this.damage_ids = StringUtils.GetStringToInt(higheff_vo.damage_id, ',');
			}
			if (StringUtils.CheckValid(higheff_vo.effectGain))
			{
				string[] stringValue3 = StringUtils.GetStringValue(higheff_vo.effectGain, '|');
				if (stringValue3.Length > 0)
				{
					this.DataType.MagicType = (EffectMagicType)int.Parse(stringValue3[0]);
				}
				if (stringValue3.Length > 1)
				{
					this.DataType.GainType = (EffectGainType)int.Parse(stringValue3[1]);
				}
				if (stringValue3.Length > 2)
				{
					this.DataType.ImmuneType = (EffectImmuneType)int.Parse(stringValue3[2]);
				}
			}
		}

		private void SetHighEffParam(HighEffType higheffType, string[] param)
		{
			this.param1 = 0f;
			this.param2 = 0f;
			this.param3 = 0f;
			this.param4 = 0f;
			this.param5 = 0f;
			this.strParam1 = string.Empty;
			this.strParam2 = string.Empty;
			switch (higheffType)
			{
			case HighEffType.JiFei:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				if (param.Length >= 3)
				{
					this.param2 = float.Parse(param[2]);
				}
				break;
			case HighEffType.WeiYi:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				this.param3 = ((param.Length <= 3) ? 0f : float.Parse(param[3]));
				break;
			case HighEffType.YunXuan:
			case HighEffType.WuDi:
			case HighEffType.DingShen:
			case HighEffType.ChengMo:
			case HighEffType.Repel:
			case HighEffType.Petrifaction:
			case HighEffType.Occoecatio:
			case HighEffType.Frozen:
			case HighEffType.Charm:
			case HighEffType.Taunt:
			case HighEffType.Chaos:
			case HighEffType.ScreenEffect:
			case HighEffType.HuiGuangFanZhao:
			case HighEffType.BackHome:
			case HighEffType.StateEffect:
			case HighEffType.Funeral:
			case HighEffType.Fear:
			case HighEffType.Temptation:
			case HighEffType.Imprisonment:
			case HighEffType.Sprint:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				break;
			case HighEffType.MoFaMianchu:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				this.param2 = ((param.Length <= 2) ? -1f : float.Parse(param[2]));
				this.param3 = ((param.Length <= 3) ? -1f : float.Parse(param[3]));
				this.param4 = ((param.Length <= 4) ? -1f : float.Parse(param[4]));
				break;
			case HighEffType.BaoZha:
				this.strParam1 = ((param.Length <= 1) ? string.Empty : param[1]);
				break;
			case HighEffType.GuangHuang:
			case HighEffType.BloodBallGuangHuan:
				this.param1 = (float)((param.Length <= 1) ? 0 : int.Parse(param[1]));
				this.param2 = (float)((param.Length <= 2) ? 0 : int.Parse(param[2]));
				this.param3 = (float)((param.Length <= 3) ? 0 : int.Parse(param[3]));
				this.param4 = (float)((param.Length <= 4) ? 0 : int.Parse(param[4]));
				this.param5 = (float)((param.Length <= 5) ? 0 : int.Parse(param[5]));
				this.param6 = (float)((param.Length <= 6) ? 0 : int.Parse(param[6]));
				this.param7 = ((param.Length <= 7) ? 0f : float.Parse(param[7]));
				break;
			case HighEffType.AddBuff:
			case HighEffType.ClearAllBuff:
			case HighEffType.ClearHalfBuff:
				this.CheackParam(param, 2);
				this.strParam1 = param[1];
				break;
			case HighEffType.JianShe:
			case HighEffType.BetweenSputter:
			case HighEffType.PassiveJianShe:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				break;
			case HighEffType.BornUnit:
				this.CheackParam(param, 2);
				this.unit_id = param[1];
				this.param2 = (float)((param.Length <= 2) ? 0 : int.Parse(param[2]));
				this.param3 = (float)((param.Length <= 3) ? 0 : int.Parse(param[3]));
				this.param4 = ((param.Length <= 4) ? 20f : float.Parse(param[4]));
				this.param5 = ((param.Length <= 5) ? 0f : float.Parse(param[5]));
				this.param6 = ((param.Length <= 6) ? 0f : float.Parse(param[6]));
				break;
			case HighEffType.MoFaHuDun:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				this.strParam1 = ((param.Length <= 3) ? "[]" : param[3]);
				break;
			case HighEffType.Tow:
			case HighEffType.HookBack:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				break;
			case HighEffType.ShakeCamera:
				this.CheackParam(param, 4);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				this.param3 = float.Parse(param[3]);
				break;
			case HighEffType.ReBirth:
				this.CheackParam(param, 2);
				this.param1 = ((param.Length <= 1) ? 1f : float.Parse(param[1]));
				this.param2 = ((param.Length <= 2) ? 1f : float.Parse(param[2]));
				break;
			case HighEffType.AttackExtraDamage:
				this.CheackParam(param, 5);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				this.param3 = float.Parse(param[3]);
				this.param4 = float.Parse(param[4]);
				break;
			case HighEffType.Growth:
				this.param1 = ((param.Length <= 1) ? 0f : float.Parse(param[1]));
				this.param2 = ((param.Length <= 2) ? 0f : float.Parse(param[2]));
				this.param3 = ((param.Length <= 3) ? 0f : float.Parse(param[3]));
				break;
			case HighEffType.AttackForTargetBuff:
				this.CheackParam(param, 2);
				this.strParam1 = param[1];
				break;
			case HighEffType.Morph:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.strParam1 = param[2];
				this.param2 = ((param.Length <= 3) ? 1f : float.Parse(param[3]));
				this.param3 = ((param.Length <= 4) ? 1f : float.Parse(param[4]));
				break;
			case HighEffType.Jump:
				this.CheackParam(param, 2);
				this.param1 = (float)int.Parse(param[1]);
				break;
			case HighEffType.PerformReplace:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.param2 = (float)int.Parse(param[2]);
				this.strParam1 = param[3];
				this.strParam2 = ((param.Length <= 4) ? string.Empty : param[4]);
				break;
			case HighEffType.PropertyReplace:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.strParam1 = param[2];
				this.strParam2 = param[3];
				break;
			case HighEffType.AttackEffctReplace:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.strParam1 = param[2];
				this.strParam2 = param[3];
				break;
			case HighEffType.PlayEffectPerform:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				this.param2 = ((param.Length <= 2) ? 0f : float.Parse(param[2]));
				break;
			case HighEffType.Invisible:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				this.param2 = (float)((param.Length <= 2) ? 0 : int.Parse(param[2]));
				break;
			case HighEffType.DoSkillEffAgain:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.param2 = (float)int.Parse(param[2]);
				this.param3 = float.Parse(param[3]);
				break;
			case HighEffType.SpawnerMonster:
				this.CheackParam(param, 6);
				this.param1 = (float)int.Parse(param[1]);
				this.strParam1 = param[2];
				this.param2 = (float)int.Parse(param[3]);
				this.param3 = (float)int.Parse(param[4]);
				this.param4 = (float)int.Parse(param[5]);
				break;
			case HighEffType.Switch:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.strParam1 = param[2];
				this.strParam2 = param[3];
				break;
			case HighEffType.ResidentProp:
				this.CheackParam(param, 4);
				this.param1 = (float)int.Parse(param[1]);
				this.param2 = (float)int.Parse(param[2]);
				this.param3 = float.Parse(param[3]);
				break;
			case HighEffType.Flyingkick:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				break;
			case HighEffType.AddDataBag:
				this.CheackParam(param, 2);
				this.param1 = (float)int.Parse(param[1]);
				this.param2 = (float)((param.Length <= 2) ? 0 : int.Parse(param[2]));
				this.strParam1 = ((param.Length <= 3) ? string.Empty : param[3]);
				break;
			case HighEffType.ReplaceMaterial:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				break;
			case HighEffType.ConjureSkill:
				this.CheackParam(param, 2);
				this.strParam1 = param[1];
				break;
			case HighEffType.RemoveHighEff:
				this.CheackParam(param, 2);
				this.strParam1 = param[1];
				break;
			case HighEffType.AddGold:
				this.CheackParam(param, 3);
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				this.param3 = ((param.Length <= 3) ? 0f : float.Parse(param[3]));
				break;
			case HighEffType.PhysicCritProp:
				this.param1 = float.Parse(param[1]);
				this.param2 = float.Parse(param[2]);
				break;
			case HighEffType.AssistAddGold:
				this.param1 = float.Parse(param[1]);
				break;
			case HighEffType.BuffLayerEffect:
				this.param1 = (float)int.Parse(param[1]);
				this.strParams1 = StringUtils.SplitVoString((param.Length <= 2) ? string.Empty : param[2], ",");
				break;
			case HighEffType.RemoveBuff:
				this.CheackParam(param, 2);
				this.strParam1 = param[1];
				break;
			case HighEffType.TengYunTuJi:
				this.CheackParam(param, 2);
				this.param1 = float.Parse(param[1]);
				break;
			case HighEffType.GaoJiYingSheng:
				this.param1 = float.Parse(param[1]);
				break;
			}
		}

		private void SetHighEffTrigerParam(int higheffType, string[] param)
		{
			switch (this.config.higheff_trigger)
			{
			}
		}

		private void CheackParam(string[] param, int count)
		{
			if (param.Length < count)
			{
				ClientLogger.Error("高级效果参数缺失，请检查高级效果表 id=" + this.config.higheff_id + " name=" + this.config.higheff_name);
			}
		}
	}
}
