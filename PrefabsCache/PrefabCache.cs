using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabsCache
{
	public class PrefabCache : MonoBehaviour, IPrefabCache
	{
		private class ReusablePrefabContainer
		{
			private readonly Stack<ReusablePrefab> _availablePrefabs;

			private readonly ReusablePrefab _prefab;

			public ReusablePrefabContainer(ReusablePrefab prefabReference)
			{
				this._prefab = prefabReference;
				this._availablePrefabs = new Stack<ReusablePrefab>();
			}

			public ReusablePrefab GetPrefab(Vector3 position, Quaternion rotation)
			{
				if (this._availablePrefabs.Count > 0)
				{
					ReusablePrefab reusablePrefab = this._availablePrefabs.Pop();
					reusablePrefab.transform.position = position;
					reusablePrefab.transform.rotation = rotation;
					return reusablePrefab;
				}
				ReusablePrefab component = (UnityEngine.Object.Instantiate(this._prefab.TransRef, position, rotation) as Transform).GetComponent<ReusablePrefab>();
				component.PrefabDeactivated += new PrefabDeactivatedEventHandler(this.ReusablePrefabDeactivatedEventHandler);
				return component;
			}

			private void ReusablePrefabDeactivatedEventHandler(ReusablePrefab prefab)
			{
				this._availablePrefabs.Push(prefab);
			}
		}

		private static GameObject prefabCacheGameObject;

		private static IPrefabCache _instance;

		private Dictionary<ReusablePrefab, PrefabCache.ReusablePrefabContainer> _prefabDict;

		public static IPrefabCache Instance
		{
			get
			{
				if (PrefabCache._instance == null || !PrefabCache.prefabCacheGameObject)
				{
					PrefabCache.prefabCacheGameObject = new GameObject("PrefabCache");
					PrefabCache._instance = PrefabCache.prefabCacheGameObject.AddComponent<PrefabCache>();
				}
				return PrefabCache._instance;
			}
		}

		public Transform Trans
		{
			get
			{
				return base.transform;
			}
		}

		public ReusablePrefab Instantiate(ReusablePrefab prefab)
		{
			return this.Instantiate(prefab, Vector3.zero);
		}

		public ReusablePrefab Instantiate(ReusablePrefab prefab, Vector3 position)
		{
			return this.Instantiate(prefab, position, default(Quaternion));
		}

		public ReusablePrefab Instantiate(ReusablePrefab prefab, Vector3 position, Quaternion rotation)
		{
			if (!this._prefabDict.ContainsKey(prefab))
			{
				this._prefabDict[prefab] = new PrefabCache.ReusablePrefabContainer(prefab);
			}
			ReusablePrefab prefab2 = this._prefabDict[prefab].GetPrefab(position, rotation);
			prefab2.Restart();
			return prefab2;
		}

		private void Awake()
		{
			this._prefabDict = new Dictionary<ReusablePrefab, PrefabCache.ReusablePrefabContainer>();
		}
	}
}
