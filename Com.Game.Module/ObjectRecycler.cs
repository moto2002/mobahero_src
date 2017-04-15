using Com.Game.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ObjectRecycler<T> where T : Component
	{
		public class Entry
		{
			public T value;

			public bool used;

			public string name;
		}

		private readonly Func<T> _getMethod;

		private readonly Action<T> _delMethod;

		private readonly List<ObjectRecycler<T>.Entry> _objectPool = new List<ObjectRecycler<T>.Entry>();

		private Transform _recycleParent;

		public ObjectRecycler(Func<T> getMethod, Action<T> delMethod = null)
		{
			this._getMethod = getMethod;
			this._delMethod = delMethod;
			GameObject gameObject = GameObject.Find("GlobalObject/recycler");
			if (!gameObject)
			{
				ClientLogger.Warn("cannot found");
			}
			else
			{
				this._recycleParent = gameObject.transform;
			}
		}

		public T Create(Transform parent = null)
		{
			ObjectRecycler<T>.Entry entry = this._objectPool.Find((ObjectRecycler<T>.Entry x) => !x.used);
			if (entry != null)
			{
				if (parent)
				{
					entry.value.transform.parent = parent;
				}
				entry.value.transform.localScale = Vector3.one;
				PerfTools.SetVisible(entry.value.transform, true);
				entry.used = true;
				return entry.value;
			}
			T t = (T)((object)null);
			if (this._getMethod != null)
			{
				t = this._getMethod();
				if (t)
				{
					t.transform.parent = parent;
					PerfTools.SetVisible(t.transform, true);
					t.transform.localScale = Vector3.one;
					ObjectRecycler<T>.Entry item = new ObjectRecycler<T>.Entry
					{
						name = t.name,
						value = t,
						used = true
					};
					this._objectPool.Add(item);
				}
			}
			return t;
		}

		public void Release(T comp)
		{
			if (!comp)
			{
				return;
			}
			ObjectRecycler<T>.Entry entry = this._objectPool.Find((ObjectRecycler<T>.Entry x) => comp == x.value);
			if (entry != null)
			{
				PerfTools.SetVisible(comp.transform, false);
				comp.transform.SetParent(this._recycleParent);
				entry.used = false;
			}
			else
			{
				ClientLogger.Error("release " + comp.name);
			}
		}

		public void Preload(int num)
		{
			List<T> list = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(this.Create(null));
			}
			for (int j = 0; j < num; j++)
			{
				this.Release(list[j]);
			}
			list.Clear();
		}

		public void DestroyPool()
		{
			if (this._delMethod != null)
			{
				this._objectPool.ForEach(delegate(ObjectRecycler<T>.Entry x)
				{
					this._delMethod(x.value);
				});
			}
			this._objectPool.Clear();
		}

		public static void DumpPanel(GameObject go)
		{
			UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>(true);
			if (componentsInChildren.Length > 0)
			{
				UnityEngine.Debug.Log(componentsInChildren[0].name, componentsInChildren[0]);
				if (componentsInChildren[0].panel)
				{
					UnityEngine.Debug.Log(componentsInChildren[0].panel, componentsInChildren[0].panel);
				}
				else
				{
					UnityEngine.Debug.Log("no panel");
				}
			}
		}

		[Conditional("DEBUG_MODE")]
		public static void Log(string msg, UnityEngine.Object context = null)
		{
			if (context)
			{
				UnityEngine.Debug.Log("====" + msg, context);
			}
			else
			{
				UnityEngine.Debug.Log("====" + msg);
			}
		}
	}
}
