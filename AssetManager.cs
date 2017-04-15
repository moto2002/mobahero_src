using Com.Game.Manager;
using Com.Game.Module;
using System;

public class AssetManager
{
	private static AssetManager m_Instance;

	public static AssetManager Instance
	{
		get
		{
			if (AssetManager.m_Instance == null)
			{
				AssetManager.m_Instance = new AssetManager();
			}
			return AssetManager.m_Instance;
		}
	}

	public void Init()
	{
	}

	private void OnDestroy()
	{
	}

	public void DownloadBinData(string url, Callback loadDataCallback)
	{
		bool useLocalData = GlobalSettings.useLocalData;
		AssetLoader.Instance.Init(useLocalData, url);
		if (!this.CheckBindata())
		{
			AssetLoader.Instance.DownloadBindata(new AssetLoader.CompleteCallback2(this.onDownloadBinDataFinished), new AssetLoader.OnDownloading(this.onDownloadBindataProgress), null);
		}
		else
		{
			this.onDownloadBinDataFinished(EAssetLoadError.eSuccess, null, url);
		}
	}

	private bool CheckBindata()
	{
		return AssetLoader.Instance.CheckBindata();
	}

	public string GetBindataMD5()
	{
		return AssetLoader.Instance.GetBindataMD5();
	}

	public void downLoadAssets()
	{
		AssetLoader.Instance.downLoadAssetsFromServer(new AssetLoader.CompleteCallback2(this.onDownloadAssetsFinished), new AssetLoader.OnDownloading(this.onDownloadAssetProgress), null);
	}

	public void CheckDownLoadAssets()
	{
		AssetLoader.Instance.CheckDownLoadAssets(new AssetLoader.CompleteCallback2(this.onCheckAssetFinished), new AssetLoader.OnDownloading(this.onCheckAssetProgress));
	}

	public void GetDownLoadInfo(ref long nFreeSpase, ref long nNeedSpase)
	{
		AssetLoader.Instance.GetDownLoadInfo(ref nFreeSpase, ref nNeedSpase);
	}

	public void LoadData_2(bool islocal, string url)
	{
		AssetLoader.Instance.Init(islocal, url);
		byte[] data;
		AssetLoader.readFile(AssetLoader.Instance.PathName_bindata, out data);
		this.InitData(data);
	}

	private void onDownloadBindataProgress(string name, float progress, object callbackData)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.DownLoadBinDataProgress, progress, false);
	}

	private void onDownloadBinDataFinished(EAssetLoadError e, object extra, string url)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.DownLoadBinDataOver, e, true);
	}

	private void onOverAllProcessChanged(float overallProcess)
	{
	}

	private void onDownloadAssetProgress(string name, float progress, object callbackData)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.DownLoadResourceProgress, new object[]
		{
			name,
			progress,
			callbackData
		}, false);
	}

	private void onDownloadAssetsFinished(EAssetLoadError e, object extra, string url)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.DownLoadResourceOver, new object[]
		{
			e,
			extra,
			url
		}, false);
	}

	private void onCheckAssetFinished(EAssetLoadError e, object extra, string url)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.CheckResourceOver, new object[]
		{
			e,
			extra,
			url
		}, false);
	}

	private void onCheckAssetProgress(string name, float progress, object callbackData)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.CheckResourceProgress, new object[]
		{
			name,
			progress,
			callbackData
		}, false);
	}

	private void InitData(byte[] data)
	{
		if (data != null)
		{
			BaseDataMgr.instance.InitBaseConfigData(data);
		}
		else
		{
			CtrlManager.ShowMsgBox(string.Empty, "配置文件错误!", new Action(this.QuitApp), PopViewType.PopOneButton, "确定", "取消", null);
		}
	}

	private void QuitApp()
	{
		GlobalObject.QuitApp();
	}
}
