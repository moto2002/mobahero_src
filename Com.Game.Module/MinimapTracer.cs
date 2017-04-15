using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Com.Game.Module
{
	internal class MinimapTracer
	{
		private static readonly List<MinimapTracer> _entries = new List<MinimapTracer>();

		private GameObject _go;

		public static event Action<MinimapTracer> OnAdd
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				MinimapTracer.OnAdd = (Action<MinimapTracer>)Delegate.Combine(MinimapTracer.OnAdd, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				MinimapTracer.OnAdd = (Action<MinimapTracer>)Delegate.Remove(MinimapTracer.OnAdd, value);
			}
		}

		public static event Action<MinimapTracer> OnRemove
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				MinimapTracer.OnRemove = (Action<MinimapTracer>)Delegate.Combine(MinimapTracer.OnRemove, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				MinimapTracer.OnRemove = (Action<MinimapTracer>)Delegate.Remove(MinimapTracer.OnRemove, value);
			}
		}

		public Vector3 position
		{
			get
			{
				return this._go.transform.position;
			}
		}

		public GameObject texture
		{
			get;
			private set;
		}

		private MinimapTracer(GameObject go, GameObject texture)
		{
			this._go = go;
			this.texture = texture;
		}

		public static MinimapTracer CreateTracer(GameObject go, GameObject textureGo)
		{
			return new MinimapTracer(go, textureGo);
		}

		public static void Add(MinimapTracer tracer)
		{
			if (tracer == null)
			{
				return;
			}
			MinimapTracer._entries.Add(tracer);
			if (MinimapTracer.OnAdd != null)
			{
				MinimapTracer.OnAdd(tracer);
			}
		}

		public static void Remove(MinimapTracer tracer)
		{
			if (tracer == null)
			{
				return;
			}
			MinimapTracer._entries.Remove(tracer);
			if (MinimapTracer.OnRemove != null)
			{
				MinimapTracer.OnRemove(tracer);
			}
		}

		public static void Clear()
		{
			foreach (MinimapTracer current in MinimapTracer._entries)
			{
				if (MinimapTracer.OnRemove != null)
				{
					MinimapTracer.OnRemove(current);
				}
			}
			MinimapTracer._entries.Clear();
		}

		public IEnumerator<MinimapTracer> GetEnumerator()
		{
			return MinimapTracer._entries.GetEnumerator();
		}
	}
}
