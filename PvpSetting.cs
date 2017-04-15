using System;

[Serializable]
public class PvpSetting
{
	public bool testGUI;

	public bool testGUIMove;

	public bool recordGUI;

	public bool DirectLinkLobby;

	public string LobbyServerName = "127.0.0.1:25000";

	public bool NoMonster;

	public bool TestCreep;

	public bool isNoDamage;

	public bool isSkillUnlock;

	public bool IsMasterGizmosShown;

	public bool isDebugData;

	public float timeSelectHero = 120f;

	public float timeCheckReady = 15f;

	public bool ignoreAllPvpCmds;

	public bool ignoreSnapCmds;

	public bool isTowerDurationWound;

	public int DebugDamage;

	public bool isReplayPVP;

	public bool isPvpSkill;

	public bool isPlayerMoveBeforeServer;

	public bool isDebugSpeed;

	public bool isNoReConnect;

	public string pveServerIp = string.Empty;

	public bool test3v3 = true;

	public bool multiThread;

	public bool IsTestPVPScene;

	private bool isNoSkillCD;

	private bool isNoCost;
}
