using System;
using UnityEngine;

namespace MobaHeros
{
	public class ResourceHandleWrapper<T> where T : Component
	{
		private ResourceHandle _handle;

		public T Component
		{
			get;
			private set;
		}

		public ResourceHandleWrapper(ResourceHandle handle)
		{
			this._handle = handle;
			this.Component = handle.Raw.GetComponent<T>();
		}

		public void Release()
		{
			this._handle.Release();
			this._handle = null;
		}
	}
}
