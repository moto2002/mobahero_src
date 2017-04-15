using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DownloadTexture : MonoBehaviour
{
	public UITexture uiTex;

	public string url = "http://www.yourwebsite.com/logo.png";

	public bool pixelPerfect = true;

	[HideInInspector]
	public bool isFromLocal;

	[HideInInspector]
	public string texName;

	private Texture2D mTex;

	private CoroutineManager corMgr = new CoroutineManager();

	private void Awake()
	{
		TextureFormat format = TextureFormat.RGB24;
		this.mTex = new Texture2D(848, 366, format, false);
		this.mTex.name = "DownloadTexture_Awake_" + Time.time.ToString();
	}

	private void Start()
	{
		this.corMgr.StartCoroutine(this.DownLoadPic(), true);
	}

	[DebuggerHidden]
	private IEnumerator DownLoadPic()
	{
		DownloadTexture.<DownLoadPic>c__Iterator6 <DownLoadPic>c__Iterator = new DownloadTexture.<DownLoadPic>c__Iterator6();
		<DownLoadPic>c__Iterator.<>f__this = this;
		return <DownLoadPic>c__Iterator;
	}

	private void SetTexture()
	{
		if (this.uiTex != null)
		{
			this.uiTex.mainTexture = this.mTex;
			this.uiTex.width = 848;
			this.uiTex.height = 366;
		}
	}

	private void OnDestroy()
	{
		if (this.mTex != null)
		{
			UnityEngine.Object.Destroy(this.mTex);
		}
	}
}
