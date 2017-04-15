using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace cn.sharesdk.unity3d
{
	public class ShareSDK : MonoBehaviour
	{
		public delegate void EventHandler(int reqID, ResponseState state, PlatformType type, Hashtable data);

		private int reqID;

		public string appKey = "14d1a18426850";

		public DevInfoSet devInfo;

		public ShareSDKImpl shareSDKUtils;

		public ShareSDK.EventHandler authHandler;

		public ShareSDK.EventHandler shareHandler;

		public ShareSDK.EventHandler showUserHandler;

		public ShareSDK.EventHandler getFriendsHandler;

		public ShareSDK.EventHandler followFriendHandler;

		private void Awake()
		{
			if (Application.isEditor)
			{
				return;
			}
			MonoBehaviour.print("ShareSDK Awake");
			Type type = this.devInfo.GetType();
			Hashtable hashtable = new Hashtable();
			FieldInfo[] fields = type.GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				DevInfo devInfo = (DevInfo)fieldInfo.GetValue(this.devInfo);
				int num = (int)devInfo.GetType().GetField("type").GetValue(devInfo);
				FieldInfo[] fields2 = devInfo.GetType().GetFields();
				Hashtable hashtable2 = new Hashtable();
				FieldInfo[] array2 = fields2;
				for (int j = 0; j < array2.Length; j++)
				{
					FieldInfo fieldInfo2 = array2[j];
					if (!"type".EndsWith(fieldInfo2.Name))
					{
						if ("Enable".EndsWith(fieldInfo2.Name) || "ShareByAppClient".EndsWith(fieldInfo2.Name) || "BypassApproval".EndsWith(fieldInfo2.Name))
						{
							hashtable2.Add(fieldInfo2.Name, Convert.ToString(fieldInfo2.GetValue(devInfo)).ToLower());
						}
						else
						{
							hashtable2.Add(fieldInfo2.Name, Convert.ToString(fieldInfo2.GetValue(devInfo)));
						}
					}
				}
				hashtable.Add(num, hashtable2);
			}
			this.shareSDKUtils = new AndroidImpl(base.gameObject);
			this.shareSDKUtils.InitSDK(this.appKey);
			this.shareSDKUtils.SetPlatformConfig(hashtable);
		}

		private void _Callback(string data)
		{
			if (data == null)
			{
				return;
			}
			Hashtable hashtable = (Hashtable)MiniJSON.jsonDecode(data);
			if (hashtable == null || hashtable.Count <= 0)
			{
				return;
			}
			int num = Convert.ToInt32(hashtable["status"]);
			int num2 = Convert.ToInt32(hashtable["reqID"]);
			PlatformType platform = (PlatformType)Convert.ToInt32(hashtable["platform"]);
			int action = Convert.ToInt32(hashtable["action"]);
			switch (num)
			{
			case 1:
			{
				Console.WriteLine(data);
				Hashtable res = (Hashtable)hashtable["res"];
				this.OnComplete(num2, platform, action, res);
				break;
			}
			case 2:
			{
				Console.WriteLine(data);
				Hashtable throwable = (Hashtable)hashtable["res"];
				this.OnError(num2, platform, action, throwable);
				break;
			}
			case 3:
				this.OnCancel(num2, platform, action);
				break;
			}
		}

		public void OnError(int reqID, PlatformType platform, int action, Hashtable throwable)
		{
			switch (action)
			{
			case 1:
				if (this.authHandler != null)
				{
					this.authHandler(reqID, ResponseState.Fail, platform, throwable);
				}
				break;
			case 2:
				if (this.getFriendsHandler != null)
				{
					this.getFriendsHandler(reqID, ResponseState.Fail, platform, throwable);
				}
				break;
			case 6:
				if (this.followFriendHandler != null)
				{
					this.followFriendHandler(reqID, ResponseState.Fail, platform, throwable);
				}
				break;
			case 8:
				if (this.showUserHandler != null)
				{
					this.showUserHandler(reqID, ResponseState.Fail, platform, throwable);
				}
				break;
			case 9:
				if (this.shareHandler != null)
				{
					this.shareHandler(reqID, ResponseState.Fail, platform, throwable);
				}
				break;
			}
		}

		public void OnComplete(int reqID, PlatformType platform, int action, Hashtable res)
		{
			switch (action)
			{
			case 1:
				if (this.authHandler != null)
				{
					this.authHandler(reqID, ResponseState.Success, platform, res);
				}
				break;
			case 2:
				if (this.getFriendsHandler != null)
				{
					this.getFriendsHandler(reqID, ResponseState.Success, platform, res);
				}
				break;
			case 6:
				if (this.followFriendHandler != null)
				{
					this.followFriendHandler(reqID, ResponseState.Success, platform, res);
				}
				break;
			case 8:
				if (this.showUserHandler != null)
				{
					this.showUserHandler(reqID, ResponseState.Success, platform, res);
				}
				break;
			case 9:
				if (this.shareHandler != null)
				{
					this.shareHandler(reqID, ResponseState.Success, platform, res);
				}
				break;
			}
		}

		public void OnCancel(int reqID, PlatformType platform, int action)
		{
			switch (action)
			{
			case 1:
				if (this.authHandler != null)
				{
					this.authHandler(reqID, ResponseState.Cancel, platform, null);
				}
				break;
			case 2:
				if (this.getFriendsHandler != null)
				{
					this.getFriendsHandler(reqID, ResponseState.Cancel, platform, null);
				}
				break;
			case 6:
				if (this.followFriendHandler != null)
				{
					this.followFriendHandler(reqID, ResponseState.Cancel, platform, null);
				}
				break;
			case 8:
				if (this.showUserHandler != null)
				{
					this.showUserHandler(reqID, ResponseState.Cancel, platform, null);
				}
				break;
			case 9:
				if (this.shareHandler != null)
				{
					this.shareHandler(reqID, ResponseState.Cancel, platform, null);
				}
				break;
			}
		}

		public void InitSDK(string appKey)
		{
			this.shareSDKUtils.InitSDK(appKey);
		}

		public void SetPlatformConfig(Hashtable configInfo)
		{
			this.shareSDKUtils.SetPlatformConfig(configInfo);
		}

		public int Authorize(PlatformType platform)
		{
			this.reqID++;
			this.shareSDKUtils.Authorize(this.reqID, platform);
			return this.reqID;
		}

		public void CancelAuthorize(PlatformType platform)
		{
			this.shareSDKUtils.CancelAuthorize(platform);
		}

		public bool IsAuthorized(PlatformType platform)
		{
			return this.shareSDKUtils.IsAuthorized(platform);
		}

		public bool IsClientValid(PlatformType platform)
		{
			return this.shareSDKUtils.IsClientValid(platform);
		}

		public int GetUserInfo(PlatformType platform)
		{
			this.reqID++;
			this.shareSDKUtils.GetUserInfo(this.reqID, platform);
			return this.reqID;
		}

		public int ShareContent(PlatformType platform, ShareContent content)
		{
			this.reqID++;
			this.shareSDKUtils.ShareContent(this.reqID, platform, content);
			return this.reqID;
		}

		public int ShareContent(PlatformType[] platforms, ShareContent content)
		{
			this.reqID++;
			this.shareSDKUtils.ShareContent(this.reqID, platforms, content);
			return this.reqID;
		}

		public int ShowPlatformList(PlatformType[] platforms, ShareContent content, int x, int y)
		{
			this.reqID++;
			this.shareSDKUtils.ShowPlatformList(this.reqID, platforms, content, x, y);
			return this.reqID;
		}

		public int ShowShareContentEditor(PlatformType platform, ShareContent content)
		{
			this.reqID++;
			this.shareSDKUtils.ShowShareContentEditor(this.reqID, platform, content);
			return this.reqID;
		}

		public int ShareWithContentName(PlatformType platform, string contentName, Hashtable customFields)
		{
			this.reqID++;
			this.shareSDKUtils.ShareWithContentName(this.reqID, platform, contentName, customFields);
			return this.reqID;
		}

		public int ShowPlatformListWithContentName(string contentName, Hashtable customFields, PlatformType[] platforms, int x, int y)
		{
			this.reqID++;
			this.shareSDKUtils.ShowPlatformListWithContentName(this.reqID, contentName, customFields, platforms, x, y);
			return this.reqID;
		}

		public int ShowShareContentEditorWithContentName(PlatformType platform, string contentName, Hashtable customFields)
		{
			this.reqID++;
			this.shareSDKUtils.ShowShareContentEditorWithContentName(this.reqID, platform, contentName, customFields);
			return this.reqID;
		}

		public int GetFriendList(PlatformType platform, int count, int page)
		{
			this.reqID++;
			this.shareSDKUtils.GetFriendList(this.reqID, platform, count, page);
			return this.reqID;
		}

		public int AddFriend(PlatformType platform, string account)
		{
			this.reqID++;
			this.shareSDKUtils.AddFriend(this.reqID, platform, account);
			return this.reqID;
		}

		public Hashtable GetAuthInfo(PlatformType platform)
		{
			return this.shareSDKUtils.GetAuthInfo(platform);
		}

		public void DisableSSO(bool open)
		{
			this.shareSDKUtils.DisableSSO(open);
		}
	}
}
