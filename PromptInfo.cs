using System;

public class PromptInfo
{
	public string npcId1;

	public string npcId2;

	public bool KillEnmeyOrDeath;

	public EntityType attackerType;

	public string voiceName;

	public EntityType deathType;

	public string summerName1;

	public string sunmerName2;

	public TeamType attackerTeam;

	public TeamType deathTeam;

	public string promptText
	{
		get;
		set;
	}

	public float showTime
	{
		get;
		set;
	}

	public PromptType promptType
	{
		get;
		set;
	}

	public int bgWidth
	{
		get;
		set;
	}

	public string soundId
	{
		get;
		set;
	}

	public string promptId
	{
		get;
		set;
	}
}
