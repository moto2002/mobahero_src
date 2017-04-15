using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using System;

public class PromptHelper
{
	public static void Prompt(string id, string text)
	{
		SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>(id);
		if (dataById != null)
		{
			UIMessageBox.ShowMessage(LanguageManager.Instance.GetStringById(text), dataById.text_time, 0);
			AudioMgr.PlayUI(dataById.sound, null, false, false);
		}
		else
		{
			ClientLogger.Error("PromptHelper.Prompt: cannot found #" + id);
		}
	}

	public static void PromptFormat(string id, params object[] args)
	{
		SysPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPromptVo>(id);
		if (dataById != null)
		{
			try
			{
				string mess = string.Format(LanguageManager.Instance.GetStringById(dataById.prompt_text), args);
				UIMessageBox.ShowMessage(mess, dataById.text_time, 0);
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
			AudioMgr.PlayUI(dataById.sound, null, false, false);
		}
		else
		{
			ClientLogger.Error("PromptHelper.Prompt: cannot found #" + id);
		}
	}

	public static string GetFriendlyText(TeamType team)
	{
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			return LanguageManager.Instance.GetStringById((team != TeamType.LM) ? "Prompt_Text_166" : "Prompt_Text_165");
		}
		bool flag = team == TeamType.LM;
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			flag = (Singleton<PvpManager>.Instance.SelfTeamType == team);
		}
		return LanguageManager.Instance.GetStringById((!flag) ? "Prompt_Text_164" : "Prompt_Text_163");
	}

	public static string CreepInControlId(Units inAttacker)
	{
		string result = (!inAttacker.isPlayer && !inAttacker.isMyTeam) ? "1151" : "1150";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1152";
				break;
			case TeamType.BL:
				result = "1153";
				break;
			}
		}
		return result;
	}

	public static string CreepKilledId(Units inAttacker)
	{
		string result = (!inAttacker.isPlayer && !inAttacker.isMyTeam) ? "1154" : "1155";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1156";
				break;
			case TeamType.BL:
				result = "1157";
				break;
			}
		}
		return result;
	}

	public static string CreepGoldKilledId(Units inAttacker)
	{
		string result = "1138";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1141";
				break;
			case TeamType.BL:
				result = "1142";
				break;
			}
		}
		return result;
	}

	public static string CreepGoldPlunderedId(Units inAttacker)
	{
		string result = (!inAttacker.isPlayer && !inAttacker.isMyTeam) ? "1139" : "1140";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1141";
				break;
			case TeamType.BL:
				result = "1142";
				break;
			}
		}
		return result;
	}

	public static string AssistantCreepKilledId(Units inAttacker)
	{
		string result = (!inAttacker.isPlayer && !inAttacker.isMyTeam) ? "1151" : "1150";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1152";
				break;
			case TeamType.BL:
				result = "1153";
				break;
			}
		}
		return result;
	}

	public static string SoldierCreepKilledId(Units inAttacker)
	{
		string result = (!inAttacker.isPlayer && !inAttacker.isMyTeam) ? "1145" : "1144";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (inAttacker.TeamType)
			{
			case TeamType.LM:
				result = "1146";
				break;
			case TeamType.BL:
				result = "1147";
				break;
			}
		}
		return result;
	}

	public static string GetTowerDestroyId(Units unit)
	{
		string result = (!unit.isMyTeam) ? "1106" : "1105";
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			switch (unit.TeamType)
			{
			case TeamType.LM:
				result = "1107";
				break;
			case TeamType.BL:
				result = "1108";
				break;
			}
		}
		return result;
	}

	public static bool IsTuanmie(string promptId)
	{
		return promptId == "1099" || promptId == "1100" || promptId == "1101" || promptId == "1102" || promptId == "1103";
	}
}
