using PathologicalGames;
using System;
using UnityEngine;

namespace MobaHeros
{
	public class ResourceHandle
	{
		private readonly ResourceSpawner _spawner;

		private readonly Transform _trans;

		public bool IsVaid
		{
			get;
			private set;
		}

		public string ResId
		{
			get;
			private set;
		}

		public bool UsePool
		{
			get;
			private set;
		}

		public DateTime CreatedTime
		{
			get;
			private set;
		}

		public SpawnPool Pool
		{
			get;
			private set;
		}

		public Transform Raw
		{
			get
			{
				return this.IsVaid ? this._trans : null;
			}
		}

		internal ResourceHandle(string resId, Transform trans, ResourceSpawner spawner, SpawnPool pool, bool usePool, bool resIdIsPath = false)
		{
			this.ResId = resId;
			this._trans = trans;
			this._spawner = spawner;
			this.UsePool = usePool;
			this.IsVaid = true;
			this.CreatedTime = DateTime.Now;
			this.Pool = pool;
		}

		public void Release()
		{
			if (!this.IsVaid)
			{
				return;
			}
			try
			{
				this._spawner.ReleaseResource(this);
			}
			finally
			{
				this.IsVaid = false;
			}
		}

		public void DelayRelease(float t)
		{
			this._spawner.ReleaseResource(this, t);
		}

		public static void SafeRelease(ref ResourceHandle handle)
		{
			if (handle != null)
			{
				handle.Release();
				handle = null;
			}
		}
	}
}
