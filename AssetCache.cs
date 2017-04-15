using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetCache
{
	public class CachedAsset
	{
		private Asset m_asset;

		private UnityEngine.Object m_assetObject;

		public bool Persistent
		{
			get
			{
				return this.m_asset.Persistent;
			}
		}

		public long CreatedTimestamp
		{
			get;
			set;
		}

		public long LastRequestTimestamp
		{
			get;
			set;
		}

		public Asset GetAsset()
		{
			return this.m_asset;
		}

		public UnityEngine.Object GetAssetObject()
		{
			return this.m_assetObject;
		}

		public void SetAsset(Asset asset)
		{
			this.m_asset = asset;
		}

		public void SetAssetObject(UnityEngine.Object assetObject)
		{
			this.m_assetObject = assetObject;
		}

		public void UnloadAssetObject()
		{
			this.m_assetObject = null;
		}
	}

	public abstract class CacheRequest
	{
		public bool Complete
		{
			get;
			set;
		}

		public long CreatedTimestamp
		{
			get;
			set;
		}

		public long LastRequestTimestamp
		{
			get;
			set;
		}

		public bool Persistent
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public bool DidFail()
		{
			return this.Complete && !this.Success;
		}

		public bool DidSuccess()
		{
			return this.Complete && this.Success;
		}

		public abstract int GetRequestCount();

		public virtual void OnLoadFailed(string name)
		{
			this.Complete = true;
			this.Success = false;
		}

		public void OnLoadSucceed()
		{
			this.Complete = true;
			this.Success = true;
		}

		public abstract void OnProgressUpdate(string name, float progress);
	}

	public class GameObjectCacheRequest : AssetCache.CacheRequest
	{
		private readonly List<AssetCache.GameObjectRequester> m_requesters;

		public void AddRequester(AssetLoader.GameObjectCallback callback, AssetLoader.OnDownloading onDownloading, object callbackData)
		{
			AssetCache.GameObjectRequester item = new AssetCache.GameObjectRequester
			{
				m_callback = callback,
				MOnDownloading = onDownloading,
				m_callbackData = callbackData
			};
			this.m_requesters.Add(item);
		}

		public override int GetRequestCount()
		{
			return this.m_requesters.Count;
		}

		public List<AssetCache.GameObjectRequester> GetRequesters()
		{
			return this.m_requesters;
		}

		public override void OnLoadFailed(string name)
		{
			base.OnLoadFailed(name);
			foreach (AssetCache.GameObjectRequester current in this.m_requesters)
			{
				AssetLoader.GameObjectCallback callback = current.m_callback;
				object callbackData = current.m_callbackData;
				if (callback != null)
				{
					callback(name, null, callbackData);
				}
			}
		}

		public override void OnProgressUpdate(string name, float progress)
		{
			foreach (AssetCache.GameObjectRequester current in this.m_requesters)
			{
				AssetLoader.OnDownloading mOnDownloading = current.MOnDownloading;
				if (mOnDownloading != null)
				{
					mOnDownloading(name, progress, current.m_callbackData);
				}
			}
		}
	}

	public class GameObjectRequester
	{
		public AssetLoader.GameObjectCallback m_callback;

		public object m_callbackData;

		public AssetLoader.OnDownloading MOnDownloading;
	}

	public class ObjectCacheRequest : AssetCache.CacheRequest
	{
		private readonly List<AssetCache.ObjectRequester> m_requesters;

		public ObjectCacheRequest()
		{
			this.m_requesters = new List<AssetCache.ObjectRequester>();
		}

		public void AddRequester(AssetLoader.ObjectCallback callback, AssetLoader.OnDownloading onDownloading, object callbackData)
		{
			AssetCache.ObjectRequester item = new AssetCache.ObjectRequester
			{
				m_callback = callback,
				MOnDownloading = onDownloading,
				m_callbackData = callbackData
			};
			this.m_requesters.Add(item);
		}

		public override int GetRequestCount()
		{
			return this.m_requesters.Count;
		}

		public List<AssetCache.ObjectRequester> GetRequesters()
		{
			return this.m_requesters;
		}

		public void OnLoadComplete(string name, UnityEngine.Object asset)
		{
			foreach (AssetCache.ObjectRequester current in this.m_requesters)
			{
				AssetLoader.ObjectCallback callback = current.m_callback;
				if (callback != null)
				{
					callback(name, asset, current.m_callbackData);
				}
			}
		}

		public override void OnLoadFailed(string name)
		{
			base.OnLoadFailed(name);
			this.OnLoadComplete(name, null);
		}

		public override void OnProgressUpdate(string name, float progress)
		{
			foreach (AssetCache.ObjectRequester current in this.m_requesters)
			{
				AssetLoader.OnDownloading mOnDownloading = current.MOnDownloading;
				if (mOnDownloading != null)
				{
					mOnDownloading(name, progress, current.m_callbackData);
				}
			}
		}
	}

	public class ObjectRequester
	{
		public AssetLoader.ObjectCallback m_callback;

		public object m_callbackData;

		public AssetLoader.OnDownloading MOnDownloading;
	}

	private static Dictionary<string, AssetCache> cacheTable;

	private static Dictionary<string, int> m_assetLoading;

	private Dictionary<string, AssetCache.CachedAsset> m_assetMap;

	private Dictionary<string, AssetCache.CacheRequest> m_assetRequestMap;

	private static long s_cacheClearTime;

	public AssetCache()
	{
		this.m_assetMap = new Dictionary<string, AssetCache.CachedAsset>();
		this.m_assetRequestMap = new Dictionary<string, AssetCache.CacheRequest>();
	}

	static AssetCache()
	{
		AssetCache.cacheTable = new Dictionary<string, AssetCache>();
		AssetCache.m_assetLoading = new Dictionary<string, int>();
		AssetCache.s_cacheClearTime = TimeUtils.BinaryStamp();
	}

	public static void Init(string family)
	{
		if (!AssetCache.cacheTable.ContainsKey(family))
		{
			AssetCache.cacheTable.Add(family, new AssetCache());
		}
	}

	public static void Add(Asset asset, AssetCache.CachedAsset item)
	{
		AssetCache.cacheTable[asset.Family].AddItem(asset.Name, item);
	}

	private void AddItem(string name, AssetCache.CachedAsset item)
	{
		this.m_assetMap.Add(name, item);
	}

	public static void AddRequest(Asset asset, AssetCache.CacheRequest request)
	{
		AssetCache.cacheTable[asset.Family].AddRequest(asset.Name, request);
	}

	public void AddRequest(string key, AssetCache.CacheRequest request)
	{
		this.m_assetRequestMap.Add(key, request);
	}

	public void Clear(bool clearPersistent = false, bool clearLoading = true)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CachedAsset> current in this.m_assetMap)
		{
			string key = current.Key;
			if (!current.Value.Persistent || clearPersistent)
			{
				if (!AssetCache.IsLoading(key) || clearLoading)
				{
					list.Add(key);
				}
			}
		}
		foreach (string current2 in list)
		{
			this.ClearItem(current2);
		}
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CacheRequest> current3 in this.m_assetRequestMap)
		{
			string key2 = current3.Key;
			if ((!current3.Value.Persistent || clearPersistent) && (!AssetCache.IsLoading(key2) || clearLoading))
			{
				list2.Add(key2);
			}
		}
		foreach (string current4 in list2)
		{
			this.ClearItem(current4);
		}
	}

	public static void ClearAsset(string name, string family)
	{
		AssetCache.cacheTable[family].ClearItem(name);
	}

	public static void ClearAssets(IEnumerable<string> names, string family)
	{
		if (names != null)
		{
			AssetCache.cacheTable[family].ClearItems(names);
		}
	}

	public static void ClearCache(string family, IEnumerable<string> names)
	{
		if (names != null && AssetCache.cacheTable.ContainsKey(family))
		{
			AssetCache.cacheTable[family].ClearItems(names);
		}
	}

	public static void ClearCache(string family)
	{
		if (AssetCache.cacheTable.ContainsKey(family))
		{
			AssetCache.cacheTable[family].Clear(false, true);
		}
	}

	public static void ClearAllCaches(bool clearPersisten = false, bool clearLoading = true)
	{
		foreach (KeyValuePair<string, AssetCache> current in AssetCache.cacheTable)
		{
			current.Value.Clear(clearPersisten, clearLoading);
		}
		AssetCache.s_cacheClearTime = TimeUtils.BinaryStamp();
	}

	public static void ClearAllCachesBetween(long startTimestamp, long endTimestamp)
	{
		foreach (KeyValuePair<string, AssetCache> current in AssetCache.cacheTable)
		{
			current.Value.ClearItemsBetween(startTimestamp, endTimestamp);
		}
	}

	private void ClearItemsBetween(long startTimestamp, long endTimestamp)
	{
		if (endTimestamp >= startTimestamp)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, AssetCache.CachedAsset> current in this.m_assetMap)
			{
				AssetCache.CachedAsset value = current.Value;
				if (!value.Persistent && value.LastRequestTimestamp >= startTimestamp && value.LastRequestTimestamp <= endTimestamp)
				{
					hashSet.Add(current.Key);
				}
			}
			foreach (KeyValuePair<string, AssetCache.CacheRequest> current2 in this.m_assetRequestMap)
			{
				AssetCache.CacheRequest value2 = current2.Value;
				if (!value2.Persistent && value2.LastRequestTimestamp >= startTimestamp && value2.LastRequestTimestamp <= endTimestamp)
				{
					hashSet.Add(current2.Key);
				}
			}
			foreach (string current3 in hashSet)
			{
				this.ClearItem(current3);
			}
		}
	}

	public static void ClearAllCachesFailedRequests()
	{
		foreach (KeyValuePair<string, AssetCache> current in AssetCache.cacheTable)
		{
			current.Value.ClearAllFailedRequests();
		}
	}

	private void ClearAllFailedRequests()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CacheRequest> current in this.m_assetRequestMap)
		{
			if (current.Value.DidFail())
			{
				list.Add(current.Key);
			}
		}
		foreach (string current2 in list)
		{
			this.m_assetRequestMap.Remove(current2);
		}
	}

	public static void ClearAllCachesSince(long sinceTimestamp)
	{
		long endTimestamp = TimeUtils.BinaryStamp();
		AssetCache.ClearAllCachesBetween(sinceTimestamp, endTimestamp);
	}

	private bool ClearItem(string key)
	{
		bool flag = false;
		AssetCache.CachedAsset cachedAsset;
		if (this.m_assetMap.TryGetValue(key, out cachedAsset))
		{
			cachedAsset.UnloadAssetObject();
			this.m_assetMap.Remove(key);
			flag = true;
		}
		AssetCache.CacheRequest cacheRequest;
		if (this.m_assetRequestMap.TryGetValue(key, out cacheRequest))
		{
			cacheRequest.Success = false;
			this.m_assetRequestMap.Remove(key);
			flag = true;
		}
		if (!flag)
		{
		}
		return flag;
	}

	private void ClearItems(IEnumerable<string> itemsToRemove)
	{
		IEnumerator<string> enumerator = itemsToRemove.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				this.ClearItem(current);
			}
		}
		finally
		{
			if (enumerator == null)
			{
			}
			enumerator.Dispose();
		}
	}

	private void ClearAllItems()
	{
	}

	public static AssetCache.CachedAsset Find(Asset asset)
	{
		return AssetCache.cacheTable[asset.Family].GetItem(asset.Name);
	}

	private AssetCache.CachedAsset GetItem(string key)
	{
		AssetCache.CachedAsset cachedAsset;
		return this.m_assetMap.TryGetValue(key, out cachedAsset) ? cachedAsset : null;
	}

	public static void ForceClearAllCaches()
	{
		foreach (KeyValuePair<string, AssetCache> current in AssetCache.cacheTable)
		{
			current.Value.ForceClear();
		}
		AssetCache.s_cacheClearTime = TimeUtils.BinaryStamp();
	}

	private void ForceClear()
	{
		foreach (KeyValuePair<string, AssetCache.CachedAsset> current in this.m_assetMap)
		{
			current.Value.UnloadAssetObject();
		}
		foreach (KeyValuePair<string, AssetCache.CacheRequest> current2 in this.m_assetRequestMap)
		{
			current2.Value.Success = false;
		}
		this.m_assetMap.Clear();
		this.m_assetRequestMap.Clear();
	}

	private string GetFamily()
	{
		foreach (KeyValuePair<string, AssetCache> current in AssetCache.cacheTable)
		{
			if (this == current.Value)
			{
				return current.Key;
			}
		}
		return null;
	}

	public static T GetRequest<T>(Asset asset) where T : AssetCache.CacheRequest
	{
		return AssetCache.cacheTable[asset.Family].GetRequest<T>(asset.Name);
	}

	private T GetRequest<T>(string key) where T : AssetCache.CacheRequest
	{
		AssetCache.CacheRequest cacheRequest;
		if (this.m_assetRequestMap.TryGetValue(key, out cacheRequest))
		{
			return cacheRequest as T;
		}
		return (T)((object)null);
	}

	public static string[] GetRequestDictKeys(string family)
	{
		List<string> list = new List<string>(AssetCache.cacheTable[family].m_assetRequestMap.Keys);
		return list.ToArray();
	}

	public static long GetCacheClearTime()
	{
		return AssetCache.s_cacheClearTime;
	}

	public static bool HasItem(Asset asset)
	{
		return AssetCache.cacheTable[asset.Family].HasItem(asset.Name);
	}

	private bool HasItem(string key)
	{
		return this.m_assetMap.ContainsKey(key);
	}

	public static bool HasRequest(Asset asset)
	{
		return AssetCache.cacheTable[asset.Family].HasRequest(asset.Name);
	}

	private bool HasRequest(string key)
	{
		return this.m_assetRequestMap.ContainsKey(key);
	}

	public static bool IsLoading(string name)
	{
		int num;
		return AssetCache.m_assetLoading.TryGetValue(name, out num) && num > 0;
	}

	public static bool CanLoadBundle(string name)
	{
		int num;
		return AssetCache.m_assetLoading.TryGetValue(name, out num) && num == 1;
	}

	public static bool RemoveRequest(Asset asset)
	{
		return AssetCache.cacheTable[asset.Family].RemoveRequest(asset.Name);
	}

	private bool RemoveRequest(string key)
	{
		return this.m_assetRequestMap.Remove(key);
	}

	public static void StartLoading(string name)
	{
		if (!AssetCache.m_assetLoading.ContainsKey(name))
		{
			AssetCache.m_assetLoading.Add(name, 0);
		}
		int num = AssetCache.m_assetLoading[name];
		AssetCache.m_assetLoading[name] = num + 1;
	}

	public static void StopLoading(string name)
	{
		int num;
		if (AssetCache.m_assetLoading.TryGetValue(name, out num))
		{
			if (num < 1)
			{
				AssetCache.m_assetLoading.Remove(name);
			}
			else
			{
				AssetCache.m_assetLoading[name] = num - 1;
			}
		}
	}
}
