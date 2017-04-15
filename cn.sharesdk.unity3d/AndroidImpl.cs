using System;
using System.Collections;
using UnityEngine;

namespace cn.sharesdk.unity3d
{
	public class AndroidImpl : ShareSDKImpl
	{
		private AndroidJavaObject ssdk;

		public AndroidImpl(GameObject go)
		{
			Debug.Log("AndroidImpl  ===>>>  AndroidImpl");
			try
			{
				this.ssdk = new AndroidJavaObject("cn.sharesdk.unity3d.ShareSDKUtils", new object[]
				{
					go.name,
					"_Callback"
				});
			}
			catch (Exception arg)
			{
				Console.WriteLine("{0} Exception caught.", arg);
			}
		}

		public override void InitSDK(string appKey)
		{
			Debug.Log("AndroidImpl  ===>>>  InitSDK === " + appKey);
			if (this.ssdk != null)
			{
				this.ssdk.Call("initSDK", new object[]
				{
					appKey
				});
			}
		}

		public override void SetPlatformConfig(Hashtable configs)
		{
			string text = MiniJSON.jsonEncode(configs);
			Debug.Log("AndroidImpl  ===>>>  SetPlatformConfig === " + text);
			if (this.ssdk != null)
			{
				this.ssdk.Call("setPlatformConfig", new object[]
				{
					text
				});
			}
		}

		public override void Authorize(int reqID, PlatformType platform)
		{
			Debug.Log("AndroidImpl  ===>>>  Authorize");
			if (this.ssdk != null)
			{
				this.ssdk.Call("authorize", new object[]
				{
					reqID,
					(int)platform
				});
			}
		}

		public override void CancelAuthorize(PlatformType platform)
		{
			if (this.ssdk != null)
			{
				this.ssdk.Call("removeAccount", new object[]
				{
					(int)platform
				});
			}
		}

		public override bool IsAuthorized(PlatformType platform)
		{
			return this.ssdk != null && this.ssdk.Call<bool>("isAuthValid", new object[]
			{
				(int)platform
			});
		}

		public override bool IsClientValid(PlatformType platform)
		{
			return this.ssdk != null && this.ssdk.Call<bool>("isClientValid", new object[]
			{
				(int)platform
			});
		}

		public override void GetUserInfo(int reqID, PlatformType platform)
		{
			Debug.Log("AndroidImpl  ===>>>  ShowUser");
			if (this.ssdk != null)
			{
				this.ssdk.Call("showUser", new object[]
				{
					reqID,
					(int)platform
				});
			}
		}

		public override void ShareContent(int reqID, PlatformType platform, ShareContent content)
		{
			Debug.Log("AndroidImpl  ===>>>  ShareContent to one platform");
			this.ShareContent(reqID, new PlatformType[]
			{
				platform
			}, content);
		}

		public override void ShareContent(int reqID, PlatformType[] platforms, ShareContent content)
		{
			Debug.Log("AndroidImpl  ===>>>  Share");
			if (this.ssdk != null)
			{
				for (int i = 0; i < platforms.Length; i++)
				{
					PlatformType platformType = platforms[i];
					this.ssdk.Call("shareContent", new object[]
					{
						reqID,
						(int)platformType,
						content.GetShareParamsStr()
					});
				}
			}
		}

		public override void ShowPlatformList(int reqID, PlatformType[] platforms, ShareContent content, int x, int y)
		{
			this.ShowShareContentEditor(reqID, PlatformType.Unknown, content);
		}

		public override void ShowShareContentEditor(int reqID, PlatformType platform, ShareContent content)
		{
			Debug.Log("AndroidImpl  ===>>>  OnekeyShare platform ===" + (int)platform);
			if (this.ssdk != null)
			{
				this.ssdk.Call("onekeyShare", new object[]
				{
					reqID,
					(int)platform,
					content.GetShareParamsStr()
				});
			}
		}

		public override void GetFriendList(int reqID, PlatformType platform, int count, int page)
		{
			Debug.Log("AndroidImpl  ===>>>  GetFriendList");
			if (this.ssdk != null)
			{
				this.ssdk.Call("getFriendList", new object[]
				{
					reqID,
					(int)platform,
					count,
					page
				});
			}
		}

		public override void AddFriend(int reqID, PlatformType platform, string account)
		{
			Debug.Log("AndroidImpl  ===>>>  FollowFriend");
			if (this.ssdk != null)
			{
				this.ssdk.Call("followFriend", new object[]
				{
					reqID,
					(int)platform,
					account
				});
			}
		}

		public override Hashtable GetAuthInfo(PlatformType platform)
		{
			Debug.Log("AndroidImpl  ===>>>  GetAuthInfo");
			if (this.ssdk != null)
			{
				string json = this.ssdk.Call<string>("getAuthInfo", new object[]
				{
					(int)platform
				});
				return (Hashtable)MiniJSON.jsonDecode(json);
			}
			return new Hashtable();
		}

		public override void DisableSSO(bool disable)
		{
			Debug.Log("AndroidImpl  ===>>>  DisableSSOWhenAuthorize");
			if (this.ssdk != null)
			{
				this.ssdk.Call("disableSSOWhenAuthorize", new object[]
				{
					disable
				});
			}
		}

		public override void ShareWithContentName(int reqId, PlatformType platform, string contentName, Hashtable customFields)
		{
			Debug.Log("#WARING : Do not support this feature in Android temporarily");
		}

		public override void ShowPlatformListWithContentName(int reqId, string contentName, Hashtable customFields, PlatformType[] platforms, int x, int y)
		{
			Debug.Log("#WARING : Do not support this feature in Android temporarily");
		}

		public override void ShowShareContentEditorWithContentName(int reqId, PlatformType platform, string contentName, Hashtable customFields)
		{
			Debug.Log("#WARING : Do not support this feature in Android temporarily");
		}
	}
}
