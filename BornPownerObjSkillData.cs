using System;
using System.Collections.Generic;

internal class BornPownerObjSkillData
{
	private string _skillId = string.Empty;

	private List<int> _damageActionIds = new List<int>();

	public string SkillId
	{
		get
		{
			return this._skillId;
		}
	}

	public List<int> DamageActionIds
	{
		get
		{
			return this._damageActionIds;
		}
	}

	public BornPownerObjSkillData(string inSkillId)
	{
		this._skillId = inSkillId;
	}

	public void AddDamageActionId(int inActionId)
	{
		if (!this._damageActionIds.Contains(inActionId))
		{
			this._damageActionIds.Add(inActionId);
		}
	}
}
