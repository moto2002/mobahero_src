using System;
using System.Collections.Generic;

public class Wave
{
	public int index;

	public int wait;

	public List<Enemies> enemylist = new List<Enemies>();

	public List<wave_dialogue> dialoguelist = new List<wave_dialogue>();

	public int count;

	private Wave_State state;

	public void setAttribute(string attribute, string text)
	{
		switch (attribute)
		{
		case "index":
			this.index = int.Parse(text);
			break;
		case "wait":
			this.wait = int.Parse(text);
			break;
		case "dialogue":
			this.addDialogue(text);
			break;
		}
	}

	public void setEnemyList(List<Enemies> list)
	{
		this.enemylist = list;
	}

	public void addEnemy(Enemies enemy)
	{
		this.enemylist.Add(enemy);
	}

	public void addDialogue(string text)
	{
		if (text != null)
		{
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array != null)
			{
				wave_dialogue item;
				item.dialogue_id = array[0];
				item.dialogue_time = int.Parse(array[1]);
				this.dialoguelist.Add(item);
			}
		}
	}

	public void dumpAttributes()
	{
		for (int i = 0; i < this.enemylist.Count; i++)
		{
			Enemies enemies = this.enemylist[i];
			enemies.dumpAttributes();
		}
		for (int j = 0; j < this.dialoguelist.Count; j++)
		{
			wave_dialogue wave_dialogue = this.dialoguelist[j];
		}
	}

	public bool CheckCondition()
	{
		return this.state == Wave_State.Spawned;
	}

	public void SetWaveState(Wave_State state)
	{
		this.state = state;
	}

	public Wave_State GetWaveState()
	{
		return this.state;
	}
}
