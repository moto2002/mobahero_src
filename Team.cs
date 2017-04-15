using System;

public class Team
{
	public int ID;

	public string name;

	private Team[] teams;

	private Relation[] relations;

	public Team(int id, string txt)
	{
		this.ID = id;
		this.name = txt;
	}

	public void setRelation(Team[] ts, Relation[] rs)
	{
		this.teams = ts;
		this.relations = rs;
	}

	public Relation getRelation(Team otherTeam)
	{
		Relation result = Relation.Hostility;
		if (otherTeam != null)
		{
			for (int i = 0; i < this.teams.Length; i++)
			{
				if (this.teams[i] != null && this.teams[i].ID == otherTeam.ID)
				{
					result = this.relations[i];
					break;
				}
			}
		}
		return result;
	}
}
