using MobaHeros.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class TowerCounter : UtilCounter
{
	public Dictionary<int, Dictionary<int, List<Tower>>> PriorityTowers;

	public Dictionary<int, Dictionary<int, List<Tower>>> ReservedTowers;

	private List<int> _priority;

	private List<Tower> _towers2unlock;

	private List<Tower> _towersAlreadyUnlock;

	private int _lmPriority = 999999999;

	private int _blPriority = 999999999;

	private VTrigger _towerDestroyTriiger;

	private VTrigger _homeDestroyTrigger;

	public TowerCounter(UtilType type) : base(type)
	{
	}

	public override void InitCounter()
	{
		this.PriorityTowers = new Dictionary<int, Dictionary<int, List<Tower>>>();
		this.ReservedTowers = new Dictionary<int, Dictionary<int, List<Tower>>>();
		this._towers2unlock = new List<Tower>();
		this._towersAlreadyUnlock = new List<Tower>();
		this._priority = new List<int>();
		this.PriorityTowers.Add(0, new Dictionary<int, List<Tower>>());
		this.PriorityTowers.Add(1, new Dictionary<int, List<Tower>>());
		this.PriorityTowers.Add(3, new Dictionary<int, List<Tower>>());
		this.ReservedTowers.Add(0, new Dictionary<int, List<Tower>>());
		this.ReservedTowers.Add(1, new Dictionary<int, List<Tower>>());
		this.ReservedTowers.Add(3, new Dictionary<int, List<Tower>>());
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Building);
		using (List<Units>.Enumerator enumerator = mapUnits.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Tower tower = (Tower)enumerator.Current;
				if (!this.PriorityTowers[tower.teamType].ContainsKey(tower.TowerPriority))
				{
					this.PriorityTowers[tower.teamType].Add(tower.TowerPriority, new List<Tower>());
					this.PriorityTowers[tower.teamType][tower.TowerPriority].Add(tower);
				}
				else
				{
					this.PriorityTowers[tower.teamType][tower.TowerPriority].Add(tower);
				}
				if (!this.ReservedTowers[tower.teamType].ContainsKey(tower.TowerPriority))
				{
					this.ReservedTowers[tower.teamType].Add(tower.TowerPriority, new List<Tower>());
					this.ReservedTowers[tower.teamType][tower.TowerPriority].Add(tower);
				}
				else
				{
					this.ReservedTowers[tower.teamType][tower.TowerPriority].Add(tower);
				}
				if (!this._priority.Contains(tower.TowerPriority))
				{
					this._priority.Add(tower.TowerPriority);
				}
				this.CheckPriority(tower);
			}
		}
		this._priority.Sort();
		this._priority.Reverse();
		this.RegisterListeners();
		Task task = new Task(this.DelayUnlock(), true);
	}

	[DebuggerHidden]
	private IEnumerator DelayUnlock()
	{
		TowerCounter.<DelayUnlock>c__Iterator1B8 <DelayUnlock>c__Iterator1B = new TowerCounter.<DelayUnlock>c__Iterator1B8();
		<DelayUnlock>c__Iterator1B.<>f__this = this;
		return <DelayUnlock>c__Iterator1B;
	}

	private void CheckPriority(Tower tower)
	{
		if (tower.teamType == 0)
		{
			if (tower.TowerPriority < this._lmPriority)
			{
				this._lmPriority = tower.TowerPriority;
			}
		}
		else if (tower.TowerPriority < this._blPriority)
		{
			this._blPriority = tower.TowerPriority;
		}
	}

	private void OnTowerDestroy()
	{
		Units triggerUnit = TriggerManager.GetTriggerUnit();
		Tower tower = triggerUnit as Tower;
		int teamType = tower.teamType;
		int towerPriority = tower.TowerPriority;
		if (this.ReservedTowers[teamType].ContainsKey(towerPriority))
		{
			List<Tower> list = this.ReservedTowers[teamType][towerPriority];
			if (list.Contains(tower))
			{
				list.Remove(tower);
			}
			if (list.Count == 0)
			{
				this.ReservedTowers[teamType].Remove(towerPriority);
			}
		}
		if (tower != null && this.IsPriorityAllUnlock(teamType, towerPriority))
		{
			int nextPriority = this.GetNextPriority(tower.TowerPriority);
			if (nextPriority != -1)
			{
				this.UnlockPriority(tower.teamType, nextPriority);
			}
		}
	}

	public Units GetTowerOfLowestPriority(Units target)
	{
		int num = (target.teamType != 0) ? 0 : 1;
		int key = 0;
		if (num == 0)
		{
			key = this._lmPriority;
		}
		else if (num == 1)
		{
			key = this._blPriority;
		}
		if (this.ReservedTowers[num].ContainsKey(key))
		{
			List<Tower> towers = this.ReservedTowers[num][key];
			return TargetSelectHelper.GetNearestTowerOfCustom(target, this.GetUnitsOfTowers(towers));
		}
		return null;
	}

	private List<Units> GetUnitsOfTowers(List<Tower> towers)
	{
		List<Units> list = new List<Units>();
		foreach (Tower current in towers)
		{
			Units item = current;
			list.Add(item);
		}
		return list;
	}

	private bool CanUnlock(Tower tower)
	{
		if (tower != null && tower.isLive && !this._towersAlreadyUnlock.Contains(tower))
		{
			int previousPriority = this.GetPreviousPriority(tower.TowerPriority);
			if (this.IsPriorityAllUnlock(tower.teamType, previousPriority))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsPriorityAllUnlock(int team, int priority)
	{
		return !this.ReservedTowers[team].ContainsKey(priority);
	}

	private int GetPreviousPriority(int priority)
	{
		foreach (int current in this._priority)
		{
			if (current < priority)
			{
				return current;
			}
		}
		return -1;
	}

	private int GetNextPriority(int priority)
	{
		int[] array = this._priority.ToArray();
		for (int i = array.Length - 1; i >= 0; i--)
		{
			if (array[i] > priority)
			{
				return array[i];
			}
		}
		return -1;
	}

	private void UnlockPriority(int team, int priority)
	{
		if (this.PriorityTowers[team].ContainsKey(priority))
		{
			List<Tower> list = this.PriorityTowers[team][priority];
			foreach (Tower current in list)
			{
				this.UnlockTower(current);
				this.ChangeCurPriority(current);
			}
		}
	}

	private void UnlockTower(Tower tower)
	{
		if (this.CanUnlock(tower))
		{
			this.ChangeCurPriority(tower);
			this._towersAlreadyUnlock.Add(tower);
			tower.highEffManager.RemoveHighEffect("HighEff_Invincibility");
		}
	}

	private void ChangeCurPriority(Tower tower)
	{
		if (tower.teamType == 0)
		{
			this._lmPriority = tower.TowerPriority;
		}
		else
		{
			this._blPriority = tower.TowerPriority;
		}
	}

	private void RegisterListeners()
	{
		this._towerDestroyTriiger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitTowerDestroy, null, new TriggerAction(this.OnTowerDestroy), -1, "Building");
		this._homeDestroyTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitTowerDestroy, null, new TriggerAction(this.OnTowerDestroy), -1, "Home");
	}

	private void UnRegisterListerners()
	{
		TriggerManager.DestroyTrigger(this._towerDestroyTriiger);
		TriggerManager.DestroyTrigger(this._homeDestroyTrigger);
		this._towerDestroyTriiger = null;
		this._homeDestroyTrigger = null;
	}

	public override void Clear()
	{
		base.Clear();
		this.UnRegisterListerners();
	}
}
