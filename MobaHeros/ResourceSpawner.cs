using Com.Game.Utils;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros
{
	internal sealed class ResourceSpawner
	{
		private readonly PoolRoot _poolRoot;

		private readonly List<ResourceHandle> _handles = new List<ResourceHandle>(60);

		private bool _shutdown;

		private CoroutineManager _coroutineManager = new CoroutineManager();

		public ResourceSpawner(PoolRoot poolRoot)
		{
			this._poolRoot = poolRoot;
		}

		public void ReleaseResource(ResourceHandle handle)
		{
			if (handle == null || !handle.IsVaid)
			{
				return;
			}
			SpawnPool pool = handle.Pool;
			ClientLogger.AssertNotNull(pool, null);
			if (!this._shutdown)
			{
				pool.Despawn(handle.Raw, pool.transform);
			}
			this._handles.RemoveAll((ResourceHandle x) => x == handle);
		}

		public void ReleaseResource(ResourceHandle handle, float t)
		{
			this._coroutineManager.StartCoroutine(this.DelayRelease(handle, t), true);
		}

		[DebuggerHidden]
		private IEnumerator DelayRelease(ResourceHandle handle, float t)
		{
			ResourceSpawner.<DelayRelease>c__Iterator1B6 <DelayRelease>c__Iterator1B = new ResourceSpawner.<DelayRelease>c__Iterator1B6();
			<DelayRelease>c__Iterator1B.t = t;
			<DelayRelease>c__Iterator1B.handle = handle;
			<DelayRelease>c__Iterator1B.<$>t = t;
			<DelayRelease>c__Iterator1B.<$>handle = handle;
			<DelayRelease>c__Iterator1B.<>f__this = this;
			return <DelayRelease>c__Iterator1B;
		}

		public ResourceHandle SpawnResource(string resId, Action<GameObject> onFirstLoad = null, int skin = 0)
		{
			GameObject gameObject = ResourceManager.Load<GameObject>(resId, true, true, onFirstLoad, skin, false);
			if (gameObject == null)
			{
				gameObject = ResourceManager.Load<GameObject>(resId, true, true, onFirstLoad, 0, false);
				if (gameObject == null)
				{
					ClientLogger.Error("ResourceManager.Load failed for resID=" + resId);
					return null;
				}
			}
			SpawnPool poolByPrefab = this._poolRoot.GetPoolByPrefab(gameObject);
			Transform transform = poolByPrefab.Spawn(gameObject.transform, new Vector3(0f, -999999f, 0f), Quaternion.identity);
			if (transform)
			{
				ResourceHandle resourceHandle = new ResourceHandle(resId, transform, this, poolByPrefab, true, false);
				this._handles.Add(resourceHandle);
				return resourceHandle;
			}
			return null;
		}

		public ResourceHandle SpawnResource(string resId, Vector3 position, Quaternion rotation, int skin = 0)
		{
			GameObject gameObject = ResourceManager.Load<GameObject>(resId, true, true, null, skin, false);
			if (gameObject == null)
			{
				gameObject = ResourceManager.Load<GameObject>(resId, true, true, null, 0, false);
				if (gameObject == null)
				{
					ClientLogger.Error("SpawnResource failed for resID=" + resId);
					return null;
				}
			}
			SpawnPool poolByPrefab = this._poolRoot.GetPoolByPrefab(gameObject);
			Transform transform = poolByPrefab.Spawn(gameObject.transform, position, rotation);
			if (transform)
			{
				ResourceHandle resourceHandle = new ResourceHandle(resId, transform, this, poolByPrefab, true, false);
				this._handles.Add(resourceHandle);
				return resourceHandle;
			}
			return null;
		}

		public ResourceHandle SpawnResourceFromPath(string path, Action<GameObject> onFirstLoad = null)
		{
			GameObject gameObject = ResourceManager.LoadPath<GameObject>(path, onFirstLoad, 0);
			if (gameObject == null)
			{
				ClientLogger.Error("SpawnResourceFromPath failed for path=" + path);
				return null;
			}
			SpawnPool poolByPrefab = this._poolRoot.GetPoolByPrefab(gameObject);
			Transform transform = poolByPrefab.Spawn(gameObject.transform);
			if (transform)
			{
				ResourceHandle resourceHandle = new ResourceHandle(path, transform, this, poolByPrefab, true, true);
				this._handles.Add(resourceHandle);
				return resourceHandle;
			}
			return null;
		}

		public void Shutdown()
		{
			this._shutdown = true;
			ResourceHandle[] array = this._handles.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Release();
			}
			this._handles.Clear();
		}

		public void Dump()
		{
			foreach (ResourceHandle current in this._handles)
			{
				UnityEngine.Debug.Log(string.Format("===> {0}: {1}", current.Raw.name, (DateTime.Now - current.CreatedTime).TotalSeconds), current.Raw);
			}
		}
	}
}
