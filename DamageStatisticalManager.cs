using System;
using System.Collections.Generic;

public class DamageStatisticalManager : BaseGameModule
{
	private readonly Dictionary<int, int> _damageDictionary = new Dictionary<int, int>();

	private readonly Dictionary<int, Units> _unitsDictionary = new Dictionary<int, Units>();

	public Dictionary<int, Units> UnitsDictionary
	{
		get
		{
			return this._unitsDictionary;
		}
	}

	public override void Init()
	{
	}

	public override void Uninit()
	{
		this._damageDictionary.Clear();
		this._unitsDictionary.Clear();
	}

	public void AddDamage(int spawnId, int damageValue)
	{
		if (this._damageDictionary.ContainsKey(spawnId))
		{
			Dictionary<int, int> damageDictionary;
			Dictionary<int, int> expr_17 = damageDictionary = this._damageDictionary;
			int num = damageDictionary[spawnId];
			expr_17[spawnId] = num + damageValue;
		}
		else
		{
			this._damageDictionary.Add(spawnId, damageValue);
		}
	}

	public void AddHero(int spawn_id, Units units)
	{
		if (!this._unitsDictionary.ContainsKey(spawn_id))
		{
			this._unitsDictionary.Add(spawn_id, units);
		}
	}
}
