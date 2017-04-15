using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Com.Game.Utils
{
	internal class XMLUtils
	{
		public static void writeData2XML(string path, Dictionary<string, Dictionary<string, object>> datas)
		{
		}

		public static Dictionary<string, object> ReadXMLToObject(string file, Type type)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string content = string.Empty;
			StreamReader streamReader = new StreamReader(file);
			content = streamReader.ReadToEnd();
			streamReader.Close();
			XMLNode xMLNode = XMLParser.Parse(content);
			XMLNodeList nodeList = xMLNode.GetNodeList("root>0>item");
			string text = string.Empty;
			string text2 = "-";
			foreach (XMLNode xMLNode2 in nodeList)
			{
				object obj = Activator.CreateInstance(type);
				if (obj == null)
				{
					throw new NullReferenceException(string.Format("type:{0} cannot be found!", type));
				}
				FieldInfo[] fields = obj.GetType().GetFields();
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					try
					{
						text = xMLNode2.GetValue(fieldInfo.Name.ToString() + ">0>_text");
						if (fieldInfo.Name.ToString().Equals("unikey"))
						{
							text2 = text;
						}
						if (fieldInfo.FieldType.Equals(typeof(string)))
						{
							fieldInfo.SetValue(obj, text);
						}
						else if (fieldInfo.FieldType.Equals(typeof(int)) || fieldInfo.FieldType.Equals(typeof(float)) || fieldInfo.FieldType.Equals(typeof(long)) || fieldInfo.FieldType.Equals(typeof(bool)) || fieldInfo.FieldType.Equals(typeof(uint)))
						{
							Type fieldType = fieldInfo.FieldType;
							MethodInfo method = fieldType.GetMethod("Parse", new Type[]
							{
								typeof(string)
							});
							BindingFlags invokeAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
							if (text != null)
							{
								object[] parameters = new object[]
								{
									text
								};
								fieldInfo.SetValue(obj, method.Invoke(null, invokeAttr, Type.DefaultBinder, parameters, null));
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"XMLUtil Error :  key = ",
							text2,
							" 解析字段",
							fieldInfo.Name,
							"出错! error=",
							ex.Message
						}));
					}
				}
				if (text2 != "-")
				{
					dictionary.Add(text2, obj);
				}
			}
			return dictionary;
		}
	}
}
