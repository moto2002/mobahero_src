using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace PathologicalGames
{
	[Serializable]
	public class PrefabPool
	{
		public class DespawnedItem
		{
			public Transform transform;

			public DateTime despawnedTime;

			public DespawnedItem(Transform trans)
			{
				this.transform = trans;
				this.despawnedTime = DateTime.Now;
			}
		}

		public float shrinkFactor = 1f;

		public Transform prefab;

		internal GameObject prefabGO;

		public int preloadAmount = 1;

		public bool preloadTime;

		public int preloadFrames = 2;

		public float preloadDelay;

		public bool limitInstances;

		public int limitAmount = 100;

		public bool limitFIFO;

		public bool cullDespawned;

		public int cullAbove = 50;

		public int cullDelay = 60;

		public int cullMaxPerPass = 5;

		public bool _logMessages;

		private bool forceLoggingSilent;

		public SpawnPool spawnPool;

		private bool cullingActive;

		internal List<Transform> _spawned = new List<Transform>();

		internal List<PrefabPool.DespawnedItem> _despawned = new List<PrefabPool.DespawnedItem>();

		private bool _preloaded;

		private DateTime _lastShrink;

		public bool logMessages
		{
			get
			{
				if (this.forceLoggingSilent)
				{
					return false;
				}
				if (this.spawnPool.logMessages)
				{
					return this.spawnPool.logMessages;
				}
				return this._logMessages;
			}
		}

		public List<Transform> spawned
		{
			get
			{
				return new List<Transform>(this._spawned);
			}
		}

		public List<Transform> despawned
		{
			get
			{
				return (from x in this._despawned
				select x.transform).ToList<Transform>();
			}
		}

		public int spawnedCount
		{
			get
			{
				return this._spawned.Count;
			}
		}

		public int despawnedCount
		{
			get
			{
				return this._despawned.Count;
			}
		}

		public int totalCount
		{
			get
			{
				int num = 0;
				num += this._spawned.Count;
				return num + this._despawned.Count;
			}
		}

		internal bool preloaded
		{
			get
			{
				return this._preloaded;
			}
			private set
			{
				this._preloaded = value;
			}
		}

		public PrefabPool(Transform prefab)
		{
			this.prefab = prefab;
			this.prefabGO = prefab.gameObject;
		}

		public PrefabPool()
		{
		}

		internal void inspectorInstanceConstructor()
		{
			this.prefabGO = this.prefab.gameObject;
			this._spawned = new List<Transform>();
			this._despawned = new List<PrefabPool.DespawnedItem>();
		}

		internal void SelfDestruct()
		{
			this.prefab = null;
			this.prefabGO = null;
			this.spawnPool = null;
			foreach (PrefabPool.DespawnedItem current in this._despawned)
			{
				if (current != null)
				{
					UnityEngine.Object.Destroy(current.transform.gameObject);
				}
			}
			foreach (Transform current2 in this._spawned)
			{
				if (current2 != null)
				{
					UnityEngine.Object.Destroy(current2.gameObject);
				}
			}
			this._spawned.Clear();
			this._despawned.Clear();
		}

		internal bool DespawnInstance(Transform xform)
		{
			return this.DespawnInstance(xform, true);
		}

		internal bool DespawnInstance(Transform xform, bool sendEventMessage)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'", this.spawnPool.poolName, this.prefab.name, xform.name));
			}
			this._spawned.Remove(xform);
			this._despawned.Add(new PrefabPool.DespawnedItem(xform));
			if (sendEventMessage)
			{
				xform.gameObject.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
			}
			if (xform == null)
			{
				return false;
			}
			PoolManagerUtils.SetActive(xform.gameObject, false);
			if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
			{
				this.cullingActive = true;
				this.spawnPool.StartCoroutine(this.CullDespawned());
			}
			return true;
		}

		[DebuggerHidden]
		internal IEnumerator CullDespawned()
		{
			PrefabPool.<CullDespawned>c__IteratorD <CullDespawned>c__IteratorD = new PrefabPool.<CullDespawned>c__IteratorD();
			<CullDespawned>c__IteratorD.<>f__this = this;
			return <CullDespawned>c__IteratorD;
		}

		internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
		{
			if (this.limitInstances && this.limitFIFO && this._spawned.Count >= this.limitAmount)
			{
				Transform transform = this._spawned[0];
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! FIFO=True. Calling despawning for {2}...", this.spawnPool.poolName, this.prefab.name, transform));
				}
				this.DespawnInstance(transform);
				this.spawnPool._spawned.Remove(transform);
			}
			Transform transform2;
			if (this._despawned.Count == 0)
			{
				transform2 = this.SpawnNew(pos, rot);
			}
			else
			{
				transform2 = this._despawned[this._despawned.Count - 1].transform;
				this._despawned.RemoveAt(this._despawned.Count - 1);
				this._spawned.Add(transform2);
				if (transform2 == null)
				{
					return null;
				}
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.", this.spawnPool.poolName, this.prefab.name, transform2.name));
				}
				transform2.position = pos;
				transform2.rotation = rot;
				PoolManagerUtils.SetActive(transform2.gameObject, true);
			}
			if (transform2 != null)
			{
				transform2.gameObject.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
			}
			return transform2;
		}

		public Transform SpawnNew()
		{
			return this.SpawnNew(Vector3.zero, Quaternion.identity);
		}

		public Transform SpawnNew(Vector3 pos, Quaternion rot)
		{
			if (this.limitInstances && this.totalCount >= this.limitAmount)
			{
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! Not creating new instances! (Returning null)", this.spawnPool.poolName, this.prefab.name));
				}
				return null;
			}
			if (this.spawnPool == null)
			{
				return null;
			}
			if (this.spawnPool.group == null)
			{
				return null;
			}
			if (DateTime.Now - this._lastShrink < TimeSpan.FromSeconds((double)(60f * this.shrinkFactor)))
			{
				this.shrinkFactor += 1f;
			}
			if (pos == Vector3.zero)
			{
				pos = this.spawnPool.group.position;
			}
			if (rot == Quaternion.identity)
			{
				rot = this.spawnPool.group.rotation;
			}
			Transform transform = (Transform)UnityEngine.Object.Instantiate(this.prefab, pos, rot);
			this.nameInstance(transform);
			if (!this.spawnPool.dontReparent)
			{
				transform.parent = this.spawnPool.group;
			}
			if (this.spawnPool.matchPoolScale)
			{
				transform.localScale = Vector3.one;
			}
			if (this.spawnPool.matchPoolLayer)
			{
				this.SetRecursively(transform, this.spawnPool.gameObject.layer);
			}
			this._spawned.Add(transform);
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.", this.spawnPool.poolName, this.prefab.name, transform.name));
			}
			return transform;
		}

		private void SetRecursively(Transform xform, int layer)
		{
			xform.gameObject.layer = layer;
			foreach (Transform xform2 in xform)
			{
				this.SetRecursively(xform2, layer);
			}
		}

		internal void AddUnpooled(Transform inst, bool despawn)
		{
			this.nameInstance(inst);
			if (despawn)
			{
				PoolManagerUtils.SetActive(inst.gameObject, false);
				this._despawned.Add(new PrefabPool.DespawnedItem(inst));
			}
			else
			{
				this._spawned.Add(inst);
			}
		}

		internal void PreloadInstances()
		{
			if (this.prefab == null)
			{
				UnityEngine.Debug.LogError(string.Format("SpawnPool {0} ({1}): Prefab cannot be null.", this.spawnPool.poolName, this.prefab.name));
				return;
			}
			if (this.limitInstances && this.preloadAmount > this.limitAmount)
			{
				UnityEngine.Debug.LogError(string.Format("SpawnPool {0} ({1}): You turned ON 'Limit Instances' and entered a 'Limit Amount' greater than the 'Preload Amount'! Setting preload amount to limit amount.", this.spawnPool.poolName, this.prefab.name));
				this.preloadAmount = this.limitAmount;
			}
			if (!this.cullDespawned || this.preloadAmount > this.cullAbove)
			{
			}
			if (this.preloadTime)
			{
				if (this.preloadFrames > this.preloadAmount)
				{
					UnityEngine.Debug.LogError(string.Format("SpawnPool {0} ({1}): Preloading over-time is on but the frame duration is greater than the number of instances to preload. The minimum spawned per frame is 1, so the maximum time is the same as the number of instances. Changing the preloadFrames value...", this.spawnPool.poolName, this.prefab.name));
					this.preloadFrames = this.preloadAmount;
				}
				this.spawnPool.StartCoroutine(this.PreloadOverTime());
			}
			else
			{
				this.forceLoggingSilent = true;
				while (this.totalCount < this.preloadAmount)
				{
					Transform xform = this.SpawnNew();
					this.DespawnInstance(xform, false);
				}
				this.forceLoggingSilent = false;
			}
		}

		[DebuggerHidden]
		private IEnumerator PreloadOverTime()
		{
			PrefabPool.<PreloadOverTime>c__IteratorE <PreloadOverTime>c__IteratorE = new PrefabPool.<PreloadOverTime>c__IteratorE();
			<PreloadOverTime>c__IteratorE.<>f__this = this;
			return <PreloadOverTime>c__IteratorE;
		}

		private void nameInstance(Transform instance)
		{
			instance.name += (this.totalCount + 1).ToString("#000");
		}

		public void TryShrink(TimeSpan duration, DateTime now)
		{
			if (this._despawned.Count == 0)
			{
				return;
			}
			bool flag = true;
			TimeSpan newDur = TimeSpan.FromSeconds(duration.TotalSeconds * (double)this.shrinkFactor);
			for (int i = this._despawned.Count - 1; i >= 0; i--)
			{
				if (now - this._despawned[i].despawnedTime >= newDur)
				{
					if (!flag || i != 0)
					{
						GameObject gameObject = this._despawned[i].transform.gameObject;
						UnityEngine.Object.Destroy(gameObject);
						this._lastShrink = DateTime.Now;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				PrefabPool.DespawnedItem item = this._despawned[0];
				this._despawned.Clear();
				this._despawned.Add(item);
			}
			else
			{
				this._despawned.RemoveAll((PrefabPool.DespawnedItem x) => now - x.despawnedTime >= newDur);
			}
		}
	}
}
