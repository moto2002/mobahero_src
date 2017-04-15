using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol.Data;
using MobaTools;
using SevenZip.Compression.LZMA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetLoader
{
	private class BundleInfoXML
	{
		public int assetType;

		public string appVersion;

		public string bundleMD5;

		public string family;

		public string bundleName;

		public string updateAppVersion;
	}

	public delegate void FileCallback(string path, WWW file, object callbackData);

	public delegate void GameObjectCallback(string name, GameObject go, object callbackData);

	public delegate void ObjectCallback(string name, UnityEngine.Object obj, object callbackData);

	public delegate void OnDownloading(string assetName, float progress, object callbackData);

	public delegate void OnDownLoadAssetsFinished(bool issuccess);

	public delegate void CompleteCallback2(EAssetLoadError e, object extra, string url);

	public delegate void LoadConfigCompleteCallback(byte[] data);

	private const string FILE_NAME = "bundleInfoes.xml";

	public const string FileName_bindata = "bindata.xml";

	private const float streamingTimeout = 60f;

	private string m_downloadPath_Android;

	private string m_downloadPath_IOS;

	private List<Bundle2Check> m_bundles2Update;

	private long m_nFreeSpase = -1L;

	private long m_nNeedSpase = -1L;

	private bool m_ready;

	private string m_bundlePath;

	private string m_bindataPath;

	private long m_lastFreeMemoryTime;

	private Dictionary<string, AssetBundle> m_assetBundleDict = new Dictionary<string, AssetBundle>();

	private Dictionary<string, AssetLoader.BundleInfoXML> m_bundleInfoDict = new Dictionary<string, AssetLoader.BundleInfoXML>();

	private static AssetLoader m_instance;

	private float _overAllProcess;

	public string PathName_bindata
	{
		get
		{
			return Application.persistentDataPath + "/bindata.xml";
		}
	}

	public static AssetLoader Instance
	{
		get
		{
			if (AssetLoader.m_instance == null)
			{
				AssetLoader.m_instance = new AssetLoader();
			}
			return AssetLoader.m_instance;
		}
	}

	public float overAllProcess
	{
		get
		{
			return this._overAllProcess;
		}
	}

	public string GetM_bundlePath()
	{
		return this.m_downloadPath_Android;
	}

	public void Init(bool islocal, string url)
	{
		this.m_bindataPath = url;
		this.m_bundlePath = url + "Android/";
		this.m_lastFreeMemoryTime = TimeUtils.BinaryStamp();
	}

	private void Awake()
	{
		AssetLoader.m_instance = this;
	}

	private void OnDestroy()
	{
		AssetLoader.m_instance = null;
	}

	public static UnityEngine.Object Load(string assetPath, string family, bool persistent = false)
	{
		if (family == null)
		{
			CtrlManager.ShowMsgBox("版本号异常", "VersionInfo没有找到对应的配置文件，或者下载失败", null, PopViewType.PopOneButton, "确定", "取消", null);
			return null;
		}
		if (!AssetBundleInfo.FamilyInfo.ContainsKey(family))
		{
			return null;
		}
		string bundlePathName = AssetLoader.Instance.m_bundlePath + AssetBundleInfo.FamilyInfo[family].BundleName;
		return AssetLoader.Instance.LoadBundle(family, bundlePathName, assetPath, persistent, null, null, null);
	}

	private UnityEngine.Object LoadBundle(string family, string bundlePathName, string assetName, bool persistent, AssetLoader.ObjectCallback callback, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return null;
		}
		long num = TimeUtils.BinaryStamp();
		Asset asset = Asset.Create(assetName, family, persistent, false, false);
		AssetCache.CachedAsset cachedAsset = new AssetCache.CachedAsset();
		AssetCache.ObjectCacheRequest objectCacheRequest;
		if (!AssetCache.HasRequest(asset))
		{
			objectCacheRequest = new AssetCache.ObjectCacheRequest();
			AssetCache.AddRequest(asset, objectCacheRequest);
		}
		else
		{
			objectCacheRequest = AssetCache.GetRequest<AssetCache.ObjectCacheRequest>(asset);
		}
		objectCacheRequest.AddRequester(callback, onDownloading, callbackData);
		AssetCache.StartLoading(asset.Family.ToString());
		if (AssetCache.HasItem(asset))
		{
			cachedAsset = AssetCache.Find(asset);
			cachedAsset.LastRequestTimestamp = num;
			asset = cachedAsset.GetAsset();
		}
		else if (this.m_assetBundleDict.ContainsKey(asset.Family) && this.m_assetBundleDict[asset.Family] != null)
		{
			if (!this.m_assetBundleDict[asset.Family].Contains(asset.Name))
			{
				return null;
			}
			UnityEngine.Object assetObject = this.m_assetBundleDict[family].Load(assetName, AssetBundleInfo.FamilyInfo[family].TypeOf);
			cachedAsset.SetAsset(asset);
			cachedAsset.SetAssetObject(assetObject);
			cachedAsset.CreatedTimestamp = num;
			cachedAsset.LastRequestTimestamp = num;
			AssetCache.Add(asset, cachedAsset);
		}
		else if (AssetCache.CanLoadBundle(asset.Family))
		{
			this.LoadBundleFromDisk(asset, objectCacheRequest, cachedAsset);
		}
		if (cachedAsset.GetAssetObject() != null)
		{
			objectCacheRequest.OnProgressUpdate(assetName, 1f);
			objectCacheRequest.OnLoadSucceed();
			objectCacheRequest.OnLoadComplete(assetName, cachedAsset.GetAssetObject());
			AssetCache.RemoveRequest(asset);
			AssetCache.StopLoading(assetName);
		}
		return cachedAsset.GetAssetObject();
	}

	private void LoadBundleFromDisk(Asset asset, AssetCache.ObjectCacheRequest request, AssetCache.CachedAsset cache)
	{
		if (asset == null || request == null || cache == null || this.m_assetBundleDict == null)
		{
			UnityEngine.Debug.LogError("LoadBundleFromDisk some null!");
			return;
		}
		string family = asset.Family;
		string path = Application.persistentDataPath + "/" + AssetBundleInfo.FamilyInfo[family].BundleName;
		if (!File.Exists(path))
		{
			return;
		}
		AssetBundle value = AssetBundle.CreateFromFile(path);
		if (this.m_assetBundleDict.ContainsKey(family))
		{
			this.m_assetBundleDict[family] = value;
		}
		else
		{
			this.m_assetBundleDict.Add(family, value);
		}
		long num = TimeUtils.BinaryStamp();
		string[] requestDictKeys = AssetCache.GetRequestDictKeys(family);
		for (int i = 0; i < requestDictKeys.Length; i++)
		{
			Asset asset2 = Asset.Create(requestDictKeys[i], family, asset.Persistent, false, false);
			UnityEngine.Object assetObject = this.m_assetBundleDict[family].Load(asset2.Name, AssetBundleInfo.FamilyInfo[family].TypeOf);
			cache.SetAsset(asset2);
			cache.SetAssetObject(assetObject);
			cache.CreatedTimestamp = num;
			cache.LastRequestTimestamp = num;
			AssetCache.Add(asset2, cache);
			if (cache.GetAssetObject() == null)
			{
				request.OnLoadFailed(asset2.Name);
				AssetCache.RemoveRequest(asset2);
				AssetCache.StopLoading(family.ToString());
			}
		}
		request.OnProgressUpdate(family.ToString(), 1f);
	}

	public void UnloadCachedAsset(string assetName, string family)
	{
		AssetCache.ClearAsset(assetName, family);
		this.FreeMemory();
	}

	public void UnloadAllCaches(bool clearPersistent = false, bool clearLoading = true)
	{
		AssetCache.ClearAllCaches(clearPersistent, clearLoading);
		this.FreeMemory();
	}

	public void FreeMemory()
	{
		if (TimeUtils.BinaryStamp() - this.m_lastFreeMemoryTime > 200L)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
			this.m_lastFreeMemoryTime = TimeUtils.BinaryStamp();
		}
	}

	public void DownloadBindata(AssetLoader.CompleteCallback2 completeCallback, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		new Task(this.DownLoadBindata2(completeCallback, onDownloading, callbackData), true);
	}

	public bool CheckBindata()
	{
		ClientData clientData = ModelManager.Instance.Get_ClientData_X();
		return this.CheckExistAndMD5(this.PathName_bindata, clientData.BindateMD5);
	}

	[DebuggerHidden]
	private IEnumerator DownLoadBindata2(AssetLoader.CompleteCallback2 completeCallback, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		AssetLoader.<DownLoadBindata2>c__Iterator10 <DownLoadBindata2>c__Iterator = new AssetLoader.<DownLoadBindata2>c__Iterator10();
		<DownLoadBindata2>c__Iterator.onDownloading = onDownloading;
		<DownLoadBindata2>c__Iterator.completeCallback = completeCallback;
		<DownLoadBindata2>c__Iterator.<$>onDownloading = onDownloading;
		<DownLoadBindata2>c__Iterator.<$>completeCallback = completeCallback;
		<DownLoadBindata2>c__Iterator.<>f__this = this;
		return <DownLoadBindata2>c__Iterator;
	}

	public string GetBindataMD5()
	{
		return MD5Creater.GenerateMd5Code(this.PathName_bindata);
	}

	private bool CheckExistAndMD5(string pathName, string md5)
	{
		bool result = false;
		if (!File.Exists(pathName))
		{
			string assetPath = "Assets/_Resources/Data/bindata.xml";
			TextAsset textAsset = Resources.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				this.write2File(this.PathName_bindata, textAsset.bytes);
			}
		}
		if (File.Exists(pathName))
		{
			string text = MD5Creater.GenerateMd5Code(pathName);
			if (text.Equals(md5))
			{
				result = true;
			}
			else if (md5 == null)
			{
			}
		}
		return result;
	}

	private bool CheckMD5Code(string filename, string md5, string bundleName)
	{
		string str = Application.persistentDataPath + "/" + filename;
		if (!File.Exists(str + ".unity3d"))
		{
			return false;
		}
		string text = MD5Creater.GenerateMd5Code(str + ".unity3d");
		if (text.Equals(md5))
		{
			PlayerPrefs.SetString(filename, bundleName);
			return true;
		}
		return false;
	}

	private void ReadXML(string text)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		XmlElement documentElement = xmlDocument.DocumentElement;
		int num = 0;
		string dataVersion = string.Empty;
		string text2 = string.Empty;
		foreach (XmlNode xmlNode in documentElement.ChildNodes)
		{
			if (xmlNode is XmlElement)
			{
				string attribute = (xmlNode as XmlElement).GetAttribute("bundleName");
				AssetLoader.BundleInfoXML bundleInfoXML = new AssetLoader.BundleInfoXML();
				bundleInfoXML.appVersion = (xmlNode as XmlElement).GetAttribute("appVersion");
				if (bundleInfoXML.appVersion == GlobalSettings.AppVersion)
				{
					GlobalSettings.DataVersion = attribute;
					text2 = "Ver " + GlobalSettings.AppVersion + "." + GlobalSettings.DataVersion.Substring(GlobalSettings.DataVersion.LastIndexOf("_") + 1);
				}
				string[] array = bundleInfoXML.appVersion.Split(new char[]
				{
					'.'
				});
				int num2 = int.Parse(int.Parse(array[0]).ToString("00") + int.Parse(array[1]).ToString("00") + int.Parse(array[2]).ToString("00"));
				if (num2 > num)
				{
					num = num2;
					dataVersion = attribute;
				}
				bundleInfoXML.bundleMD5 = (xmlNode as XmlElement).GetAttribute("bundleMD5");
				bundleInfoXML.assetType = int.Parse((xmlNode as XmlElement).GetAttribute("assetType"));
				bundleInfoXML.family = (xmlNode as XmlElement).GetAttribute("family");
				bundleInfoXML.bundleName = attribute;
				if ((xmlNode as XmlElement).HasAttribute("updateAppVersion"))
				{
					bundleInfoXML.updateAppVersion = (xmlNode as XmlElement).GetAttribute("updateAppVersion");
				}
				if (bundleInfoXML.appVersion == GlobalSettings.AppVersion && !this.m_bundleInfoDict.ContainsKey(attribute))
				{
					this.m_bundleInfoDict.Add(attribute, bundleInfoXML);
				}
				if (GlobalSettings.AppVersion == "0" || GlobalSettings.AppVersion == "0.0.0")
				{
					GlobalSettings.DataVersion = dataVersion;
					text2 = "Ver " + GlobalSettings.AppVersion + "." + GlobalSettings.DataVersion.Substring(GlobalSettings.DataVersion.LastIndexOf("_") + 1);
				}
			}
		}
	}

	private bool CheckCurrentVersion(string bundleName, string md5, string version)
	{
		if (PlayerPrefs.HasKey(bundleName))
		{
			string @string = PlayerPrefs.GetString(bundleName);
			if (@string == version)
			{
				return !this.CheckMD5Code(bundleName, md5, version);
			}
		}
		else
		{
			PlayerPrefs.SetInt(bundleName, 0);
		}
		return true;
	}

	public void CheckDownLoadAssets(AssetLoader.CompleteCallback2 onDownLoadAssetsFinished, AssetLoader.OnDownloading onDownloading)
	{
		new Task(this.CheckDownLoadAssetsFromServer(onDownLoadAssetsFinished, onDownloading), true);
	}

	public void GetDownLoadInfo(ref long nFreeSpase, ref long nNeedSpase)
	{
		nFreeSpase = this.m_nFreeSpase;
		nNeedSpase = this.m_nNeedSpase;
	}

	[DebuggerHidden]
	private IEnumerator CheckDownLoadAssetsFromServer(AssetLoader.CompleteCallback2 onDownLoadFinished, AssetLoader.OnDownloading onDownloading)
	{
		AssetLoader.<CheckDownLoadAssetsFromServer>c__Iterator11 <CheckDownLoadAssetsFromServer>c__Iterator = new AssetLoader.<CheckDownLoadAssetsFromServer>c__Iterator11();
		<CheckDownLoadAssetsFromServer>c__Iterator.onDownloading = onDownloading;
		<CheckDownLoadAssetsFromServer>c__Iterator.onDownLoadFinished = onDownLoadFinished;
		<CheckDownLoadAssetsFromServer>c__Iterator.<$>onDownloading = onDownloading;
		<CheckDownLoadAssetsFromServer>c__Iterator.<$>onDownLoadFinished = onDownLoadFinished;
		<CheckDownLoadAssetsFromServer>c__Iterator.<>f__this = this;
		return <CheckDownLoadAssetsFromServer>c__Iterator;
	}

	public void downLoadAssetsFromServer(AssetLoader.CompleteCallback2 onDownLoadAssetsFinished, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		if (this.m_bundles2Update == null)
		{
			UnityEngine.Debug.LogError("m_bundles2Update == null!");
		}
		new Task(this.downLoadAssetsFromServer2(onDownLoadAssetsFinished, onDownloading, callbackData), true);
	}

	[DebuggerHidden]
	private IEnumerator downLoadAssetsFromServer2(AssetLoader.CompleteCallback2 onDownLoadFinished, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		AssetLoader.<downLoadAssetsFromServer2>c__Iterator12 <downLoadAssetsFromServer2>c__Iterator = new AssetLoader.<downLoadAssetsFromServer2>c__Iterator12();
		<downLoadAssetsFromServer2>c__Iterator.onDownLoadFinished = onDownLoadFinished;
		<downLoadAssetsFromServer2>c__Iterator.onDownloading = onDownloading;
		<downLoadAssetsFromServer2>c__Iterator.callbackData = callbackData;
		<downLoadAssetsFromServer2>c__Iterator.<$>onDownLoadFinished = onDownLoadFinished;
		<downLoadAssetsFromServer2>c__Iterator.<$>onDownloading = onDownloading;
		<downLoadAssetsFromServer2>c__Iterator.<$>callbackData = callbackData;
		<downLoadAssetsFromServer2>c__Iterator.<>f__this = this;
		return <downLoadAssetsFromServer2>c__Iterator;
	}

	private void onOverAllProcessChanged(float overAllProcess)
	{
		MobaMessageManager.ExecuteMsg((ClientMsg)25006, overAllProcess, 0f);
	}

	[DebuggerHidden]
	private IEnumerator pullAssets(List<Bundle2Check> bundles, AssetLoader.CompleteCallback2 onDownLoadAssetsFinished, AssetLoader.OnDownloading onDownloading, object callbackData)
	{
		AssetLoader.<pullAssets>c__Iterator13 <pullAssets>c__Iterator = new AssetLoader.<pullAssets>c__Iterator13();
		<pullAssets>c__Iterator.bundles = bundles;
		<pullAssets>c__Iterator.onDownloading = onDownloading;
		<pullAssets>c__Iterator.onDownLoadAssetsFinished = onDownLoadAssetsFinished;
		<pullAssets>c__Iterator.<$>bundles = bundles;
		<pullAssets>c__Iterator.<$>onDownloading = onDownloading;
		<pullAssets>c__Iterator.<$>onDownLoadAssetsFinished = onDownLoadAssetsFinished;
		<pullAssets>c__Iterator.<>f__this = this;
		return <pullAssets>c__Iterator;
	}

	private bool CheckBundleExist(string bundleName)
	{
		string path = Application.persistentDataPath + "/" + bundleName + ".unity3d";
		return File.Exists(path);
	}

	private void DeleteBundle(string bundleName)
	{
		string path = Application.persistentDataPath + "/" + bundleName + ".unity3d";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	private void write2File(string filePathName, byte[] bytes)
	{
		if (File.Exists(filePathName))
		{
			File.Delete(filePathName);
		}
		FileStream fileStream = new FileStream(filePathName, FileMode.Create);
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Flush();
		fileStream.Close();
	}

	public static bool readFile(string filePathName, out byte[] bytes)
	{
		FileStream fileStream = new FileStream(filePathName, FileMode.Open);
		bool result = false;
		bytes = null;
		if (fileStream != null)
		{
			int num = (int)fileStream.Length;
			bytes = new byte[num];
			int num2 = fileStream.Read(bytes, 0, num);
			fileStream.Flush();
			fileStream.Close();
			result = (num2 == num);
		}
		return result;
	}

	private void DecompressFile(string inPathName, string outPathName)
	{
		if (File.Exists(outPathName))
		{
			File.Delete(outPathName);
		}
		Decoder decoder = new Decoder();
		FileStream fileStream = new FileStream(inPathName, FileMode.Open);
		FileStream fileStream2 = new FileStream(outPathName, FileMode.CreateNew);
		byte[] array = new byte[5];
		fileStream.Read(array, 0, 5);
		byte[] array2 = new byte[8];
		fileStream.Read(array2, 0, 8);
		long outSize = BitConverter.ToInt64(array2, 0);
		decoder.SetDecoderProperties(array);
		decoder.Code(fileStream, fileStream2, fileStream.Length, outSize, null);
		fileStream2.Flush();
		fileStream2.Close();
		fileStream.Flush();
		fileStream.Close();
		if (File.Exists(inPathName))
		{
			File.Delete(inPathName);
		}
	}
}
