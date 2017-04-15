using System;

namespace CsSdkIOS
{
	public class CsTvInterfaceIOS
	{
		public const int PAY_RESULT_SUCCESS = 0;

		public const int PAY_RESULT_CANCEL = 1;

		public const int PAY_RESULT_ERROR = 2;

		public const int SEARCH_TYPE_LIVE = 1;

		public const int SEARCH_TYPE_VIDEO = 2;

		private const string appKey = "a18f55afe9b38e4d";

		private const string appSecret = "9a36521971e058078b5374b22f3a9a90";

		private ICsTvListener csTvListener;

		private ICsHttpListener csHttpListener;

		public static CsTvInterfaceIOS instance;

		public CsTvInterfaceIOS()
		{
			CsTvInterfaceIOS.instance = this;
		}

		public void initialize(ICsTvListener callback)
		{
		}

		public void playLiveRoom(string roomid, bool isPortrait)
		{
		}

		public void playGameVideo(string videoid, bool isPortrait)
		{
		}

		public void presentLiveRoomList(string targetKey)
		{
		}

		public void presentGameVideoList(string targetKey)
		{
		}

		public void requestLiveRoomListData(ICsHttpListener callback, string targetKey, int pageSize, string breakpoint)
		{
		}

		public void requestGameVideoListData(ICsHttpListener callback, string targetKey, int pageSize, string breakpoint)
		{
		}

		public void requestGameZoneListData(ICsHttpListener callback)
		{
		}

		public void requestSearchCategoryData(ICsHttpListener callback, string keyword, int searchType)
		{
		}

		public void requestSearchNavListData(ICsHttpListener callback, string targetKey, int pageSize, string breakpoint)
		{
		}

		public void notifyUserInfo(ThirdAppUserInfo userInfo)
		{
		}

		public void notifyPayResult(int result, string errMsg)
		{
		}
	}
}
