using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager
{
	private static PrefabManager instance;

	private static object obj_lock = new object();

	private List<UnityEngine.Object> prefabList = new List<UnityEngine.Object>();

	private Queue<UnityEngine.Object> prefabQueue = new Queue<UnityEngine.Object>();

	private PrefabManager()
	{
	}

	public static PrefabManager GetInstance()
	{
		if (PrefabManager.instance == null)
		{
			object obj = PrefabManager.obj_lock;
			lock (obj)
			{
				if (PrefabManager.instance == null)
				{
					PrefabManager.instance = new PrefabManager();
				}
			}
		}
		return PrefabManager.instance;
	}

	public void UnloadAll()
	{
		if (this.prefabList.Count == 0 || this.prefabList == null)
		{
			return;
		}
		this.prefabList.Clear();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void AddObject(UnityEngine.Object obj)
	{
		if (this.prefabList.Count > 10)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
			this.prefabList.Clear();
		}
		this.prefabList.Add(obj);
	}

	private string PrefabName(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return null;
		}
		int num = s.IndexOf("0");
		int num2 = s.IndexOf("_");
		return (num + 2 != num2) ? s.Substring(0, num2) : s.Substring(0, num);
	}
}
