using Com.Game.Module;
using HUD.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

public static class HUDModuleMsgTools
{
	public static void Get_AttactTarget(Units _target)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25059, _target, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Get_MinimapToggle()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25060, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void SendBattleMsg(BattleMsg _msgCode, params object[] _param)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)_msgCode, _param, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void CallBattleMsg_SiderTipsModule_Kill(string npcId1, string npcId2, bool killEnmeyOrDeath, List<string> assistantList, TeamType attackerTeam, TeamType victimTeam)
	{
		SiderTipsModule module = Singleton<HUDModuleManager>.Instance.GetModule<SiderTipsModule>(EHUDModule.SiderTips);
		if (module != null)
		{
			module.AddSiderTip_Kill(npcId1, npcId2, killEnmeyOrDeath, assistantList, attackerTeam, victimTeam);
		}
	}

	public static void CallBattleMsg_SiderTipsModule_Signal(string npcId1, TeamSignalType signalType)
	{
		SiderTipsModule module = Singleton<HUDModuleManager>.Instance.GetModule<SiderTipsModule>(EHUDModule.SiderTips);
		if (module != null)
		{
			module.AddSiderTip_Signal(npcId1, signalType);
		}
	}

	public static void SetSkillPanelPivot(SkillPanelPivot _param)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26020, _param, 0f);
		MobaMessageManager.ExecuteMsg(message);
		Singleton<HUDModuleManager>.Instance.SetSkillPanelPivot(_param);
	}
}
