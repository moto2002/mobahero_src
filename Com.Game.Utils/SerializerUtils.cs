using Com.Game.Manager;
using ICSharpCode.SharpZipLib.Zip;
using MobaHeros.Replay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Com.Game.Utils
{
	public class SerializerUtils
	{
		public static void ZipStream(Stream inputStream, Stream outStream, int level)
		{
			using (ZipOutputStream zipOutputStream = new ZipOutputStream(outStream))
			{
				ZipEntry entry = new ZipEntry("content");
				zipOutputStream.PutNextEntry(entry);
				zipOutputStream.SetLevel(level);
				byte[] array = new byte[1024];
				while (true)
				{
					int num = inputStream.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					zipOutputStream.Write(array, 0, num);
				}
			}
		}

		public static void UnzipStream(Stream inStream, Stream outStream)
		{
			using (ZipInputStream zipInputStream = new ZipInputStream(inStream))
			{
				ZipEntry nextEntry;
				while ((nextEntry = zipInputStream.GetNextEntry()) != null)
				{
					if (nextEntry.Name == "content")
					{
						byte[] array = new byte[2048];
						while (true)
						{
							int num = zipInputStream.Read(array, 0, array.Length);
							if (num <= 0)
							{
								break;
							}
							outStream.Write(array, 0, num);
						}
					}
				}
			}
		}

		public static void ZipSerialize(string path, object data, int level = 2)
		{
			using (FileStream fileStream = File.Create(path))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, data);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					SerializerUtils.ZipStream(memoryStream, fileStream, level);
				}
			}
		}

		public static object ZipDeserialize(string path)
		{
			object result;
			using (FileStream fileStream = File.OpenRead(path))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					SerializerUtils.UnzipStream(fileStream, memoryStream);
					BinaryFormatter binaryFormatter = new BinaryFormatter
					{
						Binder = new UBinder()
					};
					memoryStream.Seek(0L, SeekOrigin.Begin);
					object obj = binaryFormatter.Deserialize(memoryStream);
					result = obj;
				}
			}
			return result;
		}

		public static void ReplaySerialize(string path, List<ReplayMessage> data)
		{
			using (FileStream fileStream = File.Create(path))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8))
				{
					for (int i = 0; i < data.Count; i++)
					{
						ReplayMessage replayMessage = data[i];
						if (replayMessage.param != null && replayMessage.param.Length > 0 && replayMessage.param.Length <= 31000)
						{
							binaryWriter.Write((short)replayMessage.param.Length);
							binaryWriter.Write((byte)replayMessage.code);
							binaryWriter.Write(replayMessage.time);
							binaryWriter.Write(replayMessage.param);
						}
					}
				}
			}
		}

		public static void binarySerialize(string path, object data)
		{
			try
			{
				FileStream fileStream = new FileStream(path, FileMode.Create);
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(fileStream, data);
				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("warn\n" + ex.Message);
			}
		}

		public static object binaryDeserialize(byte[] bytes)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream(bytes);
				object result = new BinaryFormatter
				{
					Binder = new UBinder()
				}.Deserialize(memoryStream);
				memoryStream.Close();
				memoryStream.Dispose();
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogError("warn\n" + ex.Message);
			}
			return null;
		}

		public static object jsonDeserialize(byte[] bytes)
		{
			return null;
		}

		public static object xmlDeserialize(Type type, string text)
		{
			if (text == null)
			{
				return null;
			}
			try
			{
				StringReader input = new StringReader(text);
				XmlReader xmlReader = XmlReader.Create(input);
				XmlSerializer xmlSerializer = new XmlSerializer(type);
				return xmlSerializer.Deserialize(xmlReader);
			}
			catch (Exception ex)
			{
				Debug.LogError("warn : text = " + text + " error=" + ex.Message);
			}
			return null;
		}

		public static string xmlSerialize(object pObject, Type type)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				XmlSerializer xmlSerializer = new XmlSerializer(type);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
				xmlSerializer.Serialize(xmlTextWriter, pObject);
				memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
				return SerializerUtils.UTF8ByteArrayToString(memoryStream.ToArray());
			}
			catch (Exception ex)
			{
				Debug.LogError("warn\n" + ex.Message);
			}
			return null;
		}

		public static Dictionary<string, object> loadXML(string fileName, Type type)
		{
			int num = 0;
			string text = string.Empty;
			string empty = string.Empty;
			try
			{
				TextAsset textAsset = Resources.Load("Data/BinData/" + fileName, typeof(TextAsset)) as TextAsset;
				if (textAsset != null)
				{
					StringReader stringReader = new StringReader(textAsset.text);
					XmlReader xmlReader = XmlReader.Create(stringReader);
					XmlDocument xmlDocument = new XmlDocument();
					string text2 = stringReader.ReadToEnd();
					text2.Replace("&", "&amp;");
					xmlDocument.LoadXml(text2);
					xmlDocument.Load(Application.dataPath + "Resources/Data/BinData/" + fileName + ".xml");
					xmlReader.Close();
					XmlNodeList xmlNodeList = xmlDocument.SelectNodes("root/item");
					int count = xmlNodeList.Count;
					if (count > 0)
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						foreach (XmlNode xmlNode in xmlNodeList)
						{
							text = string.Concat(new string[]
							{
								"<",
								fileName,
								">",
								xmlNode.InnerXml,
								"</",
								fileName,
								">"
							});
							object value = SerializerUtils.xmlDeserialize(type, text);
							dictionary.Add(xmlNode.FirstChild.InnerText, value);
							num++;
						}
						return dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat(new string[]
				{
					" ==> loadXML failed : fileName = ",
					fileName,
					" _itemData=",
					text,
					" error=",
					ex.Message
				}));
			}
			return null;
		}

		public static Dictionary<string, object> loadXML2(string filepath, Type type)
		{
			return XMLUtils.ReadXMLToObject(filepath, type);
		}

		private static string UTF8ByteArrayToString(byte[] characters)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			return uTF8Encoding.GetString(characters);
		}

		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			return uTF8Encoding.GetBytes(pXmlString);
		}
	}
}
