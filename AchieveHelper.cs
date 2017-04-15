using System;

internal static class AchieveHelper
{
	public static void BrocastMsg(string promptId, string selfName, string targetName, TeamType selfTeam, TeamType targetTeam, string selfSummerName = "", string targetSummerName = "")
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (null == player)
		{
			return;
		}
		EntityType attackerType = EntityType.Hero;
		if (LevelManager.Instance.IsPvpBattleType)
		{
			UIMessageBox.ShowKillPrompt(promptId, selfName, targetName, attackerType, EntityType.None, selfSummerName, targetSummerName, selfTeam, targetTeam);
		}
		else
		{
			UIMessageBox.ShowKillPrompt(promptId, selfName, targetName, attackerType, EntityType.None, string.Empty, string.Empty, TeamType.None, TeamType.None);
		}
	}
}
