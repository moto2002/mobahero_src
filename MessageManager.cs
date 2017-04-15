using System;
using System.Collections.Generic;

public class MessageManager
{
	private static Dictionary<Type, List<Action<GameMessage>>> _lses = new Dictionary<Type, List<Action<GameMessage>>>();

	public static void addMsgLs(Type type, Action<GameMessage> ls)
	{
		if (!MessageManager._lses.ContainsKey(type))
		{
			MessageManager._lses.Add(type, new List<Action<GameMessage>>());
		}
		MessageManager.getLs(type).Add(ls);
	}

	private static List<Action<GameMessage>> getLs(Type type)
	{
		List<Action<GameMessage>> result;
		if (MessageManager._lses.ContainsKey(type))
		{
			result = MessageManager._lses[type];
		}
		else
		{
			List<Action<GameMessage>> list = new List<Action<GameMessage>>();
			MessageManager._lses.Add(type, list);
			result = list;
		}
		return result;
	}

	public static void dispatch(GameMessage msg)
	{
		List<Action<GameMessage>> ls = MessageManager.getLs(msg.GetType());
		foreach (Action<GameMessage> current in ls)
		{
			current(msg);
		}
	}

	public static void dropLs(Action<GameMessage> ls)
	{
		foreach (Type current in MessageManager._lses.Keys)
		{
			if (MessageManager._lses[current].Contains(ls))
			{
				MessageManager._lses[current].Remove(ls);
				break;
			}
		}
	}
}
