using JPush;
using System;
using UnityEngine;

public class APush : MonoBehaviour
{
	private static string iosKey = "IOSToken";

	private static string iosReg = "IOSRegId";

	private static string andKey = "JPushID";

	public static string regId = string.Empty;

	public static string iosToken = string.Empty;

	public string msg = string.Empty;

	private static APush andriodOne;

	private string str_unity = string.Empty;

	private bool B_MESSAGE;

	private static string str_message = string.Empty;

	public static void StopAndriodPush()
	{
		if (APush.andriodOne != null)
		{
			APush.andriodOne.stopJPush(null);
		}
	}

	public static void ResumeAndriodPush()
	{
		if (APush.andriodOne != null)
		{
			APush.andriodOne.resumeJPush(null);
		}
	}

	private void Start()
	{
		APush.andriodOne = this;
		if (PlayerPrefs.HasKey(APush.andKey))
		{
			APush.regId = PlayerPrefs.GetString(APush.andKey);
		}
		try
		{
			JPushBinding.setDebug(true);
			JPushBinding.initJPush(base.gameObject.name, "InitOK");
			JPushEventManager.instance.addEventListener(CustomEventObj.EVENT_INIT_JPUSH, base.gameObject, "initJPush");
			JPushEventManager.instance.addEventListener(CustomEventObj.EVENT_STOP_JPUSH, base.gameObject, "stopJPush");
			JPushEventManager.instance.addEventListener(CustomEventObj.EVENT_RESUME_JPUSH, base.gameObject, "resumeJPush");
			JPushEventManager.instance.addEventListener(CustomEventObj.EVENT_SET_TAGS, base.gameObject, "setTags");
			JPushEventManager.instance.addEventListener(CustomEventObj.EVENT_SET_ALIAS, base.gameObject, "setAlias");
			JPushBinding.getRegistrationId(base.gameObject.name, "onGetRegId");
		}
		catch (Exception ex)
		{
			this.msg = ex.ToString();
		}
	}

	private void InitOK()
	{
		MonoBehaviour.print("unity3d---InitOK");
	}

	private void OnDestroy()
	{
		MonoBehaviour.print("unity3d---onDestroy");
		if (base.gameObject)
		{
			JPushEventManager.instance.removeAllEventListeners(base.gameObject);
		}
	}

	private void initJPush(CustomEventObj evt)
	{
		JPushBinding.initJPush(base.gameObject.name, string.Empty);
	}

	private void setTags(CustomEventObj evt)
	{
		string tags = (string)evt.arguments["tags"];
		JPushBinding.setTags(tags);
	}

	private void setAlias(CustomEventObj evt)
	{
		string alias = (string)evt.arguments["alias"];
		JPushBinding.setAlias(alias);
	}

	private void setPushTime(CustomEventObj evt)
	{
		string days = (string)evt.arguments["days"];
		int startHour = (int)evt.arguments["start_time"];
		int endHour = (int)evt.arguments["end_time"];
		JPushBinding.setPushTime(days, startHour, endHour);
	}

	private void recvMessage(string str)
	{
		this.B_MESSAGE = true;
		APush.str_message = str;
		this.str_unity = "有新消息";
		this.msg = str;
	}

	private void openNotification(string str)
	{
		this.str_unity = str;
		this.msg = str;
	}

	private void onGetRegId(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			APush.regId = str;
			PlayerPrefs.SetString(APush.andKey, APush.regId);
			PlayerPrefs.Save();
		}
	}

	private void onRegister(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			APush.regId = str;
			PlayerPrefs.SetString(APush.andKey, APush.regId);
			PlayerPrefs.Save();
		}
	}

	private void beforeQuit()
	{
		JPushBinding.isQuit();
	}

	private void stopJPush(CustomEventObj evt)
	{
		JPushBinding.stopJPush();
	}

	private void resumeJPush(CustomEventObj evt)
	{
		JPushBinding.resumeJPush();
	}
}
