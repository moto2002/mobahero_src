using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using UnityEngine;

namespace Com.Game.Utils
{
	public class FormulaTool
	{
		public static float GetFormualValue(int formula_id, Units target)
		{
			float result = 0f;
			SysSkillFormulaVo dataById = BaseDataMgr.instance.GetDataById<SysSkillFormulaVo>(formula_id.ToString());
			if (dataById == null)
			{
				Debug.LogError("没有找到相应的公式，请检查公式表！！ error id=" + formula_id);
				return 0f;
			}
			float propertyValue = FormulaTool.GetPropertyValue(dataById.x0, target);
			float propertyValue2 = FormulaTool.GetPropertyValue(dataById.x1, target);
			float propertyValue3 = FormulaTool.GetPropertyValue(dataById.x2, target);
			float propertyValue4 = FormulaTool.GetPropertyValue(dataById.x3, target);
			float propertyValue5 = FormulaTool.GetPropertyValue(dataById.x4, target);
			switch (dataById.type)
			{
			case 1:
				result = propertyValue + propertyValue2 + propertyValue3 + propertyValue4 + propertyValue5;
				break;
			case 2:
				result = propertyValue * propertyValue2 * propertyValue3 * propertyValue4 * propertyValue5;
				break;
			case 3:
				result = propertyValue + propertyValue2 * propertyValue3;
				break;
			case 4:
				result = propertyValue + propertyValue2 + propertyValue3 * propertyValue3;
				break;
			case 5:
				result = propertyValue + propertyValue2 * (propertyValue3 + propertyValue4);
				break;
			case 6:
				result = propertyValue + propertyValue2 * (propertyValue3 + propertyValue4) * (propertyValue3 + propertyValue4);
				break;
			case 7:
				result = propertyValue + propertyValue2 * (propertyValue3 - propertyValue4);
				break;
			}
			return result;
		}

		public static string GetFormualValueForIntroduce(int formula_id, Units target)
		{
			string text = string.Empty;
			float num = 0f;
			SysSkillFormulaVo dataById = BaseDataMgr.instance.GetDataById<SysSkillFormulaVo>(formula_id.ToString());
			if (dataById == null)
			{
				Debug.LogError("没有找到相应的公式，请检查公式表！！ error id=" + formula_id);
				return "(读取错误)";
			}
			float propertyValue = FormulaTool.GetPropertyValue(dataById.x0, target);
			float propertyValue2 = FormulaTool.GetPropertyValue(dataById.x1, target);
			float propertyValue3 = FormulaTool.GetPropertyValue(dataById.x2, target);
			float propertyValue4 = FormulaTool.GetPropertyValue(dataById.x3, target);
			float propertyValue5 = FormulaTool.GetPropertyValue(dataById.x4, target);
			float[] stringToFloat = StringUtils.GetStringToFloat(dataById.x0, '|');
			if ((int)stringToFloat[0] == 2)
			{
				if (dataById.type != 2)
				{
					num = propertyValue;
				}
				else
				{
					num = propertyValue * propertyValue2 * propertyValue3 * propertyValue4 * propertyValue5;
				}
				if (stringToFloat[1] == 6f || stringToFloat[1] == 24f || stringToFloat[1] == 63f)
				{
					text = "[ffa810]" + num.ToString("0");
				}
				else if (stringToFloat[1] == 25f)
				{
					text = "[29caff]" + num.ToString("0");
				}
				else if (stringToFloat[1] == 1f || stringToFloat[1] == 12f)
				{
					text = "[ff1010]" + num.ToString("0");
				}
				else
				{
					text = num.ToString("0");
				}
				text += "[-]";
			}
			else
			{
				text = propertyValue.ToString("0");
			}
			if (dataById.type != 2)
			{
				switch (dataById.type)
				{
				case 1:
					num = propertyValue2 + propertyValue3 + propertyValue4 + propertyValue5;
					break;
				case 2:
					num = propertyValue * propertyValue2 * propertyValue3 * propertyValue4 * propertyValue5;
					break;
				case 3:
					num = propertyValue2 * propertyValue3;
					break;
				case 4:
					num = propertyValue2 + propertyValue3 * propertyValue3;
					break;
				case 5:
					num = propertyValue2 * (propertyValue3 + propertyValue4);
					break;
				case 6:
					num = propertyValue2 * (propertyValue3 + propertyValue4) * (propertyValue3 + propertyValue4);
					break;
				case 7:
					num = propertyValue2 * (propertyValue3 - propertyValue4);
					break;
				}
				float[] stringToFloat2 = StringUtils.GetStringToFloat(dataById.x2, '|');
				if ((int)stringToFloat2[0] == 2)
				{
					if (dataById.type == 2)
					{
						text = text + propertyValue.ToString("0") + "[-]";
					}
					else if (stringToFloat2[1] == 6f || stringToFloat2[1] == 24f || stringToFloat2[1] == 63f)
					{
						text = text + "[ffa810](+" + num.ToString("0") + ")[-]";
					}
					else if (stringToFloat2[1] == 25f)
					{
						text = text + "[29caff](+" + num.ToString("0") + ")[-]";
					}
					else if (stringToFloat2[1] == 1f || stringToFloat[1] == 12f)
					{
						text = text + "[ff1010](+" + num.ToString("0") + ")[-]";
					}
					else
					{
						text = text + "(+" + num.ToString("0") + ")";
					}
				}
				else
				{
					text = text + "(+" + propertyValue.ToString("0") + ")";
				}
			}
			return text;
		}

		private static float GetPropertyValue(string protery_type, Units target)
		{
			if (target != null && StringUtils.CheckValid(protery_type))
			{
				float[] stringToFloat = StringUtils.GetStringToFloat(protery_type, '|');
				float result;
				if ((int)stringToFloat[0] == 1)
				{
					result = stringToFloat[1];
				}
				else if ((int)stringToFloat[1] == 64)
				{
					result = (float)target.level;
				}
				else
				{
					result = target.GetAttr((AttrType)stringToFloat[1]);
				}
				return result;
			}
			return 0f;
		}
	}
}
