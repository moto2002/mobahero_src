using System;
using UnityEngine;

namespace CsSdk
{
	public class CsRecInterface
	{
		private class AndroidOnlineListener : AndroidJavaProxy
		{
			private ICsRecListener listener;

			public AndroidOnlineListener(ICsRecListener listener) : base("tv.chushou.recordsdk.record.OnlineStatusCallback")
			{
				this.listener = listener;
			}

			public void onSuccess()
			{
				if (this.listener != null)
				{
					this.listener.onSuccess();
				}
			}

			public void onFailure(string msg)
			{
				if (this.listener != null)
				{
					this.listener.onFailure(msg);
				}
			}

			public void offline()
			{
				if (this.listener != null)
				{
					this.listener.offline();
				}
			}
		}

		private class AndroidLocalListener : AndroidJavaProxy
		{
			private ICsLocalRecListener listener;

			public AndroidLocalListener(ICsLocalRecListener listener) : base("tv.chushou.recordsdk.record.LocalCallback")
			{
				this.listener = listener;
			}

			public void onFailure(string msg)
			{
				if (this.listener != null)
				{
					this.listener.onFailure(msg);
				}
			}

			public void onRecordStart()
			{
				if (this.listener != null)
				{
					this.listener.onRecordStart();
				}
			}

			public void onRecordFinish()
			{
				if (this.listener != null)
				{
					this.listener.onRecordFinish();
				}
			}
		}

		public const int ORIENTATION_PORTRAIT = 0;

		public const int ORIENTATION_HORIZONTAL = 1;

		public const int RESOLUTION_SD = 0;

		public const int RESOLUTION_HD = 1;

		public const int RESOLUTION_UHD = 2;

		private const string appKey = "a18f55afe9b38e4d";

		private const string appSecret = "9a36521971e058078b5374b22f3a9a90";

		private AndroidJavaObject mSDK;

		private CustomUIOpHelper mRecHelper;

		public void init()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.chushou.recordsdk.ChuShouRecord");
			this.mSDK = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("tv.chushou.recordsdk.ChuShouRecConfig", new object[0]);
			androidJavaObject2.Call("setAppKey", new object[]
			{
				"a18f55afe9b38e4d"
			});
			androidJavaObject2.Call("setAppSecret", new object[]
			{
				"9a36521971e058078b5374b22f3a9a90"
			});
			androidJavaObject2.Call("enableShare", new object[]
			{
				true
			});
			androidJavaObject2.Call("enableAudioShare", new object[]
			{
				false
			});
			androidJavaObject2.Call("enableDefaultUI", new object[]
			{
				true
			});
			androidJavaObject2.Call("setDebug", new object[]
			{
				false
			});
			this.mSDK.Call("initialize", new object[]
			{
				androidJavaObject,
				androidJavaObject2
			});
			this.mRecHelper = new CustomUIOpHelper();
		}

		public CustomUIOpHelper getCustomUIOpHelper()
		{
			return this.mRecHelper;
		}

		public void startLive(string gameName, string title, string gameUid, string gameToken, string phone, string nickName, string extraData, int orentation, int resolution, ICsRecListener callback)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.recordsdk.datastruct.GameUserInfo", new object[0]);
			androidJavaObject.Set<string>("mGameUid", gameUid);
			androidJavaObject.Set<string>("gameToken", gameToken);
			androidJavaObject.Set<string>("phone", phone);
			androidJavaObject.Set<string>("nickName", nickName);
			androidJavaObject.Set<string>("gameExtraData", extraData);
			CsRecInterface.AndroidOnlineListener androidOnlineListener = null;
			if (callback != null)
			{
				androidOnlineListener = new CsRecInterface.AndroidOnlineListener(callback);
			}
			this.mSDK.Call("startOnlineRecord", new object[]
			{
				@static,
				androidJavaObject,
				gameName,
				title,
				orentation,
				resolution,
				androidOnlineListener
			});
		}

		public void startRecordFile(string gameName, string gameUid, string gameToken, string phone, string nickName, string extraData, int orentation, int resolution, ICsLocalRecListener callback)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.recordsdk.datastruct.GameUserInfo", new object[0]);
			androidJavaObject.Set<string>("mGameUid", gameUid);
			androidJavaObject.Set<string>("gameToken", gameToken);
			androidJavaObject.Set<string>("phone", phone);
			androidJavaObject.Set<string>("nickName", nickName);
			androidJavaObject.Set<string>("gameExtraData", extraData);
			CsRecInterface.AndroidLocalListener androidLocalListener = null;
			if (callback != null)
			{
				androidLocalListener = new CsRecInterface.AndroidLocalListener(callback);
			}
			this.mSDK.Call("startLocalRecord", new object[]
			{
				@static,
				androidJavaObject,
				gameName,
				orentation,
				resolution,
				androidLocalListener
			});
		}

		public void openVideoManager(string gameName, string gameUid, string gameToken, string extraData)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.recordsdk.datastruct.GameUserInfo", new object[0]);
			androidJavaObject.Set<string>("mGameUid", gameUid);
			androidJavaObject.Set<string>("gameToken", gameToken);
			androidJavaObject.Set<string>("gameExtraData", extraData);
			this.mSDK.Call("openVideoManager", new object[]
			{
				@static,
				androidJavaObject,
				gameName
			});
		}

		public bool isOnlineRecordRunning()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			return this.mSDK.Call<bool>("isRecorderRunning", new object[]
			{
				@static
			});
		}
	}
}
