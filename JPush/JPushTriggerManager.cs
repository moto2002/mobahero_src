using System;
using UnityEngine;

namespace JPush
{
	public class JPushTriggerManager : MonoBehaviour
	{
		public static void triggerInitJPush(string event_type)
		{
			CustomEventObj evt = new CustomEventObj(event_type);
			JPushEventManager.instance.dispatchEvent(evt);
		}

		public static void triggerStopJPush(string event_type)
		{
			CustomEventObj evt = new CustomEventObj(event_type);
			JPushEventManager.instance.dispatchEvent(evt);
		}

		public static void triggerResumeJPush(string event_type)
		{
			CustomEventObj evt = new CustomEventObj(event_type);
			JPushEventManager.instance.dispatchEvent(evt);
		}

		public static void triggerSetTags(string event_type, string tags)
		{
			CustomEventObj customEventObj = new CustomEventObj(event_type);
			customEventObj.arguments.Add("tags", tags);
			JPushEventManager.instance.dispatchEvent(customEventObj);
		}

		public static void triggerSetAlias(string event_type, string alias)
		{
			CustomEventObj customEventObj = new CustomEventObj(event_type);
			customEventObj.arguments.Add("alias", alias);
			JPushEventManager.instance.dispatchEvent(customEventObj);
		}

		public static void triggerSetPushTime(string event_type, string days, string start_time, string end_time)
		{
			CustomEventObj customEventObj = new CustomEventObj(event_type);
			customEventObj.arguments.Add("days", days);
			customEventObj.arguments.Add("start_time", start_time);
			customEventObj.arguments.Add("end_time", end_time);
			JPushEventManager.instance.dispatchEvent(customEventObj);
		}
	}
}
