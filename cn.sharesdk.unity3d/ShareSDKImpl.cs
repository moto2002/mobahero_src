using System;
using System.Collections;

namespace cn.sharesdk.unity3d
{
	public abstract class ShareSDKImpl
	{
		public abstract void InitSDK(string appKey);

		public abstract void SetPlatformConfig(Hashtable configs);

		public abstract void Authorize(int reqID, PlatformType platform);

		public abstract void CancelAuthorize(PlatformType platform);

		public abstract bool IsAuthorized(PlatformType platform);

		public abstract bool IsClientValid(PlatformType platform);

		public abstract void GetUserInfo(int reqID, PlatformType platform);

		public abstract void ShareContent(int reqID, PlatformType platform, ShareContent content);

		public abstract void ShareContent(int reqID, PlatformType[] platforms, ShareContent content);

		public abstract void ShowPlatformList(int reqID, PlatformType[] platforms, ShareContent content, int x, int y);

		public abstract void ShowShareContentEditor(int reqID, PlatformType platform, ShareContent content);

		public abstract void ShareWithContentName(int reqId, PlatformType platform, string contentName, Hashtable customFields);

		public abstract void ShowPlatformListWithContentName(int reqId, string contentName, Hashtable customFields, PlatformType[] platforms, int x, int y);

		public abstract void ShowShareContentEditorWithContentName(int reqId, PlatformType platform, string contentName, Hashtable customFields);

		public abstract void GetFriendList(int reqID, PlatformType platform, int count, int page);

		public abstract void AddFriend(int reqID, PlatformType platform, string account);

		public abstract Hashtable GetAuthInfo(PlatformType platform);

		public abstract void DisableSSO(bool disable);
	}
}
