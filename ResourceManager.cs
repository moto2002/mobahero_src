using Assets.Scripts.Server;
using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : IGlobalComServer
{
	private static bool init;

	private static AssetManager _assetMgr;

	private static Dictionary<string, UnityEngine.Object> m_gameResList = new Dictionary<string, UnityEngine.Object>();

	private static Dictionary<string, UnityEngine.Object> m_pathResList = new Dictionary<string, UnityEngine.Object>();

	public static bool openLog = false;

	private static ResourceManager m_Instance = null;

	public static ResourceManager Instance
	{
		get
		{
			if (ResourceManager.m_Instance == null)
			{
				ResourceManager.m_Instance = new ResourceManager();
			}
			return ResourceManager.m_Instance;
		}
	}

	public void OnAwake()
	{
		ResourceManager._assetMgr = AssetManager.Instance;
		ResourceManager._assetMgr.Init();
	}

	public static void AutoConfig()
	{
		if (GlobalSettings.useLocalXML)
		{
			BaseDataMgr.instance.InitBaseConfigDataXML();
		}
		else
		{
			BaseDataMgr.instance.InitBaseConfigData();
		}
		LanguageManager.Instance.getDataReady = true;
	}

	public void Destroy()
	{
		ResourceManager.init = false;
		ResourceManager._assetMgr = null;
	}

	public static string GetBindataMD5()
	{
		return ResourceManager._assetMgr.GetBindataMD5();
	}

	public static void CheckAndDownloadBinData(string url, Callback loadDataCallback = null)
	{
		ResourceManager._assetMgr.DownloadBinData(url, loadDataCallback);
	}

	public static void downLoadAssets()
	{
		ResourceManager._assetMgr.downLoadAssets();
	}

	public static void CheckDownLoadAssets()
	{
		ResourceManager._assetMgr.CheckDownLoadAssets();
	}

	public static void GetDownLoadInfo(ref long nFreeSpase, ref long nNeedSpase)
	{
		ResourceManager._assetMgr.GetDownLoadInfo(ref nFreeSpase, ref nNeedSpase);
	}

	public static void InitData(string url, Callback loadDataCallback = null)
	{
		if (!ResourceManager.init)
		{
			ResourceManager._assetMgr.LoadData_2(GlobalSettings.useLocalData, url);
			ResourceManager.init = true;
			LanguageManager.Instance.getDataReady = true;
		}
	}

	public static T Load<T>(string resId, bool loadFromCache = true, bool loadFromBundle = true, Action<T> firstLoadCallback = null, int skin = 0, bool isMonsterSkin = false) where T : class
	{
		if (resId == null || resId == "[]")
		{
			return (T)((object)null);
		}
		SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(resId);
		if (gameResData == null)
		{
			Debug.LogError("no such resId:" + resId + " 请检查GameRes表");
			return (T)((object)null);
		}
		if (ResourceManager.openLog)
		{
		}
		string path = gameResData.path;
		string id = gameResData.id;
		HeroSkins.GetHeroSkinResPath(skin, gameResData, ref path, ref id, isMonsterSkin);
		if (loadFromBundle && !string.IsNullOrEmpty(gameResData.bundle) && gameResData.bundle != "[]")
		{
			string bundle = gameResData.bundle;
			if (ResourceManager.openLog)
			{
			}
			if (ResourceManager.openLog)
			{
			}
			if (loadFromCache)
			{
				T t = AssetLoader.Load(path, bundle, false) as T;
				if (t != null)
				{
					return t;
				}
			}
			else
			{
				T t2 = UnityEngine.Object.Instantiate(AssetLoader.Load(gameResData.path, bundle, false)) as T;
				AssetLoader.Instance.UnloadCachedAsset(path, bundle);
				if (t2 != null)
				{
					return t2;
				}
			}
		}
		if (ResourceManager.openLog)
		{
		}
		if (ResourceManager.openLog)
		{
		}
		UnityEngine.Object @object = null;
		bool flag = ResourceManager.m_gameResList.TryGetValue(id, out @object);
		if (!flag || @object == null)
		{
			string[] array = path.Split(new char[]
			{
				'/'
			});
			if (array != null && array[array.Length - 1] != null)
			{
				@object = AssetBundleMgr.Instance.Load(array[array.Length - 1], gameResData.bundle);
			}
			if (@object == null)
			{
				@object = Resources.Load(path);
			}
			if (@object != null)
			{
				if (flag)
				{
					ResourceManager.m_gameResList[id] = @object;
				}
				else
				{
					ResourceManager.m_gameResList.Add(id, @object);
				}
			}
			else if (skin == 0)
			{
				Debug.LogError("缺少美术资源 " + resId + " 在gameres表中的path:" + path);
			}
		}
		if (!loadFromCache)
		{
			return UnityEngine.Object.Instantiate(@object) as T;
		}
		if (gameResData == null)
		{
			Debug.LogError("error gameRes id=" + resId);
			return (T)((object)null);
		}
		T t3 = @object as T;
		if (t3 != null && firstLoadCallback != null)
		{
			firstLoadCallback(t3);
		}
		return t3;
	}

	public static T LoadPath<T>(string path, Action<T> firstLoadCallback = null, int skin = 0) where T : UnityEngine.Object
	{
		if (path == null || path == "[]")
		{
			return (T)((object)null);
		}
		T t = (T)((object)null);
		UnityEngine.Object @object;
		if (ResourceManager.m_pathResList.TryGetValue(path, out @object) && @object != null)
		{
			t = (@object as T);
		}
		else
		{
			t = Resources.Load<T>(path);
			if (t != null)
			{
				if (firstLoadCallback != null)
				{
					firstLoadCallback(t);
				}
				ResourceManager.m_pathResList[path] = t;
			}
			else if (skin == 0)
			{
				Debug.LogError("LoadPath failed, cannot find " + path);
			}
		}
		return t;
	}

	public static GameObject GetSkinnedHeroPrefab(TeamType team, string heroResId)
	{
		SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(heroResId);
		return HeroSkins.GetHeroPrefabWithSkin(gameResData.path, HeroSkins.GetHeroSkin(team, heroResId));
	}

	public static void UnLoadBundle(string[] names, bool isbundle)
	{
		if (names != null)
		{
		}
	}

	public static void UnLoadBundle(string name, bool isbundle = true)
	{
		if (name != null)
		{
		}
	}

	public static void UnLoadAllAssets(bool isbundle = false)
	{
		if (isbundle)
		{
			if (ResourceManager.openLog)
			{
			}
			AssetLoader.Instance.UnloadAllCaches(false, true);
		}
		else
		{
			ResourceManager.UnloadAllUnusedAssets();
		}
	}

	public static void UnloadAllUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
		this.Destroy();
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public static void ClearResources()
	{
		if (ResourceManager.m_gameResList != null && ResourceManager.m_gameResList.Count > 0)
		{
			ResourceManager.m_gameResList.Clear();
		}
		if (ResourceManager.m_pathResList != null && ResourceManager.m_pathResList.Count > 0)
		{
			ResourceManager.m_pathResList.Clear();
		}
	}
}
