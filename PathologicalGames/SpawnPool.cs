using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IEnumerable, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>
	{
		public string poolName = string.Empty;

		public bool matchPoolScale;

		public bool matchPoolLayer;

		public bool dontReparent;

		public bool dontDestroyOnLoad;

		public bool logMessages;

		public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		[HideInInspector]
		public float maxParticleDespawnTime = 60f;

		public PrefabsDict prefabs = new PrefabsDict();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		internal List<Transform> _spawned = new List<Transform>();

		private static bool _disableAdd;

		public Transform group
		{
			get;
			private set;
		}

		public Dictionary<string, PrefabPool> prefabPools
		{
			get
			{
				Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
				foreach (PrefabPool current in this._prefabPools)
				{
					dictionary[current.prefabGO.name] = current;
				}
				return dictionary;
			}
		}

		public static bool DisableAdd
		{
			set
			{
				SpawnPool._disableAdd = value;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return this._spawned[index];
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public int Count
		{
			get
			{
				return this._spawned.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			SpawnPool.GetEnumerator>c__Iterator8 getEnumerator>c__Iterator = new SpawnPool.GetEnumerator>c__Iterator8();
			getEnumerator>c__Iterator.<>f__this = this;
			return getEnumerator>c__Iterator;
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}

		public void ShrinkPools(TimeSpan age, DateTime now)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				this._prefabPools[i].TryShrink(age, now);
			}
		}

		private void Awake()
		{
			if (this.dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			this.group = base.transform;
			if (this.poolName == string.Empty)
			{
				this.poolName = this.group.name.Replace("Pool", string.Empty);
				this.poolName = this.poolName.Replace("(Clone)", string.Empty);
			}
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));
			}
			foreach (PrefabPool current in this._perPrefabPoolOptions)
			{
				if (current.prefab == null)
				{
					UnityEngine.Debug.LogError(string.Format("Initialization Warning: Pool '{0}' contains a PrefabPool with no prefab reference. Skipping.", this.poolName));
				}
				else
				{
					current.inspectorInstanceConstructor();
					this.CreatePrefabPool(current);
				}
			}
			if (!SpawnPool._disableAdd)
			{
				PoolManager.Pools.Add(this);
			}
		}

		public void ClearResources()
		{
			PoolManager.Pools.Remove(this);
			base.StopAllCoroutines();
			this.group = null;
			this.prefabs._Clear();
			if (this._prefabPools != null && this._prefabPools.Count > 0)
			{
				for (int i = 0; i < this._prefabPools.Count; i++)
				{
					if (this._prefabPools[i] != null)
					{
						this._prefabPools[i].SelfDestruct();
						this._prefabPools[i] = null;
					}
				}
				this._prefabPools.Clear();
			}
			if (this._perPrefabPoolOptions != null && this._perPrefabPoolOptions.Count > 0)
			{
				for (int j = 0; j < this._perPrefabPoolOptions.Count; j++)
				{
					if (this._perPrefabPoolOptions[j] != null)
					{
						this._perPrefabPoolOptions[j].SelfDestruct();
						this._perPrefabPoolOptions[j] = null;
					}
				}
				this._perPrefabPoolOptions.Clear();
			}
			this._spawned.Clear();
		}

		private void OnDestroy()
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));
			}
			PoolManager.Pools.Remove(this);
			base.StopAllCoroutines();
			this._spawned.Clear();
			foreach (PrefabPool current in this._prefabPools)
			{
				current.SelfDestruct();
			}
			this._prefabPools.Clear();
			this.prefabs._Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			if (this.GetPrefab(prefabPool.prefab) == null)
			{
				prefabPool.spawnPool = this;
				this._prefabPools.Add(prefabPool);
				if (!this.prefabs.ContainsKey(prefabPool.prefab.name))
				{
					this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
				}
			}
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}", this.poolName, prefabPool.preloadAmount, prefabPool.prefab.name));
			}
			prefabPool.PreloadInstances();
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			foreach (PrefabPool current in this._prefabPools)
			{
				if (current.prefabGO == null)
				{
					UnityEngine.Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
					return;
				}
				if (current.prefabGO.name == prefabName)
				{
					current.AddUnpooled(instance, despawn);
					if (this.logMessages)
					{
						UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Adding previously unpooled instance {1}", this.poolName, instance.name));
					}
					if (parent)
					{
						instance.parent = this.group;
					}
					if (!despawn)
					{
						this._spawned.Add(instance);
					}
					return;
				}
			}
			UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.", this.poolName, prefabName));
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public void Remove(Transform item)
		{
			string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform;
			foreach (PrefabPool current in this._prefabPools)
			{
				if (current.prefabGO == prefab.gameObject)
				{
					transform = current.SpawnInstance(pos, rot);
					Transform result;
					if (transform == null)
					{
						result = null;
						return result;
					}
					if (!this.dontReparent && transform.parent != this.group)
					{
						transform.parent = this.group;
					}
					this._spawned.Add(transform);
					result = transform;
					return result;
				}
			}
			PrefabPool prefabPool = new PrefabPool(prefab);
			this.CreatePrefabPool(prefabPool);
			transform = prefabPool.SpawnInstance(pos, rot);
			transform.parent = this.group;
			this._spawned.Add(transform);
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = this.Spawn(prefab, pos, rot);
			transform.parent = parent;
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(Transform prefab, Transform parent)
		{
			Transform transform = this.Spawn(prefab, Vector3.zero, Quaternion.identity);
			transform.parent = parent;
			return transform;
		}

		public Transform Spawn(string prefabName)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(string prefabName, Transform parent)
		{
			Transform prefab = this.prefabs[prefabName];
			Transform transform = this.Spawn(prefab, Vector3.zero, Quaternion.identity);
			transform.parent = parent;
			return transform;
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab, pos, rot);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform prefab = this.prefabs[prefabName];
			Transform transform = this.Spawn(prefab, pos, rot);
			transform.parent = parent;
			return transform;
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion quat)
		{
			Transform transform = this.Spawn(prefab.transform, pos, quat);
			if (transform == null)
			{
				return null;
			}
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			base.StartCoroutine(this.ListenForEmitDespawn(component));
			return component;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion quat)
		{
			Transform transform = this.Spawn(prefab.transform, pos, quat);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.emit = true;
			base.StartCoroutine(this.ListenForEmitDespawn(component2));
			return component2;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion quat, string colorPropertyName, Color color)
		{
			Transform transform = this.Spawn(prefab.transform, pos, quat);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.renderer.material.SetColor(colorPropertyName, color);
			component2.emit = true;
			base.StartCoroutine(this.ListenForEmitDespawn(component2));
			return component2;
		}

		public void Despawn(Transform instance)
		{
			bool flag = false;
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				PrefabPool prefabPool = this._prefabPools[i];
				if (prefabPool._spawned.Contains(instance))
				{
					flag = prefabPool.DespawnInstance(instance);
					break;
				}
				if (prefabPool._despawned.FirstOrDefault((PrefabPool.DespawnedItem x) => instance == x.transform) != null)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0}: {1} has already been despawned. You cannot despawn something more than once!", this.poolName, instance.name));
					return;
				}
			}
			if (!flag)
			{
				return;
			}
			this._spawned.Remove(instance);
		}

		public void Despawn(Transform instance, Transform parent)
		{
			instance.parent = parent;
			this.Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, false, null));
			}
		}

		public void Despawn(Transform instance, float seconds, Transform parent)
		{
			base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, true, parent));
		}

		public void StopDoDespawn()
		{
			base.StopCoroutine("DoDespawnAfterSeconds");
		}

		[DebuggerHidden]
		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
		{
			SpawnPool.<DoDespawnAfterSeconds>c__Iterator9 <DoDespawnAfterSeconds>c__Iterator = new SpawnPool.<DoDespawnAfterSeconds>c__Iterator9();
			<DoDespawnAfterSeconds>c__Iterator.instance = instance;
			<DoDespawnAfterSeconds>c__Iterator.seconds = seconds;
			<DoDespawnAfterSeconds>c__Iterator.useParent = useParent;
			<DoDespawnAfterSeconds>c__Iterator.parent = parent;
			<DoDespawnAfterSeconds>c__Iterator.<$>instance = instance;
			<DoDespawnAfterSeconds>c__Iterator.<$>seconds = seconds;
			<DoDespawnAfterSeconds>c__Iterator.<$>useParent = useParent;
			<DoDespawnAfterSeconds>c__Iterator.<$>parent = parent;
			<DoDespawnAfterSeconds>c__Iterator.<>f__this = this;
			return <DoDespawnAfterSeconds>c__Iterator;
		}

		public void DespawnAll()
		{
			List<Transform> list = new List<Transform>(this._spawned);
			foreach (Transform current in list)
			{
				this.Despawn(current);
			}
		}

		public bool IsSpawned(Transform instance)
		{
			return this._spawned.Contains(instance);
		}

		public Transform GetPrefab(Transform prefab)
		{
			foreach (PrefabPool current in this._prefabPools)
			{
				if (current.prefabGO == null)
				{
					UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
				}
				if (current.prefabGO == prefab.gameObject)
				{
					return current.prefab;
				}
			}
			return null;
		}

		public GameObject GetPrefab(GameObject prefab)
		{
			foreach (PrefabPool current in this._prefabPools)
			{
				if (current.prefabGO == null)
				{
					UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
				}
				if (current.prefabGO == prefab)
				{
					return current.prefabGO;
				}
			}
			return null;
		}

		[DebuggerHidden]
		private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
		{
			SpawnPool.<ListenForEmitDespawn>c__IteratorA <ListenForEmitDespawn>c__IteratorA = new SpawnPool.<ListenForEmitDespawn>c__IteratorA();
			<ListenForEmitDespawn>c__IteratorA.emitter = emitter;
			<ListenForEmitDespawn>c__IteratorA.<$>emitter = emitter;
			<ListenForEmitDespawn>c__IteratorA.<>f__this = this;
			return <ListenForEmitDespawn>c__IteratorA;
		}

		[DebuggerHidden]
		private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
		{
			SpawnPool.<ListenForEmitDespawn>c__IteratorB <ListenForEmitDespawn>c__IteratorB = new SpawnPool.<ListenForEmitDespawn>c__IteratorB();
			<ListenForEmitDespawn>c__IteratorB.emitter = emitter;
			<ListenForEmitDespawn>c__IteratorB.<$>emitter = emitter;
			<ListenForEmitDespawn>c__IteratorB.<>f__this = this;
			return <ListenForEmitDespawn>c__IteratorB;
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Transform current in this._spawned)
			{
				list.Add(current.name);
			}
			return string.Join(", ", list.ToArray());
		}

		public bool Contains(Transform item)
		{
			string message = "Use IsSpawned(Transform instance) instead.";
			throw new NotImplementedException(message);
		}

		public void CopyTo(Transform[] array, int arrayIndex)
		{
			this._spawned.CopyTo(array, arrayIndex);
		}

		[DebuggerHidden]
		public IEnumerator<Transform> GetEnumerator()
		{
			SpawnPool.<GetEnumerator>c__IteratorC <GetEnumerator>c__IteratorC = new SpawnPool.<GetEnumerator>c__IteratorC();
			<GetEnumerator>c__IteratorC.<>f__this = this;
			return <GetEnumerator>c__IteratorC;
		}

		public int IndexOf(Transform item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Transform item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			this.DespawnAll();
			this._spawned.Clear();
		}
	}
}
