using System;
using System.Collections.Generic;

public struct SkillDataKey
{
	public class EqualityComparer : IEqualityComparer<SkillDataKey>
	{
		public bool Equals(SkillDataKey x, SkillDataKey y)
		{
			return x.SkillID == y.SkillID && x.Level == y.Level && x.Skin == y.Skin;
		}

		public int GetHashCode(SkillDataKey obj)
		{
			return obj.SkillID.GetHashCode();
		}
	}

	private string mSkillID;

	private int mLevel;

	private int mSkin;

	public string SkillID
	{
		get
		{
			return this.mSkillID;
		}
	}

	public int Level
	{
		get
		{
			return this.mLevel;
		}
	}

	public int Skin
	{
		get
		{
			return this.mSkin;
		}
	}

	public SkillDataKey(string skillID, int level, int skin)
	{
		this.mSkillID = skillID;
		this.mLevel = level;
		this.mSkin = skin;
	}
}
