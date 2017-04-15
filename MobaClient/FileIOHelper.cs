using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace MobaClient
{
	public class FileIOHelper
	{
		private const string FILE_DROPCACHE = "DropCache.dat";

		private const string FILE_DURATION = "Duration.dat";

		private const string FILE_FUCK = "SerializeError.dat";

		private const string FILE_SUSPEND = "Suspend.dat";

		public static string ReadFile(FileType fileType)
		{
			string result;
			switch (fileType)
			{
			case FileType.Account:
				result = FileIOHelper.GetAccountContent();
				break;
			case FileType.DropCache:
				result = FileIOHelper.GetDropCache();
				break;
			case FileType.Duration:
				result = FileIOHelper.GetDuration();
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}

		public static void WriteFile(FileType fileType, string content)
		{
			switch (fileType)
			{
			case FileType.Account:
				FileIOHelper.SetAccountContent(content);
				break;
			case FileType.DropCache:
				FileIOHelper.SetDropCache(content);
				break;
			case FileType.Duration:
				FileIOHelper.SetDuration(content);
				break;
			case FileType.SerializeError:
				FileIOHelper.SetSerializeError(content);
				break;
			case FileType.SuspendError:
				FileIOHelper.SetSuspendError(content);
				break;
			}
		}

		public static void DeleteFile(FileType fileType)
		{
			if (fileType == FileType.Account)
			{
				FileIOHelper.DeleteAccountFileOnApplieDevice();
			}
		}

		public static Dictionary<string, Duration> ReadDurationFile()
		{
			string path = Application.persistentDataPath + "/Duration.dat";
			Dictionary<string, Duration> result;
			try
			{
				if (File.Exists(path))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					FileStream fileStream = new FileStream(path, FileMode.Open);
					Dictionary<string, Duration> dictionary = binaryFormatter.Deserialize(fileStream) as Dictionary<string, Duration>;
					fileStream.Close();
					result = dictionary;
				}
				else
				{
					result = new Dictionary<string, Duration>();
				}
			}
			catch
			{
				File.Delete(path);
				result = new Dictionary<string, Duration>();
			}
			return result;
		}

		public static void SaveDurationFile(Dictionary<string, Duration> durations)
		{
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/Duration.dat";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = new FileStream(path, FileMode.Create);
				binaryFormatter.Serialize(fileStream, durations);
				fileStream.Close();
			}
		}

		private static void SetAccountContent(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				FileIOHelper.PutAccountContentToAppleDevice(content);
			}
		}

		private static string GetAccountContent()
		{
			string result;
			if (GlobalManager.ClientType == 2)
			{
				result = FileIOHelper.GetAccountContentFromAppleDevice();
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static void DeleteAccountFileOnApplieDevice()
		{
			if (GlobalManager.ClientType == 2)
			{
				string accountFilePath = FileIOHelper.GetAccountFilePath();
				Log.debug("@@@@ DeleteAccountFileOnApplieDevice");
				if (File.Exists(accountFilePath))
				{
					Log.debug("@@@ begin to delete file!");
					File.Delete(accountFilePath);
				}
			}
		}

		private static void PutAccountContentToAppleDevice(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				string accountFilePath = FileIOHelper.GetAccountFilePath();
				if (File.Exists(accountFilePath))
				{
					File.Delete(accountFilePath);
				}
				FileStream fileStream = new FileStream(accountFilePath, FileMode.Create);
				byte[] bytes = new UTF8Encoding(true).GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}

		private static string GetAccountFilePath()
		{
			string result = string.Empty;
			if (GlobalManager.ClientType == 2)
			{
				RuntimePlatform platform = Application.platform;
				if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android)
				{
					result = Path.Combine(Environment.CurrentDirectory, "account.dat");
				}
				else
				{
					string persistentDataPath = Application.persistentDataPath;
					result = Path.Combine(persistentDataPath, "account.dat");
				}
			}
			return result;
		}

		private static string GetAccountContentFromAppleDevice()
		{
			string text = string.Empty;
			string result;
			if (GlobalManager.ClientType == 2)
			{
				string accountFilePath = FileIOHelper.GetAccountFilePath();
				if (!File.Exists(accountFilePath))
				{
					result = text;
					return result;
				}
				FileStream fileStream = new FileStream(accountFilePath, FileMode.OpenOrCreate, FileAccess.Read);
				BinaryReader binaryReader = new BinaryReader(fileStream);
				int num = (int)fileStream.Length;
				byte[] array = new byte[num];
				binaryReader.Read(array, 0, array.Length);
				text = Encoding.Default.GetString(array);
				fileStream.Close();
			}
			result = text;
			return result;
		}

		private static void SetDropCache(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/DropCache.dat";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}

		private static string GetDropCache()
		{
			string result = string.Empty;
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/DropCache.dat";
				if (File.Exists(path))
				{
					FileStream fileStream = new FileStream(path, FileMode.Open);
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					result = Encoding.UTF8.GetString(array);
					fileStream.Close();
				}
			}
			return result;
		}

		private static string GetDuration()
		{
			string text = string.Empty;
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/Duration.dat";
				if (File.Exists(path))
				{
					FileStream fileStream = new FileStream(path, FileMode.Open);
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					text = Encoding.UTF8.GetString(array);
					fileStream.Close();
				}
				if (text.Split(new char[]
				{
					','
				}).Length < 2)
				{
					text = string.Empty;
				}
			}
			return text;
		}

		public static void SetDuration(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/Duration.dat";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}

		private static void SetSerializeError(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/SerializeError.dat";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}

		private static void SetSuspendError(string content)
		{
			if (GlobalManager.ClientType == 2)
			{
				string path = Application.persistentDataPath + "/Suspend.dat";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}
	}
}
