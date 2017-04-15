using System;
using System.Collections.Generic;
using UnityEngine;

public class UniWebViewEventProcessor : MonoBehaviour
{
	private object _queueLock = new object();

	private List<Action> _queuedEvents = new List<Action>();

	private List<Action> _executingEvents = new List<Action>();

	private static UniWebViewEventProcessor _instance;

	public static UniWebViewEventProcessor instance
	{
		get
		{
			if (!UniWebViewEventProcessor._instance)
			{
				UniWebViewEventProcessor._instance = (UnityEngine.Object.FindObjectOfType(typeof(UniWebViewEventProcessor)) as UniWebViewEventProcessor);
				if (!UniWebViewEventProcessor._instance)
				{
					GameObject gameObject = new GameObject("UniWebViewEventProcessor");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					UniWebViewEventProcessor._instance = gameObject.AddComponent<UniWebViewEventProcessor>();
				}
			}
			return UniWebViewEventProcessor._instance;
		}
	}

	public void QueueEvent(Action action)
	{
		object queueLock = this._queueLock;
		lock (queueLock)
		{
			this._queuedEvents.Add(action);
		}
	}

	private void Update()
	{
		this.MoveQueuedEventsToExecuting();
		while (this._executingEvents.Count > 0)
		{
			Action action = this._executingEvents[0];
			this._executingEvents.RemoveAt(0);
			action();
		}
	}

	private void MoveQueuedEventsToExecuting()
	{
		object queueLock = this._queueLock;
		lock (queueLock)
		{
			while (this._queuedEvents.Count > 0)
			{
				Action item = this._queuedEvents[0];
				this._executingEvents.Add(item);
				this._queuedEvents.RemoveAt(0);
			}
		}
	}
}
