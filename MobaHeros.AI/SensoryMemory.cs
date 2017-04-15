using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.AI
{
	public class SensoryMemory
	{
		private Units m_Owner;

		private List<int> m_MemoryId = new List<int>();

		private Dictionary<int, MemoryRecord> m_MemoryMap = new Dictionary<int, MemoryRecord>();

		private List<Units> target_units = new List<Units>();

		private Dictionary<string, int> m_MemoryPority = new Dictionary<string, int>();

		public SensoryMemory(Units owner, GameObject memObj, double MemorySpan)
		{
			this.m_Owner = owner;
			this.InitTargetPorityValue();
		}

		public void UpdateBotWithSpawned(Units pBot)
		{
			if (pBot == null || !pBot.isLive || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			if (!this.m_MemoryMap.ContainsKey(pBot.unique_id) || this.m_MemoryMap[pBot.unique_id] == null)
			{
				MemoryRecord memoryRecord = new MemoryRecord();
				memoryRecord.dTimeSpawned = (double)Time.time;
				memoryRecord.dTimeLastSensed = (double)Time.time;
				memoryRecord.vLastSensedPosition = pBot.transform.position;
				this.AddToMemory(pBot.unique_id, memoryRecord);
				this.ResetHatredValue(pBot);
			}
		}

		public void UpdateBotWithDeath(Units pBot)
		{
			if (pBot == null || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			if (this.m_MemoryMap.ContainsKey(pBot.unique_id))
			{
				this.m_MemoryMap[pBot.unique_id] = null;
			}
		}

		public void UpdateBotWithSoundSource(Units pBot)
		{
			if (pBot == null || !pBot.isLive || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			if (!this.m_MemoryMap.ContainsKey(pBot.unique_id) || this.m_MemoryMap[pBot.unique_id] == null)
			{
				MemoryRecord memoryRecord = new MemoryRecord();
				memoryRecord.dTimeLastSensed = (double)Time.time;
				memoryRecord.vLastSensedPosition = pBot.transform.position;
				this.AddToMemory(pBot.unique_id, memoryRecord);
			}
			else
			{
				MemoryRecord memoryRecord2 = this.m_MemoryMap[pBot.unique_id];
				memoryRecord2.dTimeLastSensed = (double)Time.time;
				memoryRecord2.vLastSensedPosition = pBot.transform.position;
			}
		}

		public void UpdateBotWithAttack(Units pBot, float damage)
		{
			if (pBot == null || !pBot.isLive || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			if (!this.m_MemoryMap.ContainsKey(pBot.unique_id) || this.m_MemoryMap[pBot.unique_id] == null)
			{
				MemoryRecord memoryRecord = new MemoryRecord();
				memoryRecord.dTimeLastSensed = (double)Time.time;
				memoryRecord.vLastSensedPosition = pBot.transform.position;
				this.AddToMemory(pBot.unique_id, memoryRecord);
			}
			else
			{
				MemoryRecord memoryRecord2 = this.m_MemoryMap[pBot.unique_id];
				memoryRecord2.dTimeLastSensed = (double)Time.time;
				memoryRecord2.vLastSensedPosition = pBot.transform.position;
			}
			this.AddHatredValue(pBot, (int)damage * 100);
		}

		public void UpdateBotWithVisibile(Units pBot)
		{
			if (pBot == null || !pBot.isLive || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			if (!this.m_MemoryMap.ContainsKey(pBot.unique_id) || this.m_MemoryMap[pBot.unique_id] == null)
			{
				MemoryRecord memoryRecord = new MemoryRecord();
				memoryRecord.dTimeBecameVisible = (double)Time.time;
				memoryRecord.dTimeLastSensed = (double)Time.time;
				memoryRecord.dTimeLastVisible = (double)Time.time;
				memoryRecord.bWithingFOV = true;
				memoryRecord.vLastSensedPosition = pBot.transform.position;
				this.AddToMemory(pBot.unique_id, memoryRecord);
			}
			else
			{
				MemoryRecord memoryRecord2 = this.m_MemoryMap[pBot.unique_id];
				memoryRecord2.dTimeLastSensed = (double)Time.time;
				memoryRecord2.dTimeLastVisible = (double)Time.time;
				memoryRecord2.bWithingFOV = true;
				memoryRecord2.vLastSensedPosition = pBot.transform.position;
			}
		}

		public void UpdateBotWithoutVisibile(Units pBot)
		{
			if (pBot == null || !pBot.isLive || pBot.unique_id == this.m_Owner.unique_id)
			{
				return;
			}
			MemoryRecord memoryRecord;
			if (this.m_MemoryMap.TryGetValue(pBot.unique_id, out memoryRecord) && memoryRecord != null)
			{
				memoryRecord.bWithingFOV = false;
				memoryRecord.dTimeLastOutOfView = (double)Time.time;
				this.ResetHatredValue(pBot);
			}
		}

		public void UpdateBotsWithSpawned()
		{
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			Dictionary<int, Units>.Enumerator enumerator = allMapUnits.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Units> current = enumerator.Current;
				Units value = current.Value;
				if (!(value == null) && value.unique_id != this.m_Owner.unique_id)
				{
					this.UpdateBotWithSpawned(value);
				}
			}
		}

		public void AddToMemory(int unique_id, MemoryRecord record)
		{
			if (!this.m_MemoryMap.ContainsKey(unique_id))
			{
				this.m_MemoryId.Add(unique_id);
				this.m_MemoryMap.Add(unique_id, record);
			}
			else
			{
				this.m_MemoryMap[unique_id] = record;
			}
		}

		public void RemoveFromMemory(int unique_id)
		{
			if (this.m_MemoryMap.ContainsKey(unique_id))
			{
				this.m_MemoryMap.Remove(unique_id);
				this.m_MemoryId.Remove(unique_id);
			}
		}

		public void UpdateMemory(List<Units> pBots)
		{
			if (this.m_MemoryId == null)
			{
				return;
			}
			MapManager instance = MapManager.Instance;
			if (instance == null)
			{
				return;
			}
			for (int i = 0; i < this.m_MemoryId.Count; i++)
			{
				int num = this.m_MemoryId[i];
				Units unit = instance.GetUnit(num);
				if (unit == null || !unit.isLive)
				{
					if (this.m_MemoryMap.ContainsKey(num))
					{
						this.m_MemoryMap[num] = null;
					}
				}
				else if (pBots != null && pBots.Contains(unit))
				{
					this.UpdateBotWithVisibile(unit);
				}
				else
				{
					this.UpdateBotWithoutVisibile(unit);
				}
			}
		}

		public void ClearMemory()
		{
			this.m_MemoryMap.Clear();
			this.m_MemoryId.Clear();
			this.target_units.Clear();
			this.m_MemoryPority.Clear();
		}

		public bool isOpponentSensed(Units pOpponent)
		{
			return pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id);
		}

		public bool isOpponentShootable(Units pOpponent)
		{
			return pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id) && this.m_MemoryMap[pOpponent.unique_id].bShootable;
		}

		public bool isOpponentWithinFOV(Units pOpponent)
		{
			return pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id) && this.m_MemoryMap[pOpponent.unique_id].bWithingFOV;
		}

		public Vector3? GetLastRecordedPositionOfOpponent(Units pOpponent)
		{
			if (pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id))
			{
				return new Vector3?(this.m_MemoryMap[pOpponent.unique_id].vLastSensedPosition);
			}
			return null;
		}

		public double GetTimeOpponentHasBeenVisible(Units pOpponent)
		{
			if (pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id))
			{
				return this.m_MemoryMap[pOpponent.unique_id].dTimeBecameVisible;
			}
			return 0.0;
		}

		public double GetTimeSinceLastSensed(Units pOpponent)
		{
			if (pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id))
			{
				return this.m_MemoryMap[pOpponent.unique_id].dTimeLastSensed;
			}
			return 0.0;
		}

		public double GetTimeOpponentHasBeenOutOfView(Units pOpponent)
		{
			if (pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id))
			{
				return this.m_MemoryMap[pOpponent.unique_id].dTimeLastSensed;
			}
			return 0.0;
		}

		public int GetCurrentTargetHateValue(Units pOpponent)
		{
			if (pOpponent != null && this.m_MemoryMap.ContainsKey(pOpponent.unique_id))
			{
				return this.m_MemoryMap[pOpponent.unique_id].fHatredValue;
			}
			return 0;
		}

		public List<Units> GetListOfRecentlySensedOpponents(Relation relation, TargetTag tagType = TargetTag.All, bool isVisibile = true, bool isCheckTaunted = false, SortType sortType = SortType.None, FindType findType = FindType.None, object param = null)
		{
			this.target_units.Clear();
			bool flag = false;
			for (int i = 0; i < this.m_MemoryId.Count; i++)
			{
				int num = this.m_MemoryId[i];
				MemoryRecord memoryRecord = this.m_MemoryMap[num];
				if (memoryRecord != null)
				{
					if (!isVisibile || memoryRecord.bWithingFOV)
					{
						Units unit = MapManager.Instance.GetUnit(num);
						if (unit == null || !unit.isLive)
						{
							this.m_MemoryMap[num] = null;
						}
						else if (relation == Relation.Hostility)
						{
							if (TeamManager.CanAttack(this.m_Owner, unit))
							{
								if (isCheckTaunted && unit.ChaoFeng.IsInState)
								{
									this.target_units.Clear();
									this.target_units.Add(unit);
									flag = true;
									break;
								}
								if (TagManager.CheckTag(unit, tagType))
								{
									this.target_units.Add(unit);
								}
							}
						}
						else if (relation == Relation.Companion && TeamManager.CanAssist(this.m_Owner, unit))
						{
							if (TagManager.CheckTag(unit, tagType))
							{
								this.target_units.Add(unit);
							}
						}
					}
				}
			}
			if (!flag)
			{
				if (sortType != SortType.None)
				{
					FindTargetHelper.SortTargets(this.m_Owner, sortType, ref this.target_units);
				}
				if (findType != FindType.None)
				{
					FindTargetHelper.FilterTargetsRef(this.m_Owner, ref this.target_units, findType, param);
				}
			}
			return this.target_units;
		}

		public Units GetTargetsOfPorityAndDistance()
		{
			List<Units> list = MapManager.Instance.EnumEnemyMapUnits(this.m_Owner.TeamType, TargetTag.All).ToList<Units>();
			if (list.Count > 0)
			{
				FindTargetHelper.SortTargets(this.m_Owner, SortType.Distance, ref list);
				return list[0];
			}
			return null;
		}

		public int GetHatredValue(Units pBot)
		{
			MemoryRecord memoryRecord;
			if (pBot != null && this.m_MemoryMap.TryGetValue(pBot.unique_id, out memoryRecord))
			{
				return memoryRecord.fHatredValue;
			}
			return 0;
		}

		private void AddHatredValue(Units pBot, int value)
		{
			MemoryRecord memoryRecord;
			if (pBot != null && this.m_MemoryMap.TryGetValue(pBot.unique_id, out memoryRecord))
			{
				memoryRecord.fHatredValue += value;
			}
		}

		private void ResetHatredValue(Units pBot)
		{
			MemoryRecord memoryRecord;
			if (pBot != null && this.m_MemoryMap.TryGetValue(pBot.unique_id, out memoryRecord))
			{
				memoryRecord.fHatredValue = 0;
			}
		}

		protected virtual void InitTargetPorityValue()
		{
			this.m_MemoryPority.Clear();
			this.m_MemoryPority.Add("Home", 100);
			this.m_MemoryPority.Add("Building", 99);
			this.m_MemoryPority.Add("Hero", 98);
			this.m_MemoryPority.Add("Player", 97);
			this.m_MemoryPority.Add("Monster", 96);
		}

		public int GetTargetPorityValue(Units target)
		{
			if (target != null && this.m_MemoryPority.ContainsKey(target.tag))
			{
				return this.m_MemoryPority[target.tag];
			}
			return 0;
		}
	}
}
