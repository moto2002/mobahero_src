using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace anysdk
{
	public class AnySDKUtil
	{
		public const string ANYSDK_PLATFORM = "PluginProtocol";

		public const int MAX_CAPACITY_NUM = 1024;

		private static AndroidJavaClass mAndroidJavaClass;

		public static string dictionaryToString(Dictionary<string, string> maps)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (maps != null)
			{
				foreach (KeyValuePair<string, string> current in maps)
				{
					if (stringBuilder.Length == 0)
					{
						stringBuilder.AppendFormat("{0}={1}", current.Key, current.Value);
					}
					else
					{
						stringBuilder.AppendFormat("&{0}={1}", current.Key, current.Value);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> stringToDictionary(string message)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (message != null)
			{
				if (message.Contains("&info="))
				{
					Regex regex = new Regex("code=(.*)&msg=([\\s\\S]*)&info=([\\s\\S]*)");
					string[] array = regex.Split(message);
					string value = array[1];
					string value2 = array[2];
					string value3 = array[3];
					dictionary.Add("code", value);
					dictionary.Add("msg", value2);
					dictionary.Add("info", value3);
				}
				else
				{
					Regex regex2 = new Regex("code=(.*)&msg=([\\s\\S]*)");
					string[] array2 = regex2.Split(message);
					string value4 = array2[1];
					string value5 = array2[2];
					dictionary.Add("code", value4);
					dictionary.Add("msg", value5);
				}
			}
			return dictionary;
		}

		public static string ListToString(List<string> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (list != null)
			{
				foreach (string current in list)
				{
					if (stringBuilder.Length == 0)
					{
						stringBuilder.AppendFormat("{0}", current);
					}
					else
					{
						stringBuilder.AppendFormat("&{0}", current);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static List<string> StringToList(string value)
		{
			List<string> list = new List<string>();
			if (value != null && string.Empty != value)
			{
				string[] array = value.Split(new char[]
				{
					'&'
				});
				if (array != null)
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string item = array2[i];
						list.Add(item);
					}
				}
			}
			return list;
		}

		public static void registerActionCallback(AnySDKType type, MonoBehaviour gameObject, string functionName)
		{
			if (AnySDKUtil.mAndroidJavaClass == null)
			{
				AnySDKUtil.mAndroidJavaClass = new AndroidJavaClass("com.anysdk.framework.unity.MessageHandle");
			}
			string name = gameObject.gameObject.name;
			AnySDKUtil.mAndroidJavaClass.CallStatic("registerActionResultCallback", new object[]
			{
				(int)type,
				name,
				functionName
			});
		}
	}
}
