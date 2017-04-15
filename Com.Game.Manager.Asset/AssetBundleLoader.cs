using Com.Game.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Com.Game.Manager.Asset
{
	public class AssetBundleLoader
	{
		private WWW downloader;

		private UnityEngine.Object[] allAsset;

		public AssetBundle assetBundle
		{
			get;
			private set;
		}

		public DownLoadState state
		{
			get;
			private set;
		}

		public int size
		{
			get;
			private set;
		}

		public string path
		{
			get;
			private set;
		}

		public string fileName
		{
			get;
			private set;
		}

		public bool isScene
		{
			get;
			private set;
		}

		public string assetName
		{
			get;
			set;
		}

		public bool cache
		{
			get;
			set;
		}

		public Type assetType
		{
			get;
			set;
		}

		public bool delayUnload
		{
			get;
			set;
		}

		public bool asynLoad
		{
			get;
			set;
		}

		public AsyncOperation loadSceneOperation
		{
			get;
			set;
		}

		public float progress
		{
			get
			{
				return this.getProgress();
			}
		}

		public AssetBundleLoader(string path, string fileName, Type type, string assetName = null, bool cache = false, DownLoadState state = DownLoadState.Init, bool isScene = false, bool asynLoad = false)
		{
			this.path = path;
			this.fileName = fileName;
			this.assetName = assetName;
			this.state = state;
			this.isScene = isScene;
			this.assetType = type;
			this.asynLoad = asynLoad;
		}

		private float getProgress()
		{
			float num = 0f;
			if (this.state == DownLoadState.Loaded || this.state == DownLoadState.Stored)
			{
				num = 1f;
			}
			else if (this.state == DownLoadState.Init || this.state == DownLoadState.LoadFailure || this.state == DownLoadState.StoreFailure)
			{
				num = 0f;
			}
			else if (this.state == DownLoadState.Loading)
			{
				num = this.downloader.progress;
			}
			if (this.isScene)
			{
				if (this.loadSceneOperation != null)
				{
					num += this.loadSceneOperation.progress;
				}
				num /= 2f;
			}
			return num;
		}

		[DebuggerHidden]
		public IEnumerator LoadAssetBundle(bool storeLocal = false, bool unload = false)
		{
			AssetBundleLoader.<LoadAssetBundle>c__Iterator46 <LoadAssetBundle>c__Iterator = new AssetBundleLoader.<LoadAssetBundle>c__Iterator46();
			<LoadAssetBundle>c__Iterator.storeLocal = storeLocal;
			<LoadAssetBundle>c__Iterator.unload = unload;
			<LoadAssetBundle>c__Iterator.<$>storeLocal = storeLocal;
			<LoadAssetBundle>c__Iterator.<$>unload = unload;
			<LoadAssetBundle>c__Iterator.<>f__this = this;
			return <LoadAssetBundle>c__Iterator;
		}

		[DebuggerHidden]
		public IEnumerator LoadAssetBundleCached(bool unload = false, int version = 1)
		{
			AssetBundleLoader.<LoadAssetBundleCached>c__Iterator47 <LoadAssetBundleCached>c__Iterator = new AssetBundleLoader.<LoadAssetBundleCached>c__Iterator47();
			<LoadAssetBundleCached>c__Iterator.version = version;
			<LoadAssetBundleCached>c__Iterator.unload = unload;
			<LoadAssetBundleCached>c__Iterator.<$>version = version;
			<LoadAssetBundleCached>c__Iterator.<$>unload = unload;
			<LoadAssetBundleCached>c__Iterator.<>f__this = this;
			return <LoadAssetBundleCached>c__Iterator;
		}

		private bool CreateFile(string path, string fileName, byte[] bytes)
		{
			Stream stream = null;
			string text = path + fileName;
			string path2 = text.Substring(0, text.LastIndexOf("/"));
			bool result = false;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(path2);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				FileInfo fileInfo = new FileInfo(text);
				stream = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write);
				stream.Write(bytes, 0, bytes.Length);
				result = true;
			}
			catch (IOException ex)
			{
				ClientLogger.Error(ex.Message);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
			}
			return result;
		}

		public UnityEngine.Object[] LoadAllAssets()
		{
			if (this.assetBundle != null && this.allAsset == null)
			{
				this.allAsset = this.assetBundle.LoadAll();
			}
			return this.allAsset;
		}

		public void UnloadAssetBundle(bool isAll = false)
		{
			if (this.assetBundle != null)
			{
				this.assetBundle.Unload(isAll);
			}
			this.allAsset = null;
		}

		private bool CreateAssetBundleFile(string fileName, byte[] bytes)
		{
			return this.CreateFile(Application.persistentDataPath + "/", fileName, bytes);
		}

		private void DeleteFile(string path, string fileName)
		{
			File.Delete(path + fileName);
		}
	}
}
