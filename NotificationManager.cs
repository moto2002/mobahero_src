using System;
using System.Collections.Generic;

internal static class NotificationManager
{
	public class BroadcastException : Exception
	{
		public BroadcastException(string msg) : base(msg)
		{
		}
	}

	public class ListenerException : Exception
	{
		public ListenerException(string msg) : base(msg)
		{
		}
	}

	public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	public static List<string> permanentMessages = new List<string>();

	public static void MarkAsPermanent(string eventType)
	{
		NotificationManager.permanentMessages.Add(eventType);
	}

	public static void Cleanup()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Delegate> current in NotificationManager.eventTable)
		{
			bool flag = false;
			foreach (string current2 in NotificationManager.permanentMessages)
			{
				if (current.Key == current2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(current.Key);
			}
		}
		foreach (string current3 in list)
		{
			NotificationManager.eventTable.Remove(current3);
		}
	}

	public static void PrintEventTable()
	{
		foreach (KeyValuePair<string, Delegate> current in NotificationManager.eventTable)
		{
		}
	}

	public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
	{
		if (!NotificationManager.eventTable.ContainsKey(eventType))
		{
			NotificationManager.eventTable.Add(eventType, null);
		}
		Delegate @delegate = NotificationManager.eventTable[eventType];
		if (@delegate != null && @delegate.GetType() != listenerBeingAdded.GetType())
		{
			throw new NotificationManager.ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, @delegate.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}

	public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
	{
		if (!NotificationManager.eventTable.ContainsKey(eventType))
		{
			throw new NotificationManager.ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
		}
		Delegate @delegate = NotificationManager.eventTable[eventType];
		if (@delegate == null)
		{
			throw new NotificationManager.ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
		}
		if (@delegate.GetType() != listenerBeingRemoved.GetType())
		{
			throw new NotificationManager.ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, @delegate.GetType().Name, listenerBeingRemoved.GetType().Name));
		}
	}

	public static void OnListenerRemoved(string eventType)
	{
		if (NotificationManager.eventTable[eventType] == null)
		{
			NotificationManager.eventTable.Remove(eventType);
		}
	}

	public static void OnBroadcasting(string eventType)
	{
	}

	public static NotificationManager.BroadcastException CreateBroadcastSignatureException(string eventType)
	{
		return new NotificationManager.BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}

	public static void AddListener(string eventType, Callback handler)
	{
		NotificationManager.OnListenerAdding(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback)Delegate.Combine((Callback)NotificationManager.eventTable[eventType], handler);
	}

	public static void AddListener<T>(string eventType, Callback<T> handler)
	{
		NotificationManager.OnListenerAdding(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T>)Delegate.Combine((Callback<T>)NotificationManager.eventTable[eventType], handler);
	}

	public static void AddListener<T, U>(string eventType, Callback<T, U> handler)
	{
		NotificationManager.OnListenerAdding(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T, U>)Delegate.Combine((Callback<T, U>)NotificationManager.eventTable[eventType], handler);
	}

	public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		NotificationManager.OnListenerAdding(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T, U, V>)Delegate.Combine((Callback<T, U, V>)NotificationManager.eventTable[eventType], handler);
	}

	public static void RemoveListener(string eventType, Callback handler)
	{
		NotificationManager.OnListenerRemoving(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback)Delegate.Remove((Callback)NotificationManager.eventTable[eventType], handler);
		NotificationManager.OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T>(string eventType, Callback<T> handler)
	{
		NotificationManager.OnListenerRemoving(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T>)Delegate.Remove((Callback<T>)NotificationManager.eventTable[eventType], handler);
		NotificationManager.OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
	{
		NotificationManager.OnListenerRemoving(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T, U>)Delegate.Remove((Callback<T, U>)NotificationManager.eventTable[eventType], handler);
		NotificationManager.OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		NotificationManager.OnListenerRemoving(eventType, handler);
		NotificationManager.eventTable[eventType] = (Callback<T, U, V>)Delegate.Remove((Callback<T, U, V>)NotificationManager.eventTable[eventType], handler);
		NotificationManager.OnListenerRemoved(eventType);
	}

	public static void Broadcast(string eventType)
	{
		NotificationManager.OnBroadcasting(eventType);
		Delegate @delegate;
		if (NotificationManager.eventTable.TryGetValue(eventType, out @delegate))
		{
			Callback callback = @delegate as Callback;
			if (callback == null)
			{
				throw NotificationManager.CreateBroadcastSignatureException(eventType);
			}
			callback();
		}
	}

	public static void Broadcast<T>(string eventType, T arg1)
	{
		NotificationManager.OnBroadcasting(eventType);
		Delegate @delegate;
		if (NotificationManager.eventTable.TryGetValue(eventType, out @delegate))
		{
			Callback<T> callback = @delegate as Callback<T>;
			if (callback == null)
			{
				throw NotificationManager.CreateBroadcastSignatureException(eventType);
			}
			callback(arg1);
		}
	}

	public static void Broadcast<T, U>(string eventType, T arg1, U arg2)
	{
		NotificationManager.OnBroadcasting(eventType);
		Delegate @delegate;
		if (NotificationManager.eventTable.TryGetValue(eventType, out @delegate))
		{
			Callback<T, U> callback = @delegate as Callback<T, U>;
			if (callback == null)
			{
				throw NotificationManager.CreateBroadcastSignatureException(eventType);
			}
			callback(arg1, arg2);
		}
	}

	public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
	{
		NotificationManager.OnBroadcasting(eventType);
		Delegate @delegate;
		if (NotificationManager.eventTable.TryGetValue(eventType, out @delegate))
		{
			Callback<T, U, V> callback = @delegate as Callback<T, U, V>;
			if (callback == null)
			{
				throw NotificationManager.CreateBroadcastSignatureException(eventType);
			}
			callback(arg1, arg2, arg3);
		}
	}
}
