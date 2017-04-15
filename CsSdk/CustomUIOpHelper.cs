using System;
using UnityEngine;

namespace CsSdk
{
	public class CustomUIOpHelper
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

		private class AndroidChatListener : AndroidJavaProxy
		{
			private ICsChatListener listener;

			public AndroidChatListener(ICsChatListener listener) : base("tv.chushou.recordsdk.OnlineChatCallback")
			{
				this.listener = listener;
			}

			public void onNewMsg(string newMsgJson)
			{
				if (this.listener != null)
				{
					this.listener.onNewMsg(newMsgJson);
				}
			}

			public void onPerNum(int count)
			{
				if (this.listener != null)
				{
					this.listener.onPerNum(count);
				}
			}
		}

		private AndroidJavaObject mCSHelper;

		public CustomUIOpHelper()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.chushou.recordsdk.ChuShouRecord");
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			this.mCSHelper = androidJavaObject.Call<AndroidJavaObject>("getCustomUIOpHelper", new object[0]);
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
			CustomUIOpHelper.AndroidOnlineListener androidOnlineListener = null;
			if (callback != null)
			{
				androidOnlineListener = new CustomUIOpHelper.AndroidOnlineListener(callback);
			}
			this.mCSHelper.Call("startOnlineRecord", new object[]
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
			CustomUIOpHelper.AndroidLocalListener androidLocalListener = null;
			if (callback != null)
			{
				androidLocalListener = new CustomUIOpHelper.AndroidLocalListener(callback);
			}
			this.mCSHelper.Call("startLocalRecord", new object[]
			{
				@static,
				androidJavaObject,
				gameName,
				orentation,
				resolution,
				androidLocalListener
			});
		}

		public bool isOnlineRecordRunning()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			return this.mCSHelper.Call<bool>("isRecorderRunning", new object[]
			{
				@static
			});
		}

		public void stop()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			this.mCSHelper.Call("stopRecord", new object[]
			{
				@static
			});
		}

		public bool isPrivacy()
		{
			return this.mCSHelper.Call<bool>("isPrivacy", new object[0]);
		}

		public void setPrivacy(bool isPrivacy)
		{
			this.mCSHelper.Call("setPrivacy", new object[]
			{
				isPrivacy
			});
		}

		public bool isCameraOpened()
		{
			return this.mCSHelper.Call<bool>("isCameraOpened", new object[0]);
		}

		public void toggleCamera(bool on)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			this.mCSHelper.Call("toggleCamera", new object[]
			{
				@static,
				on
			});
		}

		public void setChatCallback(ICsChatListener callback)
		{
			CustomUIOpHelper.AndroidChatListener androidChatListener = null;
			if (callback != null)
			{
				androidChatListener = new CustomUIOpHelper.AndroidChatListener(callback);
			}
			this.mCSHelper.Call("setChatCallback", new object[]
			{
				androidChatListener
			});
		}
	}
}
