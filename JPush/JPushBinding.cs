using System;
using UnityEngine;

namespace JPush
{
	public class JPushBinding : MonoBehaviour
	{
		private static AndroidJavaObject _plugin;

		public static string _gameObject;

		public static string _func;

		private static int notificationDefaults;

		private static int notificationFlags;

		private static readonly int DEFAULT_ALL;

		private static readonly int DEFAULT_SOUND;

		private static readonly int DEFAULT_VIBRATE;

		private static readonly int DEFAULT_LIGHTS;

		private static readonly int FLAG_SHOW_LIGHTS;

		private static readonly int FLAG_ONGOING_EVENT;

		private static readonly int FLAG_INSISTENT;

		private static readonly int FLAG_ONLY_ALERT_ONCE;

		private static readonly int FLAG_AUTO_CANCEL;

		private static readonly int FLAG_NO_CLEAR;

		private static readonly int FLAG_FOREGROUND_SERVICE;

		static JPushBinding()
		{
			JPushBinding._gameObject = string.Empty;
			JPushBinding._func = string.Empty;
			JPushBinding.notificationDefaults = -1;
			JPushBinding.notificationFlags = 16;
			JPushBinding.DEFAULT_ALL = -1;
			JPushBinding.DEFAULT_SOUND = 1;
			JPushBinding.DEFAULT_VIBRATE = 2;
			JPushBinding.DEFAULT_LIGHTS = 4;
			JPushBinding.FLAG_SHOW_LIGHTS = 1;
			JPushBinding.FLAG_ONGOING_EVENT = 2;
			JPushBinding.FLAG_INSISTENT = 4;
			JPushBinding.FLAG_ONLY_ALERT_ONCE = 8;
			JPushBinding.FLAG_AUTO_CANCEL = 16;
			JPushBinding.FLAG_NO_CLEAR = 32;
			JPushBinding.FLAG_FOREGROUND_SERVICE = 64;
			if (Application.isEditor)
			{
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.xiaomeng.mobaheros.JPushBridge"))
			{
				JPushBinding._plugin = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			}
		}

		public static void setDebug(bool debug)
		{
			JPushBinding._plugin.Call("setDebug", new object[]
			{
				debug
			});
		}

		public static void initJPush(string gameObject, string func)
		{
			Debug.Log("unity---initJPush");
			JPushBinding._gameObject = gameObject;
			JPushBinding._func = func;
			JPushBinding._plugin.Call("initJPush", new object[]
			{
				gameObject,
				func
			});
		}

		public static void stopJPush()
		{
			JPushBinding.stopJPush(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void stopJPush(string gameObject, string func)
		{
			Debug.Log("unity---stopJPush");
			JPushBinding._plugin.Call("stopJPush", new object[]
			{
				gameObject,
				func
			});
		}

		public static void resumeJPush()
		{
			JPushBinding.resumeJPush(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void resumeJPush(string gameObject, string func)
		{
			Debug.Log("unity---resumeJPush");
			JPushBinding._plugin.Call("resumeJPush", new object[]
			{
				gameObject,
				func
			});
		}

		public static bool isPushStopped()
		{
			return JPushBinding.isPushStopped(JPushBinding._gameObject, JPushBinding._func);
		}

		public static bool isPushStopped(string gameObject, string func)
		{
			Debug.Log("unity---isPushStopped");
			return JPushBinding._plugin.Call<bool>("isPushStopped", new object[]
			{
				gameObject,
				func
			});
		}

		public static string getRegistrationId()
		{
			return JPushBinding.getRegistrationId(JPushBinding._gameObject, JPushBinding._func);
		}

		public static string getRegistrationId(string gameObject, string func)
		{
			Debug.Log("unity---getRegistrationId");
			return JPushBinding._plugin.Call<string>("getRegistrationId", new object[]
			{
				gameObject,
				func
			});
		}

		public static string filterValidTags(string tags)
		{
			return JPushBinding.filterValidTags(JPushBinding._gameObject, JPushBinding._func, tags);
		}

		public static string filterValidTags(string gameObject, string func, string tags)
		{
			return JPushBinding._plugin.Call<string>("filterValidTags", new object[]
			{
				gameObject,
				func,
				tags
			});
		}

		public static void setTags(string tags)
		{
			JPushBinding.setTags(JPushBinding._gameObject, JPushBinding._func, tags);
		}

		public static void setTags(string gameObject, string func, string tags)
		{
			Debug.Log("unity---setTags");
			JPushBinding._plugin.Call("setTags", new object[]
			{
				gameObject,
				func,
				tags
			});
		}

		public static void setAlias(string alias)
		{
			JPushBinding.setAlias(JPushBinding._gameObject, JPushBinding._func, alias);
		}

		public static void setAlias(string gameObject, string func, string alias)
		{
			Debug.Log("unity---setAlias");
			JPushBinding._plugin.Call("setAlias", new object[]
			{
				gameObject,
				func,
				alias
			});
		}

		public static void setAliasAndTags(string alias, string tags)
		{
			JPushBinding.setAliasAndTags(JPushBinding._gameObject, JPushBinding._func, alias, tags);
		}

		public static void setAliasAndTags(string gameObject, string func, string alias, string tags)
		{
			Debug.Log("unity---setAliasAndTags");
			JPushBinding._plugin.Call("setAliasAndTags", new object[]
			{
				gameObject,
				func,
				alias,
				tags
			});
		}

		public static void setPushTime(string days, int startHour, int endHour)
		{
			JPushBinding.setPushTime(JPushBinding._gameObject, JPushBinding._func, days, startHour, endHour);
		}

		public static void setPushTime(string gameObject, string func, string days, int start_time, int end_time)
		{
			Debug.Log("unity---setPushTime");
			JPushBinding._plugin.Call("setPushTime", new object[]
			{
				gameObject,
				func,
				days,
				start_time,
				end_time
			});
		}

		public static void setSilenceTime(int startHour, int startMinute, int endHour, int endMinute)
		{
			JPushBinding.setSilenceTime(JPushBinding._gameObject, JPushBinding._func, startHour, startMinute, endHour, endMinute);
		}

		public static void setSilenceTime(string gameObject, string func, int startHour, int startMinute, int endHour, int endMinute)
		{
			Debug.Log("unity---setSilenceTime");
			JPushBinding._plugin.Call("setSilenceTime", new object[]
			{
				gameObject,
				func,
				startHour,
				startMinute,
				endHour,
				endMinute
			});
		}

		public static void setLatestNotificationNumber(int num)
		{
			JPushBinding.setLatestNotificationNumber(JPushBinding._gameObject, JPushBinding._func, num);
		}

		public static void setLatestNotificationNumber(string gameObject, string func, int num)
		{
			Debug.Log("unity---setLatestNotificationNumber");
			JPushBinding._plugin.Call("setLatestNotificationNumber", new object[]
			{
				gameObject,
				func,
				num
			});
		}

		public static void addLocalNotification(int builderId, string content, string title, int notiId, int broadcastTime, string extrasStr)
		{
			JPushBinding.addLocalNotification(JPushBinding._gameObject, JPushBinding._func, builderId, content, title, notiId, broadcastTime, extrasStr);
		}

		public static void addLocalNotification(string gameObject, string func, int builderId, string content, string title, int notiId, int broadcastTime, string extrasStr)
		{
			Debug.Log("unity---addLocalNotification");
			JPushBinding._plugin.Call("addLocalNotification", new object[]
			{
				gameObject,
				func,
				builderId,
				content,
				title,
				notiId,
				broadcastTime,
				extrasStr
			});
		}

		public static void addLocalNotification(int builderId, string content, string title, int notiId, int year, int month, int day, int hour, int minute, int second, string extrasStr)
		{
			JPushBinding._plugin.Call("addLocalNotification", new object[]
			{
				builderId,
				content,
				title,
				notiId,
				year,
				month,
				day,
				hour,
				minute,
				second,
				extrasStr
			});
		}

		public static void removeLocalNotification(int notiId)
		{
			JPushBinding.removeLocalNotification(JPushBinding._gameObject, JPushBinding._func, notiId);
		}

		public static void removeLocalNotification(string gameObject, string func, int notiId)
		{
			Debug.Log("unity---removeLocalNotification");
			JPushBinding._plugin.Call("removeLocalNotification", new object[]
			{
				gameObject,
				func,
				notiId
			});
		}

		public static void clearLocalNotifications()
		{
			JPushBinding.clearLocalNotifications(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void clearLocalNotifications(string gameObject, string func)
		{
			Debug.Log("unity---clearLocalNotifications");
			JPushBinding._plugin.Call("clearLocalNotifications", new object[]
			{
				gameObject,
				func
			});
		}

		public static void clearAllNotifications()
		{
			JPushBinding.clearAllNotifications(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void clearAllNotifications(string gameObject, string func)
		{
			Debug.Log("unity---clearAllNotifications");
			JPushBinding._plugin.Call("clearAllNotifications", new object[]
			{
				gameObject,
				func
			});
		}

		public static void clearNotificationById(int notiId)
		{
			JPushBinding.clearNotificationById(JPushBinding._gameObject, JPushBinding._func, notiId);
		}

		public static void clearNotificationById(string gameObject, string func, int notiId)
		{
			Debug.Log("unity---clearNotificationById");
			JPushBinding._plugin.Call("clearNotificationById", new object[]
			{
				gameObject,
				func,
				notiId
			});
		}

		public static void requestPermission()
		{
			JPushBinding.requestPermission(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void requestPermission(string gameObject, string func)
		{
			Debug.Log("unity---requestPermission");
			JPushBinding._plugin.Call("requestPermission", new object[]
			{
				gameObject,
				func
			});
		}

		public static void setBasicPushNotificationBuilder()
		{
			JPushBinding.setBasicPushNotificationBuilder(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void setBasicPushNotificationBuilder(string gameObject, string func)
		{
			Debug.Log("unity---setBasicPushNotificationBuilder");
			int num = 1;
			int num2 = JPushBinding.notificationDefaults | JPushBinding.DEFAULT_ALL;
			int num3 = JPushBinding.notificationFlags | JPushBinding.FLAG_AUTO_CANCEL;
			AndroidJavaObject arg_57_0 = JPushBinding._plugin;
			string arg_57_1 = "setBasicPushNotificationBuilder";
			object[] expr_34 = new object[6];
			expr_34[0] = gameObject;
			expr_34[1] = func;
			expr_34[2] = num;
			expr_34[3] = num2;
			expr_34[4] = num3;
			arg_57_0.Call(arg_57_1, expr_34);
		}

		public static void setCustomPushNotificationBuilder()
		{
			JPushBinding.setCustomPushNotificationBuilder(JPushBinding._gameObject, JPushBinding._func);
		}

		public static void setCustomPushNotificationBuilder(string gameObject, string func)
		{
			Debug.Log("unity---setCustomPushNotificationBuilder");
			int num = 1;
			string text = "yourNotificationLayoutName";
			string text2 = "yourStatusBarDrawableName";
			string text3 = "yourLayoutIconDrawableName";
			JPushBinding._plugin.Call("setCustomPushNotificationBuilder", new object[]
			{
				num,
				text,
				text2,
				text3
			});
		}

		public static void isQuit()
		{
			Debug.Log("unity---isQuit");
			JPushBinding._plugin.Call("isQuit", new object[0]);
		}
	}
}
