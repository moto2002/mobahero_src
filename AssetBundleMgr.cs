using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AssetBundleMgr
{
	private static AssetBundleMgr m_Instance;

	private string m_bundlePath;

	private string m_localBundlesName = "LocalBundles.xml";

	public Dictionary<string, AssetBundle> m_assetBundleDict = new Dictionary<string, AssetBundle>();

	public static AssetBundleMgr Instance
	{
		get
		{
			if (AssetBundleMgr.m_Instance == null)
			{
				AssetBundleMgr.m_Instance = new AssetBundleMgr();
			}
			return AssetBundleMgr.m_Instance;
		}
	}

	public AssetBundleMgr()
	{
		this.m_bundlePath = Application.streamingAssetsPath + "/Android/";
	}

	public void LoadBundle()
	{
		new Task(this.LoadBundleTask(), true);
	}

	[DebuggerHidden]
	public IEnumerator LoadBundleTask()
	{
		AssetBundleMgr.<LoadBundleTask>c__IteratorF <LoadBundleTask>c__IteratorF = new AssetBundleMgr.<LoadBundleTask>c__IteratorF();
		<LoadBundleTask>c__IteratorF.<>f__this = this;
		return <LoadBundleTask>c__IteratorF;
	}

	public UnityEngine.Object Load(string strName, string strBundleName)
	{
		if (strName == null || strBundleName == null || strBundleName == "[]" || this.m_assetBundleDict == null)
		{
			return null;
		}
		AssetBundle assetBundle;
		if (this.m_assetBundleDict.TryGetValue(strBundleName, out assetBundle) && assetBundle != null)
		{
			return assetBundle.Load(strName);
		}
		return null;
	}

	public void CleanAssetBundle()
	{
		if (this.m_assetBundleDict != null)
		{
			foreach (AssetBundle current in this.m_assetBundleDict.Values)
			{
				current.Unload(false);
			}
		}
		this.m_assetBundleDict.Clear();
	}
}
