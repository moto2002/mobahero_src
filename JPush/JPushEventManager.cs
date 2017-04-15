using Com.Game.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace JPush
{
	public class JPushEventManager : MonoBehaviour
	{
		public static JPushEventManager instance;

		public bool allowSingleton = true;

		public bool allowWarningOutputs = true;

		public bool allowDebugOutputs = true;

		private static bool _created;

		private Hashtable _listeners = new Hashtable();

		static JPushEventManager()
		{
			JPushEventManager.instance = new JPushEventManager();
		}

		public void Awake()
		{
			if (!JPushEventManager._created && this.allowSingleton)
			{
				UnityEngine.Object.DontDestroyOnLoad(this);
				JPushEventManager.instance = this;
				JPushEventManager._created = true;
				this.Setup();
			}
			else if (this.allowSingleton)
			{
				if (JPushEventManager.instance.allowWarningOutputs)
				{
					Debug.LogWarning("Only a single instance of " + base.name + " should exists!");
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				JPushEventManager.instance = this;
				this.Setup();
			}
		}

		public void OnApplicationQuit()
		{
			this._listeners.Clear();
		}

		public bool addEventListener(string eventType, GameObject listener, string function)
		{
			if (listener == null || eventType == null)
			{
				if (this.allowWarningOutputs)
				{
					Debug.LogWarning("Event Manager: AddListener failed due to no listener or event name specified.");
				}
				return false;
			}
			this.recordEvent(eventType);
			return this.recordListener(eventType, listener, function);
		}

		public bool removeEventListener(string eventType, GameObject listener)
		{
			if (!this.checkForEvent(eventType))
			{
				return false;
			}
			ArrayList arrayList = this._listeners[eventType] as ArrayList;
			foreach (EventListener eventListener in arrayList)
			{
				if (eventListener.name == listener.GetInstanceID().ToString())
				{
					arrayList.Remove(eventListener);
					return true;
				}
			}
			return false;
		}

		public void removeAllEventListeners(GameObject listener)
		{
			this._listeners.Clear();
		}

		public bool dispatchEvent(CustomEvent evt)
		{
			string type = evt.type;
			if (!this.checkForEvent(type))
			{
				if (this.allowWarningOutputs)
				{
					Debug.LogWarning("Event Manager: Event \"" + type + "\" triggered has no listeners!");
				}
				return false;
			}
			ArrayList arrayList = this._listeners[type] as ArrayList;
			if (this.allowDebugOutputs)
			{
				ClientLogger.Info(string.Concat(new object[]
				{
					"Event Manager: Event ",
					type,
					" dispatched to ",
					arrayList.Count,
					(arrayList.Count != 1) ? " listeners." : " listener."
				}));
			}
			foreach (EventListener eventListener in arrayList)
			{
				if (eventListener.listener && eventListener.listener.activeSelf)
				{
					eventListener.listener.SendMessage(eventListener.function, evt, SendMessageOptions.DontRequireReceiver);
				}
			}
			return false;
		}

		private void Setup()
		{
		}

		private bool checkForEvent(string eventType)
		{
			return this._listeners.ContainsKey(eventType);
		}

		private bool recordEvent(string eventType)
		{
			if (!this.checkForEvent(eventType))
			{
				this._listeners.Add(eventType, new ArrayList());
			}
			return true;
		}

		private bool deleteEvent(string eventType)
		{
			if (!this.checkForEvent(eventType))
			{
				return false;
			}
			this._listeners.Remove(eventType);
			return true;
		}

		private bool checkForListener(string eventType, GameObject listener)
		{
			if (!this.checkForEvent(eventType))
			{
				this.recordEvent(eventType);
			}
			ArrayList arrayList = this._listeners[eventType] as ArrayList;
			foreach (EventListener eventListener in arrayList)
			{
				if (eventListener.name == listener.GetInstanceID().ToString())
				{
					return true;
				}
			}
			return false;
		}

		private bool recordListener(string eventType, GameObject listener, string function)
		{
			if (!this.checkForListener(eventType, listener))
			{
				ArrayList arrayList = this._listeners[eventType] as ArrayList;
				arrayList.Add(new EventListener
				{
					name = listener.GetInstanceID().ToString(),
					listener = listener,
					function = function
				});
				return true;
			}
			if (this.allowWarningOutputs)
			{
				Debug.LogWarning("Event Manager: Listener: " + listener.name + " is already in list for event: " + eventType);
			}
			return false;
		}
	}
}
