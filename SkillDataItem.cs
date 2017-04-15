using System;
using UnityEngine;

public class SkillDataItem
{
	private string S_SkillID;

	private int S_SummonerGrade;

	private int S_UnclockGrade;

	private string S_Name;

	private Texture S_Texture;

	public string SkillID
	{
		get
		{
			return this.S_SkillID;
		}
		private set
		{
			this.S_SkillID = value;
		}
	}

	public int SummonerGrade
	{
		get
		{
			return this.S_SummonerGrade;
		}
		private set
		{
			this.S_SummonerGrade = value;
		}
	}

	public int UnclockGrade
	{
		get
		{
			return this.S_UnclockGrade;
		}
		private set
		{
			this.S_UnclockGrade = value;
		}
	}

	public string Name
	{
		get
		{
			return this.S_Name;
		}
		private set
		{
			this.S_Name = value;
		}
	}

	public Texture Texture
	{
		get
		{
			return this.S_Texture;
		}
		private set
		{
			this.S_Texture = value;
		}
	}

	public SkillDataItem(string skillID, int summonerGrade, int unclockGrade, string name, Texture tex)
	{
		this.S_SkillID = skillID;
		this.S_SummonerGrade = summonerGrade;
		this.S_UnclockGrade = unclockGrade;
		this.S_Name = name;
		this.S_Texture = tex;
	}
}
