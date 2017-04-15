using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MediaPlayerCtrl : MonoBehaviour
{
	public enum MEDIAPLAYER_ERROR
	{
		MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200,
		MEDIA_ERROR_IO = -1004,
		MEDIA_ERROR_MALFORMED = -1007,
		MEDIA_ERROR_TIMED_OUT = -110,
		MEDIA_ERROR_UNSUPPORTED = -1010,
		MEDIA_ERROR_SERVER_DIED = 100,
		MEDIA_ERROR_UNKNOWN = 1
	}

	public enum MEDIAPLAYER_STATE
	{
		NOT_READY,
		READY,
		END,
		PLAYING,
		PAUSED,
		STOPPED,
		ERROR
	}

	public enum MEDIA_SCALE
	{
		SCALE_X_TO_Y,
		SCALE_X_TO_Z,
		SCALE_Y_TO_X,
		SCALE_Y_TO_Z,
		SCALE_Z_TO_X,
		SCALE_Z_TO_Y
	}

	public delegate void VideoEnd();

	public delegate void VideoReady();

	public delegate void VideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra);

	public delegate void VideoFirstFrameReady();

	public string m_strFileName;

	public GameObject m_TargetMaterial;

	private Texture2D m_VideoTexture;

	private Texture2D m_VideoTextureDummy;

	private MediaPlayerCtrl.MEDIAPLAYER_STATE m_CurrentState;

	private int m_iCurrentSeekPosition;

	private float m_fVolume = 1f;

	public bool m_bFullScreen;

	public bool m_bSupportRockchip;

	public MediaPlayerCtrl.VideoReady OnReady;

	public MediaPlayerCtrl.VideoEnd OnEnd;

	public MediaPlayerCtrl.VideoError OnVideoError;

	public MediaPlayerCtrl.VideoFirstFrameReady OnVideoFirstFrameReady;

	public bool isInit;

	private int m_iAndroidMgrID;

	private bool m_bIsFirstFrameReady;

	private bool m_bFirst;

	public MediaPlayerCtrl.MEDIA_SCALE m_ScaleValue;

	public GameObject m_objResize;

	public bool m_bLoop;

	public bool m_bAutoPlay = true;

	private bool m_bStop;

	public bool m_bInit;

	private bool m_bCheckFBO;

	private bool m_bPause;

	private AndroidJavaObject javaObj;

	private void Awake()
	{
		if (SystemInfo.deviceModel.Contains("rockchip"))
		{
			this.m_bSupportRockchip = true;
		}
		else
		{
			this.m_bSupportRockchip = false;
		}
	}

	private void Start()
	{
		this.m_iAndroidMgrID = this.Call_InitNDK();
		this.Call_SetUnityActivity();
		if (Application.dataPath.Contains(".obb"))
		{
			this.Call_SetSplitOBB(true, Application.dataPath);
		}
		else
		{
			this.Call_SetSplitOBB(false, null);
		}
		this.m_bInit = true;
	}

	private void OnApplicationQuit()
	{
		if (Directory.Exists(Application.persistentDataPath + "/Data"))
		{
			Directory.Delete(Application.persistentDataPath + "/Data", true);
		}
	}

	private void OnDisable()
	{
		if (this.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
		{
			this.Pause();
		}
	}

	private void OnEnable()
	{
		if (this.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
		{
			this.Play();
		}
	}

	private void Update()
	{
		if (string.IsNullOrEmpty(this.m_strFileName))
		{
			return;
		}
		if (!this.m_bFirst)
		{
			string text = this.m_strFileName.Trim();
			if (this.m_bSupportRockchip)
			{
				this.Call_SetRockchip(this.m_bSupportRockchip);
				if (text.Contains("://"))
				{
					this.Call_Load(text, 0);
				}
				else
				{
					base.StartCoroutine(this.CopyStreamingAssetVideoAndLoad(text));
				}
			}
			else
			{
				this.Call_Load(text, 0);
			}
			this.Call_SetLooping(this.m_bLoop);
			this.m_bFirst = true;
		}
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
		{
			if (!this.m_bCheckFBO)
			{
				if (this.Call_GetVideoWidth() <= 0 || this.Call_GetVideoHeight() <= 0)
				{
					return;
				}
				this.Resize();
				if (this.m_VideoTexture != null)
				{
					if (this.m_VideoTextureDummy != null)
					{
						UnityEngine.Object.Destroy(this.m_VideoTextureDummy);
						this.m_VideoTextureDummy = null;
					}
					this.m_VideoTextureDummy = this.m_VideoTexture;
					this.m_VideoTexture = null;
				}
				if (this.m_bSupportRockchip)
				{
					this.m_VideoTexture = new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGB565, false);
					this.m_VideoTexture.name = "MediaPlayCtrl_1_" + Time.time.ToString();
				}
				else
				{
					this.m_VideoTexture = new Texture2D(this.Call_GetVideoWidth(), this.Call_GetVideoHeight(), TextureFormat.RGBA32, false);
					this.m_VideoTexture.name = "MediaPlayCtrl_2_" + Time.time.ToString();
				}
				this.m_VideoTexture.filterMode = FilterMode.Bilinear;
				this.m_VideoTexture.wrapMode = TextureWrapMode.Clamp;
				this.Call_SetUnityTexture(this.m_VideoTexture.GetNativeTextureID());
				this.Call_SetWindowSize();
				this.m_bCheckFBO = true;
			}
			this.Call_UpdateVideoTexture();
			this.m_iCurrentSeekPosition = this.Call_GetSeekPosition();
		}
		if (this.m_CurrentState != this.Call_GetStatus())
		{
			this.m_CurrentState = this.Call_GetStatus();
			if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
			{
				if (this.OnReady != null)
				{
					this.OnReady();
				}
				if (this.m_bAutoPlay)
				{
					this.Call_Play(0);
				}
				this.SetVolume(this.m_fVolume);
			}
			else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				if (this.OnEnd != null)
				{
					this.OnEnd();
				}
				if (this.m_bLoop)
				{
					this.Call_Play(0);
				}
			}
			else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.ERROR)
			{
				this.OnError((MediaPlayerCtrl.MEDIAPLAYER_ERROR)this.Call_GetError(), (MediaPlayerCtrl.MEDIAPLAYER_ERROR)this.Call_GetErrorExtra());
			}
		}
	}

	public void Resize()
	{
		if (this.m_CurrentState != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
		{
			return;
		}
		if (this.m_objResize != null && !this.isInit)
		{
			int width = Screen.width;
			int height = Screen.height;
			float num = (float)height / (float)width;
			int num2 = this.Call_GetVideoWidth();
			int num3 = this.Call_GetVideoHeight();
			float num4 = (float)num3 / (float)num2;
			float d = num / num4;
			if (this.m_bFullScreen)
			{
				if (num4 < 1f)
				{
					if (num < 1f && num4 > num)
					{
						this.m_objResize.transform.localScale *= d;
					}
					this.m_ScaleValue = MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y;
				}
				else
				{
					if (num > 1f && num4 > num)
					{
						this.m_objResize.transform.localScale *= d;
					}
					this.m_ScaleValue = MediaPlayerCtrl.MEDIA_SCALE.SCALE_Y_TO_X;
				}
			}
			if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Y)
			{
				float num5 = 1f;
				if ((float)Screen.width / (float)Screen.height < 1.77777779f)
				{
					num5 = 1.77777779f / ((float)Screen.width / (float)Screen.height);
				}
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.x * num5, this.m_objResize.transform.localScale.x * num4 * num5, this.m_objResize.transform.localScale.z);
			}
			else if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_X_TO_Z)
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.x, this.m_objResize.transform.localScale.y, this.m_objResize.transform.localScale.x * num4);
			}
			else if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_Y_TO_X)
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.y / num4, this.m_objResize.transform.localScale.y, this.m_objResize.transform.localScale.z);
			}
			else if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_Y_TO_Z)
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.x, this.m_objResize.transform.localScale.y, this.m_objResize.transform.localScale.y / num4);
			}
			else if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_Z_TO_X)
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.z * num4, this.m_objResize.transform.localScale.y, this.m_objResize.transform.localScale.z);
			}
			else if (this.m_ScaleValue == MediaPlayerCtrl.MEDIA_SCALE.SCALE_Z_TO_Y)
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.x, this.m_objResize.transform.localScale.z * num4, this.m_objResize.transform.localScale.z);
			}
			else
			{
				this.m_objResize.transform.localScale = new Vector3(this.m_objResize.transform.localScale.x, this.m_objResize.transform.localScale.y, this.m_objResize.transform.localScale.z);
			}
			this.isInit = true;
		}
	}

	private void OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR iCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR iCodeExtra)
	{
		string text = string.Empty;
		if (iCode != MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN)
		{
			if (iCode != MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_SERVER_DIED)
			{
				if (iCode != MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK)
				{
					text = "Unknown error " + iCode;
				}
				else
				{
					text = "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK";
				}
			}
			else
			{
				text = "MEDIA_ERROR_SERVER_DIED";
			}
		}
		else
		{
			text = "MEDIA_ERROR_UNKNOWN";
		}
		text += " ";
		switch (iCodeExtra + 1010)
		{
		case (MediaPlayerCtrl.MEDIAPLAYER_ERROR)0:
			text += "MEDIA_ERROR_UNSUPPORTED";
			goto IL_FA;
		case MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN:
		case (MediaPlayerCtrl.MEDIAPLAYER_ERROR)2:
			IL_88:
			if (iCodeExtra == MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_IO)
			{
				text += "MEDIA_ERROR_IO";
				goto IL_FA;
			}
			if (iCodeExtra != MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_TIMED_OUT)
			{
				text = "Unknown error " + iCode;
				goto IL_FA;
			}
			text += "MEDIA_ERROR_TIMED_OUT";
			goto IL_FA;
		case (MediaPlayerCtrl.MEDIAPLAYER_ERROR)3:
			text += "MEDIA_ERROR_MALFORMED";
			goto IL_FA;
		}
		goto IL_88;
		IL_FA:
		UnityEngine.Debug.LogError(text);
		if (this.OnVideoError != null)
		{
			this.OnVideoError(iCode, iCodeExtra);
		}
	}

	private void OnDestroy()
	{
		this.Call_UnLoad();
		if (this.m_VideoTextureDummy != null)
		{
			UnityEngine.Object.Destroy(this.m_VideoTextureDummy);
			this.m_VideoTextureDummy = null;
		}
		if (this.m_VideoTexture != null)
		{
			UnityEngine.Object.Destroy(this.m_VideoTexture);
		}
		this.Call_Destroy();
	}

	private void OnApplicationPause(bool bPause)
	{
		UnityEngine.Debug.Log("ApplicationPause : " + bPause);
		if (bPause)
		{
			if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
			{
				this.m_bPause = true;
			}
			this.Call_Pause();
		}
		else
		{
			this.Call_RePlay();
			if (this.m_bPause)
			{
				this.Call_Pause();
				this.m_bPause = false;
			}
		}
	}

	public MediaPlayerCtrl.MEDIAPLAYER_STATE GetCurrentState()
	{
		return this.m_CurrentState;
	}

	public Texture2D GetVideoTexture()
	{
		return this.m_VideoTexture;
	}

	public void Play()
	{
		if (this.m_bStop)
		{
			this.Call_Play(0);
			this.m_bStop = false;
		}
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
		{
			this.Call_RePlay();
		}
		else if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
		{
			this.Call_Play(0);
		}
	}

	public void Stop()
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
		{
			this.Call_Pause();
		}
		this.m_bStop = true;
		this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED;
		this.m_iCurrentSeekPosition = 0;
	}

	public void Pause()
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
		{
			this.Call_Pause();
		}
		this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED;
	}

	public void Load(string strFileName)
	{
		if (this.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
		{
			this.UnLoad();
		}
		this.m_bFirst = false;
		this.m_bCheckFBO = false;
		this.m_strFileName = strFileName;
		if (!this.m_bInit)
		{
			return;
		}
		this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY;
	}

	public void SetVolume(float fVolume)
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
		{
			this.m_fVolume = fVolume;
			this.Call_SetVolume(fVolume);
		}
	}

	public int GetSeekPosition()
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
		{
			return this.m_iCurrentSeekPosition;
		}
		return 0;
	}

	public void SeekTo(int iSeek)
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
		{
			this.Call_SetSeekPosition(iSeek);
		}
	}

	public int GetDuration()
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
		{
			return this.Call_GetDuration();
		}
		return 0;
	}

	public int GetCurrentSeekPercent()
	{
		if (this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || this.m_CurrentState == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY)
		{
			return this.Call_GetCurrentSeekPercent();
		}
		return 0;
	}

	public int GetVideoWidth()
	{
		return this.Call_GetVideoWidth();
	}

	public int GetVideoHeight()
	{
		return this.Call_GetVideoHeight();
	}

	public void UnLoad()
	{
		this.m_bCheckFBO = false;
		this.Call_UnLoad();
		this.m_CurrentState = MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY;
	}

	private AndroidJavaObject GetJavaObject()
	{
		if (this.javaObj == null)
		{
			this.javaObj = new AndroidJavaObject("com.EasyMovieTexture.EasyMovieTexture", new object[0]);
		}
		return this.javaObj;
	}

	private void Call_Destroy()
	{
		this.GetJavaObject().Call("Destroy", new object[0]);
	}

	private void Call_UnLoad()
	{
		this.GetJavaObject().Call("UnLoad", new object[0]);
	}

	private bool Call_Load(string strFileName, int iSeek)
	{
		this.GetJavaObject().Call("NDK_SetFileName", new object[]
		{
			strFileName
		});
		if (this.GetJavaObject().Call<bool>("Load", new object[0]))
		{
			return true;
		}
		this.OnError(MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN, MediaPlayerCtrl.MEDIAPLAYER_ERROR.MEDIA_ERROR_UNKNOWN);
		return false;
	}

	private void Call_UpdateVideoTexture()
	{
		if (!this.Call_IsUpdateFrame())
		{
			return;
		}
		if (this.m_VideoTextureDummy != null)
		{
			UnityEngine.Object.Destroy(this.m_VideoTextureDummy);
			this.m_VideoTextureDummy = null;
		}
		if (this.m_TargetMaterial)
		{
			if (this.m_TargetMaterial.GetComponent<MeshRenderer>() != null && this.m_TargetMaterial.GetComponent<MeshRenderer>().material.mainTexture != this.m_VideoTexture)
			{
				this.m_TargetMaterial.GetComponent<MeshRenderer>().material.mainTexture = this.m_VideoTexture;
			}
			if (this.m_TargetMaterial.GetComponent<RawImage>() != null && this.m_TargetMaterial.GetComponent<RawImage>().texture != this.m_VideoTexture)
			{
				this.m_TargetMaterial.GetComponent<RawImage>().texture = this.m_VideoTexture;
			}
		}
		this.GetJavaObject().Call("UpdateVideoTexture", new object[0]);
		if (!this.m_bIsFirstFrameReady)
		{
			this.m_bIsFirstFrameReady = true;
			if (this.OnVideoFirstFrameReady != null)
			{
				this.OnVideoFirstFrameReady();
				this.OnVideoFirstFrameReady = null;
			}
		}
	}

	private void Call_SetVolume(float fVolume)
	{
		this.GetJavaObject().Call("SetVolume", new object[]
		{
			fVolume
		});
	}

	private void Call_SetSeekPosition(int iSeek)
	{
		this.GetJavaObject().Call("SetSeekPosition", new object[]
		{
			iSeek
		});
	}

	private int Call_GetSeekPosition()
	{
		return this.GetJavaObject().Call<int>("GetSeekPosition", new object[0]);
	}

	private void Call_Play(int iSeek)
	{
		this.GetJavaObject().Call("Play", new object[]
		{
			iSeek
		});
	}

	private void Call_Reset()
	{
		this.GetJavaObject().Call("Reset", new object[0]);
	}

	private void Call_Stop()
	{
		this.GetJavaObject().Call("Stop", new object[0]);
	}

	private void Call_RePlay()
	{
		this.GetJavaObject().Call("RePlay", new object[0]);
	}

	private void Call_Pause()
	{
		this.GetJavaObject().Call("Pause", new object[0]);
	}

	private int Call_InitNDK()
	{
		return this.GetJavaObject().Call<int>("InitNative", new object[]
		{
			this.GetJavaObject()
		});
	}

	private int Call_GetVideoWidth()
	{
		return this.GetJavaObject().Call<int>("GetVideoWidth", new object[0]);
	}

	private int Call_GetVideoHeight()
	{
		return this.GetJavaObject().Call<int>("GetVideoHeight", new object[0]);
	}

	private bool Call_IsUpdateFrame()
	{
		return this.GetJavaObject().Call<bool>("IsUpdateFrame", new object[0]);
	}

	private void Call_SetUnityTexture(int iTextureID)
	{
		this.GetJavaObject().Call("SetUnityTexture", new object[]
		{
			iTextureID
		});
	}

	private void Call_SetWindowSize()
	{
		this.GetJavaObject().Call("SetWindowSize", new object[0]);
	}

	private void Call_SetLooping(bool bLoop)
	{
		this.GetJavaObject().Call("SetLooping", new object[]
		{
			bLoop
		});
	}

	private void Call_SetRockchip(bool bValue)
	{
		this.GetJavaObject().Call("SetRockchip", new object[]
		{
			bValue
		});
	}

	private int Call_GetDuration()
	{
		return this.GetJavaObject().Call<int>("GetDuration", new object[0]);
	}

	private int Call_GetCurrentSeekPercent()
	{
		return this.GetJavaObject().Call<int>("GetCurrentSeekPercent", new object[0]);
	}

	private int Call_GetError()
	{
		return this.GetJavaObject().Call<int>("GetError", new object[0]);
	}

	private void Call_SetSplitOBB(bool bValue, string strOBBName)
	{
		this.GetJavaObject().Call("SetSplitOBB", new object[]
		{
			bValue,
			strOBBName
		});
	}

	private int Call_GetErrorExtra()
	{
		return this.GetJavaObject().Call<int>("GetErrorExtra", new object[0]);
	}

	private void Call_SetUnityActivity()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		this.GetJavaObject().Call("SetUnityActivity", new object[]
		{
			@static
		});
		this.Call_InitJniManager();
	}

	private void Call_SetNotReady()
	{
		this.GetJavaObject().Call("SetNotReady", new object[0]);
	}

	private void Call_InitJniManager()
	{
		this.GetJavaObject().Call("InitJniManager", new object[0]);
	}

	private MediaPlayerCtrl.MEDIAPLAYER_STATE Call_GetStatus()
	{
		return (MediaPlayerCtrl.MEDIAPLAYER_STATE)this.GetJavaObject().Call<int>("GetStatus", new object[0]);
	}

	[DebuggerHidden]
	private IEnumerator DownloadStreamingVideoAndLoad(string strURL)
	{
		MediaPlayerCtrl.<DownloadStreamingVideoAndLoad>c__IteratorE <DownloadStreamingVideoAndLoad>c__IteratorE = new MediaPlayerCtrl.<DownloadStreamingVideoAndLoad>c__IteratorE();
		<DownloadStreamingVideoAndLoad>c__IteratorE.strURL = strURL;
		<DownloadStreamingVideoAndLoad>c__IteratorE.<$>strURL = strURL;
		<DownloadStreamingVideoAndLoad>c__IteratorE.<>f__this = this;
		return <DownloadStreamingVideoAndLoad>c__IteratorE;
	}

	[DebuggerHidden]
	private IEnumerator CopyStreamingAssetVideoAndLoad(string strURL)
	{
		MediaPlayerCtrl.<CopyStreamingAssetVideoAndLoad>c__IteratorF <CopyStreamingAssetVideoAndLoad>c__IteratorF = new MediaPlayerCtrl.<CopyStreamingAssetVideoAndLoad>c__IteratorF();
		<CopyStreamingAssetVideoAndLoad>c__IteratorF.strURL = strURL;
		<CopyStreamingAssetVideoAndLoad>c__IteratorF.<$>strURL = strURL;
		<CopyStreamingAssetVideoAndLoad>c__IteratorF.<>f__this = this;
		return <CopyStreamingAssetVideoAndLoad>c__IteratorF;
	}
}
