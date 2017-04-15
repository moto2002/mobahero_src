using System;
using UnityEngine;

public class AttrManager
{
	public static string GetName(int prop)
	{
		switch (prop)
		{
		case 1:
			return string.Empty;
		case 2:
			return "魔法";
		case 3:
			return "力量";
		case 4:
			return "敏捷";
		case 5:
			return "智力";
		case 6:
			return "攻击力";
		case 7:
			return "护甲";
		case 8:
			return "闪避";
		case 9:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 22:
			IL_72:
			switch (prop)
			{
			case 113:
				return "吸血";
			case 114:
				return "加速";
			case 115:
				return "减速";
			case 116:
				return "晕眩";
			case 117:
				return "沉默";
			case 118:
				return "定身";
			case 119:
				return "石化";
			case 120:
				return "无敌";
			case 121:
				return "免疫";
			case 122:
				return "魔法护盾";
			case 123:
				return "致盲";
			case 124:
				return "冰冻";
			case 125:
				return "魅惑";
			case 126:
				return "嘲讽";
			case 127:
				return "薄葬";
			case 128:
				return "恐惧";
			case 129:
				return "远程轰炸";
			case 130:
				return "回光返照";
			case 131:
				return "致盲";
			case 132:
				return "打断";
			case 133:
				return "斩杀致死";
			case 134:
				return "格挡";
			case 135:
				return string.Empty;
			case 136:
				return string.Empty;
			case 137:
				return "未命中";
			case 138:
				return "反弹";
			default:
				if (prop != 999)
				{
					return string.Empty;
				}
				return string.Empty;
			}
			break;
		case 10:
			return "攻速";
		case 11:
			return "护盾";
		case 21:
			return "魔抗";
		case 23:
			return string.Empty;
		case 24:
		case 25:
		case 26:
			return string.Empty;
		}
		goto IL_72;
	}

	public static Color GetColor(int type, string value = "")
	{
		Color result;
		switch (type)
		{
		case 113:
		case 137:
			goto IL_154;
		case 114:
		case 115:
			goto IL_18B;
		case 116:
		case 117:
		case 118:
		case 119:
		case 120:
		case 121:
		case 122:
		case 123:
		case 124:
		case 125:
		case 126:
		case 127:
		case 128:
		case 129:
		case 130:
		case 132:
		case 133:
		case 134:
		case 138:
			result = Color.red;
			return result;
		case 131:
			IL_77:
			switch (type)
			{
			case 1:
				if (value.Contains("+"))
				{
					result = Color.green;
				}
				else
				{
					result = Color.red;
				}
				return result;
			case 2:
				result = Color.gray;
				return result;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 10:
			case 11:
			case 21:
				goto IL_154;
			case 9:
				goto IL_18B;
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 22:
				IL_E7:
				if (type != 999)
				{
					result = Color.black;
					return result;
				}
				goto IL_154;
			case 23:
				result = Color.grey;
				return result;
			case 24:
				result = Color.yellow;
				return result;
			case 25:
				result = Color.blue;
				return result;
			case 26:
				result = Color.red;
				return result;
			}
			goto IL_E7;
		case 135:
			result = Color.green;
			return result;
		case 136:
			result = Color.blue;
			return result;
		case 139:
			result = Color.clear;
			return result;
		}
		goto IL_77;
		IL_154:
		result = Color.yellow;
		return result;
		IL_18B:
		result = Color.white;
		return result;
	}
}
