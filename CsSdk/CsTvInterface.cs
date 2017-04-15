using Com.Game.Utils;
using System;
using UnityEngine;

namespace CsSdk
{
	public class CsTvInterface
	{
		private class AndroidConfigListener : AndroidJavaProxy
		{
			private ICsTvListener listener;

			private AndroidJavaObject payResultCallback;

			public AndroidConfigListener(ICsTvListener listener) : base("tv.chushou.playsdklib.constants.CSConfigCallback")
			{
				this.listener = listener;
			}

			public AndroidJavaObject getUserInfo(AndroidJavaObject context)
			{
				GameUserInfo userInfo;
				userInfo.userId = null;
				userInfo.userToken = null;
				userInfo.phoneNumber = null;
				userInfo.extraData = null;
				if (this.listener != null)
				{
					userInfo = this.listener.getUserInfo();
				}
				if (userInfo.userId != null)
				{
					AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.playsdklib.constants.CSAppUserInfo", new object[0]);
					androidJavaObject.Set<string>("mUserID", userInfo.userId);
					androidJavaObject.Set<string>("mUserToken", userInfo.userToken);
					androidJavaObject.Set<string>("mExtraData", userInfo.extraData);
					return androidJavaObject;
				}
				return null;
			}

			public void startPay(AndroidJavaObject goodsInfo, AndroidJavaObject callback)
			{
				CsGoodsInfo goodsInfo2;
				goodsInfo2.orderNo = goodsInfo.Get<string>("mOrderNum");
				goodsInfo2.amount = goodsInfo.Get<float>("mAmount");
				if (this.listener != null)
				{
					this.payResultCallback = callback;
					this.listener.startPay(goodsInfo2);
				}
			}

			public AndroidJavaObject queryAccountBalance()
			{
				CpAccountBalance cpAccountBalance;
				cpAccountBalance.unitDesc = "触手币";
				cpAccountBalance.amount = 0;
				if (this.listener != null)
				{
					cpAccountBalance = this.listener.queryAccountBalance();
				}
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.playsdklib.constants.CPAccountBalance", new object[0]);
				androidJavaObject.Set<string>("mUnitDesc", cpAccountBalance.unitDesc);
				androidJavaObject.Set<int>("mAmount", cpAccountBalance.amount);
				return androidJavaObject;
			}

			private void notifyGiftResult(int code, string message, int accountBalance)
			{
				if (this.listener != null)
				{
					this.listener.notifyGiftResult(code, message, accountBalance);
				}
			}

			public void notifyPaySuccess()
			{
				if (this.payResultCallback != null)
				{
					this.payResultCallback.Call("paySuccess", new object[0]);
					this.payResultCallback = null;
				}
			}

			public void notifyPayCancel()
			{
				if (this.payResultCallback != null)
				{
					this.payResultCallback.Call("payCancel", new object[0]);
					this.payResultCallback = null;
				}
			}

			public void notifyPayError(string errMsg)
			{
				if (this.payResultCallback != null)
				{
					this.payResultCallback.Call("payError", new object[]
					{
						errMsg
					});
					this.payResultCallback = null;
				}
			}
		}

		private class AndroidHttpListener : AndroidJavaProxy
		{
			private ICsHttpListener listener;

			public AndroidHttpListener(ICsHttpListener listener) : base("tv.chushou.playsdklib.constants.OkHttpHandler")
			{
				this.listener = listener;
			}

			public void onStart()
			{
				if (this.listener != null)
				{
					this.listener.onStart();
				}
			}

			public void onFailure(int code, string message)
			{
				if (this.listener != null)
				{
					this.listener.onFailure(code, message);
				}
			}

			public void onSuccess(AndroidJavaObject jsonObject)
			{
				string jsonObject2 = jsonObject.Call<string>("toString", new object[0]);
				if (this.listener != null)
				{
					this.listener.onSuccess(jsonObject2);
				}
			}
		}

		public const int PAY_RESULT_SUCCESS = 0;

		public const int PAY_RESULT_CANCEL = 1;

		public const int PAY_RESULT_ERROR = 2;

		public const int SEARCH_TYPE_ROOM = 1;

		public const int SEARCH_TYPE_VIDEO = 2;

		private const string appKey = "a18f55afe9b38e4d";

		private const string appSecret = "9a36521971e058078b5374b22f3a9a90";

		private AndroidJavaObject mSDK;

		private CsTvInterface.AndroidConfigListener mSDKConfigLister;

		public void initialize(ICsTvListener callback)
		{
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^CsTvInterface initialize");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("tv.chushou.playsdk.ChuShouTVSDK");
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^SDKClz:" + androidJavaClass);
			this.mSDK = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^mSDK:" + this.mSDK);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			this.mSDKConfigLister = new CsTvInterface.AndroidConfigListener(callback);
			AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("tv.chushou.playsdklib.constants.CSGlobalConfig");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("tv.chushou.playsdklib.constants.CSGlobalConfig", new object[]
			{
				this.mSDKConfigLister
			});
			androidJavaObject.Set<AndroidJavaObject>("mContext", @static);
			androidJavaObject.Set<string>("mAppkey", "a18f55afe9b38e4d");
			androidJavaObject.Set<string>("mAppSecret", "9a36521971e058078b5374b22f3a9a90");
			androidJavaObject.Set<AndroidJavaObject>("mDebug", new AndroidJavaObject("java.lang.Boolean", new object[]
			{
				false
			}));
			string static2 = androidJavaClass3.GetStatic<string>("KEY_OPENCLOSE_LOG");
			string static3 = androidJavaClass3.GetStatic<string>("KEY_OPENCLOSE_GIFT");
			string static4 = androidJavaClass3.GetStatic<string>("KEY_OPENCLOSE_CP_CURRENCY");
			string static5 = androidJavaClass3.GetStatic<string>("KEY_OPENCLOSE_SEARCH");
			androidJavaObject.Call("setOption", new object[]
			{
				static2,
				new AndroidJavaObject("java.lang.Boolean", new object[]
				{
					false
				})
			});
			androidJavaObject.Call("setOption", new object[]
			{
				static3,
				new AndroidJavaObject("java.lang.Boolean", new object[]
				{
					false
				})
			});
			androidJavaObject.Call("setOption", new object[]
			{
				static4,
				new AndroidJavaObject("java.lang.Boolean", new object[]
				{
					false
				})
			});
			androidJavaObject.Call("setOption", new object[]
			{
				static5,
				new AndroidJavaObject("java.lang.Boolean", new object[]
				{
					false
				})
			});
			this.mSDK.Call("initialize", new object[]
			{
				@static,
				androidJavaObject
			});
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^CsTvInterface initialize end");
		}

		public void playLiveRoom(string roomid, bool isPortrait)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			bool flag = this.mSDK.Call<bool>("playLiveRoom", new object[]
			{
				@static,
				roomid,
				isPortrait
			});
		}

		public void playVideo(string videoid, bool isPortrait)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			bool flag = this.mSDK.Call<bool>("playVideo", new object[]
			{
				@static,
				videoid,
				isPortrait
			});
		}

		public void getOnlineRoomListWithUI(string gameId)
		{
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^getOnlineRoomListWithUI" + this.mSDK);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			bool flag = this.mSDK.Call<bool>("getOnlineRoomListWithUI", new object[]
			{
				@static,
				gameId
			});
		}

		public void getGameVideoListWithUI(string gameId)
		{
			ClientLogger.Error("^^^^^^^^^^^^^^^^^^^^^^getGameVideoListWithUI" + this.mSDK);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			bool flag = this.mSDK.Call<bool>("getGameVideoListWithUI", new object[]
			{
				@static,
				gameId
			});
		}

		public void getOnlineRoomList(ICsHttpListener callback, string gameId, int pageSize, string breakpoint)
		{
			this.mSDK.Call("getOnlineRoomList", new object[]
			{
				new CsTvInterface.AndroidHttpListener(callback),
				gameId,
				pageSize,
				breakpoint
			});
		}

		public void getGameVideoList(ICsHttpListener callback, string gameId, int pageSize, string breakpoint)
		{
			this.mSDK.Call("getGameVideoList", new object[]
			{
				new CsTvInterface.AndroidHttpListener(callback),
				gameId,
				pageSize,
				breakpoint
			});
		}

		public void getGameZoneList(ICsHttpListener callback)
		{
			this.mSDK.Call("getGameZoneList", new object[]
			{
				new CsTvInterface.AndroidHttpListener(callback)
			});
		}

		public void getSearchCategoryData(ICsHttpListener callback, string keyword, int type)
		{
			this.mSDK.Call("getSearchCategoryData", new object[]
			{
				new CsTvInterface.AndroidHttpListener(callback),
				keyword,
				type
			});
		}

		public void getSearchResultData(ICsHttpListener callback, string target, int pageSize, string breakpoint)
		{
			this.mSDK.Call("getSearchResultData", new object[]
			{
				new CsTvInterface.AndroidHttpListener(callback),
				target,
				pageSize,
				breakpoint
			});
		}

		public void notifyPayResult(int result, string errMsg)
		{
			if (this.mSDKConfigLister != null)
			{
				if (result == 0)
				{
					this.mSDKConfigLister.notifyPaySuccess();
				}
				else if (result == 1)
				{
					this.mSDKConfigLister.notifyPayCancel();
				}
				else if (result == 2)
				{
					this.mSDKConfigLister.notifyPayError(errMsg);
				}
			}
		}
	}
}
