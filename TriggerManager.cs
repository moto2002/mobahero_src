using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : Singleton<TriggerManager>
{
	public static Dictionary<int, VTrigger> triggers = new Dictionary<int, VTrigger>();

	public static Dictionary<int, List<GameTrigger>> game_callbacks = new Dictionary<int, List<GameTrigger>>();

	public static Dictionary<int, List<UnitTrigger>> character_callbacks = new Dictionary<int, List<UnitTrigger>>();

	public static List<TimerTrigger> timer_callbacks = new List<TimerTrigger>();

	private static Units m_trigger_unit;

	private static Units m_target_unit;

	private static UnitTrigger m_unit_trigger;

	private static GameTrigger m_game_trigger;

	private static TimerTrigger m_time_trigger;

	private static object unitLockObject = new object();

	private static object gameLockObject = new object();

	private static int _currentId;

	public static void Destroy()
	{
		TriggerManager.RemoveAllTriggers();
	}

	public static VTrigger GetTrigger(TriggerType type)
	{
		switch (type)
		{
		case TriggerType.GameTrigger:
			return TriggerManager.m_game_trigger;
		case TriggerType.UnitTrigger:
			return TriggerManager.m_unit_trigger;
		case TriggerType.TimerTrigger:
			return TriggerManager.m_time_trigger;
		default:
			return null;
		}
	}

	public static Units GetTriggerUnit()
	{
		return TriggerManager.m_trigger_unit;
	}

	public static Units GetTargetUnit()
	{
		return TriggerManager.m_target_unit;
	}

	private static int assign_trigger_id()
	{
		if (TriggerManager._currentId == 2147483647)
		{
			TriggerManager._currentId = 0;
		}
		else
		{
			TriggerManager._currentId++;
		}
		return TriggerManager._currentId;
	}

	private static VTrigger CreateTrigger(TriggerType type)
	{
		int num = TriggerManager.assign_trigger_id();
		VTrigger vTrigger = null;
		switch (type)
		{
		case TriggerType.GameTrigger:
			vTrigger = new GameTrigger(num);
			break;
		case TriggerType.UnitTrigger:
			vTrigger = new UnitTrigger(num);
			break;
		case TriggerType.TimerTrigger:
			vTrigger = new TimerTrigger(num);
			break;
		}
		if (vTrigger != null)
		{
			if (!TriggerManager.triggers.ContainsKey(num))
			{
				TriggerManager.triggers.Add(num, vTrigger);
			}
			else
			{
				TriggerManager.triggers[num] = vTrigger;
			}
		}
		return vTrigger;
	}

	private static void TriggerRemoveEvent(VTrigger trigger)
	{
		if (trigger != null)
		{
			TriggerType trigger_type = (TriggerType)trigger.trigger_type;
			int trigger_event = trigger.trigger_event;
			switch (trigger_type)
			{
			case TriggerType.GameTrigger:
				if (TriggerManager.game_callbacks.ContainsKey(trigger_event) && TriggerManager.game_callbacks[trigger_event] != null)
				{
					TriggerManager.game_callbacks[trigger_event].Remove((GameTrigger)trigger);
				}
				break;
			case TriggerType.UnitTrigger:
				if (TriggerManager.character_callbacks.ContainsKey(trigger_event) && TriggerManager.character_callbacks[trigger_event] != null)
				{
					TriggerManager.character_callbacks[trigger_event].Remove((UnitTrigger)trigger);
				}
				break;
			case TriggerType.TimerTrigger:
				TriggerManager.timer_callbacks.Remove((TimerTrigger)trigger);
				break;
			}
		}
	}

	public static void DestroyTrigger(int trigger_id)
	{
		if (TriggerManager.triggers.ContainsKey(trigger_id))
		{
			VTrigger vTrigger = TriggerManager.triggers[trigger_id];
			TriggerManager.TriggerRemoveAction(vTrigger);
			TriggerManager.TriggerRemoveCondition(vTrigger);
			TriggerManager.TriggerRemoveEvent(vTrigger);
			TriggerManager.triggers.Remove(vTrigger.trigger_id);
		}
	}

	public static void DestroyTrigger(VTrigger trigger)
	{
		if (trigger != null && TriggerManager.triggers.ContainsKey(trigger.trigger_id))
		{
			TriggerManager.TriggerRemoveAction(trigger);
			TriggerManager.TriggerRemoveCondition(trigger);
			TriggerManager.TriggerRemoveEvent(trigger);
			TriggerManager.triggers.Remove(trigger.trigger_id);
		}
	}

	public static void DestroyTriggers(TriggerType type)
	{
		Dictionary<int, VTrigger>.Enumerator enumerator = TriggerManager.triggers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, VTrigger> current = enumerator.Current;
			VTrigger value = current.Value;
			if (value != null && value.trigger_type == (int)type)
			{
				TriggerManager.DestroyTrigger(value);
			}
		}
	}

	private static void DestroyAllTriggers()
	{
		Dictionary<int, VTrigger>.Enumerator enumerator = TriggerManager.triggers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, VTrigger> current = enumerator.Current;
			VTrigger value = current.Value;
			if (value != null)
			{
				TriggerManager.DestroyTrigger(value);
			}
		}
	}

	public static void RemoveAllTriggers()
	{
		TriggerManager.triggers.Clear();
		TriggerManager.timer_callbacks.Clear();
		TriggerManager.character_callbacks.Clear();
		TriggerManager.game_callbacks.Clear();
	}

	public static bool HasTrigger(int trigger_id)
	{
		return TriggerManager.triggers.ContainsKey(trigger_id);
	}

	private static void TriggerAddCondition(VTrigger trigger, TriggerCondition tc)
	{
		if (trigger != null && tc != null && !trigger.func_conditions.Contains(tc))
		{
			trigger.func_conditions.Add(tc);
		}
	}

	private static void TriggerRemoveCondition(VTrigger trigger)
	{
		if (trigger != null)
		{
			trigger.func_conditions.Clear();
		}
	}

	private static void TriggerAddAction(VTrigger trigger, TriggerAction ta)
	{
		if (trigger != null && ta != null && !trigger.func_actions.Contains(ta))
		{
			trigger.func_actions.Add(ta);
		}
	}

	private static void TriggerRemoveAction(VTrigger trigger)
	{
		if (trigger != null)
		{
			trigger.func_actions.Clear();
		}
	}

	private static void TriggerRegisterGameEvent(GameTrigger trigger, GameEvent e)
	{
		trigger.trigger_event = (int)e;
		trigger.trigger_type = 1;
		TriggerManager.AddGameTrigger(trigger);
	}

	private static void AddGameTrigger(GameTrigger trigger)
	{
		if (!TriggerManager.game_callbacks.ContainsKey(trigger.trigger_event))
		{
			TriggerManager.game_callbacks.Add(trigger.trigger_event, new List<GameTrigger>());
		}
		TriggerManager.game_callbacks[trigger.trigger_event].Add(trigger);
	}

	private static void TriggerRegisterUnitEvent(UnitTrigger trigger, UnitEvent e, int teamtype, string u_tag)
	{
		trigger.trigger_event = (int)e;
		trigger.trigger_type = 2;
		trigger.teamtype = teamtype;
		trigger.unit_tag = u_tag;
		TriggerManager.AddUnitEvent(trigger);
	}

	private static void TriggerRegisterUnitEvent(UnitTrigger trigger, UnitEvent e, int u_id)
	{
		trigger.trigger_event = (int)e;
		trigger.trigger_type = 2;
		trigger.unit_id = u_id;
		TriggerManager.AddUnitEvent(trigger);
	}

	private static void AddUnitEvent(UnitTrigger trigger)
	{
		if (!TriggerManager.character_callbacks.ContainsKey(trigger.trigger_event))
		{
			TriggerManager.character_callbacks.Add(trigger.trigger_event, new List<UnitTrigger>());
		}
		TriggerManager.character_callbacks[trigger.trigger_event].Add(trigger);
	}

	private static void TriggerRegisterElapsedTimer(TimerTrigger trigger, float time)
	{
		if (trigger != null)
		{
			trigger.trigger_event = 20;
			trigger.trigger_type = 3;
			trigger.start_time = Singleton<VTimer>.Instance.CurTime + time;
			TriggerManager.AddTimerTrigger(trigger);
		}
	}

	private static void TriggerRegisterPeriodTimer(TimerTrigger trigger, float start_time, float interval, float duration)
	{
		if (trigger != null)
		{
			trigger.trigger_event = 21;
			trigger.trigger_type = 3;
			trigger.start_time = Singleton<VTimer>.Instance.CurTime + start_time;
			trigger.interval_time = interval;
			trigger.duration_time = duration;
			TriggerManager.AddTimerTrigger(trigger);
		}
	}

	private static void AddTimerTrigger(TimerTrigger trigger)
	{
		if (trigger != null)
		{
			TriggerManager.timer_callbacks.Add(trigger);
		}
	}

	public static VTrigger CreateGameEventTrigger(GameEvent e, TriggerCondition condition, TriggerAction action)
	{
		GameTrigger gameTrigger = (GameTrigger)TriggerManager.CreateTrigger(TriggerType.GameTrigger);
		TriggerManager.TriggerRegisterGameEvent(gameTrigger, e);
		TriggerManager.TriggerAddCondition(gameTrigger, condition);
		TriggerManager.TriggerAddAction(gameTrigger, action);
		return gameTrigger;
	}

	public static VTrigger CreateUnitEventTrigger(UnitEvent e, TriggerCondition condition, TriggerAction action, int teamtype, string u_tag)
	{
		UnitTrigger unitTrigger = (UnitTrigger)TriggerManager.CreateTrigger(TriggerType.UnitTrigger);
		TriggerManager.TriggerRegisterUnitEvent(unitTrigger, e, teamtype, u_tag);
		TriggerManager.TriggerAddCondition(unitTrigger, condition);
		TriggerManager.TriggerAddAction(unitTrigger, action);
		return unitTrigger;
	}

	public static VTrigger CreateUnitEventTrigger(UnitEvent e, TriggerCondition condition, TriggerAction action, int u_id)
	{
		UnitTrigger unitTrigger = (UnitTrigger)TriggerManager.CreateTrigger(TriggerType.UnitTrigger);
		TriggerManager.TriggerRegisterUnitEvent(unitTrigger, e, u_id);
		TriggerManager.TriggerAddCondition(unitTrigger, condition);
		TriggerManager.TriggerAddAction(unitTrigger, action);
		return unitTrigger;
	}

	public static VTrigger CreateElapsedTimerTrigger(float time, TriggerCondition condition, TriggerAction action)
	{
		TimerTrigger timerTrigger = (TimerTrigger)TriggerManager.CreateTrigger(TriggerType.TimerTrigger);
		TriggerManager.TriggerRegisterElapsedTimer(timerTrigger, time);
		TriggerManager.TriggerAddCondition(timerTrigger, condition);
		TriggerManager.TriggerAddAction(timerTrigger, action);
		return timerTrigger;
	}

	public static VTrigger CreatePeriodTimerTrigger(float time, float interval, float duration, TriggerCondition condition, TriggerAction action)
	{
		TimerTrigger timerTrigger = (TimerTrigger)TriggerManager.CreateTrigger(TriggerType.TimerTrigger);
		TriggerManager.TriggerRegisterPeriodTimer(timerTrigger, time, interval, duration);
		TriggerManager.TriggerAddCondition(timerTrigger, condition);
		TriggerManager.TriggerAddAction(timerTrigger, action);
		return timerTrigger;
	}

	public void SendUnitStateEvent(UnitEvent e, Units tu, Units tt = null, BuffVo buff = null)
	{
		object obj = TriggerManager.unitLockObject;
		lock (obj)
		{
			if (TriggerManager.character_callbacks.ContainsKey((int)e))
			{
				List<UnitTrigger> list = TriggerManager.character_callbacks[(int)e];
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						UnitTrigger unitTrigger = list[i];
						if (unitTrigger != null && tu != null)
						{
							TriggerManager.m_unit_trigger = unitTrigger;
							TriggerManager.m_trigger_unit = tu;
							TriggerManager.m_target_unit = tt;
							if (unitTrigger.unit_id == tu.unique_id)
							{
								this.doTriggerAction(unitTrigger);
							}
							else if (unitTrigger.unit_id == 0)
							{
								if (unitTrigger.unit_tag != null && unitTrigger.unit_tag.Equals(tu.tag) && unitTrigger.teamtype == tu.teamType)
								{
									this.doTriggerAction(unitTrigger);
								}
								else if (unitTrigger.unit_tag == null && unitTrigger.teamtype == tu.teamType)
								{
									this.doTriggerAction(unitTrigger);
								}
								else if (unitTrigger.unit_tag != null && unitTrigger.unit_tag.Equals(tu.tag) && unitTrigger.teamtype == -1)
								{
									this.doTriggerAction(unitTrigger);
								}
							}
						}
					}
				}
			}
		}
	}

	public void SendUnitSkillStateEvent(UnitEvent e, Units tu, Skill skill)
	{
		tu.SetSkillContext(skill);
		this.SendUnitStateEvent(e, tu, null, null);
		tu.ClearSkillContext();
	}

	public void SendUnitSkillPointerEvent(UnitEvent e, Units tu, Skill skill, Vector3 targetPos)
	{
		tu.SetSkillContext(skill);
		tu.SetPosContext(targetPos);
		this.SendUnitStateEvent(e, tu, null, null);
		tu.ClearSkillContext();
	}

	public void SendGameStateEvent(GameEvent e)
	{
		object obj = TriggerManager.gameLockObject;
		lock (obj)
		{
			List<GameTrigger> list;
			if (TriggerManager.game_callbacks.TryGetValue((int)e, out list) && list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					GameTrigger gameTrigger = list[i];
					if (gameTrigger != null)
					{
						TriggerManager.m_game_trigger = gameTrigger;
						this.doTriggerAction(gameTrigger);
					}
				}
			}
		}
	}

	public void SendTimerEvent(float elapsed_time)
	{
		for (int i = 0; i < TriggerManager.timer_callbacks.Count; i++)
		{
			TimerTrigger timerTrigger = TriggerManager.timer_callbacks[i];
			int trigger_event = timerTrigger.trigger_event;
			if (timerTrigger != null && trigger_event != 21)
			{
				if (elapsed_time >= timerTrigger.start_time)
				{
					this.doTriggerAction(timerTrigger);
					if (elapsed_time >= timerTrigger.duration_time)
					{
						TriggerManager.DestroyTrigger(timerTrigger);
					}
				}
			}
			else if (elapsed_time >= timerTrigger.start_time)
			{
				if (!timerTrigger.is_start)
				{
					timerTrigger.is_start = true;
					this.doTriggerAction(timerTrigger);
				}
				else
				{
					float num = elapsed_time - timerTrigger.start_time;
					if (timerTrigger.interval_time == 0f || num % timerTrigger.interval_time <= Singleton<VTimer>.Instance.time_interval)
					{
						this.doTriggerAction(timerTrigger);
					}
				}
				if (timerTrigger.duration_time != -1f && (timerTrigger.interval_time <= 0f || timerTrigger.duration_time == 0f || elapsed_time >= timerTrigger.duration_time))
				{
					TriggerManager.DestroyTrigger(timerTrigger);
				}
			}
		}
	}

	private void doTriggerAction(VTrigger t)
	{
		if (t != null && t.CheckConditions())
		{
			t.DoActions();
		}
	}

	private void DumpTriggers()
	{
		Dictionary<int, List<UnitTrigger>>.Enumerator enumerator = TriggerManager.character_callbacks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, List<UnitTrigger>> current = enumerator.Current;
			int key = current.Key;
			KeyValuePair<int, List<UnitTrigger>> current2 = enumerator.Current;
			List<UnitTrigger> value = current2.Value;
		}
		Dictionary<int, List<GameTrigger>>.Enumerator enumerator2 = TriggerManager.game_callbacks.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<int, List<GameTrigger>> current3 = enumerator2.Current;
			int key2 = current3.Key;
			KeyValuePair<int, List<GameTrigger>> current4 = enumerator2.Current;
			List<GameTrigger> value2 = current4.Value;
		}
	}
}
