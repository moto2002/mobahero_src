using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PoolRoot : MonoBehaviour
{
	[SerializeField]
	private SpawnPool _sample;

	private readonly Dictionary<TeamType, SpawnPool> _entryDict = new Dictionary<TeamType, SpawnPool>();

	private readonly Dictionary<int, SpawnPool> _poolDict = new Dictionary<int, SpawnPool>();

	private bool _first = true;

	private Coroutine _shrinking;

	private void Awake()
	{
		this.CreateSpawnPool();
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void OnDestroy()
	{
		if (MapManager.Instance != null)
		{
			MapManager.Instance.SaveHerosEquips();
		}
		this.DestroySpawnPool();
	}

	public void ClearResources()
	{
		if (this._shrinking != null)
		{
			base.StopCoroutine(this._shrinking);
			this._shrinking = null;
		}
		foreach (SpawnPool current in this._entryDict.Values)
		{
			if (current)
			{
				current.ClearResources();
				UnityEngine.Object.Destroy(current.gameObject);
			}
		}
		this._entryDict.Clear();
		foreach (SpawnPool current2 in this._poolDict.Values)
		{
			if (current2)
			{
				current2.ClearResources();
				UnityEngine.Object.Destroy(current2.gameObject);
			}
		}
		this._poolDict.Clear();
	}

	private SpawnPool CreatePool(string name, string poolName)
	{
		SpawnPool.DisableAdd = true;
		SpawnPool result;
		try
		{
			SpawnPool spawnPool = UnityEngine.Object.Instantiate(this._sample) as SpawnPool;
			spawnPool.poolName = poolName;
			spawnPool.transform.parent = base.transform;
			spawnPool.name = name;
			PoolManager.Pools.Add(spawnPool);
			result = spawnPool;
		}
		finally
		{
			SpawnPool.DisableAdd = false;
		}
		return result;
	}

	private void CreateSpawnPool()
	{
		this._entryDict[TeamType.LM] = this.CreatePool("LM", "LM");
		this._entryDict[TeamType.BL] = this.CreatePool("BL", "BL");
		this._entryDict[TeamType.Neutral] = this.CreatePool("NE", "NE");
		this._entryDict[TeamType.Team_3] = this.CreatePool("T3", "T3");
	}

	private void DestroySpawnPool()
	{
		if (this._shrinking != null)
		{
			base.StopCoroutine(this._shrinking);
			this._shrinking = null;
		}
		foreach (SpawnPool current in this._entryDict.Values)
		{
			if (current)
			{
				UnityEngine.Object.Destroy(current.gameObject);
			}
		}
		foreach (SpawnPool current2 in this._poolDict.Values)
		{
			if (current2)
			{
				UnityEngine.Object.Destroy(current2.gameObject);
			}
		}
	}

	public SpawnPool GetPool(TeamType teamType)
	{
		SpawnPool spawnPool;
		if (this._entryDict.TryGetValue(teamType, out spawnPool) && spawnPool)
		{
			return spawnPool;
		}
		return this._entryDict[TeamType.Neutral];
	}

	public SpawnPool GetPoolByPrefab(GameObject prefab)
	{
		int instanceID = prefab.GetInstanceID();
		SpawnPool spawnPool;
		if (this._poolDict.TryGetValue(instanceID, out spawnPool))
		{
			return spawnPool;
		}
		spawnPool = this.CreatePool(prefab.name, instanceID.ToString());
		this._poolDict[instanceID] = spawnPool;
		return spawnPool;
	}

	[DebuggerHidden]
	private IEnumerator ShrinkPools_Coroutine()
	{
		PoolRoot.<ShrinkPools_Coroutine>c__Iterator1B9 <ShrinkPools_Coroutine>c__Iterator1B = new PoolRoot.<ShrinkPools_Coroutine>c__Iterator1B9();
		<ShrinkPools_Coroutine>c__Iterator1B.<>f__this = this;
		return <ShrinkPools_Coroutine>c__Iterator1B;
	}
}
